using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace MoviePlayer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FilmSetting : Window
    {
        public static ObservableCollection<Member> memberData;        
        public FilmSetting()
        {
            InitializeComponent();
            addMember();
            ReadFilmList();
            //changeLanguage();            
        }


        public class Member : INotifyPropertyChanged
        {
            public int id { get; set; }

            string _start;

            /// <summary>
            /// 排片列表影片开始时间
            /// </summary>
            public string Start
            {
                get { return _start; }
                set { _start = value; OnPropertyChanged("Start"); }
            }

            string _end;
         
            /// <summary>
            /// 排片列表结束时间
            /// </summary>
            public string End
            {
                get { return _end; }
                set { _end = value; OnPropertyChanged("End"); }
            }

            string _movieName;

            /// <summary>
            /// 排片列表中电影文件夹名
            /// </summary>
            public string MovieName
            {
                get { return _movieName; }
                set { _movieName = value; OnPropertyChanged("MovieName"); }
            }

            string _fullMovieName;

            /// <summary>
            /// 电影文件夹全路径
            /// </summary>
            public string FullMovieName
            {
                get { return _fullMovieName; }
                set { _fullMovieName = value; OnPropertyChanged("FullMovieName"); }
            }

            //public int Age { get; set; }
            protected internal virtual void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            public event PropertyChangedEventHandler PropertyChanged;
        }

        private void changeLanguage()
        {
            textColum1.Header = "StartTime";
            textColum2.Header = "EndTime";
            textColum3.Header = "Film";
            templateColumn.Header = "Operation";
            button.Content = "Save";
            Title = "Film Setting";            
        }


        /// <summary>
        /// 初始化列表
        /// </summary>
        private void addMember()
        {
            memberData = new ObservableCollection<Member>();
            for (int i = 0; i < 10; i++)
            {
                memberData.Add(new Member()
                {
                    id = i,
                    Start = "",
                    End = "",
                    MovieName = "",
                    FullMovieName = "",
                });
            }
            dataGrid.DataContext = memberData;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog m_Dialog = new FolderBrowserDialog();
           
            DialogResult result = m_Dialog.ShowDialog();
           
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string m_Dir = m_Dialog.SelectedPath.Trim();            
            DirectoryInfo info = new DirectoryInfo(m_Dir);
            string s = info.Name;
            string s1 = info.FullName;          
            memberData[dataGrid.SelectedIndex].MovieName = s;
            memberData[dataGrid.SelectedIndex].FullMovieName = s1;
                        
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {           
            for (int i = 0; i < memberData.Count; i++)
            {
                if (memberData[i].Start != "")
                {
                    if (CheckEncode(memberData[i].Start))
                    {
                        System.Windows.MessageBox.Show(memberData[i].Start + "输入有误，不能包含中文字根");
                        memberData[i].Start = "";
                    }
                    else
                    {
                        memberData[i].Start = checkStrStartAndEnd(memberData[i].Start);
                    }
                }
                if (memberData[i].End != "")
                {
                    if (CheckEncode(memberData[i].End))
                    {
                        System.Windows.MessageBox.Show(memberData[i].End + "输入有误，不能包含中文字根");
                    }
                    else
                    {
                        memberData[i].End = checkStrStartAndEnd(memberData[i].End);
                    }
                }
            }
            SaveFilmList();        
        }


        /// <summary>
        /// 检查输入的字符串是否符合要求
        /// </summary>
        /// <param name="str">要检查的字符串</param>
        /// <returns></returns>
        private string checkStrStartAndEnd(string str)
        {
            int s = str.IndexOf(':');
            try
            {
                string strHour = str.Substring(0, s);
                string strMinute = str.Substring(s + 1);
                int strHourValue = Convert.ToInt32(strHour);
                int strMinuteValue = Convert.ToInt32(strMinute);
                if (strHourValue > 24)
                {
                    System.Windows.MessageBox.Show(str + "输入有误:小时不能大于24");
                    str = "";
                }
                if (strHour.Length == 2 && strHourValue < 10)
                {
                    System.Windows.MessageBox.Show(str + "输入有误：小时前不能有0");
                    str = "";
                }
                if (strMinuteValue > 59)
                {
                    System.Windows.MessageBox.Show(str + "输入有误:分钟不能大于59");
                    str = "";
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show(str + "输入有误");
                str = "";
            }

            return str;
        }

        static public bool CheckEncode(string srcString)
        {
            return System.Text.Encoding.UTF8.GetBytes(srcString).Length > srcString.Length;
        }
 
        private void ReadFilmList()
        {
            string xml = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 5) + @"\XML\" + "FilmList.xml";
            FileInfo finfo = new FileInfo(xml);
            if (finfo.Exists)
            {
                for (int i = 0; i < memberData.Count; i++)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(xml);
                    XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists").SelectSingleNode("List" + i.ToString());
                    XmlElement element = (XmlElement)xmlNode;                   
                    memberData[i].Start = element["StartTime"].InnerText;
                    memberData[i].End = element["StopTime"].InnerText;
                    memberData[i].MovieName = element["MovieName"].InnerText;
                    memberData[i].FullMovieName = element["FullMoviePath"].InnerText;
                }
            }
        }


        /// <summary>
        /// 保存排片数据到FilmList.xml文件中
        /// </summary>
        public void SaveFilmList()
        {
            string xml= AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 5) + @"\XML\" + "FilmList.xml";
            FileInfo finfo = new FileInfo(xml);
            if (finfo.Exists)
            {
                for (int i = 0; i < memberData.Count; i++)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(xml);
                    XmlNode xmlNode = xmlDoc.SelectSingleNode("Lists").SelectSingleNode("List"+i.ToString());
                    XmlElement element = (XmlElement)xmlNode;
                    element["StartTime"].InnerText = memberData[i].Start;
                    element["StopTime"].InnerText = memberData[i].End;
                    element["MovieName"].InnerText = memberData[i].MovieName;
                    element["FullMoviePath"].InnerText = memberData[i].FullMovieName;
                    
                    xmlDoc.Save(xml);
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            memberData[dataGrid.SelectedIndex].Start = "";
            memberData[dataGrid.SelectedIndex].End = "";
            memberData[dataGrid.SelectedIndex].MovieName = "";
            memberData[dataGrid.SelectedIndex].FullMovieName = "";
        }
    }
}
