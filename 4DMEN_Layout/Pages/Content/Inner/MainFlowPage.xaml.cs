using _4DMEN_Library.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// MainFlowPage.xaml 的互動邏輯
    /// </summary>
    public partial class MainFlowPage : UserControl
    {
        #region 屬性
        bool init = true;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private List<Image> signal_img = new List<Image>();
        #endregion 屬性
        #region 靜態動作
        public static Action<SfisParam, int, int> SetWorksheetInfo;
        public static Action<SystemFlow> SetSystemFlow;
        public static Action<List<CaseData>> UpdateStationSignal;
        public static Action ResetRunFlow;
        public static Action SetRunFlow;
        #endregion 靜態動作
        #region 功能
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChildren<T>(ithChild)) yield return childOfChild;
            }
        }
        #endregion 功能
        public MainFlowPage()
        {

            InitializeComponent();
            signal_img.Clear();
            #region 有問題待看
            signal_img.Add(DisplayStation1);
            signal_img.Add(DisplayStation2);
            signal_img.Add(DisplayStation3);
            signal_img.Add(DisplayStation4);
            signal_img.Add(DisplayStation5);
            signal_img.Add(DisplayStation6);
            signal_img.Add(DisplayStation7);
            signal_img.Add(DisplayStation8);
            signal_img.Add(DisplayStation9);
            signal_img.Add(DisplayStation10);
            signal_img.Add(DisplayStation11);
            signal_img.Add(DisplayStation12);
            //foreach (var img in )
            //{
            //    if (img.Name.Contains("DisplayStation"))
            //        signal_img.Add(img);
            //}
            #endregion 有問題待看
            #region 靜態動作
            SetWorksheetInfo = (data, count, recipe) =>
            {
                Dispatcher.Invoke(() =>
                {
                    OPNameTxt.Text = data.WorkerID;
                    WorksheetTxt.Text = data.TicketID;
                    LidLotIDTxt.Text = data.LidLotID;
                    NutLotTxt.Text = data.NutNo;
                    RunCountTxt.Text = count.ToString();
                    RecipeTxt.Text = recipe.ToString();
                    init = false;
                });
            };
            SetSystemFlow = data =>
            {
                Dispatcher.Invoke(() =>
                {
                    CaseAssembleCB.IsChecked = data.CaseAssemble;
                    CaseScanCB.IsChecked = data.CaseScan;
                    CaseBendingCB.IsChecked = data.CaseBending;
                    CasePlateCB.IsChecked = data.CasePlate;
                    CaseEstHeightCB.IsChecked = data.CaseEstHeight;
                    CaseNgOutCB.IsChecked = data.CaseNgOut;
                    CaseMarkingCB.IsChecked = data.CaseMarking;
                });

            };
            ResetRunFlow = () =>
            {
                Dispatcher.Invoke(() =>
                {
                    StartBtn.IsEnabled = true;
                    StopBtn.IsEnabled = ResumeBtn.IsEnabled = PauseBtn.IsEnabled = false;
                    BottomPage.ChangeSystemStatus("Idle");
                });
            };
            SetRunFlow = () =>
            {
                Dispatcher.Invoke(() =>
                {
                    StartBtn.IsEnabled = false;
                    StopBtn.IsEnabled = ResumeBtn.IsEnabled = PauseBtn.IsEnabled = true;
                    BottomPage.ChangeSystemStatus("Pause");
                });
            };
            UpdateStationSignal = datas =>
            {
                Dispatcher.Invoke(() =>
                {
                    signal_img.ForEach(x =>
                    {
                        var _index = int.Parse(x.Tag.ToString());
                        if (datas.Where(d => d.Station == _index && d.IsRun).Count() > 0)
                        {
                            x.Source = new BitmapImage(new Uri($"{Directory.GetCurrentDirectory()}\\Signal\\Green_Button.png"));
                            x.Visibility = Visibility.Visible;
                        }
                        else if (datas.Where(d => d.Station == _index && !d.IsRun).Count() > 0)
                        {
                            x.Source = new BitmapImage(new Uri($"{Directory.GetCurrentDirectory()}\\Signal\\Red_Button.png"));
                            x.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            x.Source = new BitmapImage(new Uri($"{Directory.GetCurrentDirectory()}\\Signal\\White_Button.png"));
                            x.Visibility = Visibility.Hidden;
                        }
                    });
                });

            };
            #endregion 靜態動作
        }

        private void Sheet_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int run_count, recipe;
                if (init) return;
                if (!int.TryParse(RunCountTxt.Text, out run_count))
                {
                    MessageBox.Show("Run Count Setting Error, Please Type Again.", "Typing Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!int.TryParse(RecipeTxt.Text, out recipe))
                {
                    MessageBox.Show("Recipe Setting Error, Please Type Again.", "Typing Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                MainWindow.SendPresenterData("set_worksheet", new SetWorkSheetArgs { WorkerID = OPNameTxt.Text, TicketID = WorksheetTxt.Text, LidLotID = LidLotIDTxt.Text, NutLotID = NutLotTxt.Text, RunCount = run_count, Recipe = recipe });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "設定工單資料失敗。");
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tag = (sender as Button).Content.ToString();
                if (MessageBox.Show($"{tag} Task?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel)
                    return;
                if (tag.ToLower() == "start")
                {
                    StartBtn.IsEnabled = false;
                    StopBtn.IsEnabled = ResumeBtn.IsEnabled = PauseBtn.IsEnabled = true;

                    BottomPage.ChangeSystemStatus("Running");
                }
                else if (tag.ToLower() == "stop")
                {
                    StartBtn.IsEnabled = true;
                    StopBtn.IsEnabled = ResumeBtn.IsEnabled = PauseBtn.IsEnabled = false;

                    BottomPage.ChangeSystemStatus("Idle");
                }
                else if (tag.ToLower() == "pause")
                {

                    BottomPage.ChangeSystemStatus("Pause");
                }
                else if (tag.ToLower() == "resume")
                {

                    BottomPage.ChangeSystemStatus("Running");
                }
                MainWindow.SendPresenterData("run_main_flow", new RunMainFlowArgs { Action = tag.Replace("Btn", "") });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "設定全線執行流程失敗。");
            }
        }

        private void ArmsHomeBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as Button;
                var _arm = btn.Name.Contains("In") ? "In" : btn.Name.Contains("Lid") ? "Lid" : "Out";
                if (MessageBox.Show($"Set Arms {_arm} Home?", "Arms Move Info", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    MainWindow.SendPresenterData("send_arms_action", new SendArmsActionArgs { arms = _arm, action = "HOME" });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "手臂回Home失敗。");
            }
        }

        private void StationNgBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var index = int.Parse(((ComboBoxItem)StationNgCB.SelectedItem).Content.ToString());
                if (MessageBox.Show($"Manual Setting {index} NG?", "Flow Info.", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    MainWindow.SendPresenterData("manual_ng_setting", new ManualNGSettingArgs { station_index = index });

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "手動設定NG失敗。");
            }
        }

        private void NgCountResetBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Reset NG Count?", "Flow Info.", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    MainWindow.SendPresenterData("reset_ng_count_action", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "重置NG計數失敗。");
            }
        }

        private void PCErrorResetBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Reset PC Error?", "Flow Info.", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    MainWindow.SendPresenterData("reset_pc_error_action", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "重置NG計數失敗。");
            }
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var flow = new SystemFlow
                {
                    CaseAssemble = CaseAssembleCB.IsChecked.Value,
                    CaseScan = CaseScanCB.IsChecked.Value,
                    CaseBending = CaseBendingCB.IsChecked.Value,
                    CasePlate = CasePlateCB.IsChecked.Value,
                    CaseEstHeight = CaseEstHeightCB.IsChecked.Value,
                    CaseNgOut = CaseNgOutCB.IsChecked.Value,
                    CaseMarking = CaseMarkingCB.IsChecked.Value,
                    
                };
                MainWindow.SendPresenterData("set_flow", new SetSystemFlowArgs { Flow = flow });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "設定流程開關功能失敗。");
            }
        }
    }
}
