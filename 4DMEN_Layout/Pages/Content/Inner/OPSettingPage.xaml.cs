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
    /// OPSettingPage.xaml 的互動邏輯
    /// </summary>
    public partial class OPSettingPage : UserControl
    {
        #region 屬性
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion 屬性
        #region 靜態動作
        public static Action<string, int, bool, MarkingParam> LoadMarkingParam;
        public static Action<string, string, string, string> ChangeMarkingParam;
        #endregion 靜態動作
        public OPSettingPage()
        {
            InitializeComponent();
            LoadMarkingParam = (ip, port, connection_status, param) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Marking_IP.Text = ip;
                    Marking_Port.Text = port.ToString();
                    TextObjectFstLine.Text = param.marking_fst_code;
                    TextObjectFstInpTxt.Text = param.marking_fst_txt;
                    TextObjectSndLine.Text = param.marking_snd_code;
                    TextObjectSndInpTxt.Text = param.marking_snd_txt;
                    TextObjectSndInpCountTxt.Text = param.marking_snd_index.ToString();
                    TextCodeLine.Text = param.marking_2d_code;
                    TextCodeInpTxt.Text = param.marking_2d_txt;
                });
            };
            ChangeMarkingParam = ( fst_txt, snd_txt, snd_index, code_txt) => {
                Dispatcher.Invoke(() =>
                {
                    
                    TextObjectFstInpTxt.Text = fst_txt;
                    TextObjectSndInpTxt.Text = snd_txt;
                    TextObjectSndInpCountTxt.Text = snd_index;
                    TextCodeInpTxt.Text = code_txt;
                });
            };
        }

        private void MarkingSettingBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fst_txt = TextObjectFstInpTxt.Text;
                var snd_txt = TextObjectSndInpTxt.Text;
                var code_txt = TextCodeInpTxt.Text;
                var index = 0;
                if (!int.TryParse(TextObjectSndInpCountTxt.Text, out index))
                {
                    MessageBox.Show("設定第二行文字編號錯誤，請重新輸入。");
                    return;
                }
                MarkingPage.ChangeMarkingParam(fst_txt, snd_txt, index.ToString(), code_txt);
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
    }
}
