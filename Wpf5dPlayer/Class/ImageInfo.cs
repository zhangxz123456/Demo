using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MoviePlayer.Class
{
    /// <summary>
    /// 图片管理类
    /// </summary>
    public class ImageInfo : INotifyPropertyChanged
    {                        
        private string filepath;
        private string _movieName;
        private double _width;
        private double _height;
        private BitmapImage _imagePath;
        public double Width
        {
            get { return _width; }
            set
            {
                if (value != _width)
                {
                    _width = value;
                    this.NotifyPropertyChanged("Width");
                }
            }
        }
        public double Height
        {
            get { return _height; }
            set
            {
                if (value != _height)
                {
                    _height = value;
                    this.NotifyPropertyChanged("Height");
                }
            }
        }
        public BitmapImage ImagePath
        {
            get { return ImagePath1; }
            set
            {
                if (value != ImagePath1)
                {
                    ImagePath1 = value;
                    this.NotifyPropertyChanged("ImagePath");
                }
            }
        }
        public string Filepath
        {
            get
            {
                return filepath;
            }

            set
            {
                filepath = value;
            }
        }
        public BitmapImage ImagePath1
        {
            get
            {
                return _imagePath;
            }
            set
            {
                _imagePath = value;
            }
        }

        public string MovieName
        {
            get
            {
                return _movieName;
            }

            set
            {
                _movieName = value;
            }
        }

        public ImageInfo(string path)
        {
            this.ImagePath = getImage(path);
            this.filepath = path;
            this.MovieName = Path.GetFileNameWithoutExtension(path);
        }
        /// <summary>
        /// 得到图片路径，解析图片来实现加载
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        //由于wpf本身自带的imagesource解析图片文件占用图片文件资源，所以自己编译一个不占用图片资源的解析方法
        public BitmapImage getImage(string file)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;        //这一行很重要
            bmp.UriSource = new Uri(file);
            bmp.EndInit();
            return bmp;
        }
       
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
