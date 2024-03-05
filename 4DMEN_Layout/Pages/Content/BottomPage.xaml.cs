using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace USIPD102_4DMEN.Pages
{
    /// <summary>
    /// BottomPage.xaml 的互動邏輯
    /// </summary>
    public partial class BottomPage : UserControl
    {
        #region 屬性
        bool system_run = true;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion 屬性
        #region 頁面動作
        public static Action<string> ChangeSystemStatus;
        public static Action<string> ChangeSystemTime;
        public static Action<string> ChangeSystemVersion;
        public static Action CloseSystem;
        public static Action<bool> ChangeSfisStatus;
        public static Action<bool> ChangeLaserHeightStatus;
        public static Action<bool> ChangeLaserStatus;
        #endregion 頁面動作
        #region 方法
        public void UpdateTime()
        {
            while (system_run)
            {
                try
                {
                    ChangeSystemTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    Thread.Sleep(1000);
                }
                catch(Exception ex)
                {

                }
                
            }

        }
        #endregion 方法
        public BottomPage()
        {
            try
            {
                InitializeComponent();
                #region Actions

                ChangeSystemStatus = status =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        if (status.ToUpper().Contains("ERROR") || status.ToUpper().Contains("PAUSE")) SystemStatusTxt.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF0000");
                        else if (status.ToUpper().Contains("WARN") || status.ToUpper().Contains("RUN")) SystemStatusTxt.Foreground = (Brush)new BrushConverter().ConvertFrom("#F9F900");
                        else if (status.ToUpper().Contains("IDLE")) SystemStatusTxt.Foreground = (Brush)new BrushConverter().ConvertFrom("#ADADAD");
                        else SystemStatusTxt.Foreground = (Brush)new BrushConverter().ConvertFrom("#1A237E");
                        SystemStatusTxt.Text = status;
                    });

                };
                ChangeSfisStatus = status =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        string url_path = status ? "Signal/Green_Button.png" : "Signal/White_Button.png";
                        var startup_path = Directory.GetCurrentDirectory();
                        DisplaySfisSignalImg.Source = new BitmapImage(new Uri($"{startup_path}\\{url_path}"));
                        DisplaySfusSignalTxt.Text = status ? "ON" : "OFF";
                    });
                };
                ChangeLaserHeightStatus = status =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        LaserHeiStatusTxt.Text = status ? "連線" : "未連線";
                        LaserHeiStatusTxt.Foreground = status ? (Brush)new BrushConverter().ConvertFrom("#00FF00") : (Brush)new BrushConverter().ConvertFrom("#FF0000");
                    });
                };
                ChangeLaserStatus = status =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        LaserStatusTxt.Text = status ? "連線" : "未連線";
                        LaserStatusTxt.Foreground = status ? (Brush)new BrushConverter().ConvertFrom("#00FF00") : (Brush)new BrushConverter().ConvertFrom("#FF0000");
                    });
                };
                ChangeSystemTime = time => Dispatcher.Invoke(() => DisplayTimeTxt.Text = time);
                ChangeSystemVersion = version => Dispatcher.Invoke(() => DisplayVersionTxt.Text = $"ver. {version}.");
                CloseSystem = () => Dispatcher.Invoke(() => system_run = false);
                #endregion Actions

                Thread t = new Thread(() => UpdateTime()) { IsBackground = true, Name = "UpdateTime" };
                t.Start();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "頁面資訊初始化錯誤。");
            }
        }
    }
}
