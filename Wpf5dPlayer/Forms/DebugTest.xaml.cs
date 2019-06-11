using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace MoviePlayer.Forms
{
    /// <summary>
    /// DebugTest.xaml 的交互逻辑
    /// </summary>
    public partial class DebugTest : Window
    {
        Image[] imageNum;                 //缸
        Image[] imageEvEffect;           //环境特效 
        Image[] imageChairEffect;        //座椅特效


        byte[] dataNum = new byte[3];
        byte[] dataEvEffect = new byte[8];
        byte[] dataChairEffect = new byte[8];

        DispatcherTimer timer = null;

        public DebugTest()
        {
            InitializeComponent();
            UdpConnect.isDebug = true;
            changPicture();
            changDOF();
            controlInit();
            timerInit();
        }


        /// <summary>
        /// image实现更改图片函数
        /// </summary>
        /// <param name="picName">传入的文件名与上一级目录</param>
        /// <returns></returns>
        private BitmapImage funcChangPicture(string picName)
        {
            string picNamePath = MainWindow.playerPath + @"\Images\" + picName;
            FileInfo finfo = new FileInfo(picNamePath);
            BitmapImage bitImage = null;
            if (finfo.Exists)
            {
                bitImage = new BitmapImage(new Uri(picNamePath, UriKind.Absolute));
            }
            return bitImage;
        }

        /// <summary>
        /// 改变调试界面显示
        /// </summary>
        private void changPicture()
        {            
            if ("CN".Equals(MainWindow.PlayLanguage))
            {
                if ("4DM".Equals(MainWindow.PlayType))
                {                    
                    imageTop.Source = funcChangPicture(@"CN\"+"4dmTopCN.jpg");
                }
                else
                {                    
                    imageTop.Source = funcChangPicture(@"CN\"+"5dTopCN.jpg");
                }
                imageTop01.Source = funcChangPicture(@"CN\" + "top01CN.jpg");
                imageTop02.Source = funcChangPicture(@"CN\" + "top02CN.jpg");
                imageTop03.Source = funcChangPicture(@"CN\" + "top03CN.jpg");
                imageUp.Source = funcChangPicture(@"CN\" + "upCN.jpg");
                imageDown.Source = funcChangPicture(@"CN\" + "downCN.jpg");

                //座椅特效                
                imageSmell.Source= funcChangPicture(@"CN\" + "smellCN.jpg");
                imageVibration.Source= funcChangPicture(@"CN\" + "vibrationCN.jpg");
                imageSweepLep.Source= funcChangPicture(@"CN\" + "sweeplegCN.jpg");
                imageSprayAir.Source= funcChangPicture(@"CN\" + "sprayairCN.jpg");
                imageSprayWater.Source= funcChangPicture(@"CN\" + "spraywaterCN.jpg");
                imagePushBack.Source= funcChangPicture(@"CN\" + "pushbackCN.jpg");

                //环境特效               
                imageLightning.Source = funcChangPicture(@"CN\" + "lightningCN.jpg");
                imageWind.Source = funcChangPicture(@"CN\" + "windCN.jpg");
                imageBubble.Source = funcChangPicture(@"CN\" + "bubbleCN.jpg");
                imageFog.Source = funcChangPicture(@"CN\" + "fogCN.jpg");
                imageFire.Source = funcChangPicture(@"CN\" + "fireCN.jpg");
                imageSnow.Source = funcChangPicture(@"CN\" + "snowCN.jpg");
                imageLaser.Source = funcChangPicture(@"CN\" + "laserCN.jpg");
                imageRain.Source= funcChangPicture(@"CN\" + "rainCN.jpg");
            }
            else
            {
                if("5D".Equals(MainWindow.PlayType))
                {                    
                    imageTop.Source= funcChangPicture(@"EN\" + "5dTopEN.jpg");
                }
            }
        }


        /// <summary>
        /// 更改自由度的数据
        /// </summary>
        private void changDOF()
        {
            if("2DOF".Equals(MainWindow.PlayDOF))
            {
                dataNum[0] = 127;
                dataNum[1] = 127;
                dataNum[2] = 127;
            }
        }

        private void controlInit()
        {
            imageNum = new Image[6] { imageNo1,imageNo2,imageNo3,imageNo4,imageNo5,imageNo6};
            imageEvEffect = new Image[8] { imageLightning, imageWind,imageBubble,imageFog,imageFire,imageSnow,imageLaser,imageRain};
            imageChairEffect = new Image[8] { imageCA,imageCB,imageSmell,imageVibration,imageSweepLep,imageSprayWater,imageSprayAir,imagePushBack };
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
            byte eEffect = 0;
            byte cEffect = 0;
            byte dataNumOne = 0;                  //1号缸的数据
            byte dataNumTwo = 0;                  //2号缸的数据 
            byte dataNumThree = 0;                //3号缸的数据

            byte[] data;
            byte[] array;          //data+addr+len 
            byte[] Data;           //最终发送的数据

            dataNumOne = (byte)(dataNum[2] * MainWindow.PlayHeight);
            dataNumTwo = (byte)(dataNum[1] * MainWindow.PlayHeight);
            dataNumThree = (byte)(dataNum[0] * MainWindow.PlayHeight);

            for (int i = 0; i < 8; i++)
            {
                eEffect += dataEvEffect[i];
                //label9.Content = eEffect.ToString();
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

        /// <summary>
        /// 点击对应的缸改变背景颜色
        /// </summary>
        /// <param name="i"></param>
        private void changeBackground(int i)
        {
            if (imageNum[i].Opacity == 1)
            {
                imageNum[i].Opacity = 0.7;
                dataNum[i] = 255;
            }
            else
            {
                imageNum[i].Opacity = 1;
                dataNum[i] = 0;
            }
        }

        private void changeEvImage(int i)
        {
            if (imageEvEffect[i].Opacity == 1)
            {
                imageEvEffect[i].Opacity = 0.5;
                dataEvEffect[i] = (Byte)Math.Pow(2, i);
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
                dataChairEffect[i] = (Byte)Math.Pow(2, i);
            }
            else
            {
                imageChairEffect[i].Opacity = 1;
                dataChairEffect[i] = 0;
            }
        }

        private void imageUp_MouseUp(object sender, MouseButtonEventArgs e)
        {
            dataNum[0] = 255;
            dataNum[1] = 255;
            dataNum[2] = 255;
            imageUp.Opacity = 1;
        }

        private void imageDown_MouseUp(object sender, MouseButtonEventArgs e)
        {
            dataNum[0] = 0;
            dataNum[1] = 0;
            dataNum[2] = 0;
            imageDown.Opacity = 1;
        }

        private void imageNo1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeBackground(0);
        }

       

        private void imageNo2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeBackground(1);
        }

        private void imageNo3_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeBackground(2);
        }

        private void imageCA_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeChairImage(0);
        }

        private void imageCB_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeChairImage(1);
        }

        private void imageSmell_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeChairImage(2);
        }

        private void imageVibration_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeChairImage(3);
        }

        private void imageSweepLep_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeChairImage(4);
        }

        private void imageSprayWater_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if("4DM".Equals(MainWindow.PlayType))
            {
                changeChairImage(5);
                changeChairImage(6);
            }
            else
            {
                changeChairImage(5);
            }
        }

        private void imageSprayAir_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeChairImage(6);
        }

        private void imagePushBack_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeChairImage(7);
        }

        private void imageLightning_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeEvImage(0);
        }

        private void imageWind_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeEvImage(1);
        }

        private void imageBubble_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeEvImage(2);
        }

        private void imageFog_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeEvImage(3);
        }

        private void imageFire_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeEvImage(4);
        }

        private void imageSnow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeEvImage(5);
        }

        private void imageLaser_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeEvImage(6);
        }

        private void imageRain_MouseUp(object sender, MouseButtonEventArgs e)
        {
            changeEvImage(7);
        }


        private void button_Click(object sender, RoutedEventArgs e)
        {

            UdpConnect.isDebug = false;
            this.Close();
            timer.Stop();
        }

        private void button_MouseMove(object sender, MouseEventArgs e)
        {

            //button.Background = new ImageBrush
            //{
            //    ImageSource = new BitmapImage(new Uri(" MoviePlayer;component/Images/close.png"))
            //};
        }

        private void imageUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            imageUp.Opacity = 0.5;
        }

        private void imageDown_MouseDown(object sender, MouseButtonEventArgs e)
        {
            imageDown.Opacity = 0.5;
        }

        private void label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UdpSend.SendReset();
            UdpConnect.isDebug = false;
            this.Close();
            timer.Stop();
        }
    }
}
