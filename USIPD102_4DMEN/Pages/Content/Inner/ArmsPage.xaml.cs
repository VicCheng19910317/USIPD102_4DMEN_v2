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
using _4DMEN_Library.Model;
using NLog;

namespace USIPD102_4DMEN.Pages
{
    /// <summary>
    /// UserControl1.xaml 的互動邏輯
    /// </summary>
    public partial class ArmsPage : UserControl
    {
        #region 屬性
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion 屬性
        #region 靜態動作
        public static Action<List<EsponArmsParam>> LoadArmsData;
        public static Action<string, string> SetResponseMessage;
        #endregion 靜態動作
        public ArmsPage()
        {
            InitializeComponent();
            #region 靜態動作
            LoadArmsData = datas =>
            {
                Dispatcher.Invoke(() =>
                {
                    datas.ForEach(data =>
                    {
                        if (data.Name == "Arms In")
                        {
                            ArmsIn_IP.Text = data.IP;
                            ArmsIn_Port.Text = data.Port.ToString();
                            ArmsIn_Timeout.Text = data.Timeout.ToString();
                            List<DisplayArmsAction> _action = new List<DisplayArmsAction>();
                            data.Action_Name.ForEach(x => _action.Add(new DisplayArmsAction { Name = x }));
                            InArmsDG.ItemsSource = _action;
                            InPickIndexCB.SelectedIndex = 0;
                        }
                        else if (data.Name == "Arms Lid")
                        {
                            ArmsLid_IP.Text = data.IP;
                            ArmsLid_Port.Text = data.Port.ToString();
                            ArmsLid_Timeout.Text = data.Timeout.ToString();
                            List<DisplayArmsAction> _action = new List<DisplayArmsAction>();
                            data.Action_Name.ForEach(x => _action.Add(new DisplayArmsAction { Name = x }));
                            LidArmsDG.ItemsSource = _action;
                            LidPickIndexCB.SelectedIndex = 0;
                        }
                        else if (data.Name == "Arms Out")
                        {
                            ArmsOut_IP.Text = data.IP;
                            ArmsOut_Port.Text = data.Port.ToString();
                            ArmsOut_Timeout.Text = data.Timeout.ToString();
                            List<DisplayArmsAction> _action = new List<DisplayArmsAction>();
                            data.Action_Name.ForEach(x => _action.Add(new DisplayArmsAction { Name = x }));
                            OutArmsDG.ItemsSource = _action;
                            OutPickIndexCB.SelectedIndex = 0;
                        }
                    });
                });
            };
            SetResponseMessage = (arms, message) =>
            {
                var message_textbox = arms == "IN" ? InArmsMessageTB : arms == "OUT" ? OutArmsMessageTB : LidArmsMessageTB;
                Dispatcher.Invoke(() => message_textbox.Text = message_textbox.Text.Insert(0, $"{message}\n"));
            };
            #endregion 靜態動作
        }
        private void ArmsInBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayArmsAction action = (DisplayArmsAction)((Button)e.Source).DataContext;
                MainWindow.SendPresenterData("send_arms_action", new SendArmsActionArgs { action = action.Name, arms = "IN" });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "手臂入料失敗。");
            }

        }
        private void ArmsLidBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayArmsAction action = (DisplayArmsAction)((Button)e.Source).DataContext;
                MainWindow.SendPresenterData("send_arms_action", new SendArmsActionArgs { action = action.Name, arms = "LID" });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "手臂組裝失敗。");
            }

        }
        private void ArmsOutBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayArmsAction action = (DisplayArmsAction)((Button)e.Source).DataContext;
                MainWindow.SendPresenterData("send_arms_action", new SendArmsActionArgs { action = action.Name, arms = "OUT" });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "手臂出料失敗。");
            }

        }
        private void ArmsLogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                var type = btn.Name.Contains("In") ? "IN" : btn.Name.Contains("Out") ? "OUT" : "LID";
                MainWindow.SendPresenterData("send_arms_action", new SendArmsActionArgs { action = "logout", arms = type });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "手臂連線測試失敗。");
            }

        }
        private void ArmsLoginBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                var type = btn.Name.Contains("In") ? "IN" : btn.Name.Contains("Out") ? "OUT" : "LID";
                MainWindow.SendPresenterData("send_arms_action", new SendArmsActionArgs { action = "CTRL", arms = type });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "手臂控制失敗。");
            }

        }
        private void PickIndexCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var index = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content.ToString();
                var name = (sender as ComboBox).Name;
                var tmp_dg = name.Substring(0, 3).ToLower().Contains("out") ? OutArmsDG : name.Substring(0, 3).ToLower().Contains("in") ? InArmsDG : LidArmsDG;
                var items = tmp_dg.ItemsSource as List<DisplayArmsAction>;
                var _item = items.Where(x => x.Name.Contains("SYPU")).FirstOrDefault();
                if(_item != null)
                    _item.Name = $"SYPU;{index}";
                _item = items.Where(x => x.Name.Contains("SYPK")).FirstOrDefault();
                if (_item != null)
                    _item.Name = $"SYPK;{index}";
                _item = items.Where(x => x.Name.Contains("PROD")).FirstOrDefault();
                if (_item != null)
                    _item.Name = $"PROD;{int.Parse(index) - 1}";
                tmp_dg.ItemsSource = null;
                tmp_dg.ItemsSource = items;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "手臂設定取料順序失敗。");
            }

        }
    }
    
}
