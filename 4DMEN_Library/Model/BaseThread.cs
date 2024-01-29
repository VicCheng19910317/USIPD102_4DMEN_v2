using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal abstract class BaseThread
    {
        #region 屬性
        /// <summary>
        /// 緒狀態列舉
        /// </summary>
        public ThreadState ThreadState { get; set; } = ThreadState.Unstarted;
        private List<LogData> logger = MainPresenter.LogDatas();
        #endregion 屬性
        #region 欄位
        private ManualResetEvent _shutdownEvent;
        internal ManualResetEvent _pauseEvent;
        internal Thread _Thread;
        #endregion 欄位
        #region 建構子
        /// <summary>
        /// 任務建構子
        /// </summary>
        /// <param name="Name">任務標籤</param>
        public BaseThread(string Name) : this()
        {
            _Thread.Name = Name;
        }
        /// <summary>
        /// 任務建構子
        /// </summary>
        public BaseThread()
        {
            _shutdownEvent = new ManualResetEvent(false);
            _pauseEvent = new ManualResetEvent(true);
            _Thread = new Thread(MyTask) { IsBackground = true};
            ThreadState = ThreadState.Unstarted;
        }
        #endregion
        #region 抽象方法
        /// <summary>
        /// 抽象方法,任務
        /// </summary>
        protected abstract void MyTask();
        #endregion
        #region 方法
        /// <summary>
        /// 開始任務
        /// </summary>
        public void StartTask(ThreadPriority Priority = ThreadPriority.Normal)
        {
            if (_Thread.ThreadState == ThreadState.Running) return;
            if (_Thread != null && _Thread.IsAlive)
            {
                _Thread.Abort();
            }
            ThreadState = ThreadState.Running;
            _Thread = new Thread(MyTask) { IsBackground = true };
            _Thread.Priority = Priority;
            _Thread.Start();
        }
        /// <summary>
        /// 暫停任務
        /// </summary>
        public virtual void PauseTask()
        {
            MainPresenter.SetRunFlow(false);
            _pauseEvent.Reset();
            ThreadState = ThreadState.SuspendRequested;
        }
        /// <summary>
        /// 暫停任務(不等待其他站完成)
        /// </summary>
        public virtual void PauseTaskWithoutWait()
        {
            MainPresenter.SetRunFlow(false);
            _pauseEvent.Reset();
            ThreadState = ThreadState.SuspendRequested;
        }
        /// <summary>
        /// 繼續任務
        /// </summary>
        public void ResumeTask()
        {
            MainPresenter.SetRunFlow(true);
            _pauseEvent.Set();
            ThreadState = ThreadState.Running;
            
        }
        /// <summary>
        /// 停止任務
        /// </summary>
        public void StopTask()
        {
            try
            {
                _shutdownEvent.Set();

                _pauseEvent.Set();

                if(_Thread != null && _Thread.IsAlive)
                    _Thread.Abort();

                ThreadState = ThreadState.Stopped;
            }
            catch(Exception ex)
            {
                logger = LoggerData.Error(ex, $"停止任務錯誤");
            }
           
        }
        /// <summary>
        /// 封鎖執行緒執行，直到收到訊號為止
        /// </summary>
        public void WaitOne()
        {
            if (ThreadState == ThreadState.SuspendRequested)
                ThreadState = ThreadState.Suspended;
            _pauseEvent.WaitOne(Timeout.Infinite);
        }
        #endregion
    }
}
