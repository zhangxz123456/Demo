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
using System.Windows.Threading;
using MoviePlayer.Class;

namespace MoviePlayer.Forms
{
    /// <summary>
    /// Setting.xaml 的交互逻辑
    /// </summary>
    public partial class Data : Window
    {
        public  DispatcherTimer timer = new DispatcherTimer();
        CheckBox[] checkBoxEvEffect;           //环境特效 
        CheckBox[] checkBoxChairEffect;        //座椅特效
      
        public Data()
        {
            InitializeComponent();
            changeWinDataLanguage();
            checkBoxInit();
            changeDOF();
            timerInit();            
        }


        /// <summary>
        /// checkBox控件数组初始化
        /// </summary>
        private void checkBoxInit()
        {            
            checkBoxEvEffect = new CheckBox[8] { evLightning, evWind, evBubble, evFog, evFire, evSnow, evLaser, evRain };
            checkBoxChairEffect = new CheckBox[8] { cCA, cCB, cSmell, cVibration, cSweepLeg, cSprayWater, cSprayAir,cPushBack };
        }


        /// <summary>
        /// 实现语言切换
        /// </summary>
        private void changeWinDataLanguage()
        {
            if ("CN".Equals(MainWindow.PlayLanguage))
            {
                Title = "数据";
                //环境特效
                evLightning.Content = "闪电";
                evWind.Content = "刮风";
                evBubble.Content = "泡泡";
                evFog.Content = "烟雾";
                evFire.Content = "火焰";
                evSnow.Content = "下雪";
                evLaser.Content = "激光";
                evRain.Content = "下雨";
                //座椅特效
                cSmell.Content = "气味";
                cVibration.Content = "震动";
                cSweepLeg.Content = "扫腿";
                cSprayWater.Content = "喷水";
                cSprayAir.Content = "喷气";
                cPushBack.Content = "推背";
            }
        }

        private void changeDOF()
        {
            //判断软件是两自由度还是三自由度
            if ("3DOF".Equals(MainWindow.PlayDOF))
            {
                this.textBox1.Visibility = Visibility.Visible;
                label2.Visibility = Visibility.Visible;
            }
        }

        private void timerInit()
        {
            
            if (timer.IsEnabled == false)
            {
                timer.Interval = TimeSpan.FromSeconds(0.01);
                timer.Tick += new EventHandler(timerMovie_tick);
                timer.Start();
            }
        }

        private void timerMovie_tick(object sender, EventArgs e)
        {     
            showData();
        }


        private void showData()
        {
            int ii;
            
            if ("5D".Equals(MainWindow.PlayType))
            {

                ii = (int)Math.Round((UserControlClass.MPPlayer.Position.TotalSeconds * 2400), 0);
                ii = (int)Math.Round((ii / 50.0), 0);
            }
            else
            {
                ii = (int)Math.Round((UdpConnect.TimeCode * 2400), 0);
                ii = (int)Math.Round((ii / 50.0), 0);
                label.Content = "TimeCode: " + UdpConnect.strLongTimeCode;
            }

            textBox3.Text = ii.ToString();

            try
            {
                this.textBox.Text = Module.actionFile[3 * ii].ToString();
                this.textBox1.Text = Module.actionFile[3 * ii + 1].ToString();
                this.textBox2.Text = Module.actionFile[3 * ii + 2].ToString();

                label11.Height = int.Parse(textBox.Text) * 115 / 255;
                label22.Height = int.Parse(textBox1.Text) * 115 / 255;
                label33.Height = int.Parse(textBox2.Text) * 115 / 255;

                Boolean[] ev = new Boolean[8];
                Boolean[] cEffect = new Boolean[8];

                for (int i = 0, n = 1; i < 8; i++)
                {
                    ev[i] = ((Module.effectFile[2 * ii] & n) == 0 ? false : true);
                    n = n << 1;
                    if (ev[i] == true)
                    {
                        checkBoxEvEffect[i].IsChecked = true;
                    }
                    else
                    {
                        checkBoxEvEffect[i].IsChecked = false;
                    }
                }

                for (int i = 0, n = 1; i < 8; i++)
                {
                    cEffect[i] = ((Module.effectFile[2 * ii + 1] & n) == 0 ? false : true);
                    n = n << 1;
                    if (cEffect[i] == true)
                    {
                        checkBoxChairEffect[i].IsChecked = true;
                    }
                    else
                    {
                        checkBoxChairEffect[i].IsChecked = false;
                    }
                }

            }
            catch
            {
                this.textBox.Text = "0";
                this.textBox1.Text = "0";
                this.textBox2.Text = "0";

                label11.Height = 0;
                label22.Height = 0;
                label33.Height = 0;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            timer.Stop();
        }
    }
}
