using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class CasePlateTask : BaseTask
    {
        internal CaseData case_data = null;
        #region 類別欄位
        private static CasePlateTask m_Singleton;
        #endregion
        #region 類別方法
        internal static CasePlateTask GetEntity()
        {
            return m_Singleton;
        }
        static CasePlateTask()
        {
            m_Singleton = new CasePlateTask(nameof(CasePlateTask));
        }
        #endregion 類別方法
        #region 建構子
        private CasePlateTask(string Name) : base(Name)
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
                    
                if (Status == EnumData.TaskStatus.Done)
                {
                    ThreadState = System.Threading.ThreadState.Stopped;
                    break;
                }

                RunStep();
            }
        }

        protected void RunStep()
        {
            switch (Step)
            {
                case 0: ///是否需要執行
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"壓平站開始");
                    Status = EnumData.TaskStatus.Running;
                    if (!MainPresenter.SystemParam().Flow.CasePlate)
                    {
                        case_data.Step = Step = 2;
                        break;
                    }
                    case_data.Step = Step = (case_data == null || !case_data.IsRun || case_data.ManualNG) ? 2 : 1;
                    case_data?.CasePlateTime.Start();
                    break;
                case 1: ///執行壓平
                    var plc = MainPresenter.NutPLC();
                    if (!DoPLCAction(() => MainPresenter.PlatePLC().RunPlate(), MainPresenter.PlatePLC(), "壓平錯誤，請先確認PLC問題後，按下「PC Error Reset」鈕等待PLC完成Reset後，再按下「Resume」重新作業。"))
                    {
                        case_data.Step = Step = MainPresenter.PlatePLC().Message.Contains("發送訊號超時") || MainPresenter.PlatePLC().Message.Contains("未收到PLC待機狀態") ? 1 : 2;
                        break;
                    }
                    case_data.Step = Step = 2;
                    break;
                case 2: ///完成流程
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"壓平站完成");
                    case_data.Step = Step = 0;
                    case_data?.CasePlateTime.Stop();
                    MainPresenter.SetRunSingleFlow(false);
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
                if (CaseScanCodeTask.GetEntity().WaitForPause || CasePutNutTask.GetEntity().WaitForPause || CaseBendTask.GetEntity().WaitForPause ||
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
