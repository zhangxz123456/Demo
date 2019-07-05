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
    /// VersionInformation.xaml 的交互逻辑
    /// </summary>
    public partial class VersionInformation : Window
    {
        public VersionInformation()
        {
            InitializeComponent();
            changeWinVersionLanguage();          
        }
        
        private void changeWinVersionLanguage()
        {
            if("CN".Equals(MainWindow.PlayLanguage))
            {
            Title = "软件信息";
            tabVersion.Header = "版本信息";
            tabUpdateRecord.Header = "历史记录";
            textBox.Text = "软件版本号：V6.2.4 \r\n"+
                          "网址：www.shuqee.cn \r\n"+
                          "电话：020 34885536  \r\n"+
                          "地址：广州市番禺区石基镇市莲路富城工业园3号楼 \r\n"+
                          "邮箱：shueee@shuqee.com \r\n"+
                          "软件归广州数祺数字科技有限公司版权所有， 任何单位和个人不得复制本程序! \r\n\r\n"+
                          "软件类型：" + MainWindow.PlayType + "\r\n" +
                          "软件语言：" + MainWindow.PlayLanguage + "\r\n" +
                          "自由度：  " + MainWindow.PlayDOF + "\r\n" +
                          "行程高度：" + MainWindow.PlayHeight;
            textBox1.Text = "shuqee版本更新信息：\r\n" +
                            "                   V6.2.4 \r\n" +
                            "更新日期：2019/4/11 \r\n" +
                            "更新内容：优化界面，删除冗余代码 \r\n" +
                            "              更改调试界面显示   \r\n" +
                            "/**************************************/ \r\n" +
                            "shuqee版本更新信息：\r\n" +
                            "                   V6.2.3 \r\n" +
                            "更新日期：2019/3/27 \r\n" +
                            "更新内容：优化界面，将类型模块语言模块整合一起 \r\n" +
                            "/**************************************/ \r\n" +
                            "shuqee版本更新信息：\r\n" +
                            "                   V6.2.2 \r\n"+
                            "更新日期：2018/12/5 \r\n"+
                            "更新内容：优化界面，更改播放影片显示问题 \r\n"+
                            "/**************************************/ \r\n"+
                            "shuqee版本更新信息：\r\n"+
                            "                   V6.2.1 \r\n"+
                            "更新日期：2018/10/25 \r\n"+
                            "更新内容：软件整体升级，修改通信方式，增快数据发送频率，增加数据采集点";
            }
            else
            {
                textBox.Text = "Software version number：V6.2.4 \r\n"+
                               "Website：www.shuqee.com \r\n"+
                               "Telephone：0086 020-34885536 \r\n" +
                               "Address：Bldg 3.Fucheng industrial park,shilian road,shiji village,shiji town,panyu district,guangzhou,CN.\r\n" +
                               "Email：shueee@shuqee.com \r\n" +
                               "      Copyright by Guangzhou Shuqee Digital Tech. Co., Ltd. Any company or personal can not copy this software! \r\n\r\n" +
                               "Software Type: " + MainWindow.PlayType + "\r\n" +
                               "Software Language: " + MainWindow.PlayLanguage + "\r\n" +
                               "PlayDOF: " + MainWindow.PlayDOF + "\r\n" +
                               "Height: " + MainWindow.PlayHeight + "\r\n";
            }         
        }
    }
}
