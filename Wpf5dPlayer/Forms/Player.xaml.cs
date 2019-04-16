
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Forms;
using Wpf5dPlayer.Forms;
using System.Windows.Threading;
using Wpf5dPlayer.Class;

namespace Wpf5dPlayer
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Player : Window
    {
        public static List<string> list = new List<string>();
        public static string fileName = "";                          //影片文件名
        Window1 win = new Window1();
        DispatcherTimer timer = null;            //开启定时器接收时间码
        DispatcherTimer timer1 = null;           //开启定时器更新播放影片时间

        public enum MediaStatus
        {
            /// <summary>
            ///播放
            /// </summary>
            Play,
            /// <summary>
            ///暂停
            /// </summary>
            Pause
        }
        public Player()
        {
            InitializeComponent();
        }

        private void InitListBox()
        {
            //获取软件当前目录的avi文件
            string[] path = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.avi");
            //string[] path = Directory.GetFiles(@"d:\电影", "*.avi");
            for (int i = 0; i < path.Length; i++)
            {
                string videoName = System.IO.Path.GetFileName(path[i]);
                listBox.Items.Add(videoName);
                list.Add(path[i]);
            }

            path = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.mp4");
            //path = Directory.GetFiles(@"d:\电影", "*.mp4");
            for (int i = 0; i < path.Length; i++)
            {
                //listBox.Items.Add(path[i].Substring(path[i].LastIndexOf('\\') + 1));
                string videoName = System.IO.Path.GetFileName(path[i]);   //获取当前路径的文件名包含后缀
                //listBox.Items.Add(videoName.Substring(0,videoName.LastIndexOf('.')));
                listBox.Items.Add(videoName);
                list.Add(path[i]);
            }

        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "avi文件|*.avi|mp4文件|*.mp4|所有文件|*.*";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "avi";
            DialogResult result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            fileName = openFileDialog.FileName;
            //listBox.Items.Add(fileName);
            list.Add(fileName);
            //将影片名字显示在列表当中，不显示路径
            listBox.Items.Add(fileName.Substring(fileName.LastIndexOf('\\') + 1));
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.MessageBox.Show(list[listBox.SelectedIndex]);
            //fileName= @"D:\电影\"+listBox.SelectedItem.ToString();          
            fileName = list[listBox.SelectedIndex];
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (fileName != "")
            {
                //module.readFile();
                // win.Visibility = Visibility.Visible;
                win.Show();
                //this.Hide();
                win.play();
                //媒体文件打开成功
                timer1 = new DispatcherTimer();
                timer1.Interval = TimeSpan.FromSeconds(0.05);   //定时器周期为50ms 
                timer1.Tick += new EventHandler(timer1_tick);
                timer1.Start();

            }
        }




        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            win.pause();
        }

        /// <summary>
        /// 定时器开启事件,显示时间码
        /// </summary>
        private void timer_tick(object sender, EventArgs e)
        {
            //Slider.Value = Window1.sliderPositionValue;        
            //textBox.Text = Window1.sliderPositionValue.ToString();
            //Slider.Maximum = Window1.sliderMaximum;           
            //textBox1.Text = Window1.currenTime+"/"+ Window1.totalTime;            
            //module.FlimValue(Window1.sliderPositionValue);

           // label1.Content = UdpConnect.strTimeCode;

            //textBox1.Text = UdpConnect.strTimeCode;
        }

        /// <summary>
        /// 显示播放进度，发送动作数据
        /// </summary>
        private void timer1_tick(object sender, EventArgs e)
        {
           // Slider.Value = Window1.sliderPositionValue;
            //textBox.Text = Window1.sliderPositionValue.ToString();
           // Slider.Maximum = Window1.sliderMaximum;
            textBox.Text = Window1.currenTime + "/" + Window1.totalTime;
            //module.FlimValue(Window1.sliderPositionValue);
            //UdpSend.SendWrite(Window1.sliderPositionValue);            
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            win.stop();
        }
    }
}
