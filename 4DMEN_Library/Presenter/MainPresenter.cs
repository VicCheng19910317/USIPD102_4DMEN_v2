using _4DMEN_Library.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library
{
    public class MainPresenter
    {
        #region 屬性參數
        private MainView view;
        /// <summary>
        /// 初始執行
        /// </summary>
        protected bool init_run = false;
        /// <summary>
        /// 暫停
        /// </summary>
        protected bool manual_pause = false;
        /// <summary>
        /// 單動流程
        /// </summary>
        protected bool run_single_step = false;
        /// <summary>
        /// 執行流程
        /// </summary>
        protected bool run_flow = false;
        /// <summary>
        /// 資料紀錄
        /// </summary>
        private List<LogData> _logger = new List<LogData>();
        private List<LogData> logger
        {
            get => _logger;
            set
            {
                _logger = value;
                OnPresentResponseEvent("update_logger_data", new UpdateLoggerArgs { Datas = _logger });
            }
        }
        /// <summary>
        /// 系統參數
        /// </summary>
        protected SystemParam system_param = new SystemParam();
        #endregion 屬性參數
        #region 事件
        /// <summary>
        /// 前後端溝通用事件
        /// </summary>
        public event EventHandler<EventArgs> PresentResponseEvent;
        private void OnPresentResponseEvent(string response_name, EventArgs e)
        {
            PresentResponseEvent?.Invoke(response_name, e);
        }
        private void PresenterSendEvent(object sender, EventArgs e)
        {
            string actions = sender as string;
            if (actions == "system_initialize")
                Initialized();
            else if (actions == "system_closing")
                Closing();
        }
        #endregion 事件
        #region 實作功能
        private void Initialized()
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, "初始化失敗。");
            }

            
            init_run = true;
        }
        private void Closing()
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, "系統關閉失敗。");
            }
        }
        #endregion 實作功能
        #region 靜態動作
        internal static Action<bool> SetRunFlow;
        #endregion 靜態動作
        #region 靜態功能
        internal static Func<List<LogData>> LogDatas;
        internal static Func<SystemParam> SystemParam;
        #endregion 靜態功能
        public MainPresenter(MainView _view)
        {
            try
            {
                view = _view;
                view.PresenterSendEvent += PresenterSendEvent;
                #region 靜態動作

                #endregion 靜態動作
                SetRunFlow = value => run_flow = value;
                #region 靜態功能
                LogDatas = () => logger;
                SystemParam = () => system_param;
                #endregion 靜態功能

            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, "初始化Presenter失敗。");
            }

        }
    }
}
