using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TableConvert
{
    /// <summary>
    /// ProgressWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressWindow : Window
    {

        private int _total;
        private int _current;
        public int Value { get; set; }
        private string _info;


        public ProgressWindow()
        {
            InitializeComponent();
        }


        public void Start(int total, string title)
        {
            Title = title;
            _total = total;
        }


        public void SetProgress( int progress, string info)
        {
            _info = info;
            _current = progress;
            Value = (int)(_current / (float)_total * 100);
            RefreshView();
        }


        public void RefreshView()
        {
            HandleLabel.Content = _info;
            ProgressLabel.Content = string.Format("{0}/{1}", _current, _total);
            UserProgressBar.Value = Value;
            //ProgressBar
            //UserProgressBar.SetValue();
            //UserProgressBar.
        }

    }
}
