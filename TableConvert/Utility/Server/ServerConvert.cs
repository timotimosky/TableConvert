using System.Collections.Generic;
using System.IO;
using TableConvert.Global;
using TableConvert.Utility.OleDb;

namespace TableConvert.Utility.Server
{
    public class ServerConvert
    {

        public void Export(ExcelData excelData)
        {
            var info = excelData.Table;
            if (info == null)
                return;

            List<string> desc, types, keys;
            Filter(info, out desc, out types, out keys);

            string tableName = desc[0];

            List<List<string>> appendDesc = new List<List<string>>();

            if (GlobalSetting.ServerAppendNote)
                appendDesc.Add(desc);

            if (GlobalSetting.ServerAppendType)
                appendDesc.Add(types);


            OleDbExcel.RemoveFirstColumu(ref appendDesc);
            OleDbExcel.RemoveFirstColumu(ref info);

            ExcelConvert excelConvert = new ExcelConvert();
            excelConvert.Export(GlobalSetting.ServerOutputPath + "/" + tableName + "_Server.xlsx", "Server", info, types, appendDesc);
        }


        private void Filter( List<List<string>> contens, out List<string> desc, out List<string> types, out List<string> keys)
        {
            desc = contens[0];
            types = contens[2];
            keys = contens[3];
            contens.RemoveRange(0, 4);

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