using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    class CaseAllTask : BaseTask
    {
        internal List<CaseData> case_datas = null;
        internal Stopwatch all_cases_time = new Stopwatch();
        internal static int put_station = int.Parse(ConfigurationManager.AppSettings["PutStation"]);
        internal static int lid_station = int.Parse(ConfigurationManager.AppSettings["LidStation"]);
        internal static int nut_station = int.Parse(ConfigurationManager.AppSettings["NutStation"]);
        internal static List<int> bend_station = new List<int> { int.Parse(ConfigurationManager.AppSettings["BendStation"].Split(',')[0]), int.Parse(ConfigurationManager.AppSettings["BendStation"].Split(',')[1]) };
        internal static int plate_station = int.Parse(ConfigurationManager.AppSettings["PlateStation"]);
        internal static int est_hei_station = int.Parse(ConfigurationManager.AppSettings["EstHeiStation"]);
        internal static int ng_station = int.Parse(ConfigurationManager.AppSettings["NgStation"]); 
        internal static int scan_station = int.Parse(ConfigurationManager.AppSettings["ScanStation"]);
        internal static int mark_station = int.Parse(ConfigurationManager.AppSettings["MarkStation"]);
        internal static int out_station = int.Parse(ConfigurationManager.AppSettings["OutStation"]);
        
        
        #region 類別欄位
        private static CaseAllTask m_Singleton;
        #endregion
        #region 類別方法
        internal static CaseAllTask GetEntity()
        {
            return m_Singleton;
        }
        static CaseAllTask()
        {
            m_Singleton = new CaseAllTask(nameof(CaseAllTask));

        }
        #endregion 類別方法
        #region 建構子
        private CaseAllTask(string Name) : base(Name)
        {

        }
        #endregion 建構子
        #region 事件
        internal EventHandler<List<CaseData>> UpdateCaseDataEvent;
        private void OnUpdateCaseData(List<CaseData> caseDatas)
        {
            UpdateCaseDataEvent?.Invoke(this, caseDatas);
        }
        internal EventHandler<RunMainFlowArgs> FinishCaseEvent;
        private void OnFinishCaseEvent(RunMainFlowArgs e)
        {
            FinishCaseEvent?.Invoke(this, e);
        }
        #endregion 事件
        #region 實作方法
        protected override void MyTask()
        {
            #region 執行數量重置與開始自動化入料與組裝站
            if (case_datas == null) case_datas = new List<CaseData>();
            case_datas.Clear();
            for (int i = 0; i < MainPresenter.SystemParam().CaseCount; i++)
            {
                case_datas.Add(new CaseData { Index = i });
            }
            all_cases_time = new Stopwatch();
            all_cases_time.Start();
            CaseDoorCheckTask.GetEntity().StartTask();
            CaseInTask.GetEntity().Status = EnumData.TaskStatus.Running;
            CaseInTask.GetEntity().case_data = case_datas[0];
            CaseInTask.GetEntity().case_datas = case_datas;
            CaseInTask.GetEntity().StartTask();
            CaseLidTask.GetEntity().Status = EnumData.TaskStatus.Running;
            CaseLidTask.GetEntity().case_data = case_datas[0];
            CaseLidTask.GetEntity().case_datas = case_datas;
            CaseLidTask.GetEntity().StartTask();
            #endregion 執行數量重置與開始自動化入料與組裝站
            Status = EnumData.TaskStatus.Running;
            while (true)
            {
                WaitOne();
                System.Threading.Thread.Sleep(200);
                OnUpdateCaseData(case_datas);
                #region 檢查流程動作是否完成
                if (!MainPresenter.GetInitRun() || MainPresenter.GetManualPause()) continue;
                if (!CaseInTask.GetEntity().PutCaseFinish || (CaseInTask.GetEntity().IsPause && CaseInTask.GetEntity().Status != EnumData.TaskStatus.Done) || 
                    !CaseLidTask.GetEntity().PutCaseFinish || (CaseLidTask.GetEntity().IsPause && CaseLidTask.GetEntity().Status != EnumData.TaskStatus.Done) || 
                    CaseScanCodeTask.GetEntity().IsRunning || CasePutNutTask.GetEntity().IsRunning || CaseBendTask.GetEntity().IsRunning || 
                    CasePlateTask.GetEntity().IsRunning || CaseEstHeightTask.GetEntity().IsRunning || CaseNgOutTask.GetEntity().IsRunning || 
                    CaseMarkingTask.GetEntity().IsRunning || CaseOutTask.GetEntity().IsRunning) continue;
                #endregion 檢查流程動作是否完成
                if (case_datas.Where(x => x.Station >= 20).Count() == MainPresenter.SystemParam().CaseCount)
                {
                    
                    RecordData.RecordDataResult(MainPresenter.SystemParam(), case_datas);
                    OnFinishCaseEvent(new RunMainFlowArgs { ReStartFlow = true, Message = "All Cases Is Runing Finish.", Image = System.Windows.MessageBoxImage.Information });
                    ThreadState = System.Threading.ThreadState.Stopped;
                    all_cases_time.Stop();
                    Stop();
                    break;
                }
                var success = RunMoveNext();
                if (success) RunFlowTask();
            }
            Status = EnumData.TaskStatus.Idle;
        }
        /// <summary>
        /// 移動到下一站
        /// </summary>
        private bool RunMoveNext()
        {
            if (!MainPresenter.MainPLC().RunToNextStep())
            {
                PauseTask();
                ErrorMessage = $"主流道錯誤，請至PLC確認異常訊息，並排除相關問題後，按下「Resume」按鈕接續流程。";
                return false;
            }
            for (int i = 0; i < case_datas.Count; i++)
            {
                var case_data = case_datas[i];
                if (case_data.Station > 0 && case_data.Station <= 20)
                    case_data.Station += 1;
                else if (case_data.Station == 0)
                {
                    case_data.Station = 1;
                    break;
                }
            }

            return true;
        }
        private void RunFlowTask()
        {
            var flow_datas = case_datas.Where(x => x.Station > 0).ToList();
            foreach (var flow_data in flow_datas)
            {
                if (flow_data.Station == put_station)
                {
                    CaseInTask.GetEntity().CanPutCase = true;
                    CaseInTask.GetEntity().PutCaseFinish = false;
                }
                else if(flow_data.Station == lid_station && MainPresenter.SystemParam().Flow.CaseAssemble)
                {
                    CaseLidTask.GetEntity().CanPutCase = true;
                    CaseLidTask.GetEntity().PutCaseFinish = false;
                }
                
                else if (flow_data.Station == nut_station)
                {
                    CasePutNutTask.GetEntity().Status = EnumData.TaskStatus.Running;
                    CasePutNutTask.GetEntity().case_data = flow_data;
                    CasePutNutTask.GetEntity().StartTask();
                }
                else if (!CaseBendTask.GetEntity().IsStartBend && bend_station.Contains(flow_data.Station))
                {
                    CaseBendTask.GetEntity().Status = EnumData.TaskStatus.Running;
                    CaseBendTask.GetEntity().IsStartBend = true;
                    CaseBendTask.GetEntity().case_data = flow_data;
                    CaseBendTask.GetEntity().StartTask();
                }
                else if (flow_data.Station == plate_station)
                {
                    CasePlateTask.GetEntity().Status = EnumData.TaskStatus.Running;
                    CasePlateTask.GetEntity().case_data = flow_data;
                    CasePlateTask.GetEntity().StartTask();
                }
                else if(flow_data.Station == est_hei_station)
                {
                    CaseEstHeightTask.GetEntity().Status = EnumData.TaskStatus.Running;
                    CaseEstHeightTask.GetEntity().case_data = flow_data;
                    CaseEstHeightTask.GetEntity().StartTask();
                }
                else if (flow_data.Station == ng_station)
                {
                    CaseNgOutTask.GetEntity().Status = EnumData.TaskStatus.Running;
                    CaseNgOutTask.GetEntity().case_data = flow_data;
                    CaseNgOutTask.GetEntity().StartTask();
                }
                else if (flow_data.Station == scan_station)
                {
                    CaseScanCodeTask.GetEntity().Status = EnumData.TaskStatus.Running;
                    CaseScanCodeTask.GetEntity().case_data = flow_data;
                    CaseScanCodeTask.GetEntity().StartTask();
                }
                else if (flow_data.Station == mark_station)
                {
                    CaseMarkingTask.GetEntity().Status = EnumData.TaskStatus.Running;
                    CaseMarkingTask.GetEntity().case_data = flow_data;
                    CaseMarkingTask.GetEntity().StartTask();
                }
                else if (flow_data.Station == out_station)
                {
                    CaseOutTask.GetEntity().Status = EnumData.TaskStatus.Running;
                    CaseOutTask.GetEntity().case_data = flow_data;
                    CaseOutTask.GetEntity().StartTask();
                }
            }
        }
        /// <summary>
        /// 暫停流程
        /// </summary>
        internal void Pause()
        {
            Status = EnumData.TaskStatus.Pause;
            PauseTask();
            if (CaseInTask.GetEntity().ThreadState == System.Threading.ThreadState.Running)
                CaseInTask.GetEntity().PauseTaskWithoutWait();
            if (CaseLidTask.GetEntity().ThreadState == System.Threading.ThreadState.Running)
                CaseLidTask.GetEntity().PauseTaskWithoutWait();
            if (CaseScanCodeTask.GetEntity().ThreadState == System.Threading.ThreadState.Running)
                CaseScanCodeTask.GetEntity().PauseTaskWithoutWait();
            if (CasePutNutTask.GetEntity().ThreadState == System.Threading.ThreadState.Running)
                CasePutNutTask.GetEntity().PauseTaskWithoutWait();
            if (CaseBendTask.GetEntity().ThreadState == System.Threading.ThreadState.Running)
                CaseBendTask.GetEntity().PauseTaskWithoutWait();
            if (CasePlateTask.GetEntity().ThreadState == System.Threading.ThreadState.Running)
                CasePlateTask.GetEntity().PauseTaskWithoutWait();
            if (CaseEstHeightTask.GetEntity().ThreadState == System.Threading.ThreadState.Running)
                CaseEstHeightTask.GetEntity().PauseTaskWithoutWait();
            if (CaseNgOutTask.GetEntity().ThreadState == System.Threading.ThreadState.Running)
                CaseNgOutTask.GetEntity().PauseTaskWithoutWait();
            if (CaseMarkingTask.GetEntity().ThreadState == System.Threading.ThreadState.Running)
                CaseMarkingTask.GetEntity().PauseTaskWithoutWait();
            if (CaseOutTask.GetEntity().ThreadState == System.Threading.ThreadState.Running)
                CaseOutTask.GetEntity().PauseTaskWithoutWait();
            if (CaseInStationTask.GetEntity().ThreadState == System.Threading.ThreadState.Running)
                CaseInStationTask.GetEntity().PauseTaskWithoutWait();
            if (CaseLidStationTask.GetEntity().ThreadState == System.Threading.ThreadState.Running)
                CaseLidStationTask.GetEntity().PauseTaskWithoutWait();
            if (CaseOutStationTask.GetEntity().ThreadState == System.Threading.ThreadState.Running)
                CaseOutStationTask.GetEntity().PauseTaskWithoutWait();
            if (CaseDoorCheckTask.GetEntity().ThreadState == System.Threading.ThreadState.Running)
                CaseDoorCheckTask.GetEntity().PauseTaskWithoutWait();
        }
        /// <summary>
        /// 恢復流程
        /// </summary>
        internal void Resume()
        {
            if (CaseDoorCheckTask.GetEntity().ThreadState == System.Threading.ThreadState.SuspendRequested || CaseDoorCheckTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
            {
                CaseDoorCheckTask.GetEntity().DoorOpen = false;
                CaseDoorCheckTask.GetEntity().ResumeTask();
                System.Threading.Thread.Sleep(200);
            }
            if (CaseInTask.GetEntity().ThreadState == System.Threading.ThreadState.SuspendRequested || CaseInTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                CaseInTask.GetEntity().ResumeTask();
            if (CaseLidTask.GetEntity().ThreadState == System.Threading.ThreadState.SuspendRequested || CaseLidTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                CaseLidTask.GetEntity().ResumeTask();
            if (CaseScanCodeTask.GetEntity().ThreadState == System.Threading.ThreadState.SuspendRequested || CaseScanCodeTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                CaseScanCodeTask.GetEntity().ResumeTask();
            if (CasePutNutTask.GetEntity().ThreadState == System.Threading.ThreadState.SuspendRequested || CasePutNutTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                CasePutNutTask.GetEntity().ResumeTask();
            if (CaseBendTask.GetEntity().ThreadState == System.Threading.ThreadState.SuspendRequested || CaseBendTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                CaseBendTask.GetEntity().ResumeTask();
            if (CasePlateTask.GetEntity().ThreadState == System.Threading.ThreadState.SuspendRequested || CasePlateTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                CasePlateTask.GetEntity().ResumeTask();
            if (CaseEstHeightTask.GetEntity().ThreadState == System.Threading.ThreadState.SuspendRequested || CaseEstHeightTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                CaseEstHeightTask.GetEntity().ResumeTask();
            if (CaseNgOutTask.GetEntity().ThreadState == System.Threading.ThreadState.SuspendRequested || CaseNgOutTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                CaseNgOutTask.GetEntity().ResumeTask();
            if (CaseMarkingTask.GetEntity().ThreadState == System.Threading.ThreadState.SuspendRequested || CaseMarkingTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                CaseMarkingTask.GetEntity().ResumeTask();
            if (CaseOutTask.GetEntity().ThreadState == System.Threading.ThreadState.SuspendRequested || CaseOutTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                CaseOutTask.GetEntity().ResumeTask();
            if (CaseInStationTask.GetEntity().ThreadState == System.Threading.ThreadState.SuspendRequested || CaseInStationTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                CaseInStationTask.GetEntity().ResumeTask();
            if (CaseLidStationTask.GetEntity().ThreadState == System.Threading.ThreadState.SuspendRequested || CaseLidStationTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                CaseLidStationTask.GetEntity().ResumeTask();
            if (CaseOutStationTask.GetEntity().ThreadState == System.Threading.ThreadState.SuspendRequested || CaseOutStationTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                CaseOutStationTask.GetEntity().ResumeTask();
            ResumeTask(); //最後在執行總流道的Resume 避免偷跑
            System.Threading.Thread.Sleep(100);
        }
        /// <summary>
        /// 停止流程
        /// </summary>
        internal void Stop()
        {
            CaseInTask.GetEntity().StopTask();
            CaseInTask.GetEntity().Status = EnumData.TaskStatus.Done;
            CaseInTask.GetEntity().ResetStep();

            CaseLidTask.GetEntity().StopTask();
            CaseLidTask.GetEntity().Status = EnumData.TaskStatus.Done;
            CaseLidTask.GetEntity().ResetStep();

            CaseScanCodeTask.GetEntity().StopTask();
            CaseScanCodeTask.GetEntity().Status = EnumData.TaskStatus.Done;
            CaseScanCodeTask.GetEntity().ResetStep();

            CasePutNutTask.GetEntity().StopTask();
            CasePutNutTask.GetEntity().Status = EnumData.TaskStatus.Done;
            CasePutNutTask.GetEntity().ResetStep();

            CaseBendTask.GetEntity().StopTask();
            CaseBendTask.GetEntity().Status = EnumData.TaskStatus.Done;
            CaseBendTask.GetEntity().ResetStep();
            

            CasePlateTask.GetEntity().StopTask();
            CasePlateTask.GetEntity().Status = EnumData.TaskStatus.Done;
            CasePlateTask.GetEntity().ResetStep();

            CaseEstHeightTask.GetEntity().StopTask();
            CaseEstHeightTask.GetEntity().Status = EnumData.TaskStatus.Done;
            CaseEstHeightTask.GetEntity().ResetStep();

            CaseNgOutTask.GetEntity().StopTask();
            CaseNgOutTask.GetEntity().Status = EnumData.TaskStatus.Done;
            CaseNgOutTask.GetEntity().ResetStep();

            CaseMarkingTask.GetEntity().StopTask();
            CaseMarkingTask.GetEntity().Status = EnumData.TaskStatus.Done;
            CaseMarkingTask.GetEntity().ResetStep();

            CaseOutTask.GetEntity().StopTask();
            CaseOutTask.GetEntity().Status = EnumData.TaskStatus.Done;
            CaseOutTask.GetEntity().ResetStep();

            CaseInStationTask.GetEntity().StopTask();
            CaseInStationTask.GetEntity().Status = EnumData.TaskStatus.Done;
            CaseInStationTask.GetEntity().ResetStep();

            CaseLidStationTask.GetEntity().StopTask();
            CaseLidStationTask.GetEntity().Status = EnumData.TaskStatus.Done;
            CaseLidStationTask.GetEntity().ResetStep();

            CaseOutStationTask.GetEntity().StopTask();
            CaseOutStationTask.GetEntity().Status = EnumData.TaskStatus.Done;
            CaseOutStationTask.GetEntity().ResetStep();

            CaseDoorCheckTask.GetEntity().StopTask();
            CaseDoorCheckTask.GetEntity().Status = EnumData.TaskStatus.Done;
            CaseDoorCheckTask.GetEntity().ResetStep();
            StopTask();
        }
        #endregion
    }
}
