using Newtonsoft.Json;

namespace TableConvert.Utility.Json
{
    //用JSON来保存工具的配置信息
    public static class JsonHelp
    {
        public static void SaveFile(string filePath, object obj, bool isIndented = true)
        {
            //Formatting.Indented生成良好的显示格式,可读性更好。
           // 另一方面,Formatting.None会跳过不必要的空格和换行符
            string text = Newtonsoft.Json.JsonConvert.SerializeObject(obj, isIndented ? Formatting.Indented : Formatting.None);
            System.IO.File.WriteAllText(filePath, text);
        }


        public static T LoadFile<T>(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return default(T);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(System.IO.File.ReadAllText(filePath));
        }
    }
}