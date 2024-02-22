using _4DMEN_Library.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// SystemInfoPage.xaml 的互動邏輯
    /// </summary>
    public partial class SystemInfoPage : UserControl
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private ObservableCollection<LogData> logger_sources = new ObservableCollection<LogData>();
        private bool init = true;

        public static Action<UIElement> ChangeBottomPage;
        public static Action<List<LogData>> UpdateLogDatas;
        public static Action<List<LogData>> LoadLogDatas;
        public SystemInfoPage()
        {
            try
            {
                InitializeComponent();
                #region 靜態動作
                ChangeBottomPage = value => ChangeBottomPanel(value);
                UpdateLogDatas = values =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        logger_sources.Clear();
                        values.ForEach(x => logger_sources.Add(x));
                        DisplayDG.ItemsSource = null;
                        DisplayDG.ItemsSource = logger_sources.Where(x => x.LogLevel == LevelCB.SelectedItem.ToString());
                    });
                };
                LoadLogDatas = values =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        logger_sources.Clear();
                        values.ForEach(x => logger_sources.Add(x));
                        DisplayDG.ItemsSource = null;
                        var level = (LevelCB.SelectedItem as ComboBoxItem).Content.ToString() == "All" ? "" : (LevelCB.SelectedItem as ComboBoxItem).Content.ToString();
                        DisplayDG.ItemsSource = logger_sources.Where(x => x.LogLevel.Contains(level));
                        init = false;
                    });
                };
                #endregion 靜態動作
                MainWindow.SendPresenterData("load_log_datas", null);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "頁面資料初始化失敗。");
            }

        }
        private void ChangeBottomPanel(UIElement element)
        {
            try
            {
                Grid.SetRowSpan(ContentPanel, 1);
                ContentPanel.Height = 980;
                InnerBottomPages.Height = 100;
                ChangePanel(InnerBottomPages, element);
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

        private void LevelCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (init) return;
            DisplayDG.ItemsSource = null;
            var level = (LevelCB.SelectedItem as ComboBoxItem).Content.ToString() == "All" ? "" : (LevelCB.SelectedItem as ComboBoxItem).Content.ToString();
            DisplayDG.ItemsSource = logger_sources.Where(x => x.LogLevel.Contains(level));
        }
    }
}
