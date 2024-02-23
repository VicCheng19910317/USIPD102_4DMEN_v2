using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class CaseOutStationTask : BaseTask
    {
        internal bool PassDetect { get; set; } = false;
        private DateTime timer { get; set; }
        private int WaitTrayTime { get; set; } = 120;
        #region 類別欄位
        private static CaseOutStationTask m_Singleton;
        #endregion
        #region 類別方法
        internal static CaseOutStationTask GetEntity()
        {
            return m_Singleton;
        }
        static CaseOutStationTask()
        {
            m_Singleton = new CaseOutStationTask(nameof(CaseOutStationTask));
        }
        #endregion 類別方法
        #region 建構子
        private CaseOutStationTask(string Name) : base(Name)
        {

        }
        #endregion 建構子
        #region 實作方法
        protected override void MyTask()
        {
            Status = EnumData.TaskStatus.Running;
            while (true)
            {
                WaitOne();
                if (MainPresenter.GetInitRun() && MainPresenter.GetRunSingleStep())
                {
                    RunStep();
                    MainPresenter.SetRunSingleStep(false);
                }
                else if (!MainPresenter.GetInitRun() ||
                    MainPresenter.GetManualPause()) continue;
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
                case 0: //出料站換Tray
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"出料站流程開始");
                    Status = EnumData.TaskStatus.Running;
                   
                    var plc = MainPresenter.OutPLC();
                    plc.GetStatus(); //取得Load-Unload狀態
                    if (plc.IsFinish || plc.IsIdle) //確認Load-Unload狀態是否完成
                    {
                        if (!DoPLCAction(() => plc.RunOneStepFlow(), plc)) break; //進行換Tray
                    }
                    Step = PassDetect ? 2 : 1;
                    timer = DateTime.Now;
                    break;
                case 1: //等待換Tray完成
                    plc = MainPresenter.OutPLC();
                    plc.GetStatus(); //取得出料平台狀態
                    if (plc.IsSend) break;
                    if (plc.IsFinish || plc.IsIdle)
                    {
                        Step = 2;
                        break;
                    }
                    if (plc.IsChangedCassette)
                    {
                        if ((DateTime.Now - timer).TotalSeconds < WaitTrayTime)
                        {
                            System.Threading.Thread.Sleep(200);
                            break;
                        }
                        PauseTask();
                        ErrorMessage = "請更換出料站Cassette，完成後按下Resume按鈕繼續。";
                    }
                    Step = 2;
                    break;
                case 2: //完成流程
                    plc = MainPresenter.OutPLC();
                    plc.GetStatus(); //取得出料平台狀態
                    if (!(plc.IsFinish || plc.IsIdle))
                    {
                        Step = 1;
                        timer = DateTime.Now;
                        break;
                    }
                    RecordData.RecordProcessData(MainPresenter.SystemParam(), $"出料站流程完成");
                    Step = 0;
                    MainPresenter.SetRunFlow(false);
                    Status = EnumData.TaskStatus.Done;
                    break;
            }
        }
        #endregion
    }
}
