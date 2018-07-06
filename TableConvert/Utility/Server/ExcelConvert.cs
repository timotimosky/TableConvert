using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using TableConvert.Global;

namespace TableConvert.Utility.Server
{
    public class ExcelConvert
    {
        public void Export(ExcelData excelData, string outputFilePath)
        {
            try
            {
                var spreadSheet = SpreadsheetDocument.Create(outputFilePath, SpreadsheetDocumentType.Workbook);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }

            using (var spreadSheet = SpreadsheetDocument.Create(outputFilePath, SpreadsheetDocumentType.Workbook))
            {
                spreadSheet.AddWorkbookPart();
                spreadSheet.WorkbookPart.Workbook = new Workbook();
                spreadSheet.WorkbookPart.Workbook.AppendChild(new Sheets());

                List<List<string>> Test1 = new List<List<string>>()
                {
                    new List<string>() {"1","2","3","4","5","6"},
                    new List<string>() {"21","22","23","24","25","26"},
                    new List<string>() {"31","32","33","34","35","36"},
                };

                CreateSheetPart(spreadSheet.WorkbookPart, "1", Test1);
                CreateSheetPart(spreadSheet.WorkbookPart, "2", Test1);

                spreadSheet.WorkbookPart.Workbook.Save();
            }
        }



        public void Export(string outputFilePath, string sheetName, List<List<string>> contents, List<string> types, List<List<string>> describe)
        {
            SpreadsheetDocument document = CreateWorkbookPart(outputFilePath);
            if (document != null)
            {
                CreateSheetPart(document.WorkbookPart, sheetName, contents, types, describe);
                document.WorkbookPart.Workbook.Save();
                document.Dispose();
            }
        }


        private static SpreadsheetDocument CreateWorkbookPart(string filePath)
        {
            try
            {
                var spreadSheet = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook);
                spreadSheet.AddWorkbookPart();
                spreadSheet.WorkbookPart.Workbook = new Workbook();
                spreadSheet.WorkbookPart.Workbook.AppendChild(new Sheets());

                return spreadSheet;
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("无法创建文件: {0}\n Error: {1}", filePath, exception.Message), string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return null;
        }


        private static WorksheetPart CreateSheetPart(WorkbookPart workbook, string sheetName, List<List<string>> contents = null, List<string> types = null, List<List<string>> describe = null)
        {
            WorksheetPart worksheet = workbook.AddNewPart<WorksheetPart>();
            worksheet.Worksheet = new Worksheet();
            SheetData sheetData = new SheetData();
            worksheet.Worksheet.AppendChild(sheetData);

            //  add data
            if (contents != null && contents.Count > 0)
            {
                CellValues[] cellTypes = new CellValues[types.Count];
                for (int i = 0; i < types.Count; i++)
                {
                    cellTypes[i] = GetValuesTypeByString(types[i]);
                }

                int contentOffset = describe.Count;

                for (int i = 0; i < describe.Count; i++)
                {
                    var rawContent = describe[i];
                    Row row = new Row();
                    for (int j = 0; j < rawContent.Count; j++)
                    {
                        Cell cell = CreateValueCell(j + 1, i + 1, describe[i][j], CellValues.String);
                        row.Append(cell);
                    }
                    sheetData.Append(row);
                }

                for (int i = 0; i < contents.Count; i++)
                {
                    var rawContent = contents[i];
                    Row row = new Row();
                    for (int j = 0; j < rawContent.Count; j++)
                    {
                        Cell cell = CreateValueCell(j + 1, i + 1 + contentOffset, contents[i][j], cellTypes[j]);
                        row.Append(cell);
                    }
                    sheetData.Append(row);
                }
            }

            worksheet.Worksheet.Save();
            workbook.Workbook.GetFirstChild<Sheets>().AppendChild(new Sheet()
            {
                Id = workbook.GetIdOfPart( worksheet ),
                SheetId = 1,
                Name = sheetName
            });

            workbook.Workbook.Save();

            return worksheet;
        }

        private static CellValues GetValuesTypeByString(string type)
        {
            type = type.ToLower();
            if (type == "int")
            {
                return CellValues.Number;
            }
            else
            {
                return CellValues.String;
            }
        }

        private static Cell CreateValueCell(int columnIndex, int rowIndex, object cellValue, CellValues type = CellValues.String)
        {
            Cell cell = new Cell();
            cell.CellReference = GetCellReference(columnIndex) + rowIndex;
            cell.DataType = type;

            //  有格式则应用格式
            //if (styleIndex.HasValue)
            //    cell.StyleIndex = styleIndex.Value;

            //  使用CellValue对象设置单元格
            CellValue value = new CellValue();
            value.Text = cellValue.ToString();
            cell.AppendChild(value);

            return cell;
        }


        private static string GetCellReference(int colIndex)
        {
            int dividend = colIndex;
            string columnName = String.Empty;
            int modifier;

            while (dividend > 0)
            {
                modifier = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modifier).ToString() + columnName;
                dividend = (int)((dividend - modifier) / 26);
            }

            return columnName;
        }


        private static int InsertSharedStringItem(string text, SharedStringTablePart sharedStringTablePart)
        {
            if (sharedStringTablePart.SharedStringTable == null)
            {
                sharedStringTablePart.SharedStringTable = new SharedStringTable();
                sharedStringTablePart.SharedStringTable.Count = 1;
                sharedStringTablePart.SharedStringTable.UniqueCount = 1;
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in sharedStringTablePart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }
                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            sharedStringTablePart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            sharedStringTablePart.SharedStringTable.Save();

            return i;
        }

    }
}