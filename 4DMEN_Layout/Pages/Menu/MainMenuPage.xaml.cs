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
    /// MainMenuPage.xaml 的互動邏輯
    /// </summary>
    public partial class MainMenuPage : UserControl
    {
        RadioButton last_index;
        public MainMenuPage()
        {
            InitializeComponent();
            last_index = MainPageRB;
        }

        private void ChangeMenu_Click(object sender, RoutedEventArgs e)
        {
            string tabItem = (sender as RadioButton).Tag as string;
            switch (tabItem)
            {
                case "System Setting":
                    Window window = new Window
                    {
                        Title = "Login Admin",
                        Content = new PasswordPage(),
                        SizeToContent = SizeToContent.WidthAndHeight,
                        ResizeMode = ResizeMode.NoResize,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        Icon = new BitmapImage(new Uri($"{AppDomain.CurrentDomain.BaseDirectory}\\公司logo.png"))
                    };
                    bool? result = window.ShowDialog();
                    if (result.Value)
                    {
                        MainWindow.SwitchMainPage(tabItem);
                    }
                    else
                    {
                        e.Handled = true;
                        last_index.IsChecked = true;
                    }
                    break;
                case "Main Page":
                case "System Info.":
                    last_index = sender as RadioButton;
                    MainWindow.SwitchMainPage(tabItem);
                    break;
                default:
                    break;
            }
        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;

            //string tabItem = ((sender as TabControl).SelectedItem as TabItem).Tag as string;
            //switch (tabItem)
            //{
            //    case "System Setting":
            //        //加一個密碼輸入的視窗
            //        Window window = new Window
            //        {
            //            Title = "Login Admin",
            //            Content = new PasswordPage(),
            //            SizeToContent = SizeToContent.WidthAndHeight,
            //            ResizeMode = ResizeMode.NoResize,
            //            WindowStartupLocation = WindowStartupLocation.CenterScreen
            //        };
            //        bool? result = window.ShowDialog();
            //        if (result.Value)
            //        {
            //            MainWindow.SwitchMainPage(tabItem);
            //        }
            //        else
            //        {
            //            e.Handled = true;
            //        }     
            //        break;
            //    case "Main Page":
            //    case "System Info.":
            //        last_index = (sender as TabControl).SelectedIndex;
            //        MainWindow.SwitchMainPage(tabItem);
            //        break;
            //    default:
            //        break;
            //}
        }

        private void AppCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Exit System?", "Info", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                //System.Windows.Application.Current.Shutdown();
                BottomPage.CloseSystem();
                App.Current.MainWindow.Close();
            }

        }
    }
}
