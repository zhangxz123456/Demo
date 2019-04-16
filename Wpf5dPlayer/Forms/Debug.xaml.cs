using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace MoviePlayer.Forms
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Debugging : Window
    {
        Label[] labelNum;                 //缸
        Image[] imageEvEffect;           //环境特效 
        Image[] imageChairEffect;        //座椅特效
        
       
        byte[] dataNum = new byte[3];
        byte[] dataEvEffect = new byte[8];
        byte[] dataChairEffect = new byte[8];

        DispatcherTimer timer = null;

        public Debugging()
        {
            InitializeComponent();
            //UdpConnect.isRegistered = false;
            UdpConnect.isDebug = true;
            controlInit();
            timerInit();
        }
        private void controlInit()
        {
            labelNum = new Label[6] { label2,label3,label4,label5,label6,label7};
            imageEvEffect = new Image[8] { image,image1,image2,image3,image4,image5,image6,image7};
            imageChairEffect = new Image[8] { image8,image9,image10,image11,image12,image13,image14,image15};
        }

        private void timerInit()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.05);
            timer.Tick += new EventHandler(timer_tick);
            timer.Start();
        }

        private void timer_tick(object sender, EventArgs e)
        {
            byte eEffect=0;
            byte cEffect=0;
            byte dataNumOne=0;                  //1号缸的数据
            byte dataNumTwo=0;                  //2号缸的数据 
            byte dataNumThree=0;                //3号缸的数据

            //byte dataNumOne = 127;                  //1号缸的数据
            //byte dataNumTwo = 127;                  //2号缸的数据 
            //byte dataNumThree = 127;                //3号缸的数据

            byte[] data;           
            byte[] array;          //data+addr+len 
            byte[] Data;           //最终发送的数据

            dataNumOne = dataNum[2];
            dataNumTwo = dataNum[1];
            dataNumThree = dataNum[0];

            for (int i = 0; i < 8; i++)
            {
                eEffect += dataEvEffect[i];
                //  label9.Content = eEffect.ToString();
            }
           // eEffect = dataEvEffect[0];
            for (int i = 0; i < 8; i++)
            {
                cEffect += dataChairEffect[i];
            }

            Debug.WriteLine(eEffect.ToString());

            data = new byte[10] { dataNumOne, dataNumTwo, dataNumThree, 0, 0, 0, 0, 0, cEffect, eEffect };
            array = Mcu.ModbusUdp.ArrayAdd(0, (ushort)data.Length, data);
            Data = Mcu.ModbusUdp.MBReqWrite(array);
            UdpSend.UdpSendData(Data, Data.Length, UdpInit.RemotePoint);
        }

        private void button_MouseMove(object sender, MouseEventArgs e)
        {
            button.Background = button.Background;
        }


        /// <summary>
        /// 点击对应的缸改变背景颜色
        /// </summary>
        /// <param name="i"></param>
        private void changeBackground(int i)
        {
            if (labelNum[i].Opacity == 1 )
            {
                labelNum[i].Opacity = 0.7;
                dataNum[i]= 255;
            }
            else
            {
                labelNum[i].Opacity=1;
                dataNum[i] = 0;
            }
        }
        private void label2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeBackground(0);
        }

        private void label3_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeBackground(1);
        }

        private void label4_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeBackground(2);
        }

        private void label5_MouseUp(object sender, MouseButtonEventArgs e)
        {
           // changeBackground(3);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            dataNum[0] = 255;
            dataNum[1] = 255;
            dataNum[2] = 255;
        }

        private void changeEvImage(int i)
        {
            if (imageEvEffect[i].Opacity==1)
            {
                imageEvEffect[i].Opacity = 0.5;
                dataEvEffect[i] = (Byte)Math.Pow(2,i);
                //label9.Content = dataEvEffect[i].ToString();
            }
            else
            {
                imageEvEffect[i].Opacity = 1;
                dataEvEffect[i] = 0;
            }
        }

        private void changeChairImage(int i)
        {
            if (imageChairEffect[i].Opacity == 1)
            {
                imageChairEffect[i].Opacity = 0.5;
                dataChairEffect[i] = (Byte)Math.Pow(2,i);
            }
            else
            {
                imageChairEffect[i].Opacity = 1;
                dataChairEffect[i] = 0;
            }
        }


        private void image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeEvImage(0);
        }

        private void image1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeEvImage(1);
        }

        private void image2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeEvImage(2);
        }

        private void image3_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeEvImage(3);
        }

        private void image4_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeEvImage(4);
        }

        private void image5_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeEvImage(5);
        }
        private void image6_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeEvImage(6);
        }

        private void image7_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeEvImage(7);
        }

        private void image8_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeChairImage(0);

        }

        private void image9_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeChairImage(1);

        }

        private void image10_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeChairImage(2);

        }

        private void image11_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeChairImage(3);

        }

        private void image12_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeChairImage(4);

        }

        private void image13_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeChairImage(5);

        }

        private void image14_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeChairImage(6);

        }

        private void image15_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeChairImage(7);

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            UdpSend.SendReset();           
            timer.Stop();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            dataNum[0] = 0;
            dataNum[1] = 0;
            dataNum[2] = 0;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //UdpConnect.isRegistered = true;
            UdpConnect.isDebug = false;
            this.Close();
            timer.Stop();
        }
    }
}
