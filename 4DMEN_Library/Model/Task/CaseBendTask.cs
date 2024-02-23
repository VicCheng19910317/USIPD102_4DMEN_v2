using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class CaseBendTask : BaseTask
    {
        internal CaseData case_data = null;
        internal bool IsStartBend { get; set; } = false;
        #region 類別欄位
        private static CaseBendTask m_Singleton;
        #endregion
        #region 類別方法
        internal static CaseBendTask GetEntity()
        {
            return m_Singleton;
        }
        static CaseBendTask()
        {
            m_Singleton = new CaseBendTask(nameof(CaseBendTask));
        }
        #endregion 類別方法
        #region 建構子
        private CaseBendTask(string Name) : base(Name)
        {

        }
        #endregion 建構子
        #region 實作方法
        protected override void MyTask()
        {
            Status = EnumData.TaskStatus.Running;
            while (true)
            {
                if (CaseDoorCheckTask.GetEntity().DoorOpen)
                    PauseTaskWithoutWait();
                WaitOne();
                if (case_data != null && !case_data.ManualNG)
                    RunStep();
                if (Status == EnumData.TaskStatus.Done)
                {
                    ThreadState = System.Threading.ThreadState.Stopped;
                    break;
                }
            }
        }

        protected void RunStep()
        {
            switch (Step)
            {
                case 0: ///是否需要執行
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"折彎站開始");
                    Status = EnumData.TaskStatus.Running;
                    if (!MainPresenter.SystemParam().Flow.CaseBending)
                    {
                        case_data.Step = Step = 2;
                        break;
                    }
                    case_data.Step = Step = (case_data == null || !case_data.IsRun) ? 2 : 1;
                    case_data?.CaseBendTime.Start();
                    break;
                case 1: ///執行折彎
                    if (!DoPLCAction(() => MainPresenter.BendPLC().RunBending(), MainPresenter.BendPLC(), "'折彎錯誤，請先確認PLC問題後，按下「PC Error Reset」鈕等待PLC完成Reset後，再按下「Resume」重新作業。"))
                    {
                        case_data.Step = Step = MainPresenter.BendPLC().Message.Contains("發送訊號超時") || MainPresenter.BendPLC().Message.Contains("未收到PLC待機狀態") ? 1 : 2;
                        break;
                    }
                    case_data.Step = Step = 2;
                    break;
                case 2: ///完成流程
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"折彎站完成");
                    case_data.Step = Step = 0;
                    case_data?.CaseBendTime.Stop();
                    IsStartBend = false;
                    MainPresenter.SetRunFlow(false);
                    Status = EnumData.TaskStatus.Done;
                    break;
            }
        }
        public override void PauseTask()
        {
            IsPause = true;
            while (true)
            {
                if (CaseScanCodeTask.GetEntity().WaitForPause || CasePutNutTask.GetEntity().WaitForPause || CasePlateTask.GetEntity().WaitForPause ||
                    CaseEstHeightTask.GetEntity().WaitForPause || CaseNgOutTask.GetEntity().WaitForPause || CaseMarkingTask.GetEntity().WaitForPause || CaseOutTask.GetEntity().WaitForPause)
                {
                    Thread.Sleep(100);
                    continue;
                }
                break;
            }
            MainPresenter.SetRunFlow(false);
            _pauseEvent.Reset();
            ThreadState = System.Threading.ThreadState.SuspendRequested;
        }

        #endregion
    }
}
