using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevComponents.WpfRibbon;
using VideoPlayer.Properties;
using System.Xml;
using System.Windows.Forms;
using System.IO;

namespace VideoPlayer
{
    /// <summary>
    /// OpenURL.xaml 的交互逻辑
    /// </summary>
    public partial class OpenURL : Window
    {
        private Player playerWin;
        public OpenURL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 有参数的结构方法
        /// </summary>
        /// <param name="win">Player主窗体</param>
        /// <param name="e"></param>
        public OpenURL(Player win)
        {
            this.playerWin = win;
            InitializeComponent();
        }

        private void btnOK_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FileInfo finfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + @"\XML\" + "OpenURL.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\XML\" + "OpenURL.xml");
                XmlNode childNodes = xmlDoc.SelectSingleNode("OpenURL");
                XmlElement element = (XmlElement)childNodes; ;
                element["Path"].InnerText = tbOpen.Text.Trim();
                xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory + @"\XML\" + "OpenURL.xml");
                if (string.IsNullOrEmpty(tbOpen.Text.Trim()))
                {
                    System.Windows.Forms.MessageBox.Show("请输入路径！");
                }
                else
                {
                    this.playerWin.OpenPathPlay();
                    this.Close();
                } 
            }
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 继续浏览打开文件功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
            this.playerWin.OpenFilePlay();
        }
    }
}
