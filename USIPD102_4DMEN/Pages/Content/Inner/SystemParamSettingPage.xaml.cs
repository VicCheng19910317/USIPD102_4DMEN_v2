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
    /// SystemParamSettingPage.xaml 的互動邏輯
    /// </summary>
    public partial class SystemParamSettingPage : UserControl
    {
        #region 屬性
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private bool init = true;
        private double shift_upper, shift_lower;
        #endregion 屬性
        #region 靜態動作
        public static Action<string, ShiftArms> SetArmsShiftData;
        public static Action<SystemParam> SetInitParam;
        public static Action<List<float>, string, List<float>, List<float>> SetEstHeighData;
        #endregion 靜態動作
        #region 功能
        public void CheckLimit(TextBox _lower, TextBox _upper, bool check_upper = false)
        {
            if (init) return;
            double lower, upper;
            if (!double.TryParse(_lower.Text, out lower))
            {
                MessageBox.Show("下極限數值設定錯誤，請確認。");
                return;
            }
            else if (!double.TryParse(_upper.Text, out upper))
            {
                MessageBox.Show("上極限數值設定錯誤，請確認。");
                return;
            }
            if (lower > upper && !check_upper)
            {
                MessageBox.Show("下極限數值無法高於上極限數值，請重新設定。");
                return;
            }
            else if (upper < lower && check_upper)
            {
                MessageBox.Show("上極限數值無法低於下極限數值，請重新設定。");
                return;
            }
        }
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
        public SystemParamSettingPage()
        {
            InitializeComponent();
            #region 靜態動作
            SetArmsShiftData = (arms, data) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (arms == "in")
                    {
                        InPickShiftXTxt.Text = data.Pick.X.ToString();
                        InPickShiftYTxt.Text = data.Pick.Y.ToString();
                        InPickShiftZTxt.Text = data.Pick.Z.ToString();
                        InPickShiftUTxt.Text = data.Pick.U.ToString();
                        InPutShiftXTxt.Text = data.Put.X.ToString();
                        InPutShiftYTxt.Text = data.Put.Y.ToString();
                        InPutShiftZTxt.Text = data.Put.Z.ToString();
                        InPutShiftUTxt.Text = data.Put.U.ToString();
                    }
                    else
                    {
                        OutPickShiftXTxt.Text = data.Pick.X.ToString();
                        OutPickShiftYTxt.Text = data.Pick.Y.ToString();
                        OutPickShiftZTxt.Text = data.Pick.Z.ToString();
                        OutPickShiftUTxt.Text = data.Pick.U.ToString();
                        OutPutShiftXTxt.Text = data.Put.X.ToString();
                        OutPutShiftYTxt.Text = data.Put.Y.ToString();
                        OutPutShiftZTxt.Text = data.Put.Z.ToString();
                        OutPutShiftUTxt.Text = data.Put.U.ToString();
                    }
                    MessageBox.Show("偏移設定取得完成", "成功");
                });

            };
            SetInitParam = data =>
           {
               Dispatcher.Invoke(() =>
               {
                   #region 基礎設定
                   NgOutCountCB.SelectedIndex = data.NgOutCountLimit - 1;
                   RecordKeepDayTxt.Text = data.DataRecordCount.ToString();
                   #endregion 基礎設定
                   #region SFIS設定
                   DisplaySfisSignalTxt.Text = data.Sfis.Enable ? "ON" : "OFF";
                   string url_path = DisplaySfisSignalTxt.Text.Contains("OFF") ? "Signal/White_Button.png" : "Signal/Green_Button.png";
                   var startup_path = Directory.GetCurrentDirectory();
                   DisplaySfisSignalImg.Source = new BitmapImage(new Uri($"{startup_path}\\{url_path}"));
                   SfisStationNameTxt.Text = data.Sfis.StationID;
                   SfisLineNameTxt.Text = data.Sfis.LineID;
                   SfisBarcodeATxt.Text = data.Sfis.BarcodeA;
                   SfisBarcodeBTxt.Text = data.Sfis.BarcodeB;
                   MarkingLevelTxt.Text = data.Sfis.InspLevel;
                   #endregion SFIS設定
                   #region 測高設定
                   LocationLV.Items.Clear();
                   data.MeasurePosition.ForEach(x => LocationLV.Items.Add($"{x.X},{x.Y}"));
                   if (data.MeasurePosition.Count > 0)
                   {
                       LocationXTxt.Text = data.MeasurePosition[0].X.ToString();
                       LocationYTxt.Text = data.MeasurePosition[0].Y.ToString();
                   }
                   FlatnessMaxTxt.Text = data.FlatnessUpperLimit.ToString();
                   if(data.HeightLimit.Count == 3)
                   {
                       HeightMin1Txt.Text = data.HeightLimit[0].Lower.ToString();
                       HeightMax1Txt.Text = data.HeightLimit[0].Upper.ToString();
                       HeightMin2Txt.Text = data.HeightLimit[1].Lower.ToString();
                       HeightMax2Txt.Text = data.HeightLimit[1].Upper.ToString();
                       HeightMin3Txt.Text = data.HeightLimit[2].Lower.ToString();
                       HeightMax3Txt.Text = data.HeightLimit[2].Upper.ToString();
                   }
                   #endregion 測高設定
                   #region 入料手臂偏移設定
                   InPickShiftXTxt.Text = data.ShiftInArms.Pick.X.ToString();
                   InPickShiftYTxt.Text = data.ShiftInArms.Pick.Y.ToString();
                   InPickShiftZTxt.Text = data.ShiftInArms.Pick.Z.ToString();
                   InPickShiftUTxt.Text = data.ShiftInArms.Pick.U.ToString();
                   InPutShiftXTxt.Text = data.ShiftInArms.Put.X.ToString();
                   InPutShiftYTxt.Text = data.ShiftInArms.Put.Y.ToString();
                   InPutShiftZTxt.Text = data.ShiftInArms.Put.Z.ToString();
                   InPutShiftUTxt.Text = data.ShiftInArms.Put.U.ToString();
                   #endregion 入料手臂偏移設定
                   #region 出料手臂偏移設定
                   OutPickShiftXTxt.Text = data.ShiftOutArms.Pick.X.ToString();
                   OutPickShiftYTxt.Text = data.ShiftOutArms.Pick.Y.ToString();
                   OutPickShiftZTxt.Text = data.ShiftOutArms.Pick.Z.ToString();
                   OutPickShiftUTxt.Text = data.ShiftOutArms.Pick.U.ToString();
                   OutPutShiftXTxt.Text = data.ShiftOutArms.Put.X.ToString();
                   OutPutShiftYTxt.Text = data.ShiftOutArms.Put.Y.ToString();
                   OutPutShiftZTxt.Text = data.ShiftOutArms.Put.Z.ToString();
                   OutPutShiftUTxt.Text = data.ShiftOutArms.Put.U.ToString();
                   #endregion 出料手臂偏移設定
                   #region 手臂偏移卡控設定
                   ShiftLowerTxt.Text = data.ShiftLimit.Lower.ToString();
                   ShiftUpperTxt.Text = data.ShiftLimit.Upper.ToString();
                   shift_lower = data.ShiftLimit.Lower;
                   shift_upper = data.ShiftLimit.Upper;
                   #endregion 手臂偏移卡控設定
                   #region 組裝精度設定
                   PlateUpperXTxt.Text = data.PlateAccuracy.X.Upper.ToString();
                   PlateLowerXTxt.Text = data.PlateAccuracy.X.Lower.ToString();
                   PlateUpperYTxt.Text = data.PlateAccuracy.Y.Upper.ToString();
                   PlateLowerYTxt.Text = data.PlateAccuracy.Y.Lower.ToString();
                   PlateUpperUTxt.Text = data.PlateAccuracy.U.Upper.ToString();
                   PlateLowerUTxt.Text = data.PlateAccuracy.U.Lower.ToString();
                   #endregion 組裝精度設定
               });
           };
            SetEstHeighData = (hei_val, func, dist, flatness) =>
            {
                Dispatcher.Invoke(() =>
                {
                    EstResultTxt.Text = hei_val.Aggregate("", (total, next) => total += total.Length == 0 ? $"{next}" : $",{next}");
                    BasePlaneTxt.Text = func;
                    PlaneDistTxt.Text = dist.Aggregate("", (total, next) => total += total.Length == 0 ? $"{next}" : $",{next}");
                    FlatnessTxt.Text = flatness.Aggregate("", (total, next) => total += total.Length == 0 ? $"{next}" : $",{next}");
                });
                

            };
            #endregion 靜態動作
        }

        private void GetArmsShift_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (init) return;
                var btn = sender as Button;
                var arms = btn.Name.Contains("In") ? "in" : "out";
                MainWindow.SendPresenterData("get_arms_shift", new ArmsShiftArgs { Arms = arms });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "取得偏移參數失敗。");
            }
        }

        private void HeightEstBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.SendPresenterData("estimate_height_action", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "測試高度功能錯誤。");
            }
        }

        private void LocationAddBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int x = 0, y = 0;
                if(!int.TryParse(LocationXTxt.Text,out x) || !int.TryParse(LocationYTxt.Text, out y))
                {
                    MessageBox.Show("設定測高位置數值錯誤，請確認。", "測高設定錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var value = $"{x},{y}";
                var item_exists = false;
                foreach (var item in LocationLV.Items)
                {
                    if (item.ToString() == value)
                        item_exists = true;
                }
                if (!item_exists)
                    LocationLV.Items.Add(value);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "設定雷射等級錯誤。");
            }
        }

        private void LocationDelBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item_name = LocationLV.SelectedItem.ToString();
                for (int i = 0; i < LocationLV.Items.Count; i++)
                {
                    var item = LocationLV.Items[i];
                    if (item.ToString() == item_name)
                        LocationLV.Items.Remove(item);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "移除雷射等級錯誤。");
            }
        }

        private void PlateLower_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (init) return;
                TextBox upper = null;
                TextBox lower = sender as TextBox;
                foreach (TextBox txt in FindVisualChildren<TextBox>(this))
                {
                    if (txt.Name == lower.Name.Replace("Lower", "Upper")) upper = txt;
                }
                CheckLimit(lower, upper);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "設定組裝下限參數失敗。");
            }
        }

        private void PlateUpper_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (init) return;
                var upper = sender as TextBox;
                TextBox lower = null;
                foreach (TextBox txt in FindVisualChildren<TextBox>(this))
                {
                    if (txt.Name == upper.Name.Replace("Upper", "Lower")) lower = txt;
                }
                CheckLimit(lower, upper, true);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "組裝上限參數設定失敗。");
            }
        }

        private void SaveParamBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var param = new SystemParam();
                int keep_day = 0;
                float flatness_upper = 0, heigh_min_1 = 0, heigh_min_2 = 0, heigh_min_3 = 0, heigh_max_1 = 0, heigh_max_2 = 0, heigh_max_3 = 0;
                if (!int.TryParse(RecordKeepDayTxt.Text, out keep_day))
                {
                    MessageBox.Show("保存紀錄天數設定錯誤，請確認。", "設定錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!float.TryParse(FlatnessMaxTxt.Text, out flatness_upper))
                {
                    MessageBox.Show("平整度異常設定錯誤，請確認。", "設定錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!float.TryParse(HeightMin1Txt.Text, out heigh_min_1) || !float.TryParse(HeightMin2Txt.Text, out heigh_min_2) || !float.TryParse(HeightMin3Txt.Text, out heigh_min_3) ||
                   !float.TryParse(HeightMax1Txt.Text, out heigh_max_1) || !float.TryParse(HeightMax2Txt.Text, out heigh_max_2) || !float.TryParse(HeightMax3Txt.Text, out heigh_max_3))
                {
                    MessageBox.Show("高度異常設定錯誤，請確認。", "設定錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                param.NgOutCountLimit = NgOutCountCB.SelectedIndex + 1;
                param.DataRecordCount = keep_day;
                param.Sfis.StationID = SfisStationNameTxt.Text;
                param.Sfis.LineID = SfisLineNameTxt.Text;
                param.Sfis.BarcodeA = SfisBarcodeATxt.Text;
                param.Sfis.BarcodeB = SfisBarcodeBTxt.Text;
                param.Sfis.InspLevel = MarkingLevelTxt.Text;
                param.Sfis.Enable = DisplaySfisSignalTxt.Text.ToLower() == "on";
                var position = new List<EstimatePosition>();
                foreach (var item in LocationLV.Items)
                {
                    var split = item.ToString().Split(',');
                    position.Add(new EstimatePosition { X = int.Parse(split[0]), Y = int.Parse(split[1]) });
                }
                param.MeasurePosition = position;
                param.FlatnessUpperLimit = flatness_upper;
                param.HeightLimit = new List<Range>
                {
                    new Range { Lower = heigh_min_1, Upper = heigh_max_1 },
                    new Range { Lower = heigh_min_2, Upper = heigh_max_2 },
                    new Range { Lower = heigh_min_3, Upper = heigh_max_3 },
                };
                BottomPage.ChangeSfisStatus(param.Sfis.Enable);
                MainWindow.SendPresenterData("save_system_param", new SystemParamArgs { Param = param });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "系統參數儲存功能失敗。");
            }
        }

        private void SetPlateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (init) return;
                PlateAccuracy accuracy = new PlateAccuracy()
                {
                    X = new Range { Lower = double.Parse(PlateLowerXTxt.Text), Upper = double.Parse(PlateUpperXTxt.Text) },
                    Y = new Range { Lower = double.Parse(PlateLowerYTxt.Text), Upper = double.Parse(PlateUpperYTxt.Text) },
                    U = new Range { Lower = double.Parse(PlateLowerUTxt.Text), Upper = double.Parse(PlateUpperUTxt.Text) },
                };
                MainWindow.SendPresenterData("set_plate_accuracy", new SetPlateAccuracyArgs { Accuracy = accuracy });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "設定組裝參數功能失敗。");
            }
        }

        private void SetArmsShift_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (init) return;
                var btn = sender as Button;
                var arms = btn.Name.Contains("In") ? "in" : "out";
                ShiftArms shift = new ShiftArms();
                if (btn.Name.Contains("In"))
                {
                    shift = new ShiftArms
                    {
                        Pick = new ShiftArmsAxis
                        {
                            X = double.Parse(InPickShiftXTxt.Text),
                            Y = double.Parse(InPickShiftYTxt.Text),
                            Z = double.Parse(InPickShiftZTxt.Text),
                            U = double.Parse(InPickShiftUTxt.Text),
                        },
                        Put = new ShiftArmsAxis
                        {
                            X = double.Parse(InPutShiftXTxt.Text),
                            Y = double.Parse(InPutShiftYTxt.Text),
                            Z = double.Parse(InPutShiftZTxt.Text),
                            U = double.Parse(InPutShiftUTxt.Text),
                        }
                    };
                }
                else
                {
                    shift = new ShiftArms
                    {
                        Pick = new ShiftArmsAxis
                        {
                            X = double.Parse(OutPickShiftXTxt.Text),
                            Y = double.Parse(OutPickShiftYTxt.Text),
                            Z = double.Parse(OutPickShiftZTxt.Text),
                            U = double.Parse(OutPickShiftUTxt.Text),
                        },
                        Put = new ShiftArmsAxis
                        {
                            X = double.Parse(OutPutShiftXTxt.Text),
                            Y = double.Parse(OutPutShiftYTxt.Text),
                            Z = double.Parse(OutPutShiftZTxt.Text),
                            U = double.Parse(OutPutShiftUTxt.Text),
                        }
                    };
                }
                MainWindow.SendPresenterData("set_arms_shift", new ArmsShiftArgs { Arms = arms, Value = shift });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "設定偏移參數功能失敗。");
            }
        }

        private void ShiftArmsLimitTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (init) return;
                var text = sender as TextBox;
                TextBox upper = text.Text.Contains("Upper") ? text : null;
                TextBox lower = text.Text.Contains("Lower") ? text : null;
                foreach (TextBox txt in FindVisualChildren<TextBox>(this))
                {
                    if (upper == null && txt.Name == lower.Name.Replace("Lower", "Upper")) upper = txt;
                    if (lower == null && txt.Name == upper.Name.Replace("Upper", "Lower")) lower = txt;
                }
                CheckLimit(lower, upper, true);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "設定手臂偏移參數上下限錯誤。");
            }
        }

        private void ShiftArmsTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (init) return;
                double val;
                var tb = (sender as TextBox);
                if (!double.TryParse(tb.Text, out val))
                {
                    MessageBox.Show("數值設定錯誤，請確認。", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (val > shift_upper)
                {
                    MessageBox.Show("數值設定高於上限，請重新設定。", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                    tb.Foreground = new SolidColorBrush(Colors.Red);
                    return;
                }
                else if (val < shift_lower)
                {
                    MessageBox.Show("數值設定低於下限，請重新設定。", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                    tb.Foreground = new SolidColorBrush(Colors.Red);
                    return;
                }
                else
                {
                    tb.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "手臂偏移數值設定失敗。");
            }
        }

        private void SFISChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string url_path = DisplaySfisSignalTxt.Text.Contains("ON") ? "Signal/White_Button.png" : "Signal/Green_Button.png";
                var startup_path = Directory.GetCurrentDirectory();
                DisplaySfisSignalImg.Source = new BitmapImage(new Uri($"{startup_path}\\{url_path}"));
                DisplaySfisSignalTxt.Text = DisplaySfisSignalTxt.Text.Contains("ON") ? "OFF" : "ON";
            }
            catch (Exception ex)
            {
                logger.Error(ex, "SFIS連線功能設定失敗。");
            }
        }

        private void LocationDownBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LocationLV.SelectedIndex == LocationLV.Items.Count - 1) return;
                var index = LocationLV.SelectedIndex;
                var tmp = LocationLV.Items[index];
                LocationLV.Items[index] = LocationLV.Items[index + 1];
                LocationLV.Items[index + 1] = tmp;
                LocationLV.SelectedIndex = index + 1;

            }
            catch (Exception ex)
            {
                logger.Error(ex, "設定測高下移錯誤。");
            }
        }

        private void LocationUpBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LocationLV.SelectedIndex == 0) return;
                var index = LocationLV.SelectedIndex;
                var tmp = LocationLV.Items[index];
                LocationLV.Items[index] = LocationLV.Items[index - 1];
                LocationLV.Items[index - 1] = tmp;
                LocationLV.SelectedIndex = index -1;

            }
            catch (Exception ex)
            {
                logger.Error(ex, "設定測高上移錯誤。");
            }
        }

        private void StepBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var step = int.Parse((sender as Button).Tag.ToString());
                var sfis = new SfisParam();
                sfis.Enable = DisplaySfisSignalTxt.Text == "ON" ? true : false;
                sfis.StationID = SfisStationNameTxt.Text;
                sfis.LineID = SfisLineNameTxt.Text;
                sfis.BarcodeA = SfisBarcodeATxt.Text;
                sfis.BarcodeB = SfisBarcodeBTxt.Text;
                sfis.InspLevel = MarkingLevelTxt.Text;
                MainWindow.SendPresenterData("sfis_step", new SfisStepArgs { Param = sfis, Step = step });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "單動流程功能失敗。");
            }
        }
    }
}
