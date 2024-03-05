using _4DMEN_Library.Model;
using NLog;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace USIPD102_4DMEN.Pages
{
    /// <summary>
    /// ReaderPage.xaml 的互動邏輯
    /// </summary>
    public partial class ReaderPage : UserControl
    {
        #region 屬性
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion 屬性
        #region 靜態動作
        public static Action<string, string> SetReaderResponseMessage;
        public static Action<KeyenceReaderParam, KeyenceReaderParam> LoadReaderData;

        public static Action<bool, int, float, string> SetLaserHeightResponseMessage;
        public static Action<bool> SetConnectionStatus;
        public static Action<bool, string, string> LoadLaserHeightParam;
        #endregion 靜態動作
        public ReaderPage()
        {
            InitializeComponent();
            #region 靜態動作
            SetConnectionStatus = status =>
            {
                Dispatcher.Invoke(() =>
                {
                    HeightStatusTxt.Text = status ? "連線" : "未連線";
                    HeightStatusTxt.Foreground = status ? (Brush)new BrushConverter().ConvertFrom("#00FF00") : (Brush)new BrushConverter().ConvertFrom("#FF0000");
                });
            };
            SetReaderResponseMessage = (station, message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var message_textbox = station.ToUpper() == "READER" ? ReaderMessageTB : OutMessageTB;
                    message_textbox.Text = message_textbox.Text.Insert(0, $"{message}\n");
                });
            };
            LoadReaderData = (reader, out_reader) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Reader_IP.Text = reader.IP;
                    Reader_Port.Text = reader.Port.ToString();
                    Reader_Timeout.Text = reader.Timeout.ToString();
                    List<DisplayReaderAction> _action = new List<DisplayReaderAction>();
                    reader.Action_Name.ForEach(x => _action.Add(new DisplayReaderAction { Name = x }));
                    ReaderDG.ItemsSource = _action;

                    Out_IP.Text = out_reader.IP;
                    Out_Port.Text = out_reader.Port.ToString();
                    Out_Timeout.Text = out_reader.Timeout.ToString();
                    _action = new List<DisplayReaderAction>();
                    out_reader.Action_Name.ForEach(x => _action.Add(new DisplayReaderAction { Name = x }));
                    OutDG.ItemsSource = _action;
                });
            };
            SetLaserHeightResponseMessage = (connect, channel, value, message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    SetConnectionStatus(connect);
                    var ch_txt = (channel == 0) ? GetCH1Text : GetCH2Text;
                    ch_txt.Text = value.ToString();
                    HightMessageTB.Text = HightMessageTB.Text.Insert(0, $"{message}\n");
                });
            };
            LoadLaserHeightParam = (connect, ip, message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    SetConnectionStatus(connect);
                    Height_IP.Text = ip;
                    HightMessageTB.Text = HightMessageTB.Text.Insert(0, $"{message}\n");
                });
            };
            #endregion 靜態動作
        }

        private void ReaderBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayReaderAction action = (DisplayReaderAction)((Button)e.Source).DataContext;
                MainWindow.SendPresenterData("send_reader_action", new SendReaderActionArgs { Action = action.Name, Station = "Reader" });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "條碼站條碼機功能失敗。");
            }

        }

        private void OutBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayReaderAction action = (DisplayReaderAction)((Button)e.Source).DataContext;
                MainWindow.SendPresenterData("send_reader_action", new SendReaderActionArgs { Action = action.Name, Station = "OUT" });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "出料站條碼機功能失敗。");
            }
        }

        private void ConnectHeightBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.SendPresenterData("send_height_action", new SendLaserHeightActionsArgs { Action = "連線", Channel = 2 });
            }
            catch(Exception ex)
            {
                logger.Error(ex, "測高站連線錯誤。");
            }
        }

        private void DisconnectHeightBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.SendPresenterData("send_height_action", new SendLaserHeightActionsArgs { Action = "中斷連線", Channel = 2 });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "測高站中斷連線錯誤。");
            }
        }

        private void GetCHBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var channel = (sender as Button).Name.Contains("CH1") ? 0 : 1;
                MainWindow.SendPresenterData("send_height_action", new SendLaserHeightActionsArgs { Action = "測高", Channel = channel });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "測高站中斷連線錯誤。");
            }
        }
    }
}
