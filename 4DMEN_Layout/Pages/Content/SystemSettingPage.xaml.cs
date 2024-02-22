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
    /// SystemSettingPage.xaml 的互動邏輯
    /// </summary>
    public partial class SystemSettingPage : UserControl
    {
        #region 屬性
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion 屬性
        public SystemSettingPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "頁面資料初始化失敗。");
            }
        }
    }
}
