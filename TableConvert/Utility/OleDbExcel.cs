using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using TableConvert.Global;

namespace TableConvert.Utility.OleDb
{
    public static class OleDbExcel
    {
        //public const string SheetName = "源表";
        //public const string TableStartWith = "std";

        public static List<List<string>> GetExcelData(string path)
        {
            DataTable dataTable = GetExcelDataTable(path);
            if (dataTable == null)
            {
                return null;
            }

            int rowCount = dataTable.Rows.Count;
            if (rowCount <= 0)
                return null;

            var result = new List<List<string>>();

            for (int i = 0; i < rowCount; i++)
            {
                List<string> rowList = new List<string>();

                DataRow dataRow = dataTable.Rows[i];
                int colCount = dataTable.Columns.Count;
                for (int j = 0; j < colCount; j++)
                {
                    string data = dataRow[j].ToString().Trim();
                    rowList.Add(string.IsNullOrEmpty(data) ? string.Empty : data);
                }

                //  忽略行
                if (IsIgnore(rowList[0]))
                    continue;

                result.Add(rowList);

                //  读取到END跳出
                if (IsBreak(rowList[0]))
                    break;
            }

            //  过滤列数
            //for (int i = 0; i < GlobalDataManager.Instance.Config.ColumnClip; i++)
            //{
            //    RemoveColumn(i, ref result);
            //}


            return result;
        }


        private static bool IsBreak( string value)
        {
            if (value == GlobalDataManager.Instance.Config.EndFlag)
                return true;
            return false;
        }


        private static bool IsIgnore(string value)
        {
            if (value == GlobalDataManager.Instance.Config.IgnoreFlag)
                return true;
            return false;
        }


        public static void RemoveColumn(int index, ref List<List<string>> info)
        {
            if (info == null || info.Count <= 0 || index >= info[0].Count)
                return;

            for (int i = 0; i < info.Count; i++)
            {
                info[i].RemoveAt(index);
            }
        }


        public static void RemoveFirstColumu(ref List<List<string>> info)
        {
            RemoveColumn(0, ref info);
        }


        public static DataTable GetExcelDataTable(string path)
        {
            string connectionString = GetConnectionStringByPath(path);

            DataSet dataSet = new DataSet();
            OleDbConnection connection = new OleDbConnection(connectionString);
            try
            {
                connection.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("Path: {0}\nError: {1}", path, e.Message), string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            
            DataTable sheetNames = connection.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables,
                new object[] {null, null, null, "TABLE"});

            string sheetName = GlobalDataManager.Instance.Config.SheetName;
            if (IsExist(sheetNames, sheetName))
            {
                string sqlStatement = string.Format(" select * from [{0}$A:DA]", sheetName);

                OleDbDataAdapter adapter = new OleDbDataAdapter(sqlStatement, connectionString);
                adapter.Fill(dataSet, sheetName);
                connection.Close();

                DataTable dtTable = dataSet.Tables[0];
                string tableName = dtTable.Rows[0][0].ToString().Trim();

                //if (tableName.StartsWith(TableStartWith))
                //{
                //    int rowCount = dtTable.Rows.Count;
                //    for (int i = rowCount - 1; i >= 0; i--)
                //    {
                //        DataRow dr = dtTable.Rows[i];
                //        bool isEmpty = true;
                //        for (int j = 0; j < dtTable.Columns.Count; j++)
                //        {
                //            if (dr[j].ToString().Trim() != string.Empty)
                //            {
                //                isEmpty = false;
                //                break;
                //            }
                //        }

                //        if (isEmpty)
                //            dtTable.Rows.Remove(dr);
                //        else
                //        {
                //            break;
                //        }
                //    }
                //}

                return dtTable;
            }
            else
            {
                MessageBox.Show(string.Format("没有找到 {0}\n{1}", sheetName, path));
            }

            return null;
        }



        public static bool IsExist(DataTable table, string sheetName)
        {
            if (table.Rows.Count > 0)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    string name = table.Rows[i]["TABLE_NAME"].ToString();
                    if (sheetName + "$" == name)
                        return true;
                }
            }
            return false;
        }


        public static string GetConnectionStringByPath(string path)
        {
            string suffix = Path.GetExtension(path).ToLower();
            if (suffix == ".xls")
            {
                return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source = " + path +
                       ";Extended Properties = 'Excel 8.0;HDR=NO;IMEX=1'";
            }

            return "Provider = Microsoft.ACE.OLEDB.12.0 ; Data Source = " + path +
                   ";Extended Properties=\"Excel 12.0;HDR=NO;IMEX=1;\"";
        }


    }
}