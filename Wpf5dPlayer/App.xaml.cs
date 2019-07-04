using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MoviePlayer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex;
        public App()
        {
            this.Startup += new StartupEventHandler(App_StartUp);
        }

        void App_StartUp(object sender,StartupEventArgs e)
        {
            bool ret;
            mutex = new System.Threading.Mutex(true, "MoviePlayer", out ret);

            if (!ret)
            {
                MessageBox.Show("已有一个程序实例运行");
                Environment.Exit(0);
            }
        }
    }
}
