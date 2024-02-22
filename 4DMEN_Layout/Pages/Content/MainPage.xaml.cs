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
    /// MainPage.xaml 的互動邏輯
    /// </summary>
    public partial class MainPage : UserControl
    {
        #region 欄位屬性
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion 欄位屬性
        #region 靜態動作
        public static Action<UIElement> ChangeBottomPage;
        #endregion 靜態動作
        #region 實作功能
        private void ChangeBottomPanel(UIElement element)
        {
            try
            {
                Grid.SetRowSpan(ContentPanel, 1);
                ContentPanel.Height = 980;
                InnerBottomPages.Height = 100;
                #region 靜態動作
                ChangePanel(InnerBottomPages, element);
                #endregion 靜態動作
            }
            catch (Exception ex)
            {
                logger.Error($"更改側邊Panel UI錯誤，錯誤資訊:{ex.Message}");
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
                logger.Error($"更改底層Panel UI錯誤，錯誤資訊:{ex.Message}");
            }
        }
        #endregion 實作功能
        public MainPage()
        {
            try
            {
                InitializeComponent();
                ChangeBottomPage = value => ChangeBottomPanel(value);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "頁面資料初始化失敗。");
            }
        }
        
    }
}
