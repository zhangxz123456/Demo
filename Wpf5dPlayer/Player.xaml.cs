using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Timers;
using System.Windows.Threading;
using AxShockwaveFlashObjects;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using MoviePlayer.Class;
using DevComponents.WpfRibbon;
using System.Xml;
using System.Xml.XPath;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Interop;
using Microsoft.Win32;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MoviePlayer.Forms;

namespace MoviePlayer
{

    /// <summary>
    /// 播放器的声音状态
    /// </summary>
    public enum SoundStatus
    {
        /// <summary>
        /// 声音
        /// </summary>
        Sound,
        /// <summary>
        /// 静音
        /// </summary>
        Mute
    }
    ///<summary>
    ///播放器的播放状态
    ///</summary>
    ///
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
    /// <summary>
    /// 播放器的计时器状态
    /// </summary>
    public enum TimeStatus
    {
        /// <summary>
        /// 系统时间
        /// </summary>
        SystemTime,
        /// <summary>
        /// 文件时间
        /// </summary>
        FileTime
    }
    /// <summary>
    /// 视频\摄像屏幕显示与隐藏
    /// </summary>
    public enum PlayCamera
    {
        /// <summary>
        /// 视频屏幕
        /// </summary>
        inkMediaPlay,
        /// <summary>
        /// 摄像屏幕
        /// </summary>
        inkCameraPlay
    }
    /// <summary>
    /// Player.xaml 的交互逻辑
    /// </summary>
    public partial class Player : Window
    {
        #region   变量声明
        //勾子管理类   
        public KeyboardHookLib _keyboardHook = null;
        /// <summary>
        /// 列表-名称
        /// </summary>
        private string ListNodeName = "";
        private string[] ListNodeNameArr ;
        /// <summary>
        /// 列表-路径
        /// </summary>
        private string ListNodePath = "";
        private string[] ListNodePathArr ;
        /// <summary>
        /// 列表-长度
        /// </summary>
        private string ListNodeLength = "";
        /// <summary>
        /// 列表-播放时刻
        /// </summary>
        private string ListNodeTime = "";      
        /// <summary>
        /// 播放模式选择
        /// </summary>
        private string ModePlayTag;

        /// <summary>
        /// 选中的影片的全路径名
        /// </summary>
        public static  string fullPathName = "";
        /// <summary>
        /// 播放文件序号（上一部，下一部）
        /// </summary>
        private int number = 0;
        ///<summary>
        ///视频\摄像屏幕
        /// </summary>
        private PlayCamera PCink = PlayCamera.inkMediaPlay;
        ///<summary>
        ///播放器声音状态
        /// </summary>
        private SoundStatus SSStatus = SoundStatus.Sound;
        ///<summary>
        ///播放器计时器状态
        /// </summary>
        private TimeStatus tSStatus = TimeStatus.SystemTime;
        /// <summary>
        /// 播放器声音数值
        /// </summary>
        double sound = 0;
        /// <summary>
        /// 播放视频的总长时间
        /// </summary>
        private string MediaCountTime = string.Empty;
        /// <summary>
        /// 播放时间的响应的委托
        /// </summary>
        private delegate void DeleSetPosition();
        /// <summary>
        /// 用于定时计算，添加播放时间
        /// </summary>
        private System.Timers.Timer Timing = new System.Timers.Timer();
        /// <summary>
        /// 播放文件的总长时间
        /// </summary>
        private int MaxLen = 0;
        ///// <summary>
        ///// 播放flash文件
        ///// </summary>
        //public AxShockwaveFlash FlashShock;
        /// <summary>
        /// 加载flash文件【AxShockwaveFlash】的类
        /// </summary>
        public WindowsFormsHost FlashFormHost;
        /// <summary>
        /// 初始化视频文件名【首次打开程序时使用】
        /// </summary>
        private object InitPlayerPath = System.Windows.Application.Current.Resources["InitArgs"] ?? null;
        /// <summary>
        /// 标识当前的时间滑动条是否可以操作
        /// </summary>
        private bool IsChangeValue = false;
        ///// <summary>
        ///// 摄像头调用类
        ///// </summary>
        //public CapPlayer CameraPlayer;
        /// <summary>
        /// 系统时间显示
        /// </summary>
        private DispatcherTimer DTimer;      
        /// <summary>
        /// 标识是否记忆播放
        /// </summary>
        private bool memoryPlay = true;
        ////定时器，定时修改ImageInfo中各属性，从而实现动画效果
        //public DispatcherTimer timer = new DispatcherTimer();       
        public TimeStatus TSStatus
        {
            get
            {
                return tSStatus;
            }
            set
            {
                tSStatus = value;
            }
        }
        ///// <summary>
        ///// 第二个窗体
        ///// </summary>
        #endregion
        public int count;
        public bool timerStart;       
    
        public Player()
        {
            InitializeComponent();
            //定义系统时间计时器
            //初始化按指定优先级处理计时器事件
            DTimer = new DispatcherTimer(DispatcherPriority.Normal);
            //设置计时器的时间间隔
            DTimer.Interval = TimeSpan.FromMilliseconds(1000);
            //超过计时器间隔时发生
            DTimer.Tick += new EventHandler(timer_Tick);
            //系统时间显示，线程开始
            DTimer.Start();
            // lv.ContextMenu=getLVMenu();
            UserControlClass.NullBorderWin(this);
            //UserControlClass.RegRun("VideoPlayer", true);
            //安装钩子，屏蔽键盘切换等按键
            //_keyboardHook = new KeyboardHookLib();
            //JugUser();
            changeWinPlayerLanguage();  
                      
        }

        //中英文切换
        private void changeWinPlayerLanguage()
        {
            if ("CN".Equals(MainWindow.PlayLanguage))
            {
                MenuFile.Header = "    文 件    ";
                MenuClearFiles.Header = "清空播放列表";
                MenuPlay.Header = "    播 放    ";
                ModePlay.Header = "   播放模式  ";
                MenuRepeat.Header = " 重复播放 ";
                MenuDefault.Header = " 默认播放 ";
                MenuLoop.Header = " 顺序播放 ";
                MenuCloseComputer.Header = "   关 机  ";
                MenuHelp.Header = "   帮 助  ";
                MenuShowData.Header = "  显示数据  ";
                MenuClose.Header = "   X         ";               
                label.Content = "5D影院播放系统";               
                labelDisplayListName.Content = "播放列表";
            }
        }
       

        ///<summary>
        ///定义系统时间显示事件
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            tbTime.Text = DateTime.Now.Hour.ToString("D2") + " : " + DateTime.Now.Minute.ToString("D2") + " : " + DateTime.Now.Second.ToString("D2");
            
            //中控指令
            //if(Module.controlCommand=="play")
            //{
                
            //    btnPlayClickFun();
            //    Module.controlCommand = "";                                                                                                                                                                                                     
            //}
            //if(Module.controlCommand=="pause")
            //{
            //    btnPlayClickFun();
            //    Module.controlCommand = "";
            //}
            //if (Module.controlCommand == "stop")
            //{
            //    btnStopClickFun();
            //    Module.controlCommand = "";
            //}
        }

        /// <summary>
        /// 加载窗体事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerWin_Loaded(object sender, RoutedEventArgs e)
        {
            UserControlClass.MPPlayer.MediaEnded += new EventHandler (MPPlayer_MediaEnded);            
            UserControlClass.MPPlayer.MediaOpened += new EventHandler(MPPlayer_MediaOpened);     
            Timing.Elapsed += new ElapsedEventHandler(Tim_Elapsed);
            ChangeShowPlay();
            ChangeShowMute();
            ChangeShowTime();
            ChangeshowInk();
            ChangeModePlay();           
            ReadVolume();
            SelectXml();           
            NewLoaded();
        }

