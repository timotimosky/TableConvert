using System.IO;

namespace TableConvert.Global
{
    public class GlobalConfig
    {

        public GlobalConfig()
        {
            string projectPath = Directory.GetCurrentDirectory();

            OutputPath = projectPath + "/Output";
            ExcelPath = projectPath + "/ExcelTest";

            if (Directory.Exists(OutputPath) == false)
                Directory.CreateDirectory(OutputPath);

            if (Directory.Exists(ExcelPath) == false)
                Directory.CreateDirectory(ExcelPath);
        }


        public string OutputPath { get; set; }

        public string ExcelPath { get; set; }

        public string EndFlag { get; set; }

        public int ClipCount { get; set; }

        public int ColumnClip { get; set; }

        public int RowClip { get; set; }

        public string SheetName { get; set; }

        public string IgnoreFlag { get; set; }

        public bool ClientNote { get; set; }

        public bool ServerAppendType { get; set; }

        public bool ServerAppendNote { get; set; }


        public void InspectDirectory()
        {
            string projectPath = Directory.GetCurrentDirectory();
            if (Directory.Exists(OutputPath) == false)
            {
                OutputPath = projectPath + "/Output";
                if (Directory.Exists(OutputPath) == false)
                    Directory.CreateDirectory(OutputPath);
            }

            if (Directory.Exists(ExcelPath) == false)
            {
                ExcelPath = projectPath + "/ExcelTest";
                if (Directory.Exists(ExcelPath) == false)
                    Directory.CreateDirectory(ExcelPath);
            }
        }
    }
}