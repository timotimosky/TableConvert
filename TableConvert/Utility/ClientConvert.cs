using System.Collections.Generic;
using TableConvert.Global;
using TableConvert.Utility.Lua;
using TableConvert.Utility.OleDb;

namespace TableConvert.Utility.Client
{
    public class ClientConvert
    {
        public void Export(ExcelData excelData)
        {
            List<List<string>> info = excelData.Table;
            if (info == null)
                return;

            //忽略列数
            OleDbExcel.RemoveFirstColumu(ref info);

            List<string> desc, keys, types;
            Filter(info, out desc, out types, out keys);

            //转换为lua
            new LuaConvert().Export(excelData.TableName, GlobalSetting.ClientOutputPath, info, desc, types, keys);
        }

        //对EXCEL格式的解析
        private void Filter(List<List<string>> contens, out List<string> desc, out List<string> types, out List<string> keys)
        {
            //
            desc = contens[0]; //第一列，描述 
            keys = contens[1]; //第二列，每一行的key
            types = contens[2];//类型
            contens.RemoveRange(0, 4); //移除前4行

            List<int> removeList = new List<int>();

            for (int i = keys.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(keys[i]))
                {
                    removeList.Add(i);
                }
            }

            for (int i = 0; i < removeList.Count; i++)
            {
                int removeIndex = removeList[i];
                desc.RemoveAt(removeIndex);
                types.RemoveAt(removeIndex);
                keys.RemoveAt(removeIndex);

                for (int j = 0; j < contens.Count; j++)
                {
                    contens[j].RemoveAt(removeIndex);
                }
            }

        }

    }
}