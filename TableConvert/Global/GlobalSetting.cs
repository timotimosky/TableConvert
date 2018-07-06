using System.IO;

namespace TableConvert.Global
{
    public static class GlobalSetting
    {


        public static int ExcelClipCount
        {
            get
            {
                GlobalConfig config = GlobalDataManager.Instance.Config;
                if (config.ClipCount == 0)
                    config.ClipCount = 1000;
                return config.ClipCount;
            }
        }

        public static string ClientOutputPath
        {
            get
            {
                string result = GlobalDataManager.Instance.Config.OutputPath + "/Client";
                if (Directory.Exists(result) == false)
                    Directory.CreateDirectory(result);
                return result;
            }
        }

        public static string ServerOutputPath
        {
            get
            {
                string result = GlobalDataManager.Instance.Config.OutputPath + "/Server";
                if (Directory.Exists(result) == false)
                    Directory.CreateDirectory(result);
                return result;

            }
        }


        public static bool ServerAppendNote
        {
            get { return GlobalDataManager.Instance.Config.ServerAppendNote; }
        }


        public static bool ServerAppendType
        {
            get { return GlobalDataManager.Instance.Config.ServerAppendType; } 
        }


        public static bool ClientAppendNode
        {
            get { return GlobalDataManager.Instance.Config.ClientNote; }
        }

    }
}