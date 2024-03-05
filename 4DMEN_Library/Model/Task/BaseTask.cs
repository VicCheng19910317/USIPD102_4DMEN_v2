using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace _4DMEN_Library.Model
{
    internal abstract class BaseTask : BaseThread
    {
        #region 欄位
        protected List<LogData> logger = MainPresenter.LogDatas();
        /// <summary>
        /// 動作錯誤代碼
        /// </summary>
        protected int ErrCode;
        /// <summary>
        /// 動作狀態
        /// </summary>
        public EnumData.TaskStatus Status = EnumData.TaskStatus.Idle;
        #endregion
        #region 屬性
        /// <summary>
        /// 單步執行
        /// </summary>
        public bool OneStepRun { get; set; }
        /// <summary>
        /// 任務是否準備好訊號
        /// </summary>
        public bool IsReady =>
           this.Status == EnumData.TaskStatus.Idle ||
           this.Status == EnumData.TaskStatus.Ready;
        public bool WaitForPause => (IsRunning && !IsPause);
        /// <summary>
        /// 任務是否完成訊號
        /// </summary>
        public bool IsDone =>
            this.ThreadState == ThreadState.Stopped ||
            this.ThreadState == ThreadState.Unstarted;
        /// <summary>
        /// 任務是否執行訊號
        /// </summary>
        public bool IsRunning =>
            this.Status == EnumData.TaskStatus.Running;
        /// <summary>
        /// 發生錯誤
        /// </summary>
        public bool IsError => ErrCode != 0x00;

        private int _Step = 0;
        /// <summary>
        /// 任務執行步驟
        /// </summary>
        public int Step
        {
            get => _Step;
            protected set
            {
                if (_Step != value)
                {
                    _Step = value;
                    StepDisplayEvent?.Invoke(this, _Step.ToString());
                }
            }
        }
        public void ResetStep()
        {
            Step = 0;
        }
        /// <summary>
        /// 錯誤顯示狀態(是否需要跳過步驟使用)
        /// </summary>
        protected System.Windows.MessageBoxButton button { get; set; } = System.Windows.MessageBoxButton.OK;
        /// <summary>
        /// 錯誤訊息(傳送錯誤事件到主頁面上)
        /// </summary>
        protected string ErrorMessage
        {
            set
            {
                RecordData.RecordProcessData(MainPresenter.SystemParam(), $"執行錯誤，錯誤訊息:{value}");
                ShowErrorMessageEvent?.Invoke(this, new SendMessageBoxArgs { Name = Name, Message = value, Button = button });
            }
        }
        /// <summary>
        /// 執行下一個步驟(錯誤後是否需要跳過步驟使用)
        /// </summary>
        protected int NextStep { get; set; }
        /// <summary>
        /// 任務名稱
        /// </summary>
        public string Name { get; protected set; }

        #endregion
        #region 事件
        public event EventHandler<string> StepDisplayEvent;
        public event EventHandler<EventArgs> ShowErrorMessageEvent;
        #endregion
        #region 建構子
        public BaseTask(string Name) : base()
        {
            this.Name = Name;

            ErrCode = 0;
        }

        #endregion
        #region 虛擬方法
        public virtual void RunNextStep()
        {
            Step = NextStep;
            ResumeTask();
        }
        internal virtual void SetStep(int _step)
        {
            Step = _step;
        }
        protected virtual bool DoPLCAction(Func<bool> _Func, KeyencePLCProcessor processor, string Message = "", System.Windows.MessageBoxButton _btn = System.Windows.MessageBoxButton.OK)
        {
            var success = _Func();
            if (!success)
            {
                PauseTask();
                button = _btn;
                ErrorMessage = Message.Length > 0 ? $"{Message}\n錯誤訊息:{processor.Message}" : processor.Message;
            }
            return success;
        }
        protected virtual bool DoWeightAction(Func<bool> _Func, RS232Processor processor, out double weight, System.Windows.MessageBoxButton _btn = System.Windows.MessageBoxButton.OK)
        {
            weight = double.NaN;
            var success = _Func();
            if (!success)
            {
                PauseTask();
                button = _btn;
                ErrorMessage = processor.Message;
            }
            weight = processor.Value;
            return success;
        }
        protected virtual bool DoArmsAction(Func<bool> _Func, EsponArmsProcessor processor, string Message = "", System.Windows.MessageBoxButton _btn = System.Windows.MessageBoxButton.OK)
        {
            var success = _Func();
            if (!success)
            {
                PauseTask();
                button = _btn;
                ErrorMessage = Message.Length > 0 ? $"{Message}\n錯誤訊息:{processor.Message}" : processor.Message;
            }
            return success;
        }
        protected virtual bool DoArmsActionErrorWithoutWait(Func<bool> _Func, EsponArmsProcessor processor, string Message = "", System.Windows.MessageBoxButton _btn = System.Windows.MessageBoxButton.OK)
        {
            var success = _Func();
            if (!success)
            {
                PauseTaskWithoutWait();
                button = _btn;
                ErrorMessage = Message.Length > 0 ? $"{Message}\n錯誤訊息:{processor.Message}" : processor.Message;
            }
            return success;
        }
        protected virtual bool DoInspAction(Func<bool> _Func, KeyenceInspectorParam processor, System.Windows.MessageBoxButton _btn = System.Windows.MessageBoxButton.OK)
        {
            var success = _Func();
            if (!success)
            {
                PauseTask();
                button = _btn;
                ErrorMessage = processor.Message;
            }
            return success;
        }
        protected virtual bool DoReaderAction(Func<bool> _Func, KeyenceReaderProcessor processor, out string read_data, out string read_level, string message = "", bool pause_flow = true, System.Windows.MessageBoxButton _btn = System.Windows.MessageBoxButton.OK)
        {
            read_data = "";
            read_level = "";
            var success = _Func();
            success = !(processor.ReadResult.Contains("超時") || processor.ReadResult.Contains("錯誤") || processor.ReadResult.ToLower().Contains("error"));
            if (!success && pause_flow)
            {
                PauseTask();
                button = _btn;
                ErrorMessage = $"{message}:{processor.Message}";
            }
            read_data = success ? processor.ReadResult : "read error";
            read_level = success ? processor.ReadLevel : "Err";
            return success;
        }
        /// <summary>
        /// 錯誤發生處理方法
        /// </summary>
        /// <param name="opt">操作選項</param>
        /// <param name="next_step">下一步</param>
        protected virtual void SetError(EnumData.Option opt, int next_step)
        {
            //ClsEQM.AutoRun = false;
            //ClsEQM.InitRun = false;
            //ClsEQM.Status = ClsHanderData.emEQStatus.Alarm;
            //Step = next_step;
            //clsErrorManag.HookUp(Name, ErrCode, opt, DateTime.Now);
            //SEQLog.Logger.Error(string.Format("Task:{0},Step:{1},Error:{2}",
            //    Name, Step, clsErrorText.GetText(ErrCode, ClsDllData.emLangType.ENG)));
        }
        /// <summary>
        /// 執行任務
        /// </summary>
        /// <param name="_Func">任務</param>
        /// <param name="_Option">發生錯誤時的選項</param>
        /// <param name="_NextStep">發生錯誤時的下一步</param>
        /// <returns>任務執失敗傳回Trun</returns>
        protected virtual bool DoTaskFail(Func<int> _Func, EnumData.Option _Option, int _NextStep, string ErrorMsg)
        {
            ErrCode = _Func();

            if (ErrCode != 0x00)
            {
                PauseTask();
                ErrorMessage = ErrorMsg;
                SetError(_Option, _NextStep);
            }

            return ErrCode != 0x00;
        }
        /// <summary>
        /// 執行任務清單
        /// </summary>
        /// <param name="Funclist">任務清單</param>
        /// <param name="_Option"></param>
        /// <param name="_NextStep"></param>
        /// <returns>任務執失敗傳回Trun</returns>
        protected virtual bool DoTasksFail(Func<int>[] FuncList, EnumData.Option _Option, int _NextStep)
        {
            Task<int>[] TaskList = new Task<int>[FuncList.Count()];

            for (int i = 0; i < TaskList.Length; i++)
            {
                TaskList[i] = new Task<int>(FuncList[i]);
            }

            TaskList.ToList().ForEach(t => t.Start());

            Task.WhenAll(TaskList);

            var FailTask = TaskList.FirstOrDefault(node => node.Result != 0x00);

            ErrCode = FailTask != null ? FailTask.Result : 0x00;

            if (ErrCode != 0x00)
                SetError(_Option, _NextStep);

            return ErrCode != 0x00;
        }
        #endregion
    }
}
