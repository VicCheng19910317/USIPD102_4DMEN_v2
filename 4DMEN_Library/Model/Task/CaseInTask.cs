using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class CaseInTask : BaseTask
    {
        internal CaseData case_data = null;
        internal List<CaseData> case_datas { get; set; } = new List<CaseData>();
        /// <summary>
        /// 是否可以放料到台車上
        /// </summary>
        internal bool CanPutCase { get; set; } = false;
        /// <summary>
        /// 是否完成放料到台車上
        /// </summary>
        internal bool PutCaseFinish { get; set; } = true;
        #region 類別欄位
        private static CaseInTask m_Singleton;
        #endregion
        #region 類別方法
        internal static CaseInTask GetEntity()
        {
            return m_Singleton;
        }
        static CaseInTask()
        {
            m_Singleton = new CaseInTask(nameof(CaseInTask));
        }
        #endregion 類別方法
        #region 建構子
        private CaseInTask(string Name) : base(Name)
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
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"入料流程開始");
                    Status = EnumData.TaskStatus.Running;
                    case_data.Step = Step = (!case_data.IsRun || case_data.ManualNG) ? 6 : 1;
                    case_data?.CaseInTime.Start();
                    case_data?.CaseTotalTime.Start();
                    
                    break;
                case 1: ///設定手臂取料順序
                    if (!DoArmsAction(() => MainPresenter.CaseInArms().SetPickPos((case_data.Index % 12) + 1), MainPresenter.CaseInArms())) break;
                    case_data.Step = Step = 2;
                    break;
                case 2: ///判斷換Tray是否完成
                    if (!(CaseInStationTask.GetEntity().Status == EnumData.TaskStatus.Done || CaseInStationTask.GetEntity().Status == EnumData.TaskStatus.Idle))
                    {
                        System.Threading.Thread.Sleep(300);
                        break;
                    }
                    case_data.Step = Step = 3;
                    break;
                case 3: ///手臂取料
                    if (!DoArmsAction(() => MainPresenter.CaseInArms().Pick(), MainPresenter.CaseInArms(), "入料手臂取料錯誤，請重新將手臂回Home再行後續動作"))
                    {
                        case_data.Step = Step = 1;
                        break;
                    }
                    case_data.Step = Step = 4;
                    break;
                case 4: ///判斷是否可以進行放料
                    if (!CanPutCase) break;
                    case_data.Step = Step = 5;
                    CanPutCase = false;
                    break;

                case 5: ///手臂放料台車上
                    if (!DoArmsAction(() => MainPresenter.CaseInArms().Put(), MainPresenter.CaseInArms(), "入料手臂放料錯誤，請重新將手臂回Home再行後續動作"))
                    {
                        case_data.IsRun = false;
                        case_data.PickToCast = false;
                        case_data.Step = Step = 6;
                        break;
                    }
                    PutCaseFinish = true;
                    case_data.Step = Step = 6;
                    break;
                case 6: ///判斷入料平台是否要換Tray
                    if (!MainPresenter.GetRunSingleFlow() && ((case_data.Index + 1) == MainPresenter.SystemParam().CaseCount || ((case_data.Index + 1) % MainPresenter.SystemParam().TrayCount == 0 && case_data.Index > 0)))
                    {
                        CaseInStationTask.GetEntity().PassDetect = (case_data.Index + 1) == MainPresenter.SystemParam().CaseCount;
                        CaseInStationTask.GetEntity().StartTask(); //開始換Tray
                        System.Threading.Thread.Sleep(500);
                    }
                    case_data.Step = Step = 7;
                    break;

                case 7: ///完成流程
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"入料流程完成");
                    case_data.Step = Step = 0;
                    case_data?.CaseInTime.Stop();
                    if((case_data.Index + 1) == MainPresenter.SystemParam().CaseCount || MainPresenter.GetRunSingleFlow())
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

        #endregion
    }
}