        /// <summary>
        /// 计时器响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tim_Elapsed(object sender, ElapsedEventArgs e)
        {
            SliPlayerTime.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DeleSetPosition(SetPosition));
        }

        /// <summary>
        /// 打开播放器响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MPPlayer_MediaOpened(object sender, EventArgs e)
        {
            //UdpSend.range = int.Parse(textBox.Text);        //震动幅度
            //UdpSend.rate = int.Parse(textBox2.Text);        //震动频率
            if (UserControlClass.MPPlayer.NaturalDuration.HasTimeSpan)
            {
                MaxLen = (int)Math.Round(UserControlClass.MPPlayer.NaturalDuration.TimeSpan.TotalSeconds);
                SliPlayerTime.Maximum = MaxLen;
                SliPlayerTime.Minimum = 0;
                tbSeek.Text = "00:00:00";
            }
            //画刷，把视频文件绘制在指定的矩形中
            VideoDrawing drawing = new VideoDrawing();
            //描述矩形的宽度，高度和位置
            Rect rect = new Rect(0.0, 0.0, UserControlClass.MPPlayer.NaturalVideoWidth, UserControlClass.MPPlayer.NaturalVideoHeight);
            //设置可在其中绘制视频的矩形区域
            drawing.Rect = rect;
            //设置与绘制关联的媒体播放器
            drawing.Player = UserControlClass.MPPlayer;
            //用Drawing绘制图形
            DrawingBrush brush = new DrawingBrush(drawing);
            //UserControlClass.sc2.FInkCanvas_Player.Background = brush;
            NewOpenPlay();
            //timerInit();
            if (timerStart == false)
            {
                timerInit();
            }
            else
            {
                Module.timerMovie.Start();
            }

            //int realityWidth = UserControlClass.MPPlayer.NaturalVideoWidth> Screen.PrimaryScreen.Bounds.Width? Screen.PrimaryScreen.Bounds.Width : UserControlClass.MPPlayer.NaturalVideoWidth;
            int realityWidth = Screen.PrimaryScreen.Bounds.Width;
            //int realityHeight = UserControlClass.MPPlayer.NaturalVideoHeight > Screen.PrimaryScreen.Bounds.Height ? Screen.PrimaryScreen.Bounds.Height*9/16 : UserControlClass.MPPlayer.NaturalVideoHeight;
            int realityHeight = realityWidth * UserControlClass.MPPlayer.NaturalVideoHeight / UserControlClass.MPPlayer.NaturalVideoWidth;
            Screen[] sc1 = System.Windows.Forms.Screen.AllScreens;//获取当前所有显示器的数组

            //如果有多个屏幕，就定义secondwindow的位置，让他在第二个屏幕全屏显示                                                      
            if (sc1.Length > 1)
            {
                //var workingArea = sc1[1].Bounds; 
                var workingArea = sc1[0].WorkingArea;
                UserControlClass.NullBorderWin(UserControlClass.sc2.returnWinSc2(), workingArea.Left, workingArea.Top, workingArea.Width, workingArea.Height);
            }
            else
            {
                UserControlClass.NullBorderWin(UserControlClass.sc2.returnWinSc2(), realityWidth, realityHeight);
            }

        }

        /// <summary>
        /// 定时器初始化,用于调用发送数据
        /// </summary>
        private void timerInit()
        {
            if (Module.timerMovie == null)
            {
                Module.timerMovie = new DispatcherTimer();
            }
            if (Module.timerMovie.IsEnabled == false)
            {
                Module.timerMovie.Interval = TimeSpan.FromSeconds(0.01);
                Module.timerMovie.Tick += new EventHandler(timerMovie_tick);
                Module.timerMovie.Start();
            }
            timerStart = true;
           
        }

        private void timerMovie_tick(object sender, EventArgs e)
        {
            Debug.WriteLine("player发数据");
            UdpSend.SendWrite(UserControlClass.MPPlayer.Position.TotalSeconds);
            //UdpSend.SendWrite();
            if (UserControlClass.MPPlayer.Position.TotalSeconds == 0)
            {
                count++;
                Debug.WriteLine("player发数据" + count);
                if (count == 100)
                {
                    Module.timerMovie.IsEnabled = false;
                    Module.timerMovie.Stop();
                   //System.Windows.MessageBox.Show("停止");
                }

            }
        }

        /// <summary>
        /// 播放完成响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MPPlayer_MediaEnded(object sender, EventArgs e)
        {
            //System.Windows.MessageBox.Show("停止");
            SliPlayerTime.Value = 0;
            UserControlClass.MPPlayer.Position = new TimeSpan(0, 0, 0);
            UserControlClass.MPPlayer.Stop();
            UserControlClass.MSStatus = MediaStatus.Play;
            TSStatus = TimeStatus.FileTime;
            ChangeShowPlay();
            //重复播放
            if (ModePlayTag == "RepeatPlay" || ModePlayTag == "LoopPlay")
            {
                NextPlayer();
                string Currentname = UserControlClass.FileName;
                //string aa = fullPathName;
                ListPlay(Currentname);
                ChangeShowTime();
                ChangeShowPlay();
            }
            else
            {
                UserControlClass.sc2.Close();
            }
            Module.timerMovie.Stop();
            UdpSend.SendReset();
        }

        /// <summary>
        /// 点击屏幕播放\暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InkCanvas_Player_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                PlayOrPause();
            }
        }

        /// <summary>
        /// 播放或暂停
        /// </summary>
        public void PlayOrPause()
        {

            if (!string.IsNullOrEmpty(UserControlClass.FileName))
            {

                if (!UserControlClass.FileName.Contains(".swf") && UserControlClass.FileName != "Camera")
                {
                    if (UserControlClass.MSStatus == MediaStatus.Play)
                    {
                        UserControlClass.MSStatus = MediaStatus.Pause;
                        Pause();
                    }
                    else
                    {
                        UserControlClass.MSStatus = MediaStatus.Play;
                        Play();
                    }
                }
            }
            ChangeShowPlay();
        }
        /// <summary>
        /// 时间轴控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SliPlayerTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (!string.IsNullOrEmpty(UserControlClass.FileName) && IsChangeValue)
                {
                    int PlayerTime = (int)Math.Round(e.NewValue);
                    if (UserControlClass.FileName.Contains(".swf"))
                    {
                    }
                    else
                    {
                        TimeSpan span = new TimeSpan(0, 0, PlayerTime);
                        UserControlClass.MPPlayer.Position = span;
                    }
                }
            }
            catch (Exception ChangeEx)
            {
                System.Windows.MessageBox.Show(ChangeEx.ToString());
            }
        }

        /// <summary>
        /// 时间轴控制（按下）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SliPlayerTime_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsChangeValue = true;
        }

        /// <summary>
        /// 时间轴控制（松开）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SliPlayerTime_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsChangeValue = false;
        }
        
        /// <summary>
        /// 静音控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgMute_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (SSStatus == SoundStatus.Sound)
                {
                    SSStatus = SoundStatus.Mute;
                    sound = UserControlClass.MPPlayer.Volume;
                    UserControlClass.MPPlayer.Volume = 0.0;
                }
                else
                {
                    SSStatus = SoundStatus.Sound;
                    UserControlClass.MPPlayer.Volume = sound;
                }
                ChangeShowMute();
            }
        }

        /// <summary>
        /// 声音轴控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SldVolumn_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SSStatus == SoundStatus.Sound)
            {
                UserControlClass.MPPlayer.Volume = e.NewValue;
            }
            else
            {
                sound = e.NewValue;
            }
        }

      
        /// <summary>
        /// 关闭播放器事件
        ///【要关闭窗体，计时器，播放器工具，清除相关摄像设备】
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerWin_Closed(object sender, EventArgs e)
        {
            
            //UserControlClass.MPPlayer.Close();
            //UserControlClass.MSStatus = MediaStatus.Pause;
            //UserControlClass.FileName = string.Empty;
            //UserControlClass.sc2.FInkCanvas_Player.Children.Clear();
            //UserControlClass.sc2.FInkCanvas_Camera.Children.Clear();
            //this.Close();

            // ManageTaskManager(0);
            // System.Windows.Application.Current.Shutdown();           
            //System.Environment.Exit(System.Environment.ExitCode);
            //System.Windows.Forms.Application.Exit();
            //System.Environment.Exit(0);
        }
        /// <summary>
        /// 菜单栏--清空播放列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearPlaylists_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RemoveAllFiles();
        }

 
        /// <summary>
        /// ListView控件鼠标双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = ListView.SelectedIndex;
            try
            {
                if (index != -1)
                {
                    ScreenJug();
                    memoryPlay = true;
                    ListPlay(ListView.SelectedItem.ToString());
                    //textBox.Text = ListView.SelectedItem.ToString();
                    UserControlClass.MSStatus = MediaStatus.Pause;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 添加文件（按钮）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgAddFile_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                AddFiles();
            }
        }

        /// <summary>
        /// 删除文件（按钮）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgRemoveFile_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {              
                RemoveFiles();
                //RemoveAllFiles();
            }
        }

        /// <summary>
        /// 选择播放模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeMode_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.MenuItem menu = (System.Windows.Controls.MenuItem)sender;
            ModePlayTag = menu.Tag.ToString();                       
            SaveMode();
            ChangeModePlay();
        }


        /// <summary>
        /// 给选中的菜单项打上勾
        /// </summary>
        /// <param name="modeTag">选中的播放模式</param>
        private void MenuModePlayTick(string modeTag )
        {
            switch (modeTag)
            {
                case "RepeatPlay":
                    MenuRepeat.IsChecked = true;
                    MenuDefault.IsChecked = false;
                    MenuLoop.IsChecked = false;
                    break;
                case "DefaultPlay":
                    MenuRepeat.IsChecked = false;
                    MenuDefault.IsChecked = true;
                    MenuLoop.IsChecked = false;
                    break;
                case "LoopPlay":
                    MenuRepeat.IsChecked = false;
                    MenuDefault.IsChecked = false;
                    MenuLoop.IsChecked = true; 
                    break;
            }
        }

        /// <summary>
        /// 播放器默认大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DefaultSize_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = System.Windows.WindowState.Normal;
        }

        #region  播放列表右键菜单
        /// <summary>
        /// 播放列表鼠标右击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //void itemDelete_Click(object sender, RoutedEventArgs e)
        //{
        //    System.Windows.Controls.MenuItem item = (System.Windows.Controls.MenuItem)sender;
        //    string stringName = item.Tag.ToString();
        //    switch (stringName)
        //    {
        //        case "ListPlay":
        //            int play = ListView.SelectedIndex;
        //            if (play != -1)
        //            {
        //                memoryPlay = false;
        //                ListPlay(ListView.SelectedItem.ToString());
        //            }
        //            break;

        //        case "Delete":
        //            RemoveFiles();
        //            break;

        //        case "DeleteAll":
        //            RemoveAllFiles();
        //            break;
        //    }
        //}

        /// <summary>
        /// 获取播放列表鼠标右击下拉菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //public System.Windows.Controls.ContextMenu getListMenu()
        //{
        //    //System.Windows.Controls.MenuItem itemDelete;
        //    //System.Windows.Controls.ContextMenu contextMenu = new System.Windows.Controls.ContextMenu();

        //    //itemDelete = new System.Windows.Controls.MenuItem();
        //    //itemDelete.Tag = "Delete";
        //    //itemDelete.Header = "  删   除  ";
        //    //itemDelete.Click += new RoutedEventHandler(itemDelete_Click);
        //    //contextMenu.Items.Add(itemDelete);

        //    //itemDelete = new System.Windows.Controls.MenuItem();
        //    //itemDelete.Tag = "DeleteAll";
        //    //itemDelete.Header = "删 除 所 有";
        //    //itemDelete.Click += new RoutedEventHandler(itemDelete_Click);
        //    //contextMenu.Items.Add(itemDelete);

        //    //return contextMenu;
        //}
        #endregion


        /// <summary>
        /// 窗体初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewLoaded()
        {
            try
            {
                if (InitPlayerPath != null && !String.IsNullOrEmpty(InitPlayerPath.ToString().Trim()))
                {
                    string[] paths = InitPlayerPath.ToString().Split('\\');
                    UserControlClass.FileName = paths[paths.Length - 1];

                    if (UserControlClass.FileName.Contains(".swf"))
                    {
                    }
                    else
                    {
                        Open(InitPlayerPath.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        /// <summary>
        /// 计算播放的时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetPosition()
        {
            //System.Windows.MessageBox.Show((count++).ToString());
            if (string.IsNullOrEmpty(UserControlClass.FileName))
            {
                return;
            }
            if (UserControlClass.FileName.Contains(".swf"))
            {
            }
            else
            {
                int num = ((UserControlClass.MPPlayer.Position.Hours * 0xe10) + (UserControlClass.MPPlayer.Position.Minutes * 60)) + UserControlClass.MPPlayer.Position.Seconds;
                //double dbNum =   UserControlClass.MPPlayer.Position.Ticks / 10000000;
                //int intNum = (int)Math.Round(dbNum);
                string str = SetTime(UserControlClass.MPPlayer.Position.Hours) + ":" + SetTime(UserControlClass.MPPlayer.Position.Minutes) + ":" + SetTime(UserControlClass.MPPlayer.Position.Seconds);
                tbSeek.Text = string.Format("{0}/{1}", SetTime(UserControlClass.MPPlayer.Position.Hours) + ":" + SetTime(UserControlClass.MPPlayer.Position.Minutes) + ":" + SetTime(UserControlClass.MPPlayer.Position.Seconds), MediaCountTime);
                SliPlayerTime.Value = num;
                string next = SetTime(UserControlClass.MPPlayer.Position.Hours) + ":" + SetTime(UserControlClass.MPPlayer.Position.Minutes) + ":" + SetTime(UserControlClass.MPPlayer.Position.Seconds);
                if (next.Equals(MediaCountTime))
                {
                   // NextPlayer();
                   // PlayNextPre();
                }
                ChangeShowPlay();
            }
        }


        /// <summary>
        /// 显示与隐藏播放、暂停按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeShowPlay()
        {
            if (!string.IsNullOrEmpty(UserControlClass.FileName))
            {
                switch (UserControlClass.MSStatus)
                {
                    case MediaStatus.Pause:
                        //ImgPlayer.Visibility = Visibility.Hidden;
                        //ImgPause.Visibility = Visibility.Visible;
                        //btnPlay.Content = "Play";
                        FileInfo fo = new FileInfo(MainWindow.playerPath + @"\Images\" + "play.jpg");
                        if (fo.Exists)
                        {
                            Uri uriPlay = new Uri(MainWindow.playerPath + @"\Images\" + "play.jpg");
                            BitmapImage imagePlay = new BitmapImage(uriPlay);
                            Image imgPlay = new Image();
                            imgPlay.Source = imagePlay;
                            btnPlay.Content = imgPlay;
                        }
                        break;
                    case MediaStatus.Play:

                        FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\Images\" + "pause.jpg");
                        if (finfo.Exists)
                        {
                            Uri uriPause = new Uri(MainWindow.playerPath + @"\Images\" + "pause.jpg");
                            BitmapImage imagePause = new BitmapImage(uriPause);
                            Image imgPause = new Image();
                            imgPause.Source = imagePause;
                            btnPlay.Content = imgPause;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 列表播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListPlay(string name)
        {
            //创建打开xml文件的流
            FileInfo fileInfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
            if (fileInfo.Exists)//判断文件是否存在
            {
                XmlDocument xmlDoc = new XmlDocument();
                //加载路径的xml文件
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "List.xml");
                XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");
                string lists = xmlNode.InnerText;
                if (!string.IsNullOrEmpty(lists))
                {
                    XmlNodeList nodes = xmlNode.SelectNodes("List");
                    foreach (XmlNode node in nodes)
                    {
                        XmlElement element = (XmlElement)node;
                        if (name.Equals(element["Name"].InnerText))
                        {
                            fullPathName = element["Path"].InnerText;
                        }
                    }
                }
            }
            //SavePlayTime();
            PCink = PlayCamera.inkMediaPlay;
            ChangeshowInk();
            Microsoft.Win32.OpenFileDialog OpenFile = new Microsoft.Win32.OpenFileDialog();
            OpenFile.FileName = fullPathName;
            UserControlClass.FileName = OpenFile.SafeFileName;
            // System.Windows.Forms.MessageBox.Show( UserControlClass.FileName+"");
            FileInfo finfo = new FileInfo("" + fullPathName + "");
            if (finfo.Exists)
            {
                UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
                UserControlClass.sc2.FInkCanvas_Player.Children.Clear();                
                //播放flash文件判断
                if (UserControlClass.FileName.Contains(".swf"))
                {
                    return;
                }
                else
                {
                    Pause();
                    //播放视频文件
                    if (UserControlClass.MPPlayer.HasVideo)
                    {
                        UserControlClass.MPPlayer.Close();
                    }
                    //ListNodeName
                    ListNodeName = UserControlClass.FileName;
                    Open(fullPathName);
                    //系统时间与播放时间切换
                    TSStatus = TimeStatus.FileTime;
                    ChangeShowTime();
                    //SelectPlayTime();
                    double time = 0;
                    int intTime = 0;
                    if (memoryPlay)
                    {
                        ListNodeTime = "";
                    }
                    if (!string.IsNullOrEmpty(ListNodeTime))
                    {
                        time = double.Parse(ListNodeTime);
                    }
                    intTime = (int)Math.Round(time);
                    TimeSpan span = new TimeSpan(0, 0, intTime);
                    UserControlClass.MPPlayer.Position = span;
                    UserControlClass.MPPlayer.Play();
                }
            }
            else
            {
                UserControlClass.MPPlayer.Close();
                SliPlayerTime.Value = 0;
                UserControlClass.MSStatus = MediaStatus.Pause;
                TSStatus = TimeStatus.SystemTime;
                UserControlClass.sc2.Close();
                //UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
                //PCink = PlayCamera.inkMediaPlay;
                ChangeshowInk();
                tbText.Text = "";
                ChangeShowPlay();
                ChangeShowTime();
                RemoveFiles();
            }
        }
        /// <summary>
        /// 开始播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Play()
        {
            try
            {
                if (string.IsNullOrEmpty(UserControlClass.FileName))
                {
                    return;
                }
                if (UserControlClass.FileName.Contains(".swf"))
                {
                }
                else
                {
                    Module.readFile();
                    UserControlClass.MPPlayer.Play();
                }
                Timing.Start();
            }
            catch (Exception PlayEx)
            {
                throw PlayEx;
            }
        }

        /// <summary>
        /// 暂停播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Pause()
        {
            try
            {
                if (string.IsNullOrEmpty(UserControlClass.FileName))
                {
                    return;
                }
                if (UserControlClass.FileName.Contains(".swf"))
                {
                }
                else
                {
                    UserControlClass.MPPlayer.Pause();
                    Timing.Stop();
                }
                UserControlClass.MSStatus = MediaStatus.Pause;
            }
            catch (Exception PauseEx)
            {
                throw PauseEx;
            }
        }

        /// <summary>
        /// 重新打开播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewOpenPlay()
        {
            Play();
            //ChangeshowInk();
            if ((UserControlClass.FileName.Contains(".mp3")) | (UserControlClass.FileName.Contains(".wma")) | (UserControlClass.FileName.Contains(".wav")) | (UserControlClass.FileName.Contains(".mid")))
            {
                UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
                UserControlClass.sc2.FInkCanvas_Camera.Visibility = Visibility.Hidden;
                UserControlClass.sc2.FInkCanvas_Player.Visibility = Visibility.Visible;
            }
            MediaCountTime = UserControlClass.MPPlayer.NaturalDuration.ToString().Split('.')[0];
            UserControlClass.MPPlayer.Volume = SldVolumn.Value;
            UserControlClass.MSStatus = MediaStatus.Play;
            ChangeShowPlay();

            //提取文件时间长度，并且保存在List.xml文件中
            ListNodeLength = UserControlClass.MPPlayer.NaturalDuration.ToString().Split('.')[0];
            if (!string.IsNullOrEmpty(ListNodeName) && !string.IsNullOrEmpty(ListNodePath) && !string.IsNullOrEmpty(ListNodeLength))
            {
                if (!IsExist())
                {
                    AddList();
                }
                SelectXml();
            }
            //显示播放文件名
            tbText.Text = UserControlClass.FileName;
            ListView.SelectedValue = UserControlClass.FileName;
        }

        /// <summary>
        /// 打开播放的文件内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Open(string fn)
        {
            try
            {
                FileInfo finfo = new FileInfo(fn);
                if (finfo.Exists)
                {
                    if (UserControlClass.MPPlayer.HasVideo)
                    {
                        UserControlClass.MPPlayer.Close();
                    }
                    UserControlClass.MPPlayer.Open(new Uri(fn, UriKind.Absolute));
                    VideoDrawing drawing = new VideoDrawing();
                    Rect rect = new Rect(0.0, 0.0, UserControlClass.sc2.FInkCanvas_Player.ActualWidth, UserControlClass.sc2.FInkCanvas_Player.ActualHeight);
                    drawing.Rect = rect;
                    drawing.Player = UserControlClass.MPPlayer;
                    DrawingBrush brush = new DrawingBrush(drawing);
                    UserControlClass.sc2.FInkCanvas_Player.Background = brush;
                    NewOpenPlay();
                }
                else
                {
                    RemoveFiles();
                }
            }
            catch (Exception OpenEx)
            {
                throw OpenEx;
            }
        }


        /// <summary>
        /// 显示与隐藏声音\静音按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeShowMute()
        {
            switch (SSStatus)
            {
                case SoundStatus.Sound:
                    ImgMute2.Visibility = Visibility.Hidden;
                    ImgMute.Visibility = Visibility.Visible;
                    break;
                case SoundStatus.Mute:
                    ImgMute.Visibility = Visibility.Hidden;
                    ImgMute2.Visibility = Visibility.Visible;
                    break;
            }
        }

        /// <summary>
        /// 显示与隐藏视频\摄像屏幕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeshowInk()
        {
            switch (PCink)
            {
                case PlayCamera.inkMediaPlay:
                    UserControlClass.sc2.FInkCanvas_Camera.Visibility = Visibility.Hidden;
                    UserControlClass.sc2.FInkCanvas_Player.Visibility = Visibility.Visible;
                    break;
                case PlayCamera.inkCameraPlay:
                    UserControlClass.sc2.FInkCanvas_Player.Visibility = Visibility.Hidden;
                    UserControlClass.sc2.FInkCanvas_Camera.Visibility = Visibility.Visible;
                    break;
            }
        }

        /// <summary>
        /// 显示时间重置方法
        /// </summary>
        /// <param name="currentTime">当前的时间值</param>
        /// <param>格式为：00 的时间</param>
        private string SetTime(int currentTime)
        {
            return currentTime < 10 ? "0" + currentTime.ToString() : currentTime.ToString();
        }

        /// <summary>
        /// 显示与隐藏系统时间\文件时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeShowTime()
        {
            switch (TSStatus)
            {
                case TimeStatus.SystemTime:
                    tbSeek.Visibility = Visibility.Hidden;
                    tbTime.Visibility = Visibility.Visible;
                    break;
                case TimeStatus.FileTime:
                    tbTime.Visibility = Visibility.Hidden;
                    tbSeek.Visibility = Visibility.Visible;
                    break;
            }
        }
        #region 选择路径播放影片
        /// <summary>
        /// 打开文件播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //public void OpenFilePlay()
        //{
        //    PCink = PlayCamera.inkMediaPlay;
        //    //ChangeshowInk();
        //    Microsoft.Win32.OpenFileDialog OpenFile = new Microsoft.Win32.OpenFileDialog();
        //    string from = "Media Files(*.wmv;*.avi;*.asf;*.mpg;*.rmvb)|*.wmv;*.avi;*.asf;*.mpg;*.rmvb;*.mp3;*.wma;*.wav;*.mid|Flash Files(*.swf)|*.swf|All Files(*.*)|*.*";
        //    OpenFile.FileName = "";
        //    OpenFile.Filter = from;
        //    OpenFile.Multiselect = false;
        //    OpenFile.ShowDialog();
        //    string fileName = OpenFile.FileName;
        //    if (OpenFile.CheckPathExists && !string.IsNullOrEmpty(fileName))
        //    {
        //        try
        //        {
        //            UserControlClass.FileName = OpenFile.SafeFileName;
        //            if (UserControlClass.FileName.Contains(".swf"))
        //            {
        //            }
        //            else
        //            {
        //                Pause();
        //                //播放视频文件
        //                if (UserControlClass.MPPlayer.HasVideo)
        //                {
        //                    UserControlClass.MPPlayer.Close();
        //                }
        //                Open(fileName);
        //                //系统时间与播放时间切换
        //                TSStatus = TimeStatus.FileTime;
        //                ChangeShowTime();
        //                UserControlClass.MPPlayer.Play();
        //            }

        //            //提取文件名称和路径
        //            ListNodeName = OpenFile.SafeFileName;
        //            ListNodePath = OpenFile.FileName;
        //            ScreenJug();
        //        }
        //        catch (Exception OpenFileEx)
        //        {
        //            throw OpenFileEx;
        //        }
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(fileName))
        //        {
        //            UserControlClass.MPPlayer.Pause();
        //            UserControlClass.MPPlayer.Play();
        //            UserControlClass.MSStatus = MediaStatus.Pause;
        //            Play();
        //            ChangeShowPlay();
        //            if ((UserControlClass.FileName.Contains(".mp3")) | (UserControlClass.FileName.Contains(".wma")) | (UserControlClass.FileName.Contains(".mp4")) | (UserControlClass.FileName.Contains(".wav")) | (UserControlClass.FileName.Contains(".mid")))
        //            {
        //                UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
        //                UserControlClass.sc2.FInkCanvas_Camera.Visibility = Visibility.Hidden;
        //                UserControlClass.sc2.FInkCanvas_Player.Visibility = Visibility.Hidden;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 打开路径文件播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //public void OpenPathPlay()
        //{
        //    PCink = PlayCamera.inkMediaPlay;
        //    ChangeshowInk();
        //    //SavePlayTime();
        //    FileInfo fileInfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "OpenURL.xml");
        //    if (fileInfo.Exists)
        //    {
        //        XmlDocument ScreenXml = new XmlDocument();
        //        ScreenXml.Load(MainWindow.playerPath + @"\XML\" + "OpenURL.xml");
        //        XmlNode ScreenNode = ScreenXml.SelectSingleNode("OpenURL");
        //        XmlElement element = (XmlElement)ScreenNode;
        //        string fileName = element["Path"].InnerText;
        //        Microsoft.Win32.OpenFileDialog OpenFile = new Microsoft.Win32.OpenFileDialog();
        //        OpenFile.FileName = fileName;
        //        UserControlClass.FileName = OpenFile.SafeFileName;
        //        if (fileName.Length >= 3)
        //        {
        //            string sub = fileName.Substring(1, 2);
        //            if (sub == ":\\")
        //            {
        //                UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
        //                UserControlClass.sc2.FInkCanvas_Player.Children.Clear();

        //                //播放flash文件判断
        //                if (UserControlClass.FileName.Contains(".swf"))
        //                {
        //                }
        //                else
        //                {
        //                    Pause();
        //                    //播放视频文件
        //                    if (UserControlClass.MPPlayer.HasVideo)
        //                    {
        //                        UserControlClass.MPPlayer.Close();
        //                    }
        //                    Open(fileName);
        //                    FileInfo finfo = new FileInfo("" + fileName + "");
        //                    if (finfo.Exists)
        //                    {
        //                        ListNodePath = fileName;
        //                        ListNodeName = UserControlClass.FileName;
        //                    }
        //                    //系统时间与播放时间切换
        //                    TSStatus = TimeStatus.FileTime;
        //                    ChangeShowTime();
        //                    UserControlClass.MPPlayer.Play();
        //                }
        //            }
        //            else
        //            {
        //                System.Windows.Forms.MessageBox.Show("输入的路径格式错误！");
        //            }
        //        }
        //        else
        //        {
        //            System.Windows.Forms.MessageBox.Show("输入的路径格式错误！");
        //        }
        //    }
        //}

        /// <summary>
        /// 打开播放的文件内容（URL方式）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //public void OpenUrl(string fn)
        //{
        //    if (UserControlClass.MPPlayer.HasVideo)
        //    {
        //        UserControlClass.MPPlayer.Close();
        //    }
        //    UserControlClass.MPPlayer.Open(new Uri(fn, UriKind.Absolute));

        //    VideoDrawing drawing = new VideoDrawing();
        //    Rect rect = new Rect(0.0, 0.0, UserControlClass.sc2.FInkCanvas_Player.ActualWidth, UserControlClass.sc2.FInkCanvas_Player.ActualHeight);
        //    drawing.Rect = rect;
        //    drawing.Player = UserControlClass.MPPlayer;
        //    DrawingBrush brush = new DrawingBrush(drawing);
        //    UserControlClass.sc2.FInkCanvas_Player.Background = brush;
        //    ScreenJug();
        //}
        #endregion



        /// <summary>
        /// 保存选择的播放模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveMode()
        {
            FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "Mode.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "Mode.xml");
                XmlNode childNode = xmlDoc.SelectSingleNode("Mode");
                XmlElement element = (XmlElement)childNode;
                element["Change"].InnerText = ModePlayTag;
                xmlDoc.Save(MainWindow.playerPath + @"\XML\" + "Mode.xml");
            }
        }

        /// <summary>
        /// 播放模式选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeModePlay()
        {
            FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "Mode.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "Mode.xml");
                XmlNode childNode = xmlDoc.SelectSingleNode("Mode");
                XmlElement element = (XmlElement)childNode;
                ModePlayTag = element["Change"].InnerText;
            }
            MenuModePlayTick(ModePlayTag);
        }
        /// <summary>
        /// 读取声音大小信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadVolume()
        {
            FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "Volume.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "Volume.xml");
                XmlNode childNodes = xmlDoc.SelectSingleNode("Volume");
                XmlElement element = (XmlElement)childNodes;
                SldVolumn.Value = double.Parse(element["Size"].InnerText);
            }
        }

        /// <summary>
        /// 保存声音大小信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveVolume()
        {
            FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "Volume.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "Volume.xml");
                XmlNode childNodes = xmlDoc.SelectSingleNode("Volume");
                XmlElement element = (XmlElement)childNodes;
                element["Size"].InnerText = SldVolumn.Value.ToString().Trim();
                xmlDoc.Save(MainWindow.playerPath + @"\XML\" + "Volume.xml");
            }
        }

        /// <summary>
        /// 添加保存List.xml信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddList()
        {
            FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "List.xml");
                XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");

                XmlNode ListNode = xmlDoc.CreateElement("List");

                XmlNode NameNode = xmlDoc.CreateElement("Name");
                XmlText NameText = xmlDoc.CreateTextNode(ListNodeName);
                NameNode.AppendChild(NameText);

                XmlNode PathNode = xmlDoc.CreateElement("Path");
                XmlText PathText = xmlDoc.CreateTextNode(ListNodePath);
                PathNode.AppendChild(PathText);

                XmlNode LengthNode = xmlDoc.CreateElement("Length");
                XmlText LengthText = xmlDoc.CreateTextNode(ListNodeLength);
                LengthNode.AppendChild(LengthText);

                XmlNode TimeNode = xmlDoc.CreateElement("Time");
                XmlText TimeText = xmlDoc.CreateTextNode(ListNodeTime);
                TimeNode.AppendChild(TimeText);

                ListNode.AppendChild(NameNode);
                ListNode.AppendChild(PathNode);
                ListNode.AppendChild(LengthNode);
                ListNode.AppendChild(TimeNode);

                xmlNode.AppendChild(ListNode);

                xmlDoc.Save(MainWindow.playerPath + @"\XML\" + "List.xml");
            }
        }

        /// <summary>
        /// 添加保存List.xml信息支持多选
        /// </summary>
        /// <param name="file"></param>
        private void AddList(int file)
        {
            FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
            if (finfo.Exists)
            {
                for (int i = 0; i < file; i++)
                {

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "List.xml");
                    XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");

                    XmlNode ListNode = xmlDoc.CreateElement("List");

                    XmlNode NameNode = xmlDoc.CreateElement("Name");
                    XmlText NameText = xmlDoc.CreateTextNode(ListNodeNameArr[i]);
                    NameNode.AppendChild(NameText);

                    XmlNode PathNode = xmlDoc.CreateElement("Path");
                    XmlText PathText = xmlDoc.CreateTextNode(ListNodePathArr[i]);
                    PathNode.AppendChild(PathText);

                    ListNode.AppendChild(NameNode);
                    ListNode.AppendChild(PathNode);
                    xmlNode.AppendChild(ListNode);
                    xmlDoc.Save(MainWindow.playerPath + @"\XML\" + "List.xml");
                }

            }
        }
        /// <summary>
        /// 判定文件是否存在于List.xml文件中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public bool IsExist()
        {
            FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
            if (finfo.Exists)
            {
                XPathDocument xPath = new XPathDocument(MainWindow.playerPath + @"\XML\" + "List.xml");
                XPathNavigator navigator = xPath.CreateNavigator();
                XPathNodeIterator iterator = navigator.Select("/Lists/List[Name='" + ListNodeName + "']");
                if (iterator.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 查询List.xml文件，并于播放列表显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectXml()
        {
            ListView.Items.Clear();
            FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "List.xml");
                XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");
                string lists = xmlNode.InnerText;
                if (!string.IsNullOrEmpty(lists))
                {
                    XmlNodeList nodes = xmlNode.SelectNodes("List");
                    foreach (XmlNode node in nodes)
                    {
                        XmlElement element = (XmlElement)node;
                        string[] subItems = new string[3];
                        subItems[0] = element["Name"].InnerText;
                        subItems[1] = element["Path"].InnerText;
                        System.Windows.Forms.ListViewItem listViewItem = new System.Windows.Forms.ListViewItem(subItems);
                        string listViewItems = listViewItem.ToString().Substring(15, listViewItem.ToString().Length - 16);
                        ListView.Items.Add(listViewItems);
                    }
                }
            }
        }

        /// <summary>
        /// 定义播放下一部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private string  NextPlayer()
        {
            FileInfo fileInfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
            if (fileInfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "List.xml");
                XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");
                string lists = xmlNode.InnerText;
                if (!string.IsNullOrEmpty(lists) && !string.IsNullOrEmpty(UserControlClass.FileName))
                {
                    XmlNodeList nodes = xmlNode.SelectNodes("List");
                    int i = 0;
                    foreach (XmlNode node in nodes)
                    {
                        XmlElement element = (XmlElement)node;
                        i++;
                        if (UserControlClass.FileName.Equals(element["Name"].InnerText))
                        {
                            number = i;
                        }
                    }
                    string name = "";
                    if (ModePlayTag.Equals("RepeatPlay"))
                    {
                        name = ListView.Items[number - 1].ToString();
                    }
                    else
                        if (ModePlayTag.Equals("OrderPlay"))
                    {
                        if (ListView.Items.Count == number)
                        {
                            name = "";
                        }
                        else
                        {
                            name = ListView.Items[number].ToString();
                        }
                    }
                    else
                            if (ModePlayTag.Equals("LoopPlay"))
                    {
                        if (ListView.Items.Count == number)
                        {
                            name = ListView.Items[0].ToString();
                        }
                        else
                        {
                            name = ListView.Items[number].ToString();
                        }
                    }
                    if (name == "")
                    {
                        fullPathName = "";
                    }
                    else
                    {
                        foreach (XmlNode node in nodes)
                        {
                            XmlElement element = (XmlElement)node;
                            if (name.Equals(element["Name"].InnerText))
                            {
                                fullPathName = element["Path"].InnerText;
                            }
                        }
                        UserControlClass.FileName = fullPathName;
                    }
                }

            }
            return fullPathName;
        }

        /// <summary>
        /// 定义播放上一部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrePlayer()
        {
            FileInfo fileInfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
            //System.Windows.Forms.MessageBox.Show(""+ fileInfo);
            if (fileInfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "List.xml");
                XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");
                string lists = xmlNode.InnerText;
                //System.Windows.Forms.MessageBox.Show(""+ lists);
                if (!string.IsNullOrEmpty(lists) && !string.IsNullOrEmpty(UserControlClass.FileName))
                {
                    XmlNodeList nodes = xmlNode.SelectNodes("List");
                    int i = 0;
                    foreach (XmlNode node in nodes)
                    {
                        XmlElement element = (XmlElement)node;
                        i++;
                        if (UserControlClass.FileName.Equals(element["Name"].InnerText))
                        {
                            number = i;
                        }

                    }
                    string name = "";
                    if (ModePlayTag.Equals("RepeatPlay"))
                    {
                        name = ListView.Items[number - 1].ToString();
                    }
                    else
                        if (ModePlayTag.Equals("OrderPlay"))
                    {
                        if (number == 1)
                        {
                            name = "";
                        }
                        else
                        {
                            name = ListView.Items[number - 2].ToString();
                        }
                    }
                    else
                            if (ModePlayTag.Equals("LoopPlay"))
                    {
                        if (number == 1)
                        {
                            name = ListView.Items[ListView.Items.Count - 1].ToString();
                        }
                        else
                        {
                            name = ListView.Items[number - 2].ToString();
                        }
                    }
                    if (name == "")
                    {
                        fullPathName = "";
                    }
                    else
                    {
                        foreach (XmlNode node in nodes)
                        {
                            XmlElement element = (XmlElement)node;
                            if (name.Equals(element["Name"].InnerText))
                            {
                                fullPathName = element["Path"].InnerText;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 实现播放（上一部，下一部）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayNextPre()
        {
            PCink = PlayCamera.inkMediaPlay;
            ChangeshowInk();
            Microsoft.Win32.OpenFileDialog OpenFile = new Microsoft.Win32.OpenFileDialog();
            OpenFile.FileName = fullPathName;
            UserControlClass.FileName = OpenFile.SafeFileName;

            if (string.IsNullOrEmpty(fullPathName))
            {
                UserControlClass.MPPlayer.Close();
                SliPlayerTime.Value = 0;
                UserControlClass.MSStatus = MediaStatus.Pause;
                TSStatus = TimeStatus.SystemTime;
                UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
                PCink = PlayCamera.inkMediaPlay;
                ChangeshowInk();
                tbText.Text = "";
                ChangeShowPlay();
                ChangeShowTime();
            }
            else
            {
                FileInfo finfo = new FileInfo("" + fullPathName + "");
                if (finfo.Exists)
                {
                    UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
                    UserControlClass.sc2.FInkCanvas_Player.Children.Clear();
                    
                    //播放flash文件判断
                    if (UserControlClass.FileName.Contains(".swf"))
                    {

                    }
                    else
                    {
                        Pause();
                        //播放视频文件
                        if (UserControlClass.MPPlayer.HasVideo)
                        {
                            UserControlClass.MPPlayer.Close();
                        }
                        Open(fullPathName);
                        //系统时间与播放时间切换
                        TSStatus = TimeStatus.FileTime;
                        ChangeShowTime();
                        UserControlClass.MPPlayer.Play();
                    }
                }
                else
                {
                    UserControlClass.MPPlayer.Close();
                    SliPlayerTime.Value = 0;
                    UserControlClass.MSStatus = MediaStatus.Pause;
                    TSStatus = TimeStatus.SystemTime;
                    UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
                    PCink = PlayCamera.inkMediaPlay;
                    ChangeshowInk();
                    tbText.Text = "";
                    ChangeShowPlay();
                    ChangeShowTime();
                }
            }
        }
        /// <summary>
        /// 定义添加文件（按钮）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFiles()
        {
            Microsoft.Win32.OpenFileDialog OpenFile = new Microsoft.Win32.OpenFileDialog();
            string from = "Media Files(*.wmv;*.avi;*.mp4;*.mkv;*.rmvb)|*.wmv;*.avi;*.mp4;*.mkv;*.rmvb;*.mp3;*.wma;*.wav;*.mid|Flash Files(*.swf)|*.swf|All Files(*.*)|*.*";
            OpenFile.FileName = "";
            OpenFile.Filter = from;
            OpenFile.Multiselect = true;
            OpenFile.ShowDialog();
            if (OpenFile.CheckFileExists && !string.IsNullOrEmpty(OpenFile.FileName))
            {
                ListNodeName = OpenFile.SafeFileName;
                ListNodePath = OpenFile.FileName;
                ListNodeNameArr = OpenFile.SafeFileNames;
                ListNodePathArr = OpenFile.FileNames;
                if (!IsExist())
                {
                    AddList(ListNodeNameArr.Length);
                    //AddList();
                }
                SelectXml();               
                if (UserControlClass.FileName != null)
                {
                    ListView.SelectedValue = UserControlClass.FileName;
                }
            }
        }
      
 
        /// <summary>
        /// 定义删除列表选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveFiles()
        {
            int index = ListView.SelectedIndex;
            if (index != -1)
            {
                string name = ListView.SelectedItem.ToString();
                FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
                if (finfo.Exists)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "List.xml");
                    XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");
                    XmlNodeList nodeList = xmlNode.SelectNodes("List");
                    foreach (XmlNode node in nodeList)
                    {
                        if (node.SelectSingleNode("Name").InnerText.Equals(name))
                        {
                            xmlNode.RemoveChild(node);
                            xmlDoc.Save(MainWindow.playerPath + @"\XML\" + "List.xml");
                            SelectXml();
                        }
                    }
                }

            }
            //聚焦到listview上
            if (UserControlClass.FileName != null)
            {
                ListView.SelectedValue = UserControlClass.FileName;
            }
        }


        /// <summary>
        /// 定义删除列表所有项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveAllFiles()
        {
            //创建filestream对象流，读取文件
            FileInfo finfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
            if (finfo.Exists)
            {
                //实例一个xml文档对象
                XmlDocument xmlDoc = new XmlDocument();
                //加载url路径里面的文件
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "List.xml");
                //读取xml里面的lists单个节点
                XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");
                //读取xml里面的list节点集合对象
                XmlNodeList nodeList = xmlNode.SelectNodes("List");
                foreach (XmlNode node in nodeList)
                {
                    //移除子节点
                    xmlNode.RemoveChild(node);
                    //将xml保存到指定的文件
                    xmlDoc.Save(MainWindow.playerPath + @"\XML\" + "List.xml");
                    SelectXml();
                }
            }
        }
        /// <summary>
        /// 判断是否有第二个屏
        /// </summary>
        private void ScreenJug()
        {
            System.Windows.Forms.Screen[] sc;
            sc = System.Windows.Forms.Screen.AllScreens;
            if (UserControlClass.sc2 == null || UserControlClass.sc2.IsVisible == false)
            {
                UserControlClass.sc2 = new SecondScreen();
                //把当前的player对象传给公共类
                UserControlClass.sc2.Player = this;
                UserControlClass.sc2.Show();
            }
            else
            {
                //把当前的player对象传给公共类
                UserControlClass.sc2.Player = this;
                UserControlClass.sc2.Activate();
                UserControlClass.sc2.WindowState = WindowState.Normal;
            }
            //获取当前所有显示器的数组
            //如果有多个屏幕，就定义secondwindow的位置，让他在第二个屏幕全屏显示   
            if (sc.Length == 1)
            {
                this.WindowState = WindowState.Minimized;
            }
        }
        /// <summary>
        /// 初始位置为设置第一个屏的正中间位置
        /// </summary>
        private void Currentjug()
        {
            System.Windows.Forms.Screen[] sc;
            sc = System.Windows.Forms.Screen.AllScreens;
            if (sc.Length > 1)
            {
                var handle = new WindowInteropHelper(this).Handle;
                //获取当前显示器屏幕
                System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromHandle(handle);
                this.Left = (screen.WorkingArea.Width - this.Width) / 2;
                this.Top = (screen.WorkingArea.Height - this.Height) / 2;
            }
        }

        /// <summary>
        /// 进度条的变换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridTime_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ListView.SelectedValue = UserControlClass.FileName;
        }

        /// <summary>   
        /// 客户端键盘捕捉事件.   
        /// </summary>   
        /// <param name="hookStruct">由Hook程序发送的按键信息</param>   
        /// <param name="handle">是否拦截</param>   
        public void OnKeyPress(KeyboardHookLib.HookStruct hookStruct, out bool handle)
        {
            handle = false; //预设不拦截任何键   
            //截获Ctrl+Alt+Delete   
            if ((int)System.Windows.Forms.Control.ModifierKeys == (int)Keys.Control && (int)System.Windows.Forms.Control.ModifierKeys == (int)Keys.Alt && (int)System.Windows.Forms.Control.ModifierKeys == (int)Keys.Delete)
            {
                handle = true;
            }
            // 截获左win(开始菜单键) 
            if (hookStruct.vkCode == 91)
            {
                handle = true;
            }
            // 截获右win  
            if (hookStruct.vkCode == 92)
            {
                handle = true;
            }
            //截获Ctrl+Esc   
            if (hookStruct.vkCode == (int)Keys.Escape && (int)System.Windows.Forms.Control.ModifierKeys == (int)Keys.Control)
            {
                handle = true;
            }
            //截获alt+f4   
            if (hookStruct.vkCode == (int)Keys.F4 && (int)System.Windows.Forms.Control.ModifierKeys == (int)Keys.Alt)
            {
                handle = true;
            }
            //截获alt+空格键  
            if (hookStruct.vkCode == (int)Keys.Space && (int)System.Windows.Forms.Control.ModifierKeys == (int)Keys.Alt)
            {
                handle = true;
            }
            //截获alt+tab   
            if (hookStruct.vkCode == (int)Keys.Tab && (int)System.Windows.Forms.Control.ModifierKeys == (int)Keys.Alt)
            {
                handle = true;
            }
            //截获alt+esc   
            if (hookStruct.vkCode == (int)Keys.Alt && (int)System.Windows.Forms.Control.ModifierKeys == (int)Keys.Escape)
            {
                handle = true;
            }
            //crtl+shift+esc   
            if (hookStruct.vkCode == (int)Keys.Control && hookStruct.vkCode == (int)Keys.Shift && (int)System.Windows.Forms.Control.ModifierKeys == (int)Keys.Escape)
            {
                handle = true;
            }
            //截获F1   
            if (hookStruct.vkCode == (int)Keys.F1)
            {
                handle = true;
            }
        }

        /// <summary>
        /// 判断用户是否登录
        /// </summary>
        public void JugUser()
        {
            if (UserControlClass.username != "" && UserControlClass.username != null)
            {
                but_login.Visibility = Visibility.Collapsed;
                lab_username.Content = "管理员:" + UserControlClass.username;
                but_Cancel.Visibility = Visibility.Visible;
                lab_username.Visibility = Visibility.Visible;
                _keyboardHook.UninstallHook();
                ManageTaskManager(0);
            }
            else
            {
                but_login.Visibility = Visibility.Visible;
                lab_username.Content = "";
                but_Cancel.Visibility = Visibility.Collapsed;
                lab_username.Visibility = Visibility.Collapsed;
                _keyboardHook.InstallHook(this.OnKeyPress);
                ManageTaskManager(1);
            }
        }
        /// 管理任务管理器的方法
        /// </summary>
        /// <param name="arg">0：启用任务管理器 1：禁用任务管理器</param>
        private void ManageTaskManager(int arg)
        {
            RegistryKey currentUser = Registry.CurrentUser;
            RegistryKey system = currentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true);
            //如果system项不存在就创建这个项
            if (system == null)
            {
                system = currentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
            }
            system.SetValue("DisableTaskmgr", arg, RegistryValueKind.DWord);
            currentUser.Close();
        }
        private void but_login_Click(object sender, RoutedEventArgs e)
        {
            //UserLogin lg = new UserLogin(this);
            //lg.ShowDialog();
        }
        private void but_Cancel_Click(object sender, RoutedEventArgs e)
        {
            UserControlClass.username = "";
            JugUser();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ListView.SelectedItem != null)
                {                    
                    fullPathName = GetMovieFullPathName(ListView.SelectedItem.ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }            
        }

        /// <summary>
        /// 获取当前选中影片全路径名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetMovieFullPathName(string name)
        {
            //创建打开xml文件的流
            FileInfo fileInfo = new FileInfo(MainWindow.playerPath + @"\XML\" + "List.xml");
            if (fileInfo.Exists)//判断文件是否存在
            {
                XmlDocument xmlDoc = new XmlDocument();
                //加载路径的xml文件
                xmlDoc.Load(MainWindow.playerPath + @"\XML\" + "List.xml");
                XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");
                string lists = xmlNode.InnerText;
                if (!string.IsNullOrEmpty(lists))
                {
                    XmlNodeList nodes = xmlNode.SelectNodes("List");
                    foreach (XmlNode node in nodes)
                    {
                        XmlElement element = (XmlElement)node;
                        if (name.Equals(element["Name"].InnerText))
                        {
                            fullPathName = element["Path"].InnerText;
                        }
                    }
                }
                return fullPathName;
            }
            return null;
        }

     


        private void btnAddFile_Click(object sender, RoutedEventArgs e)
        {
            AddFiles();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            btnPlayClickFun();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {            
            btnStopClickFun();
        }

        private void btnFast_Click(object sender, RoutedEventArgs e)
        {
            if (UserControlClass.FileName != null && UserControlClass.FileName != "")
            {
                UserControlClass.MPPlayer.Position = UserControlClass.MPPlayer.Position + TimeSpan.FromSeconds(20);
            }
        }

        private void btnSlow_Click(object sender, RoutedEventArgs e)
        {
            if (UserControlClass.FileName != null && UserControlClass.FileName != "")
            {
                UserControlClass.MPPlayer.Position = UserControlClass.MPPlayer.Position - TimeSpan.FromSeconds(20);
            }
        }

        private void ShowData_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Data winData = new Data();
            winData.ShowDialog();
        }

        private void checkBox_Click(object sender, RoutedEventArgs e)
        {
            if(checkBox.IsChecked==true)
            {
                UdpSend.range = int.Parse(textBox.Text);        //震动幅度
                UdpSend.rate = int.Parse(textBox2.Text);        //震动频率
            }
            else
            {
                UdpSend.range = 0;
                UdpSend.rate = 0;
            }
        }
        #region MyVibrationCheckboxRegion
        private void checkBox1_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox1.IsChecked == true)
            {
                UdpSend.range = int.Parse(textBox.Text);        //震动幅度
                UdpSend.rate = int.Parse(textBox2.Text);        //震动频率
                UdpSend.circleFlag = true;
                UdpSend.tempRate = int.Parse(textBox2.Text);
            }
            else
            {
                UdpSend.range = 0;
                UdpSend.rate = 0;
                UdpSend.circleFlag = false;
            }
        }
        #endregion


        private void MenuBack_Click(object sender, RoutedEventArgs e)
        {

        }


        /// <summary>
        /// 点击播放按钮触发函数
        /// </summary>
        private void btnPlayClickFun()
        {
            //this.ListView.Select();
            //ListView.Focus();
            //ListView.SelectedIndex = 2;
            
            int index =  ListView.SelectedIndex;
            if (ListView.SelectedValue != null)
            {
                string filename = ListView.SelectedValue.ToString();
                if (UserControlClass.FileName != filename)
                {
                    if (index != -1)
                    {
                        ScreenJug();
                        memoryPlay = true;
                        ListPlay(ListView.SelectedItem.ToString());
                        UserControlClass.MSStatus = MediaStatus.Pause;
                    }
                }
                if (UserControlClass.MSStatus == MediaStatus.Pause)
                {
                    UserControlClass.MSStatus = MediaStatus.Play;
                    if (UserControlClass.sc2.WindowState == WindowState.Minimized)
                    {
                        UserControlClass.sc2.Activate();
                        UserControlClass.sc2.WindowState = WindowState.Normal;
                    }
                    Play();
                }
                else
                {
                    UserControlClass.MSStatus = MediaStatus.Pause;
                    Pause();
                }
                ChangeShowPlay();
            }
        }

        /// <summary>
        /// 点击停止按钮触发函数
        /// </summary>
        private void btnStopClickFun()
        {
            if (!string.IsNullOrEmpty(UserControlClass.FileName))
            {
                if (UserControlClass.FileName.Contains(".swf"))
                {
                }
                else
                {
                    UserControlClass.MSStatus = MediaStatus.Pause;
                    ChangeShowPlay();
                    UserControlClass.MPPlayer.Close();
                    SliPlayerTime.Value = 0;
                    UserControlClass.FileName = null;
                    TSStatus = TimeStatus.SystemTime;
                    UserControlClass.sc2.Close();
                    //UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
                    //PCink = PlayCamera.inkMediaPlay;
                    //ChangeshowInk();
                    tbText.Text = "";
                    ChangeShowTime();
                }
                if (Module.timerMovie != null)
                {
                    Module.timerMovie.Stop();
                    UdpSend.SendReset();
                }
            }
        }


        /// <summary>
        /// 菜单项关机选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuCloseComputer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult dr;
            if ("CN".Equals(MainWindow.PlayLanguage))
            {
                dr = System.Windows.MessageBox.Show("点击确定，电脑将会关闭", "提示", MessageBoxButton.OKCancel);
            }
            else
            {
                dr = System.Windows.MessageBox.Show("Are you sure ? Computer will be shutdown", "Tips", MessageBoxButton.OKCancel);
            }

            if (dr == MessageBoxResult.OK)
            {
                //System.Diagnostics.Process.Start("cmd.exe", "/cshutdown -s -t 0");
                Process.Start("shutdown.exe", "-s -t 0");
            }
        }


        /// <summary>
        /// 菜单项关闭选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuClose_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SaveVolume();
            if (Timing != null)
            {
                Timing.Stop();
            }
            DTimer.Stop();
            if (Module.timerMovie != null)
            {
                Module.timerMovie.Stop();
            }
            UdpSend.SendReset();
            //this.Close();
            UserControlClass.MPPlayer.Close();
            this.Hide();
        }

    }
}



