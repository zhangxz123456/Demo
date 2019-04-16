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
using System.Windows.Shapes;

namespace MoviePlayer.Forms
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Setting : Window
    {
        public Setting()
        {
            InitializeComponent();
            changeWinSettingLanguage();
        }

        private void changeWinSettingLanguage()
        {
            if ("CN".Equals(MainWindow.PlayLanguage))
            {
                Title = "设置";
                label.Content = "    待更新.....";
            }
        }
    }
}
