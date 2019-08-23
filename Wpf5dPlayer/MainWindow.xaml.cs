using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using MoviePlayer.Forms;
using System.Net.Sockets;

namespace MoviePlayer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
       
        DispatcherTimer timerJudge = null;            //验证播放器类型与软件是否注册 
        DispatcherTimer timerFilm = null;             //监控排片时间与当前时间
        Module module = new Module();
        FilmSetting fs = new FilmSetting();
        Data winData;
        Player winPlayer;
        UdpInit myUdpInit = new UdpInit();
        public static string PlayType;               //播放器类型 4DM为4DM播放器 5D为5D播放器
        public static string PlayLanguage;           //播放器语言 EN为英文播放器 CN为中文播放器   
        public static string PlayDOF;                //自由度类型 2DOF为两自由度播放器 3DOF为三自由度播放器
        public static string playerPath;
        public static double PlayHeight;             //高度数据  1为原始数据 0.9为百分90行程数据
        public static string PlayProjector;          //设置播放画面显示在主屏还是副屏  参数分别为0或1
        private int a;                               //验证软件正常打开后发复位指令（只发第一次）
        Socket tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(
            int uAction,
            int uParam,
            string lpvParam,
            int fuWinIni
       );

        public MainWindow()
        {
            InitializeComponent();
            //SystemParametersInfo(20, 0, @"c:\Windows\shuqee.bmp", 0x2);   
            getPlayerPath();
            SystemParametersInfo(20, 0, playerPath + @"\Images\" + "shuqee.bmp", 0x2);          
            Module.GetNowTime();
            Module.readUuidFile();                
            ReadType();
            SelectMode();
            myUdpInit.udpInit();
            Thread.Sleep(2000);
            timerInit();
            changeBackgroundImage();
            changeLanguage();
            timerFilmInit();
            //UdpSend.SendReset();
            //Thread.Sleep(1000);            
        }

        /// <summary>
        /// 获取软件当前路径的上一级目录的路径名
        /// </summary>
        private void getPlayerPath()
        {
            playerPath = AppDomain.CurrentDomain.BaseDirectory.Substring(0,AppDomain.CurrentDomain.BaseDirectory.Length-5);
        }


        /// <summary>
        /// 更改主界面背景图片
        /// </summary>
        private void changeBackgroundImage()
        {            
            if ("5D".Equals(PlayType))
            {
                if("CN".Equals(PlayLanguage))
                {
                    functionChangeImage(@"CN\"+"5dCN.png");
                }
                else
                {
                    functionChangeImage(@"EN\"+"5dEN.png");
                }
                
            }
            else
            {
                if ("CN".Equals(PlayLanguage))
                {
                    functionChangeImage(@"CN\"+"4dmCN.png");
                }
                else
                {
                    functionChangeImage(@"EN\"+"4dmEN.png");
                }
            }
        }


        /// <summary>
        /// image控制更改图片实现函数
        /// </summary>
        /// <param name="strImageName"></param>
        private void functionChangeImage(string strImageName)
        {

            FileInfo finfo = new FileInfo(playerPath + @"\Images\" + strImageName);

            if (finfo.Exists)
            {
                //绝对路径            
                BitmapImage imagetemp = new BitmapImage(new Uri(playerPath + @"\Images\" + strImageName, UriKind.Absolute));
                //相对路径
                //BitmapImage imagetemp = new BitmapImage(new Uri("Winter.jpg", UriKind.Relative));
                Image1_png.Source = imagetemp;
            }
        }

       
        /// <summary>
        /// 改变软件语言
        /// </summary>
        private void changeLanguage()
        {
            if ("CN".Equals(PlayLanguage))
            {
                btnSetting.Content = "设置";
                btnHelp.Content = "帮助";
                btnPlayer.Content = "播放";
                btnDebug.Content = "调试";
                btnData.Content = "数据";
                btnRegister.Content = "注册";                
            }
        }

        private void timerJudge_tick(object sender,EventArgs e)
        {       
            if (UdpConnect.connectFlag == false)  //未与中控板连接    
            {
                btnPlayer.IsEnabled = false;
                btnPlayer.FontSize = 24;
                if ("CN".Equals(PlayLanguage))
                {
                    btnPlayer.Content = "未连接";
                }
                else
                {
                    btnPlayer.Content = "Unconnected";
                }
            }
            else      //与中控板已连接
            {
                if (UdpConnect.isRegistered == false)  //软件到期或者未注册        
                {
                    btnPlayer.IsEnabled = false;
                    btnPlayer.FontSize = 24;
                    if ("CN".Equals(PlayLanguage))
                    {
                        btnPlayer.Content = "未注册";
                    }
                    else
                    {
                        btnPlayer.Content = "UnRegistered";
                    }
                }
                else  //软件正常打开            
                {
                    btnPlayer.IsEnabled = true;
                    btnPlayer.FontSize = 40;
                    if (a == 0)
                    {
                        UdpSend.SendReset();
                        a = 1;
                    }
                    if ("CN".Equals(PlayLanguage))
                    {
                        btnPlayer.Content = "播放";
                    }
                    else
                    {
                        btnPlayer.Content = "Play";
                    }

                    if (Module.hintShow == true)
                    {
                        if ("CN".Equals(PlayLanguage))
                        {
                            label1.Content = "提示：软件还有" + Module.deadlineDay + "天到期";
                        }
                        else
                        {
                            label1.Content = "Tips: The software expires in " + Module.deadlineDay + " days";
                        }
                    }
                    if ("4DM".Equals(PlayType))
                    {
                        label.Content = "TimeCode: " + UdpConnect.strTimeCode;
                    }
                }
            }

        }

        private void timerFilm_tick(object sender, EventArgs e)
        {
            if ("4DM".Equals(PlayType))
            {                
                string s = DateTime.Now.ToShortTimeString().ToString();
                //compareDateTime(FilmSetting.memberData[1].Start);
                for (int i = 0; i < 10; i++)
                {
                    if (FilmSetting.memberData[i] != null)
                    {
                        //if (s.Equals(FilmSetting.memberData[i].Start))
                        if(compareDateTime(FilmSetting.memberData[i].Start,FilmSetting.memberData[i].End))
                        {
                            //MessageBox.Show("开始" + s + FilmSetting.memberData[i].FullMovieName);
                            labCurrentFilm.Content = "当前影片：" + FilmSetting.memberData[i].MovieName;
                            if (i < 9)
                            {
                                labNextFilm.Content = "下一场影片：" + FilmSetting.memberData[i + 1].MovieName;
                            }
                            Module.readDefultFile(FilmSetting.memberData[i].FullMovieName);
                        }
                        if (i < 9)
                        {
                            if (compareDateTime(FilmSetting.memberData[i].End,FilmSetting.memberData[i+1].Start))
                            {
                                //MessageBox.Show("结束" + s);
                                labCurrentFilm.Content = "当前影片：";
                                Module.actionFile = null;
                                Module.shakeFile = null;
                                Module.effectFile = null;
                            }
                        }
                    }
                }
            }
        }

        private bool compareDateTime(string str1,string str2)
        {
            try
            {
                DateTime dt1 =new DateTime();
                DateTime dt2 =new DateTime();
                DateTime dt3 =new DateTime();

                if (str1 != "" && str2 != "")
                {

                    dt1 = Convert.ToDateTime(str1);
                    dt2 = Convert.ToDateTime(str2);
                    dt3 = DateTime.Now;
                }
                if (str1 != "" && str2 == "")
                {
                    dt1 = Convert.ToDateTime(str1);
                    dt3 = DateTime.Now;
                    return DateTime.Compare(dt3, dt1) >= 0;
                }
                return DateTime.Compare(dt3, dt1) >= 0 && DateTime.Compare(dt3, dt2) < 0;
               
            }
            catch (Exception)
            {
                //throw;
                return false;
            }
            
        }

        private void timerInit()
        {
            //udp程序启动定时器
            timerJudge = new DispatcherTimer();
            timerJudge.Interval = TimeSpan.FromSeconds(0.01);   //定时器周期为10ms 
            timerJudge.Tick += new EventHandler(timerJudge_tick);
            timerJudge.Start();
        }

        private void timerFilmInit()
        {
            timerFilm = new DispatcherTimer();
            timerFilm.Interval = TimeSpan.FromSeconds(10);
            timerFilm.Tick += new EventHandler(timerFilm_tick);
            timerFilm.Start();
        }
        

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            //Setting winSetting = new Setting();
            //winSetting.ShowDialog();

            FilmSetting fs = new FilmSetting();
            fs.ShowDialog();

            //Projector pj = new Projector();
            //pj.ShowDialog();
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {           
            VersionInformation vr = new VersionInformation();
            vr.ShowDialog();
        }

        private void btnData_Click(object sender, RoutedEventArgs e)
        {
            winData = new Data();
            winData.ShowDialog();
        }

        private void btnPlayer_Click(object sender, RoutedEventArgs e)
        {
            if ("5D".Equals(PlayType))
            {
                if (winPlayer == null)
                {
                    winPlayer = new Player();
                    winPlayer.Show();
                }
                else
                {
                    winPlayer.Show();
                }
            }
        }

        private void btnDebug_Click(object sender, RoutedEventArgs e)
        {
            if (UdpConnect.isRegistered == true)
            {
                DebugTest winDebug = new DebugTest();
                //winDebug.Show();
                winDebug.ShowDialog();
                //winDebug.ResizeMode = ResizeMode.NoResize;
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            Register winRegister = new Register();            
            winRegister.ShowDialog();           
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }


        /// <summary>
        /// 读取配置文件Type.xml,获取播放器类型
        /// </summary>
        private void ReadType()
        {            
            FileInfo finfo = new FileInfo(playerPath + @"\XML\" + "Type.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(playerPath + @"\XML\" + "Type.xml");
                XmlNode childNodes = xmlDoc.SelectSingleNode("Type");
                XmlElement element = (XmlElement)childNodes;
                PlayType = element["Style"].InnerText;
                PlayLanguage = element["Language"].InnerText;
                PlayDOF = element["DOF"].InnerText;
                PlayHeight = Double.Parse(element["Height"].InnerText);
                PlayProjector = element["Projector"].InnerText;
            }
        }

        private void SelectMode()
        {
            if ("4DM".Equals(PlayType))
            {
                Module.readDefultFile();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //ReadType();
            //SelectMode();
        }

        private void labClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PlayDOF = "";
            UdpSend.movieStop = true;
            UdpSend.SendReset();
            System.Windows.Application.Current.Shutdown();
        }

        private void openProjector()
        {
            byte[] data = {0x30,0x30,0x50,0x4F,0x4E,0x0D};
            //tcpClient.Connect(UdpInit.transformIP(""));
        }

        private void closeProjector()
        {
            byte[] data = { 0x30, 0x30, 0x50, 0x4F, 0x46, 0x0D };
            
        }

        
    }
}
