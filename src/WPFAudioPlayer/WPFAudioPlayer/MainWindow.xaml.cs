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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFAudioPlayer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnClickInitMP3(object sender, RoutedEventArgs e)
        {
            string url = txtUrl.Text;
            if (!string.IsNullOrEmpty(url))
            {
                listenBacker.PlayMp3Url(url);
            }
        }

        private void btnCLikMark(object sender, RoutedEventArgs e)
        {
            string mark = txtMark.Text;
            if (int.TryParse(mark, out int ret))
            {
                listenBacker.Mark(ret);
            }
        }
    }
}
