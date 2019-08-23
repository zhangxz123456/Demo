using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

namespace MoviePlayer.Forms
{
    /// <summary>
    /// Projector.xaml 的交互逻辑
    /// </summary>
    public partial class Projector : Window
    {
        Socket tcpClient;    
        public Projector()
        {
            InitializeComponent();
            //tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //tcpClient1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            changWinProjectLanguage();
        }

        private void changWinProjectLanguage()
        {
            if (MainWindow.PlayLanguage.Equals("CN"))
            {
                Title = "投影机";
                label11.Content = "投影机1";
                label12.Content = "IP";
                label13.Content = "端口";
                btn1Connect.Content = "连接";
                btn1Open.Content = "打开";
                btn1Close.Content = "关闭";

                label21.Content = "投影机2";
                label22.Content = "IP";
                label23.Content = "端口";
                btn2Connect.Content = "连接";
                btn2Open.Content = "打开";
                btn2Close.Content = "关闭";
            }
        }

     

        private bool reConnect(string ip,string port)
        {           
            try
            {
                tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tcpClient.Connect(UdpInit.transformIP(ip, port));
                return true;
            }
            catch
            {
                MessageBox.Show("连接有误");
                return false;
            }
        }

        private void close()
        {
            if (tcpClient.Connected)
            {
                tcpClient.Close();
            }
        }


        private void btn1Connect_Click(object sender, RoutedEventArgs e)
        {
            if (btn1Connect.Content.Equals("Connect"))
            {
                //tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                btn1Connect.Content = "DisConnect";
                //tcpClient.Connect(UdpInit.transformIP(textBox.Text,textBox1.Text)); 
                reConnect(pj1IP.Text,pj1Port.Text);
            }
            else
            {
                btn1Connect.Content = "Connect";
                tcpClient.Close();
            }
        }

        private void btn1Open_Click(object sender, RoutedEventArgs e)
        {
            bool isConnect = reConnect(pj1IP.Text, pj1Port.Text);
            if (isConnect)
            {
                byte[] data1 = { 0x30, 0x30, 0x50, 0x4F, 0x4E, 0x0D };
                tcpClient.Send(data1);
                close();
            }
        }

        private void btn1Close_Click(object sender, RoutedEventArgs e)
        {
            bool isConnect = reConnect(pj1IP.Text, pj1Port.Text);
            if (isConnect)
            {
                byte[] data2 = { 48, 48, 80, 79, 70, 13 };
                tcpClient.Send(data2);
                close();
            }
        }

        private void btn2Connect_Click(object sender, RoutedEventArgs e)
        {
            if (btn1Connect.Content.Equals("Connect"))
            {
                //tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                btn1Connect.Content = "DisConnect";
                //tcpClient.Connect(UdpInit.transformIP(textBox.Text,textBox1.Text)); 
                reConnect(pj2IP.Text,pj2Port.Text);
            }
            else
            {
                btn1Connect.Content = "Connect";
                tcpClient.Close();
            }
        }

        private void btn2Open_Click(object sender, RoutedEventArgs e)
        {
            bool isConnect = reConnect(pj2IP.Text, pj2Port.Text);
            if (isConnect)
            {
                byte[] data1 = { 0x30, 0x30, 0x50, 0x4F, 0x4E, 0x0D };
                tcpClient.Send(data1);
                close();
            }
        }

        private void btn2Close_Click(object sender, RoutedEventArgs e)
        {
            bool isConnect = reConnect(pj2IP.Text, pj2Port.Text);
            if (isConnect)
            {
                byte[] data2 = { 48, 48, 80, 79, 70, 13 };
                tcpClient.Send(data2);
                close();
            }
        }
    }
}
