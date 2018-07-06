using System.Collections.Generic;
using TableConvert.Global;
using TableConvert.Utility.Tpl;

namespace TableConvert.Utility.Lua
{
    public static class LuaTemplate
    {

        public const string Field = "Filed.txt";
        public const string Mould = "Mould.txt";
        public const string Line = "Line.txt";
        public const string TableMould = "TableMould.txt";


        public const string ValueEmpty = "nil";

        public const string NumberType = "int";
        public const string StringType = "string";
        public const string NumberArrayType = "int_array";
        public const string StringArrayType = "string_array";
        public const string BoolArrayType = "bool_array";
        public const string BoolType = "bool";
        public const string Array = "array";

        public const char ArraySplitChar = ',';


        private static readonly Dictionary<string, ITemplate> Templates;


        static LuaTemplate()
        {
            Templates = new Dictionary<string, ITemplate>()
            {
                {Field, new LuaFieldTpl()},
                {Mould, new LuaModelTpl()},
                {Line, new LuaLineTpl()},
                {TableMould, new LuaTableMouldTpl()}
            };
        }


        public static ITemplate GeTemplate(string name)
        {
            if (Templates.ContainsKey(name))
                return Templates[name];
            return null;
        }

        //模板
        public static string GetTemplateValue(string templateName)
        {
            string filePath = GetTemplateFilePath(templateName);
            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllText(filePath);
            }
            return string.Empty;
        }


        public static string GetTemplateFilePath(string fileName)
        {
            return GlobalDataManager.Instance.TemplatePath + "/" + fileName;
        }

    }
}