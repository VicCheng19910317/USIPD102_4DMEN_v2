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
    /// StationFlowPage.xaml 的互動邏輯
    /// </summary>
    public partial class StationFlowPage : UserControl
    {
        #region 屬性
        private Logger logger = LogManager.GetCurrentClassLogger();
        #endregion 屬性
        #region 靜態動作
        public static Action<List<CaseData>> UpdateCaseData;
        List<CaseData> caseDatas = new List<CaseData>();
        #endregion 靜態動作
        public StationFlowPage()
        {
            InitializeComponent();
            caseDatas.Add(new CaseData());
            CaseDataDG.ItemsSource = null;
            CaseDataDG.ItemsSource = caseDatas;
            #region 靜態動作
            UpdateCaseData = data =>
            {
                Dispatcher.Invoke(() =>
                {
                    caseDatas = data;
                    CaseDataDG.ItemsSource = null;
                    CaseDataDG.ItemsSource = caseDatas;
                });
            };

            #endregion 靜態動作
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var name = (sender as Button).Name.Replace("Btn", "").Replace("Case", "");
                if (MessageBox.Show($"開始執行 {(sender as Button).Content.ToString()} 單動流程?", "系統訊息", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                {
                    MainWindow.SendPresenterData("send_single_station_flow", new SendSingleStationFlowArgs { CaseDatas = caseDatas, Station = name });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "執行單動流程失敗。");
            }


        }
        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("確定將資料清空?", "系統訊息", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    caseDatas.Clear();
                    caseDatas.Add(new CaseData());
                    CaseDataDG.ItemsSource = null;
                    CaseDataDG.ItemsSource = caseDatas;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "重置單動流程失敗。");
            }


        }
        private void FlowButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var name = (sender as Button).Name.Replace("Btn", "").Replace("Case", "");
                MainWindow.SendPresenterData("send_single_station_control_flow", new SendSingleStationFlowArgs { Station = name });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "流程按鈕設定失敗。");
            }

        }
    }
}
