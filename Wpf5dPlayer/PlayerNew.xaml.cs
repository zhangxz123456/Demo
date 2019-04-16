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
using Wpf5dPlayer.Class;
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


namespace Wpf5dPlayer
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
    public partial class PlayerNew : Window
    {
        #region   变量声明
        //勾子管理类   
        public KeyboardHookLib _keyboardHook = null;
        /// <summary>
        /// 列表-名称
        /// </summary>
        private string ListNodeName = "";
        /// <summary>
        /// 列表-路径
        /// </summary>
        private string ListNodePath = "";
        /// <summary>
        /// 列表-长度
        /// </summary>
        private string ListNodeLength = "";
        /// <summary>
        /// 列表-播放时刻
        /// </summary>
        private string ListNodeTime = "";
        /// <summary>
        /// 皮肤颜色选择
        /// </summary>
        private string ColorsTag;
        /// <summary>
        /// 播放模式选择
        /// </summary>
        private string ModePlayTag;
        /// <summary>
        /// 底部图片选择
        /// </summary>
        private string ImagesTag;
        /// <summary>
        /// 底部图片选择
        /// </summary>
        private string ImagesChanges;
        /// <summary>
        /// 选择播放文件（上一部，下一部）
        /// </summary>
        private string changes = "";
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
        public AxShockwaveFlash FlashShock;
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
       // public CapPlayer CameraPlayer;
        /// <summary>
        /// 系统时间显示
        /// </summary>
        private DispatcherTimer DTimer;
        /// <summary>
        /// 数据 实例
        /// </summary>
        public ObservableCollection<ImageInfo> data = new ObservableCollection<ImageInfo>();
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

        //public static  System.Windows.Forms.Screen[] AllScreens { get; }
        //public System.Drawing.Rectangle WorkingArea { get; }

        //public void OtherDisplay(System.Windows.Forms.Screen screen)
        //{
        //    var secondaryScreen = System.Windows.Forms.Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();
        //    ScreensRect = new System.Drawing.Rectangle[AllScreens.Length];
        //    for (int i = 0; i < s.Length; i++)
        //    {
        //        AllScreens[i] = s[i].WorkingArea;
        //    }
        //}
        public PlayerNew()
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
        }
        /// <summary>
        /// 设置动画
        /// </summary>

        ///// <summary>
        ///// lv的img插入选项
        ///// </summary>
        ///// <returns></returns>
        //public System.Windows.Controls.ContextMenu getLVMenu()
        //{
        //    System.Windows.Controls.MenuItem itemplay;
        //    System.Windows.Controls.ContextMenu contextMenu = new System.Windows.Controls.ContextMenu();
        //    itemplay = new System.Windows.Controls.MenuItem();
        //    itemplay.Tag = "Delete";
        //    itemplay.Header = "播放";
        //    itemplay.Click += new RoutedEventHandler(itemDelete_Click);
        //    contextMenu.Items.Add(itemplay);
        //    return contextMenu;
        //}
        /// <summary>
        /// 为mianplay中的lv添加图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lv_Loaded(object sender, RoutedEventArgs e)
        {

            UserControlClass.ListviewAddImage(data);
            lv.ItemsSource = data;
            lv.ContextMenu = getLVMenu();
            //设置ListView的ItemsSource

        }

        ///<summary>
        ///定义系统时间显示事件
        /// </summary>
        private void timer_Tick(object sender, EventArgs e)
        {
            tbTime.Text = DateTime.Now.Hour.ToString("D2") + " : " + DateTime.Now.Minute.ToString("D2") + " : " + DateTime.Now.Second.ToString("D2");
        }

        /// <summary>
        /// 加载窗体事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerWin_Loaded(object sender, RoutedEventArgs e)
        {
            UserControlClass.MPPlayer.MediaOpened += new EventHandler(MPPlayer_MediaOpened);
            UserControlClass.MPPlayer.MediaEnded += new EventHandler(MPPlayer_MediaEnded);

            Timing.Elapsed += new ElapsedEventHandler(Tim_Elapsed);
            ChangeColors();
            ChangeShowPlay();
            ChangeShowMute();
            ChangeShowTime();
            ChangeshowInk();
            ChangeModePlay();
            ReadVolumn();
            SelectXml();
            ChangeShowFilm();
            NewLoaded();
            PlayerList.ContextMenu = getListMenu();

        }
        /// <summary>
        /// 计时器响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tim_Elapsed(object sender, ElapsedEventArgs e)
        {
            SliPlayerTime.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DeleSetPosition(setPosition));
        }
        /// <summary>
        /// 打开播放器响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MPPlayer_MediaOpened(object sender, EventArgs e)
        {
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
        }
        /// <summary>
        /// 播放完成响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MPPlayer_MediaEnded(object sender, EventArgs e)
        {
            SliPlayerTime.Value = 0;
            UserControlClass.MPPlayer.Position = new TimeSpan(0, 0, 0);
            UserControlClass.MPPlayer.Stop();
            UserControlClass.MSStatus = MediaStatus.Pause;
            TSStatus = TimeStatus.FileTime;

            //重复播放
            if (ModePlayTag == "RepeatPlay" || ModePlayTag == "LoopPlay")
            {
                NextPlayer();
                string Currentname = UserControlClass.FileName;
                ListPlay(Currentname);
                ChangeShowTime();
                ChangeShowPlay();
            }
            else
            {
                UserControlClass.sc2.Close();
            }

        }
        ///// <summary>
        ///// 改变播放器的显示Size
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Player_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    timer.Start();
        //}

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
        /// 播放/暂停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ImgPlayer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int index = ListView.SelectedIndex;
            if (ListView.SelectedValue != null)
            {
                string filename = ListView.SelectedValue.ToString();
                if (e.LeftButton == MouseButtonState.Pressed)
                {
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
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgStop_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
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
                }
            }
        }

        /// <summary>
        /// 上一部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgPre_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (UserControlClass.FileName != null && UserControlClass.FileName != "")
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    ScreenJug();
                    PrePlayer();
                    //SavePlayTime();
                    PlayNextPre();
                    ListView.SelectedValue = UserControlClass.FileName;
                }
            }
        }

        /// <summary>
        /// 下一部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgNext_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (UserControlClass.FileName != null && UserControlClass.FileName != "")
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    NextPlayer();
                    ScreenJug();
                    //SavePlayTime();
                    PlayNextPre();
                    ListView.SelectedValue = UserControlClass.FileName;
                }
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgOpen_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                OpenFilePlay();
            }
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

        /// <summary>;
        /// 菜单栏--打开URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //OpenURL url = new OpenURL(this);
            //url.ShowDialog();
        }

        /// <summary>
        /// 关闭播放器事件
        ///【要关闭窗体，计时器，播放器工具，清除相关摄像设备】
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerWin_Closed(object sender, EventArgs e)
        {
            SaveVolumn();
            //SavePlayTime();
            UserControlClass.MPPlayer.Close();
            UserControlClass.MSStatus = MediaStatus.Pause;
            UserControlClass.FileName = string.Empty;
           // UserControlClass.sc2.FInkCanvas_Player.Children.Clear();
           // UserControlClass.sc2.FInkCanvas_Camera.Children.Clear();
            System.Windows.Forms.Application.Exit();
            //if (CameraPlayer != null)
            //{
            //    CameraPlayer.Dispose();
            //    CameraPlayer = null;
            //}
        }
        /// <summary>
        /// 菜单栏--退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WinClose_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            MessageBoxResult dr = System.Windows.MessageBox.Show("确认退出吗？您的电脑将会关机!\n\r\n\rDo you make sure to quit? Your computer will turn off!", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (dr == MessageBoxResult.OK)
            {
                SaveVolumn();
                //SavePlayTime();
                //执行cmd命令，使电脑关机
                //System.Diagnostics.Process.Start("cmd.exe", "/cshutdown -s -t 0");
                if (Timing != null)
                {
                    Timing.Stop();
                }
                DTimer.Stop();
                this.Close();
                UserControlClass.MPPlayer.Close();
                UserControlClass.MSStatus = MediaStatus.Pause;
                UserControlClass.FileName = string.Empty;
                UserControlClass.sc2.FInkCanvas_Player.Children.Clear();
                UserControlClass.sc2.FInkCanvas_Camera.Children.Clear();
               
                //if (CameraPlayer != null)
                //{
                //    CameraPlayer.Dispose();
                //    CameraPlayer = null;
                //}
            }
        }

        /// <summary>
        /// 主窗体关闭，所有进程都关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerWin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ManageTaskManager(0);
            System.Windows.Application.Current.Shutdown();
            //System.Windows.MessageBox.Show("进入Closing");
            //System.Environment.Exit(System.Environment.ExitCode);
            //System.Windows.Forms.Application.Exit();
            //System.Environment.Exit(0);
            //System.Windows.MessageBox.Show("2342");
        }

        /// <summary>
        /// 点击选择皮肤颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Colors_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.MenuItem menu = (System.Windows.Controls.MenuItem)sender;
            ColorsTag = menu.Tag.ToString();
            SaveColors();
            ChangeColors();
        }

        /// <summary>
        /// ListView控件鼠标双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = ListView.SelectedIndex;
            if (index != -1)
            {
                ScreenJug();
                memoryPlay = true;
                ListPlay(ListView.SelectedItem.ToString());
                UserControlClass.MSStatus = MediaStatus.Pause;
            }
        }

        /// <summary>
        /// 选择背景图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeImages_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.MenuItem menu = (System.Windows.Controls.MenuItem)sender;
            ImagesTag = menu.Tag.ToString();
            switch (ImagesTag)
            {
                case "ACustom":
                    ChangeShowFilms();
                    break;
                case "DefaultImages":
                    ImagesChanges = "gd.jpg";
                    ImageChangeFilm();
                    ChangeShowFilm();
                    break;
                case "SpringGreen":
                    ImagesChanges = "Spring.jpg";
                    ImageChangeFilm();
                    ChangeShowFilm();
                    break;
                case "SummerBlue":
                    ImagesChanges = "Summer.jpg";
                    ImageChangeFilm();
                    ChangeShowFilm();
                    break;
                case "AutumnYellow":
                    ImagesChanges = "Autumn.jpg";
                    ImageChangeFilm();
                    ChangeShowFilm();
                    break;
                case "WinterWhite":
                    ImagesChanges = "Winter.jpg";
                    ImageChangeFilm();
                    ChangeShowFilm();
                    break;
            }
        }

        /// <summary>
        /// 取消背景图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelImages_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CancelFilms();
            ChangeShowFilm();
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
                if (ListView.SelectedValue != null)
                {
                    string selectvalue = ListView.SelectedValue.ToString();
                    RemoveSameNameImage(selectvalue);
                }
                RemoveFiles();
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
        /// 关于播放器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutPlayer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //About about = new About();
            //about.ShowDialog();
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

        /// <summary>
        /// 播放列表鼠标右击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void itemDelete_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = (System.Windows.Controls.MenuItem)sender;
            string stringName = item.Tag.ToString();
            switch (stringName)
            {
                case "ListPlay":
                    int play = ListView.SelectedIndex;
                    if (play != -1)
                    {
                        memoryPlay = false;
                        ListPlay(ListView.SelectedItem.ToString());
                    }
                    break;

                case "Delete":
                    RemoveFiles();
                    break;

                case "DeleteAll":
                    RemoveAllFiles();
                    break;
            }
        }
        /// <summary>
        /// 判断选项LVmenuitem的tag,从而执行不同的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LVitemDelete_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = (System.Windows.Controls.MenuItem)sender;
            string stringName = item.Tag.ToString();
            switch (stringName)
            {
                case "AddImage":
                    AddImage();
                    break;
                case "Play":
                    ItemPlay();
                    break;
                case "Delete":
                    RemoveImages();
                    break;
                case "DeleteAll":
                    MessageBoxResult dr = System.Windows.MessageBox.Show("确定要删除所有图片吗?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (dr == MessageBoxResult.OK)
                    {
                        RemoveAllImage();
                    }
                    break;
            }
        }
        /// <summary>
        /// 图片所选影片播放
        /// </summary>
        private void ItemPlay()
        {
            int index = lv.SelectedIndex;
            if (index > -1)
            {
                string imagepath = data[lv.SelectedIndex].Filepath;
                string filename = Path.GetFileNameWithoutExtension(imagepath);
                foreach (var item in ListView.Items)
                {
                    string[] names = item.ToString().Split('.');
                    string name = null;
                    for (int i = 0; i < names.Length - 1; i++)
                    {
                        name += "." + names[i];
                    }
                    string fname = name.Remove(0, 1);

                    if (name != null)
                    {
                        if (fname.Equals(filename))
                        {
                            ScreenJug();
                            memoryPlay = true;
                            ListPlay(item.ToString());
                            UserControlClass.MSStatus = MediaStatus.Pause;
                            return;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 删除所有图片
        /// </summary>
        private void RemoveAllImage()
        {
            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\UseThings\images");
            foreach (var file in files)
            {
                File.Delete(file);
            }
            data.Clear();
        }
        /// <summary>
        /// 删除图片
        /// </summary>
        private void RemoveImages()
        {
            int index = lv.SelectedIndex;
            if (index != -1)
            {
                //获取路径
                string path = data[index].Filepath;
                //BitmapImage img = (BitmapImage)listview.SelectedItem;
                //文件已打开，无法执行
                data.Remove(data[index]);
                File.Delete(path);
                //data.Clear();
                //UserControlClass.ListviewAddImage(data);
            }
        }
        /// <summary>
        /// 获取播放列表鼠标右击下拉菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public System.Windows.Controls.ContextMenu getListMenu()
        {
            System.Windows.Controls.MenuItem itemDelete;
            System.Windows.Controls.ContextMenu contextMenu = new System.Windows.Controls.ContextMenu();

            itemDelete = new System.Windows.Controls.MenuItem();
            itemDelete.Tag = "Delete";
            itemDelete.Header = "  删   除  ";
            itemDelete.Click += new RoutedEventHandler(itemDelete_Click);
            contextMenu.Items.Add(itemDelete);

            itemDelete = new System.Windows.Controls.MenuItem();
            itemDelete.Tag = "DeleteAll";
            itemDelete.Header = "删 除 所 有";
            itemDelete.Click += new RoutedEventHandler(itemDelete_Click);
            contextMenu.Items.Add(itemDelete);

            return contextMenu;
        }
        /// <summary>
        /// 写一个contextmenu插入各个图片选项
        /// </summary>
        /// <returns></returns>
        public System.Windows.Controls.ContextMenu getLVMenu()
        {
            System.Windows.Controls.MenuItem menuitem;
            System.Windows.Controls.ContextMenu contextMenu = new System.Windows.Controls.ContextMenu();

            menuitem = new System.Windows.Controls.MenuItem();
            menuitem.Tag = "Play";
            menuitem.Header = "  播   放  ";
            menuitem.Click += new RoutedEventHandler(LVitemDelete_Click);
            contextMenu.Items.Add(menuitem);

            menuitem = new System.Windows.Controls.MenuItem();
            menuitem.Tag = "AddImage";
            menuitem.Header = "添 加 图 片";
            menuitem.Click += new RoutedEventHandler(LVitemDelete_Click);
            contextMenu.Items.Add(menuitem);

            menuitem = new System.Windows.Controls.MenuItem();
            menuitem.Tag = "Delete";
            menuitem.Header = "  删   除  ";
            menuitem.Click += new RoutedEventHandler(LVitemDelete_Click);
            contextMenu.Items.Add(menuitem);

            menuitem = new System.Windows.Controls.MenuItem();
            menuitem.Tag = "DeleteAll";
            menuitem.Header = "删 除 所 有";
            menuitem.Click += new RoutedEventHandler(LVitemDelete_Click);
            contextMenu.Items.Add(menuitem);

            return contextMenu;
        }
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
        private void setPosition()
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
                    NextPlayer();
                    PlayNextPre();
                }
                ChangeShowPlay();
            }
        }

        /// <summary>
        /// 显示底片背景
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeShowFilm()
        {
            ImageBrush image = new ImageBrush();
            FileInfo fileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Images.xml");
            if (fileInfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Images.xml");
                XmlNode childNodes = xmlDoc.SelectSingleNode("Images");
                XmlElement element = (XmlElement)childNodes;
                string images = element["Path"].InnerText;
                if (!string.IsNullOrEmpty(images))
                {
                    FileInfo finfo = new FileInfo("" + images + "");
                    if (finfo.Exists)
                    {
                        image.ImageSource = new BitmapImage(new Uri("" + images + "", UriKind.Relative));
                    }
                }
            }
            stackPanel.Background = image;
            stackPanel.Opacity = 0.7;
        }

        /// <summary>
        /// 选择底部图片（自带（保存））
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageChangeFilm()
        {
            FileInfo finfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\Images\" + "" + ImagesChanges + "");
            if (finfo.Exists)
            {
                FileInfo fileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Images.xml");
                if (fileInfo.Exists)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Images.xml");
                    XmlNode childNode = xmlDoc.SelectSingleNode("Images");
                    XmlElement element = (XmlElement)childNode;
                    element["Path"].InnerText = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\Images\" + "" + ImagesChanges + "";
                    xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Images.xml");
                }
            }
        }
        /// <summary>
        /// 显示底片背景（自定义）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeShowFilms()
        {
            Microsoft.Win32.OpenFileDialog OpenFile = new Microsoft.Win32.OpenFileDialog();
            string from = "Images Files(*.jpg;*.bmp;*.png;*.ico)|*.jpg;*.bmp;*.png;*.ico";
            OpenFile.FileName = "";
            OpenFile.Filter = from;
            OpenFile.Multiselect = false;
            OpenFile.ShowDialog();
            string fileName = OpenFile.FileName;
            if (!string.IsNullOrEmpty(fileName))
            {
                ImageBrush image = new ImageBrush();
                image.ImageSource = new BitmapImage(new Uri("" + fileName + "", UriKind.Relative));
                stackPanel.Background = image;
                stackPanel.Opacity = 0.7;
                FileInfo finfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Images.xml");
                if (finfo.Exists)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Images.xml");
                    XmlNode childNodes = xmlDoc.SelectSingleNode("Images");
                    XmlElement element = (XmlElement)childNodes;
                    element["Path"].InnerText = fileName;
                    xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Images.xml");
                }
            }
        }
        /// <summary>
        /// 取消底片背景（删除XML文件对应信息）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelFilms()
        {
            FileInfo fileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Images.xml");
            if (fileInfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Images.xml");
                XmlNode childNodes = xmlDoc.SelectSingleNode("Images");
                XmlElement element = (XmlElement)childNodes;
                element["Path"].InnerText = "";
                xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Images.xml");
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
                        ImgPlayer.Visibility = Visibility.Hidden;
                        ImgPause.Visibility = Visibility.Visible;
                        break;
                    case MediaStatus.Play:
                        ImgPause.Visibility = Visibility.Hidden;
                        ImgPlayer.Visibility = Visibility.Visible;
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
            FileInfo fileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
            if (fileInfo.Exists)//判断文件是否存在
            {
                XmlDocument xmlDoc = new XmlDocument();
                //加载路径的xml文件
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
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
                            changes = element["Path"].InnerText;
                        }
                    }
                }
            }
            //SavePlayTime();
            PCink = PlayCamera.inkMediaPlay;
            ChangeshowInk();
            Microsoft.Win32.OpenFileDialog OpenFile = new Microsoft.Win32.OpenFileDialog();
            OpenFile.FileName = changes;
            UserControlClass.FileName = OpenFile.SafeFileName;
            // System.Windows.Forms.MessageBox.Show( UserControlClass.FileName+"");
            FileInfo finfo = new FileInfo("" + changes + "");
            if (finfo.Exists)
            {
                UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
                UserControlClass.sc2.FInkCanvas_Player.Children.Clear();
                //清空flash播放文件
                //if (FlashShock != null)
                //{
                //    FlashShock.Dispose();
                //    // FlashShock.ScaleMode = 0;
                //}
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
                    Open(changes);
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
                UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
                PCink = PlayCamera.inkMediaPlay;
                ChangeshowInk();
                tbText.Text = "";
                ChangeShowPlay();
                ChangeShowTime();
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
                //else if (CameraPlayer != null)
                //{
                //    if (UserControlClass.FileName == "Camera")
                //    {
                //        CameraPlayer.Continue();
                //    }
                //    if (UserControlClass.FileName != "Camera" && !UserControlClass.FileName.Contains(".swf"))
                //    {
                //    }
                //}
                else
                {
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
                //else if (CameraPlayer != null)
                //{
                //    CameraPlayer.Pause();
                //    if (UserControlClass.FileName != "Camera")
                //    {
                //        UserControlClass.MPPlayer.Pause();
                //        Timing.Stop();
                //    }
                //}
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
            ChangeshowInk();
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
            catch (Exception OpenEx)
            {
                throw OpenEx;
            }
        }

        /// <summary>
        /// 打开播放的文件内容（URL方式）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OpenUrl(string fn)
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
            ScreenJug();
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
        /// <summary>
        /// 打开文件播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OpenFilePlay()
        {
            PCink = PlayCamera.inkMediaPlay;
            //ChangeshowInk();
            Microsoft.Win32.OpenFileDialog OpenFile = new Microsoft.Win32.OpenFileDialog();
            string from = "Media Files(*.wmv;*.avi;*.asf;*.mpg;*.rmvb)|*.wmv;*.avi;*.asf;*.mpg;*.rmvb;*.mp3;*.wma;*.wav;*.mid|Flash Files(*.swf)|*.swf|All Files(*.*)|*.*";
            OpenFile.FileName = "";
            OpenFile.Filter = from;
            OpenFile.Multiselect = false;
            OpenFile.ShowDialog();
            string fileName = OpenFile.FileName;
            if (OpenFile.CheckPathExists && !string.IsNullOrEmpty(fileName))
            {
                try
                {
                    UserControlClass.FileName = OpenFile.SafeFileName;
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
                        Open(fileName);
                        //系统时间与播放时间切换
                        TSStatus = TimeStatus.FileTime;
                        ChangeShowTime();
                        UserControlClass.MPPlayer.Play();
                    }

                    //提取文件名称和路径
                    ListNodeName = OpenFile.SafeFileName;
                    ListNodePath = OpenFile.FileName;
                    ScreenJug();
                }
                catch (Exception OpenFileEx)
                {
                    throw OpenFileEx;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    UserControlClass.MPPlayer.Pause();
                    UserControlClass.MPPlayer.Play();
                    UserControlClass.MSStatus = MediaStatus.Play;
                    Play();
                    ChangeShowPlay();
                    if ((UserControlClass.FileName.Contains(".mp3")) | (UserControlClass.FileName.Contains(".wma")) | (UserControlClass.FileName.Contains(".mp4")) | (UserControlClass.FileName.Contains(".wav")) | (UserControlClass.FileName.Contains(".mid")))
                    {
                        UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
                        UserControlClass.sc2.FInkCanvas_Camera.Visibility = Visibility.Hidden;
                        UserControlClass.sc2.FInkCanvas_Player.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        /// <summary>
        /// 打开路径文件播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OpenPathPlay()
        {
            PCink = PlayCamera.inkMediaPlay;
            ChangeshowInk();
            //SavePlayTime();
            FileInfo fileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "OpenURL.xml");
            if (fileInfo.Exists)
            {
                XmlDocument ScreenXml = new XmlDocument();
                ScreenXml.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "OpenURL.xml");
                XmlNode ScreenNode = ScreenXml.SelectSingleNode("OpenURL");
                XmlElement element = (XmlElement)ScreenNode;
                string fileName = element["Path"].InnerText;
                Microsoft.Win32.OpenFileDialog OpenFile = new Microsoft.Win32.OpenFileDialog();
                OpenFile.FileName = fileName;
                UserControlClass.FileName = OpenFile.SafeFileName;
                if (fileName.Length >= 3)
                {
                    string sub = fileName.Substring(1, 2);
                    if (sub == ":\\")
                    {
                        UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
                        UserControlClass.sc2.FInkCanvas_Player.Children.Clear();
                        //清空flash播放文件
                        //if (FlashShock != null)
                        //{
                        //    FlashShock.Dispose();
                        //}

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
                            Open(fileName);
                            FileInfo finfo = new FileInfo("" + fileName + "");
                            if (finfo.Exists)
                            {
                                ListNodePath = fileName;
                                ListNodeName = UserControlClass.FileName;
                            }
                            //系统时间与播放时间切换
                            TSStatus = TimeStatus.FileTime;
                            ChangeShowTime();
                            UserControlClass.MPPlayer.Play();
                        }
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("输入的路径格式错误！");
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("输入的路径格式错误！");
                }
            }
        }

        /// <summary>
        /// 保存选择的皮肤颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveColors()
        {
            FileInfo finfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Colors.xml");
            if (finfo.Exists)
            {

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Colors.xml");
                XmlNode childNodes = xmlDoc.SelectSingleNode("Colors");
                XmlElement element = (XmlElement)childNodes; ;
                element["Change"].InnerText = ColorsTag;
                xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Colors.xml");
            }
        }

        /// <summary>
        /// 皮肤颜色选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeColors()
        {
            FileInfo finfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Colors.xml");
            if (finfo.Exists)
            {
                XmlDocument ScreenXml = new XmlDocument();
                ScreenXml.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Colors.xml");
                XmlNode ScreenNode = ScreenXml.SelectSingleNode("Colors");
                XmlElement element = (XmlElement)ScreenNode;
                ColorsTag = element["Change"].InnerText;
                switch (ColorsTag)
                {
                    case "Custom":
                        System.Windows.Forms.ColorDialog selectColor = new System.Windows.Forms.ColorDialog();
                        if (selectColor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            GridPlayer.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xff, selectColor.Color.R, selectColor.Color.G, selectColor.Color.B));
                            MenuMain.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xff, selectColor.Color.R, selectColor.Color.G, selectColor.Color.B));
                            GridTitle.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xff, selectColor.Color.R, selectColor.Color.G, selectColor.Color.B));
                            Head.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xff, selectColor.Color.R, selectColor.Color.G, selectColor.Color.B));
                            ColorsTag = System.Windows.Media.Color.FromArgb(0xff, selectColor.Color.R, selectColor.Color.G, selectColor.Color.B).ToString();

                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Colors.xml");
                            XmlNode childNodes = xmlDoc.SelectSingleNode("Colors");
                            XmlElement ColorElement = (XmlElement)childNodes; ;
                            ColorElement["Change"].InnerText = ColorsTag;
                            xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Colors.xml");
                        }
                        break;
                    case "LightBlue":
                        GridPlayer.Background = Brushes.LightBlue;
                        MenuMain.Background = Brushes.LightBlue;
                        GridTitle.Background = Brushes.LightBlue;
                        Head.Background = Brushes.LightBlue;
                        break;
                    case "Beige":
                        GridPlayer.Background = Brushes.Beige;
                        MenuMain.Background = Brushes.Beige;
                        GridTitle.Background = Brushes.Beige;
                        Head.Background = Brushes.Beige;
                        break;
                    case "SkyBlue":
                        GridPlayer.Background = Brushes.SkyBlue;
                        MenuMain.Background = Brushes.SkyBlue;
                        GridTitle.Background = Brushes.SkyBlue;
                        Head.Background = Brushes.SkyBlue;
                        break;
                    case "Wheat":
                        GridPlayer.Background = Brushes.Wheat;
                        MenuMain.Background = Brushes.Wheat;
                        GridTitle.Background = Brushes.Wheat;
                        Head.Background = Brushes.Wheat;
                        break;
                    case "Violet":
                        GridPlayer.Background = Brushes.Violet;
                        MenuMain.Background = Brushes.Violet;
                        GridTitle.Background = Brushes.Violet;
                        Head.Background = Brushes.Violet;
                        break;
                    case "SlateBlue":
                        GridPlayer.Background = Brushes.SlateBlue;
                        MenuMain.Background = Brushes.SlateBlue;
                        GridTitle.Background = Brushes.SlateBlue;
                        Head.Background = Brushes.SlateBlue;
                        break;
                    case "MediumPurple":
                        GridPlayer.Background = Brushes.MediumPurple;
                        MenuMain.Background = Brushes.MediumPurple;
                        GridTitle.Background = Brushes.MediumPurple;
                        Head.Background = Brushes.MediumPurple;
                        break;
                    case "LightGreen":
                        GridPlayer.Background = Brushes.LightGreen;
                        MenuMain.Background = Brushes.LightGreen;
                        GridTitle.Background = Brushes.LightGreen;
                        Head.Background = Brushes.LightGreen;
                        break;
                    case "HotPink":
                        GridPlayer.Background = Brushes.HotPink;
                        MenuMain.Background = Brushes.HotPink;
                        GridTitle.Background = Brushes.HotPink;
                        Head.Background = Brushes.HotPink;
                        break;
                    case "LightSlateGray":
                        GridPlayer.Background = Brushes.LightSlateGray;
                        MenuMain.Background = Brushes.LightSlateGray;
                        GridTitle.Background = Brushes.LightSlateGray;
                        Head.Background = Brushes.LightSlateGray;
                        break;
                    case "Gold":
                        GridPlayer.Background = Brushes.Gold;
                        MenuMain.Background = Brushes.Gold;
                        GridTitle.Background = Brushes.Gold;
                        Head.Background = Brushes.Gold;
                        break;
                    case "DarkViolet":
                        GridPlayer.Background = Brushes.DarkViolet;
                        MenuMain.Background = Brushes.DarkViolet;
                        GridTitle.Background = Brushes.DarkViolet;
                        Head.Background = Brushes.DarkViolet;
                        break;
                    case "Crimson":
                        GridPlayer.Background = Brushes.Crimson;
                        MenuMain.Background = Brushes.Crimson;
                        GridTitle.Background = Brushes.Crimson;
                        Head.Background = Brushes.Crimson;
                        break;
                    default:
                        BrushConverter brushConverter = new BrushConverter();
                        GridPlayer.Background = (Brush)brushConverter.ConvertFromString(ColorsTag);
                        MenuMain.Background = (Brush)brushConverter.ConvertFromString(ColorsTag);
                        GridTitle.Background = (Brush)brushConverter.ConvertFromString(ColorsTag);
                        Head.Background = (Brush)brushConverter.ConvertFromString(ColorsTag);
                        break;
                }
            }
        }

        /// <summary>
        /// 保存选择的播放模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveMode()
        {
            FileInfo finfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Mode.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Mode.xml");
                XmlNode childNode = xmlDoc.SelectSingleNode("Mode");
                XmlElement element = (XmlElement)childNode;
                element["Change"].InnerText = ModePlayTag;
                xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Mode.xml");
            }
        }

        /// <summary>
        /// 播放模式选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChangeModePlay()
        {
            FileInfo finfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Mode.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Mode.xml");
                XmlNode childNode = xmlDoc.SelectSingleNode("Mode");
                XmlElement element = (XmlElement)childNode;
                ModePlayTag = element["Change"].InnerText;
            }
        }
        /// <summary>
        /// 读取声音大小信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadVolumn()
        {
            FileInfo finfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Volumn.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Volumn.xml");
                XmlNode childNodes = xmlDoc.SelectSingleNode("Volumn");
                XmlElement element = (XmlElement)childNodes;
                SldVolumn.Value = double.Parse(element["Size"].InnerText);
            }
        }

        /// <summary>
        /// 保存声音大小信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveVolumn()
        {
            FileInfo finfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Volumn.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Volumn.xml");
                XmlNode childNodes = xmlDoc.SelectSingleNode("Volumn");
                XmlElement element = (XmlElement)childNodes;
                element["Size"].InnerText = SldVolumn.Value.ToString().Trim();
                xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "Volumn.xml");
            }
        }

        /// <summary>
        /// 添加保存List.xml信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddList()
        {
            FileInfo finfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
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

                xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
            }
        }
        /// <summary>
        /// 判定文件是否存在于List.xml文件中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public bool IsExist()
        {
            FileInfo finfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
            if (finfo.Exists)
            {
                XPathDocument xPath = new XPathDocument(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
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
            FileInfo finfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
            if (finfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
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
        private void NextPlayer()
        {
            FileInfo fileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
            if (fileInfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
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
                        changes = "";
                    }
                    else
                    {
                        foreach (XmlNode node in nodes)
                        {
                            XmlElement element = (XmlElement)node;
                            if (name.Equals(element["Name"].InnerText))
                            {
                                changes = element["Path"].InnerText;
                            }
                        }
                    }
                }

            }
        }

        /// <summary>
        /// 定义播放上一部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrePlayer()
        {
            FileInfo fileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
            //System.Windows.Forms.MessageBox.Show(""+ fileInfo);
            if (fileInfo.Exists)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
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
                        changes = "";
                    }
                    else
                    {
                        foreach (XmlNode node in nodes)
                        {
                            XmlElement element = (XmlElement)node;
                            if (name.Equals(element["Name"].InnerText))
                            {
                                changes = element["Path"].InnerText;
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
            OpenFile.FileName = changes;
            UserControlClass.FileName = OpenFile.SafeFileName;

            if (string.IsNullOrEmpty(changes))
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
                FileInfo finfo = new FileInfo("" + changes + "");
                if (finfo.Exists)
                {
                    UserControlClass.sc2.FInkCanvas_Player.Background = Brushes.White;
                    UserControlClass.sc2.FInkCanvas_Player.Children.Clear();
                    //清空flash播放文件
                    //if (FlashShock != null)
                    //{
                    //    FlashShock.Dispose();
                    //}

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
                        Open(changes);
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
            string from = "Media Files(*.wmv;*.avi;*.asf;*.mpg;*.rmvb)|*.wmv;*.avi;*.asf;*.mpg;*.rmvb;*.mp3;*.wma;*.wav;*.mid|Flash Files(*.swf)|*.swf|All Files(*.*)|*.*";
            OpenFile.FileName = "";
            OpenFile.Filter = from;
            OpenFile.Multiselect = false;
            OpenFile.ShowDialog();
            if (OpenFile.CheckFileExists && !string.IsNullOrEmpty(OpenFile.FileName))
            {
                ListNodeName = OpenFile.SafeFileName;
                ListNodePath = OpenFile.FileName;
                if (!IsExist())
                {
                    AddList();
                }
                SelectXml();

                JudgeSameName(ListNodePath);
                //
                if (UserControlClass.FileName != null)
                {
                    ListView.SelectedValue = UserControlClass.FileName;
                }
            }
        }
        /// <summary>
        /// 添加图片的图片选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                AddImage();
            }
        }
        /// <summary>
        /// 添加图片
        /// </summary>
        private void AddImage()
        {
            Microsoft.Win32.OpenFileDialog OpenFile = new Microsoft.Win32.OpenFileDialog();
            string from = "JPG文件|*.jpg|PNG图片|*.png|ICO图片|*.ico|JPEG图片|*.jpeg|GIF图片|*.gif|BMP图片|*.bmp|All Image Files | *.bmp; *.ico; *.gif; *.jpeg; *.jpg; *.png";
            OpenFile.FileName = "";
            //设置文件类型
            OpenFile.Filter = from;
            OpenFile.ShowDialog();
            if (OpenFile.CheckFileExists && !string.IsNullOrEmpty(OpenFile.FileName))
            {
                //实例一个fileinfo文件路径
                FileInfo picFile = new FileInfo(OpenFile.FileName);
                //获取文件的完整目录
                string image = picFile.FullName;
                //获取文件类型，截取图片的后缀名并转为大写
                string exname = image.Substring(image.LastIndexOf(".") + 1).ToUpper();
                if (exname == "JPG" || exname == "JPEG" || exname == "GIF" || exname == "PNG" || exname == "BMP" || exname == "ICO")
                {
                    if (picFile.Length > 524288)
                    {
                        System.Windows.Forms.MessageBox.Show("上传的图片大于 0.5M, 请处理图片后再上传");
                        return;
                    }
                    else
                    {
                        UserControlClass.Addimage(data, image);
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("当前文件类型不支持上传");
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
                FileInfo finfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
                if (finfo.Exists)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
                    XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");
                    XmlNodeList nodeList = xmlNode.SelectNodes("List");
                    foreach (XmlNode node in nodeList)
                    {
                        if (node.SelectSingleNode("Name").InnerText.Equals(name))
                        {

                            xmlNode.RemoveChild(node);
                            xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
                            SelectXml();
                        }
                    }
                }

                RemoveSameNameImage(name);


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
            foreach (var item in ListView.Items)
            {
                RemoveSameNameImage(item.ToString());
            }
            //创建filestream对象流，读取文件
            FileInfo finfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
            if (finfo.Exists)
            {
                //实例一个xml文档对象
                XmlDocument xmlDoc = new XmlDocument();
                //加载url路径里面的文件
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
                //读取xml里面的lists单个节点
                XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists");
                //读取xml里面的list节点集合对象
                XmlNodeList nodeList = xmlNode.SelectNodes("List");
                foreach (XmlNode node in nodeList)
                {
                    //移除子节点
                    xmlNode.RemoveChild(node);
                    //将xml保存到指定的文件
                    xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\XML\" + "List.xml");
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
        /// 验证同级目录下有没有和视频文件名称相同的图片，如果有就添加图片
        /// </summary>
        /// <param name="path"></param>
        private void JudgeSameName(string path)
        {
            string FilePath = Path.GetDirectoryName(path);
            string FilePathName = Path.GetFileNameWithoutExtension(path);
            List<string> ImageFiles = new List<string>();
            //获取程序所在目录中的images文件夹
            DirectoryInfo di = new DirectoryInfo(FilePath);
            foreach (var file in di.GetFiles())
            {
                //验证是否为图片文件 
                if (file.Extension.ToLower() == ".jpg" || file.Extension.ToLower() == ".png" || file.Extension.ToLower() == ".jpeg" || file.Extension.ToLower() == ".bmp" || file.Extension.ToLower() == ".ico" || file.Extension.ToLower() == ".gif")
                {
                    //视频名字与图片名字相同
                    if (FilePathName.Equals(Path.GetFileNameWithoutExtension(file.FullName)))
                    {
                        ImageFiles.Add(file.FullName);
                    }
                }
            }
            if (ImageFiles.Count > 0)
            {
                foreach (string file in ImageFiles)
                {
                    UserControlClass.Addimage(data, file);
                }
            }
        }

        /// <summary>
        /// 删除播放列表选中项的名称相同的图片
        /// </summary>
        /// <param name="selectvalue"></param>
        private void RemoveSameNameImage(string selectvalue)
        {
            string[] names = selectvalue.Split('.');
            string name = null;
            for (int i = 0; i < names.Length - 1; i++)
            {
                name += "." + names[i];
            }
            if (name != null)
            {
                string imagename = name.Remove(0, 1);
                DirectoryInfo path = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + @"\UseThings\images");
                FileInfo[] files = path.GetFiles();
                foreach (FileInfo file in files)
                {
                    //筛选出图片文件
                    if (file.Name.EndsWith(".jpg") || file.Name.EndsWith(".png") || file.Name.EndsWith(".jpeg") || file.Name.EndsWith(".bmp") || file.Name.EndsWith(".gif") || file.Name.EndsWith(".ico"))
                    {
                        if (imagename.Equals(Path.GetFileNameWithoutExtension(file.FullName)))
                        {
                            File.Delete(file.FullName);
                            data.Clear();
                            UserControlClass.ListviewAddImage(data);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 图片双击播放影片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // timer.Stop();
            //图片双击\
            if (e.ClickCount == 2)
            {
                string imagepath = data[lv.SelectedIndex].Filepath;
                string filename = Path.GetFileNameWithoutExtension(imagepath);
                foreach (var item in ListView.Items)
                {
                    string[] names = item.ToString().Split('.');
                    string name = null;
                    for (int i = 0; i < names.Length - 1; i++)
                    {
                        name += "." + names[i];
                    }
                    string fname = name.Remove(0, 1);

                    if (name != null)
                    {
                        if (fname.Equals(filename))
                        {
                            ScreenJug();
                            memoryPlay = true;
                            ListPlay(item.ToString());
                            UserControlClass.MSStatus = MediaStatus.Pause;
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 随着图片的变换更改播放列表里面的选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = lv.SelectedIndex;

            if (index > -1)
            {
                string imagepath = data[lv.SelectedIndex].Filepath;
                string filename = Path.GetFileNameWithoutExtension(imagepath);
                foreach (var item in ListView.Items)
                {
                    string[] names = item.ToString().Split('.');
                    string name = null;
                    for (int i = 0; i < names.Length - 1; i++)
                    {
                        name += "." + names[i];
                    }
                    string fname = name.Remove(0, 1);
                    if (fname.Equals(filename))
                    {
                        // ListView.Focus();
                        ListView.SelectedValue = item.ToString();
                    }
                }
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
           // UserLogin lg = new UserLogin(this);
            //lg.ShowDialog();
        }
        private void but_Cancel_Click(object sender, RoutedEventArgs e)
        {
            UserControlClass.username = "";
            JugUser();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        
    }
}
