using System.Collections.Generic;
using System.IO;
using System.Windows;
using TableConvert.Utility.FileWatcher;
using TableConvert.Utility.Json;
using TableConvert.Utility.Tpl.CodeKey;

namespace TableConvert.Global
{
    public class GlobalDataManager
    {


        #region Instance

        private static GlobalDataManager _instance;
        public static GlobalDataManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GlobalDataManager();
                return _instance;
            }
        }

        #endregion


        private GlobalConfig _globalConfig;
        private GlobalExcelCache _globalExcelCache;
        private CodeKeyDataManager _codeKeyDataManager;

        private readonly string _templatePath;



        public List<string> ExcelFiles { get; set; }


        public MainWindow MainWindow
        {
            get; internal set;
        }

        // file path
        public readonly string GlobalConfigPath;
        public readonly string CodeKeyDataPath;

        private FileListener _excelDirectoryListener;
        private FileListener _configDirectoryListener;

        public GlobalDataManager()
        {
            string configPath = Directory.GetCurrentDirectory() + "/Config";
            if (!Directory.Exists(configPath))
                Directory.CreateDirectory(configPath);
            GlobalConfigPath = configPath + "/global.json";
            CodeKeyDataPath = configPath + "/codekey.json";
            _templatePath = configPath + "/Template";


            _excelDirectoryListener = FileListener.FetchFileListener(Config.ExcelPath, OnExcelPathChange);
            _configDirectoryListener = FileListener.FetchFileListener(configPath, OnConfigPathChange);
        }


        public string TemplatePath
        {
            get { return _templatePath; }
        }


        public GlobalConfig Config 
        {
            get
            {
                if (_globalConfig == null)
                {
                    _globalConfig = JsonHelp.LoadFile<GlobalConfig>(GlobalConfigPath);
                    if (_globalConfig == null)
                    {
                        _globalConfig = new GlobalConfig();
                        JsonHelp.SaveFile(GlobalConfigPath, _globalConfig);
                    }
                    else
                    {
                        _globalConfig.InspectDirectory();
                    }
                }
                return _globalConfig;
            }
        }


        public CodeKeyDataManager CodeKeyData
        {
            get
            {
                if (_codeKeyDataManager == null)
                {
                    _codeKeyDataManager = JsonHelp.LoadFile<CodeKeyDataManager>(CodeKeyDataPath);
                    if (_codeKeyDataManager == null)
                    {
                        _codeKeyDataManager = new CodeKeyDataManager();
                        JsonHelp.SaveFile(CodeKeyDataPath, _codeKeyDataManager);
                    }
                }

                return _codeKeyDataManager;
            }
        }


        public GlobalExcelCache ExcelCache
        {
            get
            {
                if (_globalExcelCache == null)
                    _globalExcelCache = new GlobalExcelCache();
                return _globalExcelCache;
            }
        }


        public void AddFile(string[] files)
        {
            if (ExcelFiles == null)
                ExcelFiles = new List<string>();

            ExcelFiles.Clear();
            for (int i = 0; i < files.Length; i++)
            {
                if (!ExcelFiles.Contains(files[i]))
                    ExcelFiles.Add(files[i]);

                ExcelCache.AddExcaleCache(files[i]);
            }
        }


        public ExcelData GetExcelDataByName(string name)
        {
            return _globalExcelCache.FindByName(name);
        }


        public void ChangeExcelPath( string newPath )
        {
            if (_excelDirectoryListener != null)
                _excelDirectoryListener.Stop();

            //  save
            Config.ExcelPath = newPath;
            JsonHelp.SaveFile(GlobalConfigPath, _globalConfig);

            _excelDirectoryListener = FileListener.FetchFileListener(Config.ExcelPath, OnExcelPathChange);

            ReloadExcelPath();
        }


        public void ChangeOutputPath(string newPath)
        {
            Config.OutputPath = newPath;
            JsonHelp.SaveFile(GlobalConfigPath, _globalConfig);
        }


        public string[] GetAllExcelByPath( string path )
        {
            string[] files2 = Directory.GetFiles(path, "*.xls");
            return files2;
        }


        public void ReloadExcelPath()
        {
            string[] excels = GetAllExcelByPath(Config.ExcelPath);
            ExcelCache.CacheDatas.Clear();


            List<string> filter = new List<string>();
            for (int i = 0; i < excels.Length; i++)
            {
                string fileName = Path.GetFileName(excels[i]);
                if (fileName.StartsWith("~$"))
                    continue;
                filter.Add(excels[i]);
            }


            excels = filter.ToArray();
            AddFile(excels);
            MainWindow.Dispatcher.Invoke(() => { MainWindow.RefreshExcelListBox(excels); });
        }


        #region 文件监控

        private void OnExcelPathChange(WatcherChangeTypes type, FileSystemEventArgs args)
        {
            ReloadExcelPath();
        }


        private void OnConfigPathChange(WatcherChangeTypes type, FileSystemEventArgs args)
        {
            if ((type & WatcherChangeTypes.Deleted) != 0 ||
                (type & WatcherChangeTypes.Changed) != 0)
            {
                _globalConfig = null;
                _codeKeyDataManager = null;
            }
        }

        #endregion

    }
}