using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class CaseScanCodeTask : BaseTask
    {
        internal CaseData case_data = null;
        #region 類別欄位
        private static CaseScanCodeTask m_Singleton;
        #endregion
        #region 類別方法
        internal static CaseScanCodeTask GetEntity()
        {
            return m_Singleton;
        }
        static CaseScanCodeTask()
        {
            m_Singleton = new CaseScanCodeTask(nameof(CaseScanCodeTask));
        }
        #endregion 類別方法
        #region 建構子
        private CaseScanCodeTask(string Name) : base(Name)
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
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"掃碼站開始");
                    Status = EnumData.TaskStatus.Running;
                    if (!MainPresenter.SystemParam().Flow.CaseScan)
                    {
                        case_data.Step = Step = 2;
                        break;
                    }
                    case_data.Step = Step = (case_data == null || !case_data.IsRun || case_data.ManualNG) ? 2 : 1;
                    case_data?.CaseReaderTime.Start();
                    break;
                case 1: ///執行掃碼
                    var reader = MainPresenter.Reader();
                    string read_data = "", read_level = "" ;
                    var internal_finish = DoReaderAction(() => reader.Read(), reader, out read_data, out read_level, "掃碼讀取訊號超時，請按下「Resume」系統進行後續放料流程。\n錯誤訊號：", false);
                    if (internal_finish) ///判斷是否成功
                        case_data.ReaderResult1 = read_data;
                    else
                    {
                        Step = 2;
                        if (!case_data.DefectCode.Contains(MainPresenter.SystemParam().DefectMapping["BasePlate 2D Code"]))
                            case_data.DefectCode.Add(MainPresenter.SystemParam().DefectMapping["BasePlate 2D Code"]);
                        if (!case_data.NGPosition.Contains(7))
                            case_data.NGPosition.Add(7);
                        case_data.IsRun = false;
                        break;
                    }
                    case_data.Step = Step = 2;
                    break;
                case 2: ///完成流程
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"掃碼站完成");
                    case_data.Step = Step = 0;
                    case_data?.CaseReaderTime.Stop();
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
                if(CasePutNutTask.GetEntity().WaitForPause || CaseBendTask.GetEntity().WaitForPause || CasePlateTask.GetEntity().WaitForPause ||
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
