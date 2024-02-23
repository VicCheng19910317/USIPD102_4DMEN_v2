using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class CaseDoorCheckTask : BaseTask
    {
        internal bool DoorOpen { get; set; } = false;
        #region 類別欄位
        private static CaseDoorCheckTask m_Singleton;
        #endregion
        #region 類別方法
        internal static CaseDoorCheckTask GetEntity()
        {
            return m_Singleton;
        }
        static CaseDoorCheckTask()
        {
            m_Singleton = new CaseDoorCheckTask(nameof(CaseDoorCheckTask));
        }
        #endregion 類別方法
        #region 建構子
        private CaseDoorCheckTask(string Name) : base(Name)
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
                else if (!MainPresenter.GetInitRun() || MainPresenter.GetManualPause()) continue;
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
                    DoorOpen = MainPresenter.MainPLC().GetDoorStatus(typeof(CaseDoorCheckTask).ToString());
                    if (DoorOpen)
                    {
                        PauseTask();
                        ErrorMessage = "Machine door opened, please close the door and click「Resume」 button.";
                        break;
                    }
                    System.Threading.Thread.Sleep(100);
                    break;
            }
        }
        #endregion 實作方法
    }
}
