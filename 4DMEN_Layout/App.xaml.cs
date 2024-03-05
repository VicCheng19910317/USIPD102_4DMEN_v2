using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace USIPD102_4DMEN
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Mutex _instanceMutex = null;
        public App()
        {
            bool createdNew;
            _instanceMutex = new Mutex(true, @"4DMEN_PD102", out createdNew);
            string system_name = "Bending & Flatness Auto Machine System.";
            if (!createdNew)
            {
                _instanceMutex = null;
                MessageBox.Show($"請勿重複開啟 {system_name}");
                Application.Current.Shutdown();
                return;
            }
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            Current.DispatcherUnhandledException += OnCurrentDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
        }
        protected override void OnStartup(StartupEventArgs e)
        {


            base.OnStartup(e);
        }
        private static void OnCurrentDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            logger.Fatal($"An unhandled current dispatcher exception occurred:{args.Exception.Message}\r\nStackTrace:{args.Exception.StackTrace}");
            args.Handled = true;
        }

        private static void OnTaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs args)
        {
            logger.Fatal($"An unhandled task exception occurred:{args.Exception.Message}\r\nStackTrace:{args.Exception.StackTrace}");
            args.SetObserved();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            logger.Fatal($"Application_ThreadException: {ex.Message}\r\nStackTrace:{ex.StackTrace}");
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format($"An unhandled exception occurred: {e.Exception.Message}\r\nStackTrace:{e.Exception.StackTrace}");
            logger.Fatal(errorMessage);
            e.Handled = true;
        }
    }
}
