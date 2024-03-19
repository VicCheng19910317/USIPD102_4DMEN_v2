using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class CaseNgOutTask : BaseTask
    {
        internal int NgCountLimit { get; set; } = 3;
        internal int NgCount { get; set; } = 0;
        internal CaseData case_data = null;
        #region 類別欄位
        private static CaseNgOutTask m_Singleton;
        #endregion
        #region 類別方法
        internal static CaseNgOutTask GetEntity()
        {
            return m_Singleton;
        }
        static CaseNgOutTask()
        {
            m_Singleton = new CaseNgOutTask(nameof(CaseNgOutTask));
        }
        #endregion 類別方法
        #region 建構子
        private CaseNgOutTask(string Name) : base(Name)
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
                case 0: //是否需要執行
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"NG出料流程開始");
                    Status = EnumData.TaskStatus.Running;

                    if(case_data.ManualNG)
                    {
                        case_data.Step = Step = 4;
                        break;
                    }
                    if (!case_data.IsRun && case_data.PickToCast)
                    {
                        case_data.Step = Step = 3;
                        break;
                    }
                    else if (!MainPresenter.SystemParam().Flow.CaseNgOut)
                    {
                        case_data.Step = Step = 4;
                        break;
                    }
                    case_data.Step = Step = (case_data == null) ? 4 : 1;
                    case_data?.CaseNgTime.Start();
                    break;
                case 1: //檢查平整度
                    if (case_data.Flatness.Count() < 3 || case_data.PlaneDist.Contains(float.NaN) || case_data.Flatness.Where(x => x > MainPresenter.SystemParam().FlatnessUpperLimit).Count() > 0)
                    {
                        if (!case_data.DefectCode.Contains(MainPresenter.SystemParam().DefectMapping["平整度異常"]))
                            case_data.DefectCode.Add(MainPresenter.SystemParam().DefectMapping["平整度異常"]);
                        if (!case_data.NGPosition.Contains(13))
                            case_data.NGPosition.Add(13);
                    }
                    case_data.Step = Step = 2;
                    break;
                case 2: //檢查高度
                    if(case_data.PlaneDist.Count < 9 || case_data.PlaneDist.Contains(float.NaN))
                    {
                        if (!case_data.DefectCode.Contains(MainPresenter.SystemParam().DefectMapping["高度異常"]))
                            case_data.DefectCode.Add(MainPresenter.SystemParam().DefectMapping["高度異常"]);
                        if (!case_data.NGPosition.Contains(13))
                            case_data.NGPosition.Add(13);
                        case_data.MeasureNG = true;
                        case_data.Step = Step = 3;
                        break;
                    }
                    var dist1 = new List<float> { case_data.PlaneDist[0], case_data.PlaneDist[1], case_data.PlaneDist[2] };
                    var dist2 = new List<float> { case_data.PlaneDist[3], case_data.PlaneDist[4], case_data.PlaneDist[5] };
                    var dist3 = new List<float> { case_data.PlaneDist[6], case_data.PlaneDist[7], case_data.PlaneDist[8] };
                    if (dist1.Where(x => x > (float)MainPresenter.SystemParam().HeightLimit[0].Upper || x < (float)MainPresenter.SystemParam().HeightLimit[0].Lower).Count() > 0 ||
                        dist2.Where(x => x > (float)MainPresenter.SystemParam().HeightLimit[1].Upper || x < (float)MainPresenter.SystemParam().HeightLimit[1].Lower).Count() > 0 ||
                        dist3.Where(x => x > (float)MainPresenter.SystemParam().HeightLimit[2].Upper || x < (float)MainPresenter.SystemParam().HeightLimit[2].Lower).Count() > 0)
                    {
                        if (!case_data.DefectCode.Contains(MainPresenter.SystemParam().DefectMapping["高度異常"]))
                            case_data.DefectCode.Add(MainPresenter.SystemParam().DefectMapping["高度異常"]);
                        if (!case_data.NGPosition.Contains(13))
                            case_data.NGPosition.Add(13);
                        case_data.MeasureNG = true;
                        case_data.Step = Step = 3;
                        break;
                    }
                    case_data.Step = Step = 4;
                    break;

                case 3: //NG出料
                    if (!DoPLCAction(() => MainPresenter.NgPLC().RunNGOut(), MainPresenter.NgPLC(), "NG出料錯誤，請檢察PLC錯誤訊息，並按下「Resume」接續流程。")) break;
                    NgCount++;
                    case_data.IsRun = false;
                    if (NgCount >= NgCountLimit)
                    {
                        PauseTask();
                        ErrorMessage = "NG區已滿料，請清除對應料件後，按下Resume按鈕";
                    }
                    case_data.Step = Step = 4;
                    break;
                case 4: //完成流程
                    case_data.Step = Step = 0;
                    MainPresenter.SetRunSingleFlow(false);
                    MainPresenter.SetRunFlow(false);
                    Status = EnumData.TaskStatus.Done;
                    case_data?.CaseNgTime.Stop();
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"NG出料流程完成");
                    break;
            }

        }
        public override void PauseTask()
        {
            IsPause = true;
            while (true)
            {
                if (CaseScanCodeTask.GetEntity().WaitForPause || CasePutNutTask.GetEntity().WaitForPause || CaseBendTask.GetEntity().WaitForPause || CasePlateTask.GetEntity().WaitForPause ||
                    CaseEstHeightTask.GetEntity().WaitForPause || CaseMarkingTask.GetEntity().WaitForPause || CaseOutTask.GetEntity().WaitForPause)
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
