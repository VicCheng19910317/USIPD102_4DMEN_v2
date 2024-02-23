using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using MaterialDesignColors;
using NLog;
using _4DMEN_Library;
using USIPD102_4DMEN.Pages;
using _4DMEN_Library.Model;

namespace USIPD102_4DMEN
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window, MainView
    {
        #region 屬性參數
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();
        private readonly string version = "2.31.1.20240223";
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion 屬性參數
        #region 靜態動作
        public static Action<UIElement> ChangeMainPage;
        public static Action<string> SwitchMainPage;
        public static Action<string, EventArgs> SendPresenterData;
        #endregion 靜態動作
        #region MVP架構事件

        public event EventHandler<EventArgs> PresenterSendEvent;
        private void OnPresenterSendEvent(string run_name, EventArgs e)
        {
            PresenterSendEvent?.Invoke(run_name, e);
        }
        private void Presenter_PresentResponseEvent(object sender, EventArgs e)
        {
            try
            {
                string response_name = sender as string;
                if (response_name == "show_message")
                    MessageBox.Show(((SendMessageBoxArgs)e).Message, ((SendMessageBoxArgs)e).Name, ((SendMessageBoxArgs)e).Button, ((SendMessageBoxArgs)e).Image);
                else if (response_name == "system_initialize")
                    SystemInitResponse((SendInitResponseArgs)e);
                else if (response_name == "send_arms_action")
                    SendArmsActionResponse((SendArmsActionResponseArgs)e);
                else if (response_name == "load_arms_params")
                    LoadArmsParam((LoadArmsParamArgs)e);
                else if (response_name == "send_plc_action")
                    SendPLCActionResponse((SendPLCActionResponseArgs)e);
                else if (response_name == "load_plc_param")
                    LoadPLCParam((DisplayPLCActionArgs)e);
                else if (response_name == "send_reader_action")
                    SendReaderActionResponse((SendReaderActionResponseArgs)e);
                else if (response_name == "load_reader_param")
                    LoadReaderParam((LoadReaderParamArgs)e);
                else if (response_name == "send_height_action")
                    SendLaserHeightActionResponse((SendLaserHeightActionsResponseArgs)e);
                else if (response_name == "load_laser_heigh_param")
                    LoadLaserHeightParam((LoadLaserHeightParamArgs)e);
                else if (response_name == "send_marking_action")
                    SendMarkingActionResponse((SendMarkingActionResponseArgs)e);
                else if (response_name == "load_system_param")
                    LoadSystemParam((LoadSystemParamResponseArgs)e);
                else if (response_name == "get_arms_shift_response")
                    GetArmsShiftResponse((ArmsShiftArgs)e);
                else if (response_name == "estimate_height_response_action")
                    EstimateHeightResponse((EstimateHeightResponseActionArgs)e);
                else if (response_name == "run_main_flow")
                    RunMainFlowResponse((RunMainFlowArgs)e);
                else if (response_name == "update_all_loop_data")
                    UpdateAllLoopData((UpdateCaseDatasArgs)e);
                else if (response_name == "send_single_station_response")
                    SingleStationRunResponse((SendSingleStationFlowArgs)e);
                else if (response_name == "send_flow_error")
                    SendFlowErrorResponse((SendMessageBoxArgs)e);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Presenter回傳資料錯誤。");
            }

        }
        private void LoadArmsParam(LoadArmsParamArgs e)
        {
            try
            {
                ArmsPage.LoadArmsData(e.arms_param);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "讀取手臂資料錯誤。");
            }

        }
        private void SendArmsActionResponse(SendArmsActionResponseArgs e)
        {
            try
            {
                ArmsPage.SetResponseMessage(e.arms, e.message);// e.success ? "Run Finish!" : e.message);
                if (e.show_message)
                    MessageBox.Show(e.message, "手臂回傳訊息", MessageBoxButton.OK, e.success ? MessageBoxImage.Information : MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "手臂動作回傳錯誤。");
            }

        }
        
        private void LoadPLCParam(DisplayPLCActionArgs e)
        {
            try
            {
                PLCPage.LoadPLCData(e.plc_param.plcNet.IP, e.plc_param.plcNet.Port, e.plc_param.Timeout, e.plc_action);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "讀取PLC資料錯誤。");
            }

        }
        private void SendPLCActionResponse(SendPLCActionResponseArgs e)
        {
            try
            {
                PLCPage.SetResponseMessage(e.message);//e.success ? "Run Finish!" : e.message);
                if (e.show_message)
                    MessageBox.Show(e.message, "PLC回傳訊息", MessageBoxButton.OK, e.success ? MessageBoxImage.Information : MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "PLC動作回傳錯誤。");
            }
        }
        public void SendReaderActionResponse(SendReaderActionResponseArgs e)
        {
            try
            {
                ReaderPage.SetReaderResponseMessage(e.station, e.message);//success ? "Run Finish!" : e.message);
                if (e.show_message)
                    MessageBox.Show(e.message, "條碼機傳訊息", MessageBoxButton.OK, e.success ? MessageBoxImage.Information : MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "條碼動作回傳錯誤。");
            }

        }
        private void LoadReaderParam(LoadReaderParamArgs e)
        {
            try
            {
                ReaderPage.LoadReaderData(e.reader, e.out_reader);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "初始化條碼資料錯誤。");
            }

        }
        private void SendLaserHeightActionResponse(SendLaserHeightActionsResponseArgs e)
        {
            try
            {
                ReaderPage.SetLaserHeightrResponseMessage(e.connect_state, e.channel,e.value,e.message);
                if (e.show_message)
                    MessageBox.Show(e.message, "測高機傳訊息", MessageBoxButton.OK, e.success ? MessageBoxImage.Information : MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "測高動作回傳錯誤。");
            }
        }
        private void LoadLaserHeightParam(LoadLaserHeightParamArgs e)
        {
            try
            {
                ReaderPage.LoadLaserHeightParam(e.connect_state, e.IP, e.message);
                if (!e.success)
                    MessageBox.Show(e.message, "測高機傳訊息", MessageBoxButton.OK, e.success ? MessageBoxImage.Information : MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "初始化測高機錯誤。");
            }
        }
        private void SendMarkingActionResponse(SendMarkingActionResponseArgs e)
        {
            MarkingPage.SetConnectionStatus(e.connection);
            MarkingPage.SetMessage(e.message);
            MessageBox.Show(e.message, "雷雕機傳訊息", MessageBoxButton.OK, e.success ? MessageBoxImage.Information : MessageBoxImage.Error);
        }
        public void GetArmsShiftResponse(ArmsShiftArgs e)
        {
            try
            {
                SystemParamSettingPage.SetArmsShiftData(e.Arms, e.Value);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "取得手臂偏移資料錯誤。");
            }

        }
        public void EstimateHeightResponse(EstimateHeightResponseActionArgs e)
        {
            try
            {
                SystemParamSettingPage.SetEstHeighData(e.HeightValue, e.PlaneFunc, e.Distance, e.Flatness);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "取得手臂偏移資料錯誤。");
            }
        }
        public void RunMainFlowResponse(RunMainFlowArgs e)
        {
            try
            {
                if (e.ReStartFlow)
                    MainFlowPage.ResetRunFlow();
                else
                    MainFlowPage.SetRunFlow();
                MessageBox.Show(e.Message, e.Title, MessageBoxButton.OK, e.Image);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "運行總流程錯誤。");
            }

        }
        public void UpdateAllLoopData(UpdateCaseDatasArgs e)
        {
            try
            {
                FlowDataInfoPage.UpdateCaseDatas(e.CaseDatas);
                MainFlowPage.UpdateStationSignal(e.CaseDatas);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "更新總執行流程錯誤。");
            }

        }
        public void SingleStationRunResponse(SendSingleStationFlowArgs e)
        {
            try
            {
                StationFlowPage.UpdateCaseData(e.CaseDatas);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "更新單站流程資料錯誤。");
            }

        }
        public void SendFlowErrorResponse(SendMessageBoxArgs e)
        {
            try
            {
                FlowDataInfoPage.InsertMessage(e.Message);
                BottomPage.ChangeSystemStatus("Pause");
                if (e.Button == MessageBoxButton.OKCancel)
                {
                    if (MessageBox.Show(e.Message, "流程錯誤", e.Button, e.Image) == MessageBoxResult.OK)
                    {
                        OnPresenterSendEvent("send_flow_error", new SendMessageBoxArgs { Name = e.Name });
                    }
                }
                else
                {
                    MessageBox.Show(e.Message, "流程錯誤", e.Button, e.Image);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "總流程資料回傳更新錯誤。");
            }

        }
        private void LoadSystemParam(LoadSystemParamResponseArgs e)
        {
            try
            {
                MainFlowPage.SetSystemFlow(e.Param.Flow);
                MainFlowPage.SetWorksheetInfo(e.Param.Sfis, e.Param.CaseCount, e.Param.Recipe);
                SystemParamSettingPage.SetInitParam(e.Param);
                MarkingPage.LoadMarkingParam(e.IP, e.Port, e.MarkingStatus, e.Param.MarkParam);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "初始化測高機錯誤。");
            }
        }
        #endregion MVP架構事件
        #region 頁面屬性
        private MainPage main_page = new MainPage();
        private SystemSettingPage setting_page = new SystemSettingPage();
        private BottomPage bottom_page = new BottomPage();
        #endregion 頁面屬性
        #region 實作功能
        private void ChangeMainPanel(UIElement element)
        {

            try
            {
                ChangePanel(MainPanel, element);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"更改側邊Panel UI錯誤，錯誤資訊:{ex.Message}");
            }
        }
        private void ChangePanel(Panel panel, UIElement element)
        {
            try
            {
                panel.Children.Clear();
                panel.Children.Add(element);
            }
            catch (Exception ex)
            {

                logger.Error(ex, $"更改底層Panel UI錯誤，錯誤資訊:{ex.Message}");
            }
        }
        private void SwitchMainPanel(string title)
        {
            try
            {
                switch (title)
                {
                    case "Main Page":
                        ChangeMainPage(main_page);
                        break;
                    case "System Setting":
                        ChangeMainPage(setting_page);
                        break;
                    case "System Info.":
                        ChangeMainPage(new SystemInfoPage());
                        break;
                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "頁面切換錯誤。");
            }

        }
        #region MVP回應功能
        public void SystemInitResponse(SendInitResponseArgs e)
        {
            try
            {
                if (!e.success)
                    MessageBox.Show($"初始化失敗，錯誤訊息：{e.message}。", "初始化失敗", MessageBoxButton.OK, e.success ? MessageBoxImage.Information : MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "系統初始化回傳錯誤。");
            }

        }
        #endregion MVP回應功能
        #endregion 實作功能
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                _paletteHelper.ChangeDarkMode(false);
                _paletteHelper.ChangePrimaryColor((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1A237E"));
                _paletteHelper.ChangeSecondaryColor((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#01B468"));
                ChangeMainPage = value => ChangeMainPanel(value);
                SwitchMainPage = value => SwitchMainPanel(value);
                SendPresenterData = (run_name, e) =>
                {
                    OnPresenterSendEvent(run_name, e);
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex, "頁面內容初始化錯誤。");
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                #region 底層資料關閉
                OnPresenterSendEvent("system_closing", null);
                #endregion 底層資料關閉
            }
            catch (Exception ex)
            {
                logger.Error(ex, "系統關閉錯誤。");
            }

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MainPresenter presenter = new MainPresenter(this);
                presenter.PresentResponseEvent += Presenter_PresentResponseEvent;
                #region 頁面初始化
                BottomPage.ChangeSystemVersion(version);
                MainPage.ChangeBottomPage(bottom_page);
                ChangeMainPage(main_page);
                #endregion 頁面初始化
                #region 底層資料初始化
                OnPresenterSendEvent("system_initialize", null);
                #endregion 底層資料初始化
            }
            catch (Exception ex)
            {
                logger.Error(ex, "頁面Load錯誤。");
            }

        }
    }
}
