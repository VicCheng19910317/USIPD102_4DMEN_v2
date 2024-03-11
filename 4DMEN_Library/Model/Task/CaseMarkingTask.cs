using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class CaseMarkingTask : BaseTask
    {
        internal CaseData case_data = null;
        #region 類別欄位
        private static CaseMarkingTask m_Singleton;
        #endregion
        #region 類別方法
        internal static CaseMarkingTask GetEntity()
        {
            return m_Singleton;
        }
        static CaseMarkingTask()
        {
            m_Singleton = new CaseMarkingTask(nameof(CaseMarkingTask));
        }
        #endregion 類別方法
        #region 建構子
        private CaseMarkingTask(string Name) : base(Name)
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
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"雷雕站開始");
                    Status = EnumData.TaskStatus.Running;
                    if (!MainPresenter.SystemParam().Flow.CaseMarking)
                    {
                        case_data.Step = Step = 5;
                        break;
                    }
                    case_data.Step = Step = (case_data == null || !case_data.IsRun) ? 5 : 1;
                    case_data?.CaseMarkingTime.Start();
                    break;
                case 1: ///檢查雷雕訊號是否正常
                    var processor = MainPresenter.LFJProcessor();
                    if (!processor.ConnectedState)
                    {
                        ErrorMessage = "雷雕設備網路連線失敗，請確認網路連線是否正常，再按下「Resume」按鈕繼續流程。";
                        PauseTask();
                        break;
                    }
                    if (MainPresenter.MainPLC().GetLaserOnStatus())
                    {
                        ErrorMessage = "雷射出光中，請確認雷射狀態是否異常，再按下「Resume」按鈕繼續流程。";
                        PauseTask();
                        break;
                    }
                    if (!MainPresenter.MainPLC().GetLaserReadyStatus())
                    {
                        ErrorMessage = "雷射準備狀態尚未完成，請確認鑰匙是否開啟，再按下「Resume」按鈕繼續流程。";
                        PauseTask();
                        break;
                    }
                    if (!MainPresenter.MainPLC().GetLaserFileStatus())
                    {
                        ErrorMessage = "雷射檔案尚未準備完成，請確認LFJ軟體是否開啟，再按下「Resume」按鈕繼續流程。";
                        PauseTask();
                        break;
                    }
                    case_data.Step = Step = 2;
                    break;
                case 2: ///設定雷射雕刻文字
                    processor = MainPresenter.LFJProcessor();
                    MarkingParam param = new MarkingParam(MainPresenter.SystemParam().MarkParam);
                    param.marking_snd_index = case_data.Index + 1;
                    param.marking_2d_txt = case_data.ReaderResult1;
                    if(case_data.ReaderResult1 == null || case_data.ReaderResult1 == "")
                    {
                        case_data.Step = Step = 5;
                        break;
                    }
                    if (!processor.SetTextParam(param))
                    {
                        ErrorMessage = "雷射參數設定失敗，請確認LFJ軟體是否開啟，再按下「Resume」按鈕繼續流程。";
                        PauseTask();
                        break;
                    }
                    case_data.Step = Step = 3;
                    break;
                case 3: ///執行雷雕
                    processor = MainPresenter.LFJProcessor();
                    if (!processor.StartMarking(MainPresenter.SystemParam().MarkParam.start_marking_code))
                    {
                        ErrorMessage = "執行雷射雕刻設定失敗，請確認LFJ軟體是否開啟，再按下「Resume」按鈕繼續流程。";
                        PauseTask();
                        break;
                    }
                    case_data.Step = Step = 4;
                    break;
                case 4: //確認雷雕是否結束
                    if (MainPresenter.MainPLC().GetLaserOnStatus())
                        break;
                    case_data.Step = Step = 5;
                    break;
                case 5: //完成流程
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"雷雕站完成");
                    case_data.Step = Step = 0;
                    case_data?.CaseMarkingTime.Stop();
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
                if (CaseScanCodeTask.GetEntity().WaitForPause || CasePutNutTask.GetEntity().WaitForPause || CaseBendTask.GetEntity().WaitForPause || CasePlateTask.GetEntity().WaitForPause ||
                    CaseEstHeightTask.GetEntity().WaitForPause || CaseNgOutTask.GetEntity().WaitForPause || CaseOutTask.GetEntity().WaitForPause)
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
