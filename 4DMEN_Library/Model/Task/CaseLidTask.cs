using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class CaseLidTask : BaseTask
    {
        internal CaseData case_data = null;
        internal List<CaseData> case_datas { get; set; } = new List<CaseData>();
        /// <summary>
        /// 設定手臂取Lid順序
        /// </summary>
        internal int PickSequence { get; set; } = 1;
        /// <summary>
        /// 是否可以放料到台車上
        /// </summary>
        internal bool CanPutCase { get; set; } = false;
        /// <summary>
        /// 是否完成放料到台車上
        /// </summary>
        internal bool PutCaseFinish { get; set; } = true;
        #region 類別欄位
        private static CaseLidTask m_Singleton;
        #endregion 類別欄位
        #region 類別方法
        internal static CaseLidTask GetEntity()
        {
            return m_Singleton;
        }
        static CaseLidTask()
        {
            m_Singleton = new CaseLidTask(nameof(CaseLidTask));
        }
        #endregion 類別方法
        #region 建構子
        private CaseLidTask(string Name) : base(Name)
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
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"組裝上蓋流程開始");
                    Status = EnumData.TaskStatus.Running;
                    if (!MainPresenter.SystemParam().Flow.CaseAssemble)
                    {
                        MainPresenter.SetRunSingleFlow(false);
                        MainPresenter.SetRunFlow(false);
                        PutCaseFinish = true;
                        Status = EnumData.TaskStatus.Done;
                        break;
                    }
                    case_data?.CaseAssembleTime.Start();
                    case_data.Step = Step = 1;
                    break;
                case 1: ///設定手臂取料順序
                    if (!DoArmsAction(() => MainPresenter.CaseLidArms().SetPickPos(PickSequence % MainPresenter.SystemParam().LidTrayCount), MainPresenter.CaseLidArms())) break;
                        case_data.Step = Step = 2;
                    break;
                case 2: ///判斷換Tray是否完成
                    if (!(CaseLidStationTask.GetEntity().Status == EnumData.TaskStatus.Done || CaseLidStationTask.GetEntity().Status == EnumData.TaskStatus.Idle))
                    {
                        System.Threading.Thread.Sleep(300);
                        break;
                    }
                    case_data.Step = Step = 3;
                    break;
                case 3: ///手臂取料
                    if (!DoArmsActionErrorWithoutWait(() => MainPresenter.CaseLidArms().Pick(), MainPresenter.CaseLidArms(), "組裝手臂取料錯誤，請重新將手臂回Home再行後續動作"))
                    {
                        Step = 1;
                        if (PickSequence == MainPresenter.SystemParam().LidTrayCount)
                        {
                            PickSequence = 1;
                            CaseLidStationTask.GetEntity().PassDetect = false;
                            CaseLidStationTask.GetEntity().StartTask(); //開始換Tray
                            System.Threading.Thread.Sleep(500);
                        }
                        PickSequence++;
                        break;
                    }
                    PickSequence++;
                    case_data.Step = Step = 4;
                    break;
                case 4: ///判斷是否可以進行放料
                    if (!CanPutCase) break;
                    case_data.Step = Step = 5;
                    CanPutCase = false;
                    break;
                case 5: ///手臂放料台車上
                    if (!case_data.IsRun || case_data.ManualNG || !MainPresenter.SystemParam().Flow.CaseAssemble)
                    {
                        case_data.Step = Step = 6;
                        PutCaseFinish = true;
                        break;
                    }
                    if (!DoArmsAction(() => MainPresenter.CaseLidArms().Put(), MainPresenter.CaseLidArms(), "組裝手臂放料錯誤，請先移除上蓋，並重新將手臂回Home後，再行後續動作"))
                    {
                        case_data.Step = Step = 1;
                        CanPutCase = true;
                        break;
                    }
                    PutCaseFinish = true;
                    case_data.Step = Step = 6;
                    break;
                case 6: ///判斷入料平台是否要換Tray
                    if (!MainPresenter.GetRunSingleFlow() && ((case_data.Index + 1) == MainPresenter.SystemParam().CaseCount || PickSequence % MainPresenter.SystemParam().TrayCount == 0))
                    {
                        CaseInStationTask.GetEntity().PassDetect = (case_data.Index + 1) == MainPresenter.SystemParam().CaseCount;
                        CaseInStationTask.GetEntity().StartTask(); //開始換Tray
                        PickSequence = 1;
                        System.Threading.Thread.Sleep(500);
                    }
                    case_data.Step = Step = 7;
                    break;

                case 7: ///完成流程
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"組裝上蓋流程完成");
                    case_data.Step = Step = 0;
                    if (!case_data.IsRun || case_data.ManualNG || !MainPresenter.SystemParam().Flow.CaseAssemble)
                        Step = 4;
                    case_data?.CaseAssembleTime.Stop();
                    if ((case_data.Index + 1) == MainPresenter.SystemParam().CaseCount || MainPresenter.GetRunSingleFlow())
                    {
                        MainPresenter.SetRunSingleFlow(false);
                        MainPresenter.SetRunFlow(false);
                        Status = EnumData.TaskStatus.Done;
                    }
                    else
                    {
                        case_data = case_datas[case_data.Index + 1];
                    }
                    break;
            }
        }

        #endregion 實作方法
        #region 覆寫功能
        public override void PauseTask()
        {
            IsPause = true;
            while (true)
            {
                if (CaseScanCodeTask.GetEntity().WaitForPause || CasePutNutTask.GetEntity().WaitForPause || CaseBendTask.GetEntity().WaitForPause || CasePlateTask.GetEntity().WaitForPause ||
                    CaseEstHeightTask.GetEntity().WaitForPause || CaseNgOutTask.GetEntity().WaitForPause || CaseMarkingTask.GetEntity().WaitForPause || CaseOutTask.GetEntity().WaitForPause)
                {
                    System.Threading.Thread.Sleep(100);
                    continue;
                }
                break;
            }
            MainPresenter.SetRunFlow(false);
            _pauseEvent.Reset();
            ThreadState = System.Threading.ThreadState.SuspendRequested;
        }
        #endregion 覆寫功能
    }
}
