using System.Collections.Generic;
using System.IO;
using TableConvert.Utility.OleDb;

namespace TableConvert.Global
{

    public class ExcelData
    {
        private string _filePath;
        private string _tableName;
        private List<List<string>> _table; 


        public ExcelData(string filePath)
        {
            _filePath = filePath;
        }


        public bool IsCache()
        {
            return _table != null;
        }


        public List<List<string>> Table
        {
            get
            {
                if (_table == null)
                {
                    Load();
                }

                if (_table != null)
                {
                    var result = new List<List<string>>();
                    for (int i = 0; i < _table.Count; i++)
                    {
                        result.Add(new List<string>(_table[i]));
                    }
                    return result;
                }
           

                return null;
            }
        }

        public string TableName
        {
            get { return _tableName; }
        }

        public string FileName
        {
            get { return Path.GetFileName(_filePath); }
        }


        public bool Load()
        {
            if (File.Exists(_filePath))
            {
                _table = OleDbExcel.GetExcelData(_filePath);
                if (_table != null)
                {
                    _tableName = _table[0][0];
                    return true;
                }
            }
            return false;
        }


        public void Clear()
        {
            _table = null;
        }
    }



    public class GlobalExcelCache
    {

        private Dictionary<string, ExcelData> _cacheDatas = new Dictionary<string, ExcelData>();

        public Dictionary<string, ExcelData> CacheDatas
        {
            get { return _cacheDatas;}
        }


        public void AddExcaleCache(string filePath)
        {
            if (!_cacheDatas.ContainsKey(filePath))
                _cacheDatas.Add(filePath, new ExcelData(filePath));
        }


        public void LoadAll()
        {
            Dictionary<string, ExcelData>.Enumerator itor = _cacheDatas.GetEnumerator();
            while (itor.MoveNext())
            {
                var data = itor.Current.Value;
                if (data.IsCache())
                    data.Load();
            }
        }


        public ExcelData FindByName( string name )
        {
            Dictionary<string, ExcelData>.Enumerator itor = _cacheDatas.GetEnumerator();
            while (itor.MoveNext())
            {
                if (itor.Current.Key.EndsWith(name))
                {
                    return itor.Current.Value;
                }
            }
            return null;
        }


    }
}