using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TableConvert.Global;
using TableConvert.Utility.Client;
using TableConvert.Utility.Process;
using TableConvert.Utility.Server;
using MessageBox = System.Windows.MessageBox;

namespace TableConvert
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            GlobalDataManager.Instance.MainWindow = this;
            GlobalDataManager.Instance.ReloadExcelPath();
        }

        public void RefreshExcelListBox( string [] files )
        {
            for (int i = ExcelList.Items.Count - 1; i >= 0; i--)
            {
                ExcelList.Items.RemoveAt(i);
            }

            for (int i = 0; i < files.Length; i++)
            {
                ExcelList.Items.Add(new ListBoxItem() {Content = System.IO.Path.GetFileName(files[i])});
            }

            ExcelList.Items.Refresh();
        }


       //临时刷新窗口
        public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrames), frame);
            Dispatcher.PushFrame(frame);
        }

        public object ExitFrames(object f)
        {
            ((DispatcherFrame)f).Continue = false;

            return null;
        }



        #region 事件处理

        /// <summary>
        /// 导出目标文件
        /// </summary>
        private void OnExportSelection(object sender, RoutedEventArgs e)
        {

            List<ExcelData> exportList = FetchSelect();
            Export(exportList);
        }


        /// <summary>
        /// 导出所有
        /// </summary>
        private void OnExprotAll(object sender, RoutedEventArgs e)
        {
            var exports = FetchAll();
            Export(exports);
        }


        /// <summary>
        /// 打开目录,获取所有excel文件.
        /// </summary>        
        private void OnOpenDirectory(object sender, RoutedEventArgs e)
        {
            //弹出一个对话框，
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GlobalDataManager.Instance.ChangeExcelPath(folderBrowserDialog.SelectedPath);
            }

        }


        /// <summary>
        /// 打开输出目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void OnOpenOutputPath(object sender, RoutedEventArgs e)
        {
            ProcessUtility.OpenFolder(GlobalDataManager.Instance.Config.OutputPath);
        }


        /// <summary>
        /// 设置输出目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void OnSetOutputPath(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GlobalDataManager.Instance.ChangeOutputPath(folderBrowserDialog.SelectedPath);
            }
        }

        private void OnOpenExcelDirectory(object sender, RoutedEventArgs e)
        {
            ProcessUtility.OpenFolder(GlobalDataManager.Instance.Config.ExcelPath);
        }


        /// <summary>
        /// 测试
        /// </summary>
        private void OnTest(object sender, RoutedEventArgs e)
        {
            ExcelConvert excelConvert = new ExcelConvert();
            excelConvert.Export(null, GlobalDataManager.Instance.Config.OutputPath + "/test.xlsx");
            ProcessUtility.OpenFolder(GlobalDataManager.Instance.Config.OutputPath);
        }

        #endregion


        #region Utility

        public List<ExcelData> FetchSelect()
        {
            var selections = ExcelList.SelectedItems;

            List<ExcelData> exportList = new List<ExcelData>();

            for (int i = 0; i < selections.Count; i++)
            {
                if (selections[i] is ListBoxItem)
                {
                    ListBoxItem item = (ListBoxItem)selections[i];

                    var data = GlobalDataManager.Instance.GetExcelDataByName(item.Content as string);
                    if (data != null)
                        exportList.Add(data);
                }
            }

            return exportList;
        }

        public List<ExcelData> FetchAll()
        {
            var cache = GlobalDataManager.Instance.ExcelCache.CacheDatas;

            var itor = cache.GetEnumerator();
            List<ExcelData> result = new List<ExcelData>();
            while (itor.MoveNext())
            {
                result.Add( itor.Current.Value);
            }

            return result;
        }

        #endregion


        // Export 
        private void Export(List<ExcelData> exportList)
        {

            if (exportList.Count == 0)
            {
                MessageBox.Show("列表为空!");                
                return;
            }

            var progress = new ProgressWindow();
            progress.Start(exportList.Count * 2, "Convert");
            progress.Show();

            ClientConvert clientConvert = new ClientConvert();
            for (int i = 0; i < exportList.Count; i++)
            {
                progress.SetProgress(i + 1, exportList[i].FileName);
                DoEvents();
                clientConvert.Export(exportList[i]);
            }

            ServerConvert serverConvert = new ServerConvert();
            for (int i = 0; i < exportList.Count; i++)
            {
                progress.SetProgress(i + 1 + exportList.Count, exportList[i].FileName);
                DoEvents();
                serverConvert.Export(exportList[i]);
            }

            progress.Close();
            ProcessUtility.OpenFolder(GlobalDataManager.Instance.Config.OutputPath);
        }


       
    }
}
