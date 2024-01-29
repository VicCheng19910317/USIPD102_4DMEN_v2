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
using _4DMEN_Layout.Pages;
using _4DMEN_Library.Model;

namespace _4DMEN_Layout
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window, MainView
    {
        #region 屬性參數
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();
        private readonly string version = "2.0.0.20240129";
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
                //if (response_name == "load_arms_params")
                //    LoadArmsParam((LoadArmsParamArgs)e);
                //else if (response_name == "send_arms_action")
                //    SendArmsActionResponse((SendArmsActionResponseArgs)e);
                //else if (response_name == "load_plc_param")
                //    LoadPLCParam((DisplayPLCActionArgs)e);
                //else if (response_name == "send_plc_action")
                //    SendPLCActionResponse((SendPLCActionResponseArgs)e);
                //else if (response_name == "load_reader_param")
                //    LoadReaderParam((LoadReaderParamArgs)e);
                //else if (response_name == "send_reader_action")
                //    SendReaderActionResponse((SendReaderActionResponseArgs)e);
                //else if (response_name == "load_inspector_param")
                //    LoadInspectorParam((LoadInspectorParamArgs)e);
                //else if (response_name == "send_inspector_action")
                //    SendInspectorActionResponse((SendInspectorActionResponseArgs)e);
                //else if (response_name == "load_weight_param")
                //    LoadWeightParam((LoadRS232ParamArgs)e);
                //else if (response_name == "send_weight_action")
                //    SendWeightActionResponse((SendWeightActionResponseArgs)e);
                //else if (response_name == "load_glue_param")
                //    LoadGlueParam((LoadGlueParamArgs)e);
                //else if (response_name == "send_glue_action")
                //    SendGlueActionResponse((SendGlueActionResponseArgs)e);
                //else if (response_name == "system_initialize")
                //    SystemInitResponse((SendInitResponseArgs)e);
                //else if (response_name == "send_flow_error")
                //    SendFlowErrorResponse((SendMessageBoxArgs)e);
                //else if (response_name == "show_message")
                //    MessageBox.Show(((SendMessageBoxArgs)e).Message, ((SendMessageBoxArgs)e).Name, ((SendMessageBoxArgs)e).Button, ((SendMessageBoxArgs)e).Image);
                //else if (response_name == "get_arms_shift_response")
                //    GetArmsShiftResponse((ArmsShiftArgs)e);
                //else if (response_name == "get_needle_teach_response")
                //    GetNeedleTeachResponse((NeedleTeachArgs)e);
                //else if (response_name == "load_system_param")
                //    LoadSystemParamResponse((SystemParamArgs)e);
                //else if (response_name == "send_single_station_response")
                //    SingleStationRunResponse((SendSingleStationFlowArgs)e);
                //else if (response_name == "update_all_loop_data")
                //    UpdateAllLoopData((UpdateCaseDatasArgs)e);
                //else if (response_name == "run_main_flow")
                //    RunMainFlow((RunMainFlowArgs)e);
                //else if (response_name == "load_glue_life")
                //    LoadGlueLife((SetGlueLifeArgs)e);
                //else if (response_name == "update_logger_data")
                //    UpdateLoggerData((UpdateLoggerArgs)e);
                //else if (response_name == "load_log_datas")
                //    LoadLogDatas((UpdateLoggerArgs)e);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Presenter回傳資料錯誤。");
            }

        }
        #endregion MVP架構事件
        #region 頁面屬性
        private MainPage main_page = null;
        private SystemSettingPage setting_page = null;
        private BottomPage bottom_page = null;
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
                main_page = new MainPage();
                setting_page = new SystemSettingPage();
                bottom_page = new BottomPage();
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
