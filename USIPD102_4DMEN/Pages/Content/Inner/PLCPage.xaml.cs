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
    /// PLCPage.xaml 的互動邏輯
    /// </summary>
    public partial class PLCPage : UserControl
    {
        #region 屬性
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion 屬性
        #region 靜態動作
        public static Action<string, int, int, List<DisplayPLCAction>> LoadPLCData;
        public static Action<int, int> SetHeightValue;
        public static Action<string> SetResponseMessage;
        #endregion 靜態動作
        public PLCPage()
        {
            try
            {
                InitializeComponent();
                #region 靜態動作
                LoadPLCData = (ip, port, timeout, data) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        PLCIPTxt.Text = ip;
                        PLCPortTxt.Text = port.ToString();
                        PLCTimeoutTxt.Text = timeout.ToString();
                        AddressPLCDG.ItemsSource = data;
                    });
                };
                SetResponseMessage = (message) =>
                {
                    var message_textbox = MessageTB;
                    Dispatcher.Invoke(() => message_textbox.Text = message_textbox.Text.Insert(0, $"{message}\n"));
                };
                SetHeightValue = (x, y) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        HeightXTxt.Text = x.ToString();
                        HeightYTxt.Text = y.ToString();
                    });
                };
                #endregion 靜態動作
            }
            catch (Exception ex)
            {
                logger.Error(ex, "頁面資料初始化失敗。");
            }
        }

        private void RunSettingBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int x, y;
                if(!int.TryParse(HeightXTxt.Text,out x) || !int.TryParse(HeightYTxt.Text, out y))
                {
                    MessageBox.Show("高度數值x,y設定錯誤，請確認。", "參數設定", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                DisplayPLCAction action = (DisplayPLCAction)((Button)e.Source).DataContext;
                MainWindow.SendPresenterData("send_plc_action", new SendPLCActionArgs { Station = action.Station, ActionDetail = action.ActionDetail, PosX = x, PosY = y });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "執行流程功能設定失敗。");
            }

        }
        private void PLCBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                var station = btn.Tag.ToString();
                var action_detail = btn.Content.ToString();
                MainWindow.SendPresenterData("send_plc_action", new SendPLCActionArgs { Station = station, ActionDetail = action_detail });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "執行流程功能設定失敗。");
            }
        }
    }
}
