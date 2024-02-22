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
    /// FlowDataInfoPage.xaml 的互動邏輯
    /// </summary>
    public partial class FlowDataInfoPage : UserControl
    {
        #region 屬性
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion 屬性
        #region 靜態動作
        public static Action<List<CaseData>> UpdateCaseDatas;
        public static Action<string> InsertMessage;
        #endregion 靜態動作
        public FlowDataInfoPage()
        {
            try
            {
                InitializeComponent();
                #region 靜態動作
                UpdateCaseDatas = datas =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        CaseDatasDG.ItemsSource = null;
                        CaseDatasDG.ItemsSource = datas;
                    });
                };
                InsertMessage = msg =>
                {
                    Dispatcher.Invoke(() => MessageTB.Text = MessageTB.Text.Insert(0, msg + "\n"));
                };
                #endregion 靜態動作
            }
            catch (Exception ex)
            {
                logger.Error(ex, "頁面資料初始化失敗。");
            }
        }
    }
}
