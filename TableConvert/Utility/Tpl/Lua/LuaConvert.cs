using System;
using System.Collections.Generic;
using System.Text;
using TableConvert.Global;
using TableConvert.Utility.OleDb;

namespace TableConvert.Utility.Lua
{
    public class LuaConvert
    {

        private StringBuilder _stringBuilder;
        private readonly StringBuilder _tempBuilder = new StringBuilder(256);

        public void Export(string tableName, string outputPath, List<List<string>> info, List<string> desc, List<string> types, List<string> keys)
        {
            string name = tableName;

            //  字段名称检查,不能是关键字.
            InspectTypes(ref keys);

            //  分页,当excel行数超过最大行数时分页.
            int clipCount = GlobalSetting.ExcelClipCount;
            int page = (int)Math.Ceiling(info.Count / (float)clipCount);
            Dictionary<string, List<List<string>>> clipTable = new Dictionary<string, List<List<string>>>();
            List<string> subTableName = new List<string>();

            for (int i = 0; i < page; i++)
            {
                string subName = name + i;
                subTableName.Add(subName);

                if (info.Count < clipCount)
                    clipTable.Add(subName, new List<List<string>>(info));
                else
                {
                    int start = i*clipCount;
                    int count = info.Count - start < clipCount ? info.Count - start : clipCount;
                    clipTable.Add(subName, info.GetRange(start, count));
                }
            }

            //  生成主表
            WriteTableMould(name,outputPath, clipCount, page, subTableName.ToArray());

            //  生成子表
            var itor = clipTable.GetEnumerator();
            while (itor.MoveNext())
            {
                string lua = Convert(itor.Current.Key, itor.Current.Value, desc, keys, types);
                string outputFilePath = outputPath + "/" + itor.Current.Key + ".lua";
                System.IO.File.WriteAllText(outputFilePath, lua);
            }
        }

        private void InspectTypes(ref List<string> types)
        {
            var key = GlobalDataManager.Instance.CodeKeyData.Get("Lua");
            if (key != null)
            {
                for (int i = 0; i < types.Count; i++)
                {
                    string legal;
                    if (key.IsLegal(types[i], out legal) == false)
                    {
                        types[i] = legal;
                    }
                }   
            }
        }


        private void WriteTableMould(string tabName, string outputPath, int pageTabCount, int tabCount, string[] subTabName)
        {
            var template = LuaTemplate.GeTemplate(LuaTemplate.TableMould);

            string combineTabName = string.Empty;

            if (subTabName.Length > 0)
                combineTabName = "\"" + subTabName[0] + "\"";

            for (int i = 1; i < subTabName.Length; i++)
            {
                combineTabName += ",\"" + subTabName[i] + "\"";
            }

            string info = template.Structure(tabName, pageTabCount.ToString(), tabName.ToString(), combineTabName);
            System.IO.File.WriteAllText(outputPath + "/" + tabName + ".lua", info);
        }

        //转成lua
        private string Convert( string tableName, List<List<string>> data, List<string> desc, List<string> keys, List<string> types)
        {
            _stringBuilder = new StringBuilder(2048);

            var template = LuaTemplate.GeTemplate(LuaTemplate.Mould);

            for (int i = 0; i < data.Count; i++)
            {
                _stringBuilder.Append(ConvertLine(data[i], desc, keys, types, i)).Append(",").Append("\r\n");
                //if (data[i][0] == "END")
                //    break;
            }

            //return _stringBuilder.ToString();
            return template.Structure(tableName, _stringBuilder.ToString());
        }

        //将每一行excel转成一行lua
        public string ConvertLine(List<string> data, List<string> desc, List<string> keys, List<string> types, int index)
        {
            _tempBuilder.Length = 0;

            var template = LuaTemplate.GeTemplate(LuaTemplate.Line);

            for (int i = 0; i < data.Count; i++)
            {
                _tempBuilder.Append(ConvertField(data[i], desc[i], keys[i], types[i])).Append(",").Append("\r\n");
            }

            return template.Structure((index + 1).ToString(), _tempBuilder.ToString());
        }


        public string ConvertField(string data, string desc, string key, string type)
        {
            var template = LuaTemplate.GeTemplate(LuaTemplate.Field);
            if (template != null)
            {
                return template.Structure(key, GetStringByType(data, type), desc);
            }

            return string.Format("Error type {0} desc {1} key {2} type {3}", data, desc, key, type);
        }



        public string GetStringByType(string value, string type)
        {
            type = type.ToLower();

            if (type == LuaTemplate.StringType)
            {
                if (string.IsNullOrEmpty(value))
                    return LuaTemplate.ValueEmpty;
                else if (value == LuaTemplate.ValueEmpty)
                    return LuaTemplate.ValueEmpty;
                else
                    return "\"" + value + "\"";
            }
            else if (type == LuaTemplate.BoolType)
            {
                if (string.IsNullOrEmpty(value))
                    return LuaTemplate.ValueEmpty;
                if (value == "真")
                    return "true";
                if (value == "假")
                    return "false";
                return value;
            }
            else if (type == LuaTemplate.Array)
            {
                return GetArrayString(value, 0);
            }
            else if (type == LuaTemplate.NumberArrayType)
            {
                return GetArrayString(value, 1);
            }
            else if (type == LuaTemplate.StringArrayType)
            {
                return GetArrayString(value, 2);
            }
            else if (type == LuaTemplate.BoolArrayType)
                return GetArrayString(value, 3);
            else
            {
                if (string.IsNullOrEmpty(value))
                    return "nil";
                return value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayType"> 0 自适应，1 number，2 string，3 bool</param>
        /// <returns></returns>

        public string GetArrayString(string array, int arrayType )
        {
            if (string.IsNullOrEmpty(array))
                return "nil";

            array = array.Replace("[", string.Empty);
            array = array.Replace("]", string.Empty);

            string[] items = array.Split(LuaTemplate.ArraySplitChar);
            if (items == null || items.Length == 0)
                return "nil";

            string result = "{";
            for (int i = 0; i < items.Length; i++)
            {
                result += GetArrayValueString(items[i], arrayType);
                if (i < items.Length - 1)
                    result += ",";
            }
            return result + "}";
        }


        private string GetArrayValueString(string value , int type)
        {
            if (string.IsNullOrEmpty(value))
                return "nil";

            value = value.Trim();

            if (type == 0)
            {
                if (IsString(ref value))
                {
                    return value;
                }

                if (IsInt(ref value) || IsFloat(ref value) || IsBool(ref value))
                    return value;

                return "\"" + value + "\"";
            }
            else if (type == 1)
            {
                if (IsFloat(ref value))
                    return value;
                return "nil";
            }
            else if (type == 2)
            {
                if (IsString(ref value))
                    return value;
                return "\"" + value + "\"";
            }
            else if (type == 3)
            {
                if (IsBool(ref value))
                    return value;
                return "nil";
            }
            return "nil";
        }


        private bool IsInt(ref string value)
        {
            int temp;
            return int.TryParse(value, out temp);
        }


        private bool IsFloat(ref string value)
        {
            float temp;
            return float.TryParse(value, out temp);
        }


        private bool IsBool(ref string value)
        {
            bool temp;
            return bool.TryParse(value, out temp);
        }

        private bool IsString(ref string value)
        {
            return value.IndexOf("\"") == 0 && value.LastIndexOf("\"") == value.Length - 1;
        }

    }
}