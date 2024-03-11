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
    /// MarkingPage.xaml 的互動邏輯
    /// </summary>
    public partial class MarkingPage : UserControl
    {
        #region 屬性
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion 屬性
        #region 靜態動作
        public static Action<bool> SetConnectionStatus;
        public static Action<string> SetMessage;
        public static Action<string,int,bool, MarkingParam> LoadMarkingParam;
        public static Action<string, string, string, string> ChangeMarkingParam;
        #endregion 靜態動作
        public MarkingPage()
        {
            InitializeComponent();
            #region 靜態動作
            SetConnectionStatus = status =>
            {
                Dispatcher.Invoke(() =>
                {
                    LaserStatusTxt.Text = status ? "連線" : "未連線";
                    LaserStatusTxt.Foreground = status ? (Brush)new BrushConverter().ConvertFrom("#00FF00") : (Brush)new BrushConverter().ConvertFrom("#FF0000");
                });
            };
            SetMessage = message =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessageTB.Text = MessageTB.Text.Insert(0,$"{message}\n");
                });
            };
            LoadMarkingParam = (ip,port,connection_status, param) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Marking_IP.Text = ip;
                    Marking_Port.Text = port.ToString();
                    SetConnectionStatus(connection_status);
                    TextObjectFstLine.Text = param.marking_fst_code;
                    TextObjectFstInpTxt.Text = param.marking_fst_txt;
                    TextObjectSndLine.Text = param.marking_snd_code;
                    TextObjectSndInpTxt.Text = param.marking_snd_txt;
                    TextObjectSndInpCountTxt.Text = param.marking_snd_index.ToString();
                    TextCodeLine.Text = param.marking_2d_code;
                    TextCodeInpTxt.Text = param.marking_2d_txt;
                    ShiftCodeTxt.Text = param.shift_code;
                    ShiftCodeXTxt.Text = param.shift_x.ToString();
                    ShiftCodeYTxt.Text = param.shift_y.ToString();
                    ShiftCodeATxt.Text = param.shift_a.ToString();
                    PassLevelTxt.Text = param.pass_level[0];
                    PassLevelLV.Items.Clear();
                    param.pass_level.ForEach(x => PassLevelLV.Items.Add(x));
                });
            };
            ChangeMarkingParam = (fst_txt, snd_txt, snd_index, code_txt) => {
                Dispatcher.Invoke(() =>
                {

                    TextObjectFstInpTxt.Text = fst_txt;
                    TextObjectSndInpTxt.Text = snd_txt;
                    TextObjectSndInpCountTxt.Text = snd_index;
                    TextCodeInpTxt.Text = code_txt;
                });
            };
            #endregion 靜態動作
        }

        private void MarkingSettingBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fst_txt = TextObjectFstInpTxt.Text;
                var snd_txt = TextObjectSndInpTxt.Text;
                var code_txt = TextCodeInpTxt.Text;
                var index = 0;
                if(!int.TryParse(TextObjectSndInpCountTxt.Text,out index))
                {
                    MessageBox.Show("設定第二行文字編號錯誤，請重新輸入。");
                    return;
                }
                OPSettingPage.ChangeMarkingParam(fst_txt, snd_txt, index.ToString(), code_txt);
                MainWindow.SendPresenterData("send_marking_action", new SendMarkingActionsArgs { Action = "SettingParam", FirstText = fst_txt, SecondText = snd_txt, SecondIndex = index, CodeText = code_txt });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "設定雷射參數錯誤。");
            }
        }

        private void TextObjectTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var first_inp = TextObjectFstInpTxt.Text;
                var snd_inp = TextObjectSndInpTxt.Text;
                var snd_index = TextObjectSndInpCountTxt.Text;
                var code_inp = TextCodeInpTxt.Text;
                TextCodeResultLine.Text = $"{first_inp}-{snd_inp}{snd_index}-{code_inp}";
            }
            catch (Exception ex)
            {
                logger.Error(ex, "雷射文字合併錯誤。");
            }
            
        }

        private void MarkingBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.SendPresenterData("send_marking_action", new SendMarkingActionsArgs { Action = "Marking" });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "執行雷射錯誤。");
            }
        }

        private void SettingShiftBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int x = 0, y = 0, a = 0;
                if(!int.TryParse(ShiftCodeXTxt.Text,out x) || !int.TryParse(ShiftCodeYTxt.Text, out y) || !int.TryParse(ShiftCodeATxt.Text, out a))
                {
                    MessageBox.Show("偏移數值設定錯誤，請重新確認。", "雷射設定錯誤訊息", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                MainWindow.SendPresenterData("send_marking_action", new SendMarkingActionsArgs { Action = "SetShift", OffsetX = x, OffsetY = y, OffsetA = a });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "設定雷射偏移錯誤。");
            }
        }

        private void PassLevelBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var pass_text = PassLevelTxt.Text;
                var item_exists = false;
                foreach(var item in PassLevelLV.Items)
                {
                    if (item.ToString() == pass_text)
                        item_exists = true;
                }
                if (!item_exists)
                    PassLevelLV.Items.Add(pass_text);
                
            }
            catch (Exception ex)
            {
                logger.Error(ex, "設定雷射等級錯誤。");
            }
        }

        private void PassLevelRemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item_name = ((ListViewItem)PassLevelLV.SelectedItem).Content.ToString();
                for (int i = 0; i < PassLevelLV.Items.Count; i++)
                {
                    var item = (ListViewItem)PassLevelLV.Items[i];
                    if (item.Content.ToString() == item_name)
                        PassLevelLV.Items.Remove(item);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "移除雷射等級錯誤。");
            }
        }

        private void PassLevelSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> pass_level = new List<string>();
                foreach(ListViewItem item in PassLevelLV.Items)
                {
                    if (!pass_level.Contains(item.Content.ToString()))
                        pass_level.Add(item.Content.ToString());
                }
                MainWindow.SendPresenterData("send_marking_action", new SendMarkingActionsArgs { Action = "SetLevel", PassLevel = pass_level });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "儲存雷射等級錯誤。");
            }
        }


        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.SendPresenterData("send_marking_action", new SendMarkingActionsArgs { Action = "Connect" });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "雷射連線錯誤。");
            }
        }

        private void DisconnectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.SendPresenterData("send_marking_action", new SendMarkingActionsArgs { Action = "Disconnect" });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "雷射關閉連線錯誤。");
            }
        }
    }
}
