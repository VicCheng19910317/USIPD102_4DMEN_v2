using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class CaseOutTask: BaseTask
    {
        internal CaseData case_data = null;
        internal bool IsNG { get; set; } = false;
        internal int NGCount { get; set; } = 0;
        internal int NGCountLimit { get; set; } = 2;
        private DateTime CTTime { get; set; } = DateTime.Now;
        #region 類別欄位
        private static CaseOutTask m_Singleton;
        #endregion 類別欄位
        #region 類別方法
        internal static CaseOutTask GetEntity()
        {
            return m_Singleton;
        }
        static CaseOutTask()
        {
            m_Singleton = new CaseOutTask(nameof(CaseOutTask));
        }
        #endregion 類別方法
        #region 建構子
        private CaseOutTask(string Name) : base(Name)
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
                case 0:
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"出料流程開始");
                    Status = EnumData.TaskStatus.Running;
                    if (case_data == null || !case_data.IsRun || case_data.ManualNG)
                    {
                        case_data.Step = Step = 11;
                        break;
                    }
                    IsNG = false;
                    case_data.Step = Step = 1;
                    if(!MainPresenter.SystemParam().Flow.CaseScan)
                        case_data.Step = Step = 6;
                    case_data?.CaseOutTime.Start();
                    break;
                case 1: //手臂移到雷雕掃碼處
                    if (!DoArmsAction(() => MainPresenter.CaseOutArms().SetCOne(), MainPresenter.CaseOutArms(), "出料手臂移至雷雕掃碼處錯誤，請重新將手臂回Home再行後續動作")) break;
                    case_data.Step = Step = 2;
                    break;
                case 2: //執行雷雕掃碼辨識
                    var reader = MainPresenter.OutReader();
                    string read_data = "", read_level = "";
                    var internal_finish = DoReaderAction(() => reader.Read(), reader, out read_data,out read_level, "掃碼讀取訊號超時，請按下「Resume」系統進行後續放料流程。\n錯誤訊號：", false);
                    if (internal_finish)
                    { ///判斷是否成功
                        case_data.ReaderResult2 = read_data;
                        case_data.MarkingLevel = read_level;
                    }
                    else
                    {
                        IsNG = true;
                        Step = 6;
                        if (!case_data.DefectCode.Contains(MainPresenter.SystemParam().DefectMapping["BasePlate/ Lid 2D code異常"]))
                            case_data.DefectCode.Add(MainPresenter.SystemParam().DefectMapping["BasePlate/ Lid 2D code異常"]);
                        if (!case_data.NGPosition.Contains(20))
                            case_data.NGPosition.Add(20);
                        break;
                    }
                    case_data.Step = Step = 3;
                    break;
                case 3: //判斷雷雕品質
                    if (!MainPresenter.SystemParam().MarkParam.pass_level.Contains(case_data.MarkingLevel))
                    {
                        IsNG = true;
                        Step = 6;
                        if (!case_data.DefectCode.Contains(MainPresenter.SystemParam().DefectMapping["鐳碼等級異常"]))
                            case_data.DefectCode.Add(MainPresenter.SystemParam().DefectMapping["鐳碼等級異常"]);
                        if (!case_data.NGPosition.Contains(20))
                            case_data.NGPosition.Add(20);
                        break;
                    }
                    Step = 4;
                    break;
                case 4: //手臂移到底板掃碼處
                    if (!DoArmsAction(() => MainPresenter.CaseOutArms().SetCTwo(), MainPresenter.CaseOutArms(), "出料手臂移至底板掃碼處錯誤，請重新將手臂回Home再行後續動作")) break;
                    case_data.Step = Step = 5;
                    break;
                case 5: // 進行底板掃碼並判斷是否正確
                    reader = MainPresenter.OutReader();
                    read_data = "";
                    read_level = "";
                    internal_finish = DoReaderAction(() => reader.Read(), reader, out read_data, out read_level, "掃碼讀取訊號超時，請按下「Resume」系統進行後續放料流程。\n錯誤訊號：", false);
                    if (internal_finish && read_data == case_data.ReaderResult1)  { ///判斷是否成功
                        case_data.Step = Step = 6;
                        break;
                    }
                    else
                    {
                        Step = 6;
                        if (!case_data.DefectCode.Contains(MainPresenter.SystemParam().DefectMapping["BasePlate/ Lid 2D code異常"]))
                            case_data.DefectCode.Add(MainPresenter.SystemParam().DefectMapping["BasePlate/ Lid 2D code異常"]);
                        if (!case_data.NGPosition.Contains(20))
                            case_data.NGPosition.Add(20);
                        IsNG = true;
                        break;
                    }
                case 6: //手臂取料
                    if (!DoArmsAction(() => MainPresenter.CaseOutArms().Pick(), MainPresenter.CaseOutArms(), "出料手臂取料錯誤，請重新將手臂回Home再行後續動作")) break;
                    case_data.Step = Step = 7;
                    break;
                case 7: //設定手臂放料順序
                    if (!DoArmsAction(() => MainPresenter.CaseOutArms().SetPickPos((case_data.Index % 12) + 1), MainPresenter.CaseInArms(), "出料手臂放料順序設定錯誤，請重新將手臂回Home再行後續動作")) break;
                    case_data.Step = Step = 8;
                    break;
                case 8: ///判斷換Tray是否完成
                    if (!(CaseOutStationTask.GetEntity().Status == EnumData.TaskStatus.Done || CaseOutStationTask.GetEntity().Status == EnumData.TaskStatus.Idle))
                    {
                        System.Threading.Thread.Sleep(300);
                        break;
                    }
                    case_data.Step = Step = 9;
                    break;
                case 9://手臂放料台車上
                    if (IsNG)
                    {
                        if (!DoArmsAction(() => MainPresenter.CaseOutArms().SetMoveNG(), MainPresenter.CaseOutArms(), "出料手臂放料至NG區錯誤，請重新將手臂回Home再行後續動作")) break;
                        NGCount++;
                    }   
                    else
                        if (!DoArmsAction(() => MainPresenter.CaseOutArms().Put(), MainPresenter.CaseInArms(), "出料手臂放料錯誤，請重新將手臂回Home再行後續動作")) break;
                    case_data.IsRun = false;
                    
                    case_data.Step = Step = 10;
                    break;
                case 10: //判斷NG是否超過上限
                    if(NGCount >= NGCountLimit)
                    {
                        ErrorMessage = "出料NG區已滿載，請將出料NG區除料後，按下「」後，再按下「Resume接續流程。";
                        PauseTask();
                        break;
                    }
                    case_data.Step = Step = 11;
                    break;
                case 11: //發送SFIS資料
                    if (MainPresenter.SystemParam().Sfis.Enable)
                    {
                        MainPresenter.SFIS().SendStep(3, MainPresenter.SystemParam().Sfis, case_data);
                    }
                    case_data.CTTime = (DateTime.Now - CTTime).TotalSeconds;
                    CTTime = DateTime.Now;
                    case_data.Step = Step = 12;
                    break;
                case 12: //判斷出料平台是否要換Tray
                    if (!MainPresenter.GetRunSingleFlow() && ((case_data.Index + 1) == MainPresenter.SystemParam().CaseCount || ((case_data.Index + 1) % MainPresenter.SystemParam().TrayCount == 0 && case_data.Index > 0)))
                    {
                        Thread.Sleep(1000);
                        if ((case_data.Index + 1) == MainPresenter.SystemParam().CaseCount) CaseOutStationTask.GetEntity().PassDetect = true;
                        CaseOutStationTask.GetEntity().StartTask(); ///開始換Tray
                        System.Threading.Thread.Sleep(200);
                    }
                    case_data.Step = Step = 13;
                    break;
                case 13: //完成流程
                    case_data.Step = Step = 0;
                    case_data?.CaseOutTime.Stop();
                    case_data?.CaseTotalTime.Stop();
                    MainPresenter.SetRunSingleFlow(false);
                    MainPresenter.SetRunFlow(false);
                    case_data.IsRun = false;
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"出料流程完成");
                    Status = EnumData.TaskStatus.Done;
                    break;
            }
        }
        #endregion 實作方法
        #region 覆寫方法
        public override void PauseTask()
        {
            IsPause = true;
            while (true)
            {
                if (CaseScanCodeTask.GetEntity().WaitForPause || CasePutNutTask.GetEntity().WaitForPause || CaseBendTask.GetEntity().WaitForPause || CasePlateTask.GetEntity().WaitForPause ||
                    CaseEstHeightTask.GetEntity().WaitForPause || CaseNgOutTask.GetEntity().WaitForPause || CaseMarkingTask.GetEntity().WaitForPause)
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
        #endregion 覆寫方法
    }
}
