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

namespace _4DMEN_Layout.Pages
{
    /// <summary>
    /// PasswordPage.xaml 的互動邏輯
    /// </summary>
    public partial class PasswordPage : UserControl
    {
        #region 屬性
        private Logger logger = LogManager.GetCurrentClassLogger();
        #endregion 屬性
        public PasswordPage()
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

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Parent is Window parent)
                {
                    parent.DialogResult = PasswordPB.Password == "7" ? true : false;
                    parent.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "密碼確認功能失敗。");
            }

        }
    }
}
