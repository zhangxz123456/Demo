using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

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
                //label.Content = "    待更新.....";
                label.Content = "参数";
                label1.Content = "播放器类型";
                label2.Content = "语言";
                label3.Content = "自由度";
                label4.Content = "行程";
                label5.Content = "投影";
                button.Content = "修改";
            }
            comboBox.Text = MainWindow.PlayType;
            comboBox1.Text = MainWindow.PlayLanguage;
            comboBox2.Text = MainWindow.PlayDOF;
            comboBox3.Text = MainWindow.PlayProjector;
            textBox.Text = MainWindow.PlayHeight.ToString();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            SaveType();
            MessageBox.Show("修改成功，软件将重新启动");
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();

        }

        private void SaveType()
        {
            string path = MainWindow.playerPath + @"\XML\" + "Type.xml";
            FileInfo finfo = new FileInfo(path);
            if(finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNode childNodes = xmlDoc.SelectSingleNode("Type");
                XmlElement element = (XmlElement)childNodes;
                element["Style"].InnerText=comboBox.Text;
                element["Language"].InnerText=comboBox1.Text;
                element["DOF"].InnerText=comboBox2.Text;
                element["Height"].InnerText=textBox.Text;
                element["Projector"].InnerText=comboBox3.Text;

                xmlDoc.Save(path);
            }
        }
    }
}
