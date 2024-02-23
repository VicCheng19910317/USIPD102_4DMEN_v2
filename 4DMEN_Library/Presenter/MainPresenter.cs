using _4DMEN_Library.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _4DMEN_Library
{
    public class MainPresenter
    {
        #region 屬性參數
        private MainView view;
        /// <summary>
        /// 初始執行
        /// </summary>
        protected bool init_run = false;
        /// <summary>
        /// 暫停
        /// </summary>
        protected bool manual_pause = false;
        /// <summary>
        /// 單動步驟
        /// </summary>
        protected bool run_single_step = false;
        /// <summary>
        /// 單動流程
        /// </summary>
        protected bool run_single_flow = false;
        /// <summary>
        /// 執行流程
        /// </summary>
        protected bool run_flow = false;
        /// <summary>
        /// 是否執行中
        /// </summary>
        protected bool is_run = false;
        /// <summary>
        /// 資料紀錄
        /// </summary>
        private List<LogData> _logger = new List<LogData>();
        private List<LogData> logger
        {
            get => _logger;
            set
            {
                _logger = value;
                OnPresentResponseEvent("update_logger_data", new UpdateLoggerArgs { Datas = _logger });
            }
        }
        /// <summary>
        /// 系統參數
        /// </summary>
        protected SystemParam system_param = new SystemParam();
        
        /// <summary>
        /// 入料手臂
        /// </summary>
        internal EsponArmsProcessor_In in_arms = new EsponArmsProcessor_In();
        /// <summary>
        /// 組裝上蓋手臂
        /// </summary>
        internal EsponArmsProcessor lid_arms = new EsponArmsProcessor();
        /// <summary>
        /// 出料手臂
        /// </summary>
        internal EsponArmsProcessor out_arms = new EsponArmsProcessor();
        /// <summary>
        /// 主流道PLC
        /// </summary>
        internal KeyencePLCProcessor main_plc = new KeyencePLCProcessor();
        /// <summary>
        /// 螺帽站PLC
        /// </summary>
        internal KeyencePLCProcessor nut_plc = new KeyencePLCProcessor();
        /// <summary>
        /// 折彎站PLC
        /// </summary>
        internal KeyencePLCProcessor bend_plc = new KeyencePLCProcessor();
        /// <summary>
        /// 下壓站PLC
        /// </summary>
        internal KeyencePLCProcessor plate_plc = new KeyencePLCProcessor();
        /// <summary>
        /// 測高站PLC
        /// </summary>
        internal KeyencePLCProcessor_Height height_plc = new KeyencePLCProcessor_Height();
        /// <summary>
        /// NG站PLC
        /// </summary>
        internal KeyencePLCProcessor ng_plc = new KeyencePLCProcessor();
        /// <summary>
        /// 入料站PLC
        /// </summary>
        internal KeyenceLoadPLCProcessor in_station_plc = new KeyenceLoadPLCProcessor();
        /// <summary>
        /// 組裝站PLC
        /// </summary>
        internal KeyenceLoadPLCProcessor lid_station_plc = new KeyenceLoadPLCProcessor();
        /// <summary>
        /// 出料站PLC
        /// </summary>
        internal KeyenceLoadPLCProcessor out_station_plc = new KeyenceLoadPLCProcessor();
        /// <summary>
        /// 掃碼站reader
        /// </summary>
        internal KeyenceReaderProcessor reader = new KeyenceReaderProcessor();
        /// <summary>
        /// 出料站reader
        /// </summary>
        internal KeyenceReaderProcessor out_reader = new KeyenceReaderProcessor();
        /// <summary>
        /// 測高處理器
        /// </summary>
        internal LKIFProcessor lk_processor = new LKIFProcessor();
        /// <summary>
        /// 雷雕處理器
        /// </summary>
        internal LFJProcessor lfj_processor = new LFJProcessor();
        /// <summary>
        /// 資料上報
        /// </summary>
        private SfisProcessor _sfis = new SfisProcessor();
        #endregion 屬性參數
        #region 事件
        /// <summary>
        /// 前後端溝通用事件
        /// </summary>
        public event EventHandler<EventArgs> PresentResponseEvent;
        private void OnPresentResponseEvent(string response_name, EventArgs e)
        {
            PresentResponseEvent?.Invoke(response_name, e);
        }
        private void PresenterSendEvent(object sender, EventArgs e)
        {
            string actions = sender as string;
            if (actions == "system_initialize")
                Initialized();
            else if (actions == "system_closing")
                Closing();
            else if (actions == "send_reader_action")
                SendReaderAction((SendReaderActionArgs)e);
            else if (actions == "send_arms_action")
                SendArmsAction((SendArmsActionArgs)e);
            else if (actions == "send_plc_action")
                SendPLCAction((SendPLCActionArgs)e);
            else if (actions == "send_height_action")
                SendLaserHeightAction((SendLaserHeightActionsArgs)e);
            else if (actions == "send_marking_action")
                SendMarkingAction((SendMarkingActionsArgs)e);
            else if (actions == "sfis_step")
                RunSfisStepAction((SfisStepArgs)e);
            else if (actions == "set_arms_shift")
                SetArmsShiftAction((ArmsShiftArgs)e);
            else if (actions == "get_arms_shift")
                GetArmsShiftAction((ArmsShiftArgs)e);
            else if (actions == "set_plate_accuracy")
                SetPlateAccuracyAction((SetPlateAccuracyArgs)e);
            else if (actions == "save_system_param")
                SaveSystemParamAction((SystemParamArgs)e);
            else if (actions == "estimate_height_action")
                EstHeighAction();
            else if (actions == "set_worksheet")
                SetWorksheetAction((SetWorkSheetArgs)e);
            else if (actions == "run_main_flow")
                RunMainFlowAction((RunMainFlowArgs)e);
            else if (actions == "manual_ng_setting")
                ManualNgSettingAction((ManualNGSettingArgs)e);
            else if (actions == "reset_ng_count_action")
                ResetNgCountAction();
            else if (actions == "reset_out_ng_count_action")
                ResetOutNgCountAction();
            else if (actions == "reset_pc_error_action")
                ResetPCErrorAction();
            else if (actions == "set_flow")
                SetSystemFlowAction((SetSystemFlowArgs)e);
            else if (actions == "send_single_station_flow")
                SendSingleStationFlow((SendSingleStationFlowArgs)e);
            else if (actions == "send_single_station_control_flow")
                SendSingleStationControlFlow((SendSingleStationFlowArgs)e);
            else if (actions == "load_log_datas")
                LoadLogDatasAction();
        }
        private void MainPresenter_UpdateCaseDataEvent(object sender, List<CaseData> caseDatas)
        {
            OnPresentResponseEvent("update_all_loop_data", new UpdateCaseDatasArgs { CaseDatas = caseDatas });
        }
        private void MainPresenter_ShowFlowErrorEvent(object sender, EventArgs e)
        {
            CaseAllTask.GetEntity().Status = EnumData.TaskStatus.Pause;
            if (!((SendMessageBoxArgs)e).Message.Contains("Cassette"))
                main_plc.PCErrorSet(1);
            is_run = false;
            OnPresentResponseEvent("send_flow_error", e);
        }
        private void MainPresenter_ShowMessageEvent(object sender, EventArgs e)
        {
            OnPresentResponseEvent("show_message", e);
        }
        private void MainPresenter_FinishCaseEvent(object sender, RunMainFlowArgs e)
        {
            OnPresentResponseEvent("run_main_flow", e);
            main_plc.PCErrorSet(0);
            is_run = false;
        }
        #endregion 事件
        #region 實作功能
        private void Initialized()
        {
            try
            {
                LoadSystemParam();
                CaseNgOutTask.GetEntity().NgCountLimit = system_param.NgOutCountLimit;
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, "系統參數初始化失敗。");
            }
            try
            {
                LoadArmsParam();
                var arms_in_init = in_arms.SendActionFunction("LOGIN");
                if (!arms_in_init) OnPresentResponseEvent("system_initialize", new SendInitResponseArgs { message = in_arms.Message, success = arms_in_init });
                var arms_glue_init = lid_arms.SendActionFunction("LOGIN");
                if (!arms_glue_init) OnPresentResponseEvent("system_initialize", new SendInitResponseArgs { message = lid_arms.Message, success = arms_glue_init });
                var arms_out_init = out_arms.SendActionFunction("LOGIN");
                if (!arms_out_init) OnPresentResponseEvent("system_initialize", new SendInitResponseArgs { message = out_arms.Message, success = arms_out_init });

            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, "手臂初始化失敗。");
            }
            try
            {
                LoadPLCParam();
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, "PLC初始化失敗。");
            }
            try
            {
                LoadReaderParam();
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, "條碼機初始化失敗。");
            }
            try
            {
                LoadLaserHeighParam();
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, "測高機初始化失敗。");
            }
            try
            {
                logger = LoggerData.LoadLogData();
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, "歷史資料Logger數初始化失敗。");
            }
            init_run = true;
        }
        private void Closing()
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, "系統關閉失敗。");
            }
        }
        #region 系統參數功能
        private void LoadArmsParam()
        {
            try
            {
                List<EsponArmsParam> arms_params = new List<EsponArmsParam>();
                if (!File.Exists("Arms_Settings.ini"))
                {
                    var ini_file = new IniFile("Arms_Settings.ini");
                    ini_file.Write("IP", "192.168.0.71", "Arms In");
                    ini_file.Write("Port", "2000", "Arms In");
                    ini_file.Write("Timeout", "10000", "Arms In");
                    ini_file.Write("Actions", "PICK,PUTP,PROD;0,HOME,SYPK;1,SYPU;1,PUBK;1,RESET", "Arms In");
                    arms_params.Add(new EsponArmsProcessor { Name = "Arms In", IP = "192.168.0.71", Port = 2000, Timeout = 10000, Action_Name = new string[] { "PICK", "PUTP", "PROD;0", "HOME", "SYPU;1", "SYPK;1", "PUBK;1", "RESET" }.ToList() });
                    ini_file.Write("IP", "192.168.0.72", "Arms Lid");
                    ini_file.Write("Port", "2000", "Arms Lid");
                    ini_file.Write("Timeout", "10000", "Arms Lid");
                    ini_file.Write("Actions", "PICK,PUTP,PROD;0,HOME,SYPU;1,SYPK;1,RESET", "Arms Lid");
                    arms_params.Add(new EsponArmsProcessor { Name = "Arms Lid", IP = "192.168.0.72", Port = 2000, Timeout = 10000, Action_Name = new string[] { "PICK", "PUTP", "PROD;0", "HOME", "SYPU;1", "SYPK;1", "RESET" }.ToList() });
                    ini_file.Write("IP", "192.168.0.73", "Arms Out");
                    ini_file.Write("Port", "2000", "Arms Out");
                    ini_file.Write("Timeout", "10000", "Arms Out");
                    ini_file.Write("Actions", "CONE,CTWO,PICK,PUTP,PROD;0,HOME,SYPU;1,SYPK;1,GONG,RESET", "Arms Out");
                    arms_params.Add(new EsponArmsProcessor { Name = "Arms Out", IP = "192.168.0.73", Port = 2000, Timeout = 10000, Action_Name = new string[] { "CONE", "CTWO", "PICK", "PUTP", "PROD;0", "HOME", "SYPU;1", "SYPK;1", "GONG", "RESET" }.ToList() });
                }
                else
                {
                    var ini_file = new IniFile("Arms_Settings.ini");
                    for (int i = 0; i < 3; i++)
                    {
                        EsponArmsParam param = new EsponArmsParam() { Name = i == 0 ? "Arms In" : i == 1 ? "Arms Lid" : "Arms Out" };
                        param.IP = ini_file.Read("IP", param.Name);
                        param.Port = int.Parse(ini_file.Read("Port", param.Name));
                        param.Timeout = int.Parse(ini_file.Read("Timeout", param.Name));
                        param.Action_Name = ini_file.Read("Actions", param.Name).Split(',').ToList();
                        arms_params.Add(param);
                    }
                }
                in_arms.SetArmsParam(arms_params[0]);
                lid_arms.SetArmsParam(arms_params[1]);
                out_arms.SetArmsParam(arms_params[2]);
                OnPresentResponseEvent("load_arms_params", new LoadArmsParamArgs { arms_param = arms_params });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, "讀取手臂參數失敗。");
            }
        }
        private void LoadPLCParam()
        {
            try
            {
                List<DisplayPLCAction> plc_action = new List<DisplayPLCAction>();
                KeyencePLCNetParam param = new KeyencePLCNetParam();
                if (!File.Exists("PLC_Settings.ini"))
                {
                    var ini_file = new IniFile("PLC_Settings.ini");
                    ini_file.Write("IP", "192.168.0.10", "PLC Param");
                    ini_file.Write("Port", "8501", "PLC Param");
                    ini_file.Write("Timeout", "10000", "PLC Param");
                    ini_file.Write("Station", "Main,Nut,Bend,Plate,Height,NG,In,Lid,Out", "PLC Param");
                    param = new KeyencePLCNetParam { plcNet = new PLCNet { IP = "192.168.0.10", Port = 8501, timeout = 10000 } };
                    ini_file.Write("Read Address", "100", "Main Station");
                    ini_file.Write("Write Address", "101", "Main Station");
                    ini_file.Write("Action Detail", "臺車移動,單站初始化,全機初始化,關閉通訊", "Main Station");
                    plc_action.Add(new DisplayPLCAction { Station = "Main", ReadAddress = "100", WriteAddress = "101", ActionDetail = "臺車移動" });
                    plc_action.Add(new DisplayPLCAction { Station = "Main", ReadAddress = "100", WriteAddress = "101", ActionDetail = "單站初始化" });
                    plc_action.Add(new DisplayPLCAction { Station = "Main", ReadAddress = "100", WriteAddress = "101", ActionDetail = "全機初始化" });
                    plc_action.Add(new DisplayPLCAction { Station = "Main", ReadAddress = "100", WriteAddress = "101", ActionDetail = "關閉通訊" });
                    ini_file.Write("Read Address", "112", "Nut Station");
                    ini_file.Write("Write Address", "113", "Nut Station");
                    ini_file.Write("Action Detail", "放置螺帽,單站初始化,關閉通訊", "Nut Station");
                    plc_action.Add(new DisplayPLCAction { Station = "Nut", ReadAddress = "112", WriteAddress = "113", ActionDetail = "放置螺帽" });
                    plc_action.Add(new DisplayPLCAction { Station = "Nut", ReadAddress = "112", WriteAddress = "113", ActionDetail = "單站初始化" });
                    plc_action.Add(new DisplayPLCAction { Station = "Nut", ReadAddress = "112", WriteAddress = "113", ActionDetail = "關閉通訊" });
                    ini_file.Write("Read Address", "116", "Bend Station");
                    ini_file.Write("Write Address", "117", "Bend Station");
                    ini_file.Write("Action Detail", "折彎,單站初始化,關閉通訊", "Bend Station");
                    plc_action.Add(new DisplayPLCAction { Station = "Bend", ReadAddress = "116", WriteAddress = "117", ActionDetail = "折彎" });
                    plc_action.Add(new DisplayPLCAction { Station = "Bend", ReadAddress = "116", WriteAddress = "117", ActionDetail = "單站初始化" });
                    plc_action.Add(new DisplayPLCAction { Station = "Bend", ReadAddress = "116", WriteAddress = "117", ActionDetail = "關閉通訊" });
                    ini_file.Write("Read Address", "120", "Plate Station");
                    ini_file.Write("Write Address", "121", "Plate Station");
                    ini_file.Write("Action Detail", "壓平,單站初始化,關閉通訊", "Plate Station");
                    plc_action.Add(new DisplayPLCAction { Station = "Plate", ReadAddress = "120", WriteAddress = "121", ActionDetail = "壓平" });
                    plc_action.Add(new DisplayPLCAction { Station = "Plate", ReadAddress = "120", WriteAddress = "121", ActionDetail = "單站初始化" });
                    plc_action.Add(new DisplayPLCAction { Station = "Plate", ReadAddress = "120", WriteAddress = "121", ActionDetail = "關閉通訊" });
                    ini_file.Write("Read Address", "124", "Height Station");
                    ini_file.Write("Write Address", "125", "Height Station");
                    ini_file.Write("Action Detail", "測高,單站初始化,關閉通訊", "Height Station");
                    plc_action.Add(new DisplayPLCAction { Station = "Height", ReadAddress = "124", WriteAddress = "125", ActionDetail = "測高" });
                    plc_action.Add(new DisplayPLCAction { Station = "Height", ReadAddress = "124", WriteAddress = "125", ActionDetail = "單站初始化" });
                    plc_action.Add(new DisplayPLCAction { Station = "Height", ReadAddress = "124", WriteAddress = "125", ActionDetail = "關閉通訊" });
                    ini_file.Write("Read Address", "128", "NG Station");
                    ini_file.Write("Write Address", "129", "NG Station");
                    ini_file.Write("Action Detail", "NG出料,單站初始化,關閉通訊", "NG Station");
                    plc_action.Add(new DisplayPLCAction { Station = "NG", ReadAddress = "128", WriteAddress = "129", ActionDetail = "NG出料" });
                    plc_action.Add(new DisplayPLCAction { Station = "NG", ReadAddress = "128", WriteAddress = "129", ActionDetail = "單站初始化" });
                    plc_action.Add(new DisplayPLCAction { Station = "NG", ReadAddress = "128", WriteAddress = "129", ActionDetail = "關閉通訊" });
                    ini_file.Write("Read Address", "104", "In Station");
                    ini_file.Write("Write Address", "105", "In Station");
                    ini_file.Write("Action Detail", "換盤,單站初始化,關閉通訊", "In Station");
                    plc_action.Add(new DisplayPLCAction { Station = "In", ReadAddress = "104", WriteAddress = "105", ActionDetail = "換盤" });
                    plc_action.Add(new DisplayPLCAction { Station = "In", ReadAddress = "104", WriteAddress = "105", ActionDetail = "單站初始化" });
                    plc_action.Add(new DisplayPLCAction { Station = "In", ReadAddress = "104", WriteAddress = "105", ActionDetail = "關閉通訊" });
                    ini_file.Write("Read Address", "108", "Lid Station");
                    ini_file.Write("Write Address", "109", "Lid Station");
                    ini_file.Write("Action Detail", "換盤,單站初始化,關閉通訊", "Lid Station");
                    plc_action.Add(new DisplayPLCAction { Station = "Lid", ReadAddress = "108", WriteAddress = "109", ActionDetail = "換盤" });
                    plc_action.Add(new DisplayPLCAction { Station = "Lid", ReadAddress = "108", WriteAddress = "109", ActionDetail = "單站初始化" });
                    plc_action.Add(new DisplayPLCAction { Station = "Lid", ReadAddress = "108", WriteAddress = "109", ActionDetail = "關閉通訊" });
                    ini_file.Write("Read Address", "132", "Out Station");
                    ini_file.Write("Write Address", "133", "Out Station");
                    ini_file.Write("Action Detail", "換盤,單站初始化,關閉通訊", "Out Station");
                    plc_action.Add(new DisplayPLCAction { Station = "Out", ReadAddress = "132", WriteAddress = "133", ActionDetail = "換盤" });
                    plc_action.Add(new DisplayPLCAction { Station = "Out", ReadAddress = "132", WriteAddress = "133", ActionDetail = "單站初始化" });
                    plc_action.Add(new DisplayPLCAction { Station = "Out", ReadAddress = "132", WriteAddress = "133", ActionDetail = "關閉通訊" });
                }
                else
                {
                    var ini_file = new IniFile("PLC_Settings.ini");

                    param = new KeyencePLCNetParam { plcNet = new PLCNet { IP = ini_file.Read("IP", "PLC Param"), Port = int.Parse(ini_file.Read("Port", "PLC Param")), timeout = int.Parse(ini_file.Read("Timeout", "PLC Param")) } };
                    var station = ini_file.Read("Station", "PLC Param").Split(',').ToList();
                    station.ForEach(x =>
                    {
                        var read_addrsss = ini_file.Read("Read Address", $"{x} Station");
                        var write_addrsss = ini_file.Read("Write Address", $"{x} Station");
                        var action_detail = ini_file.Read("Action Detail", $"{x} Station").Split(',').ToList();
                        action_detail.ForEach(y =>
                        {
                            plc_action.Add(new DisplayPLCAction { Station = x, ReadAddress = read_addrsss, WriteAddress = write_addrsss, ActionDetail = y });
                        });
                    });
                }
                #region 設定PLC
                param.plcNet.Init();
                main_plc.WriteAddress = plc_action.Find(x => x._station == "Main").WriteAddress;
                main_plc.ReadAddress = plc_action.Find(x => x._station == "Main").ReadAddress;
                main_plc.plcNet = param.plcNet;
                nut_plc.WriteAddress = plc_action.Find(x => x._station == "Nut").WriteAddress;
                nut_plc.ReadAddress = plc_action.Find(x => x._station == "Nut").ReadAddress;
                nut_plc.plcNet = param.plcNet;
                bend_plc.WriteAddress = plc_action.Find(x => x._station == "Bend").WriteAddress;
                bend_plc.ReadAddress = plc_action.Find(x => x._station == "Bend").ReadAddress;
                bend_plc.plcNet = param.plcNet;
                plate_plc.WriteAddress = plc_action.Find(x => x._station == "Plate").WriteAddress;
                plate_plc.ReadAddress = plc_action.Find(x => x._station == "Plate").ReadAddress;
                plate_plc.plcNet = param.plcNet;
                height_plc.WriteAddress = plc_action.Find(x => x._station == "Height").WriteAddress;
                height_plc.ReadAddress = plc_action.Find(x => x._station == "Height").ReadAddress;
                height_plc.plcNet = param.plcNet;
                ng_plc.WriteAddress = plc_action.Find(x => x._station == "NG").WriteAddress;
                ng_plc.ReadAddress = plc_action.Find(x => x._station == "NG").ReadAddress;
                ng_plc.plcNet = param.plcNet;
                in_station_plc.WriteAddress = plc_action.Find(x => x._station == "In").WriteAddress;
                in_station_plc.ReadAddress = plc_action.Find(x => x._station == "In").ReadAddress;
                in_station_plc.plcNet = param.plcNet;
                in_station_plc.Timeout = 20000;
                lid_station_plc.WriteAddress = plc_action.Find(x => x._station == "Lid").WriteAddress;
                lid_station_plc.ReadAddress = plc_action.Find(x => x._station == "Lid").ReadAddress;
                lid_station_plc.plcNet = param.plcNet;
                lid_station_plc.Timeout = 20000;
                out_station_plc.WriteAddress = plc_action.Find(x => x._station == "Out").WriteAddress;
                out_station_plc.ReadAddress = plc_action.Find(x => x._station == "Out").ReadAddress;
                out_station_plc.plcNet = param.plcNet;
                out_station_plc.Timeout = 20000;
                #endregion 設定PLC
                OnPresentResponseEvent("load_plc_param", new DisplayPLCActionArgs { plc_param = param, plc_action = plc_action });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, "PLC參數讀取失敗。");
            }
        }
        private void LoadReaderParam()
        {
            try
            {
                KeyenceReaderParam reader_param = new KeyenceReaderParam();
                KeyenceReaderParam out_param = new KeyenceReaderParam();
                if (!File.Exists("Reader_Settings.ini"))
                {
                    var ini_file = new IniFile("Reader_Settings.ini");
                    ini_file.Write("IP", "192.168.0.18", "Reader");
                    ini_file.Write("Port", "9004", "Reader");
                    ini_file.Write("Timeout", "10000", "Reader");
                    ini_file.Write("Actions", "READ,CLOSE,QUIT", "Reader");
                    reader_param = new KeyenceReaderParam { IP = "192.168.0.18", Port = 9004, Timeout = 10000, Action_Name = new string[] { "READ", "CLOSE", "QUIT" }.ToList() };
                    ini_file.Write("IP", "192.168.0.19", "Out Reader");
                    ini_file.Write("Port", "9004", "Out Reader");
                    ini_file.Write("Timeout", "10000", "Out Reader");
                    ini_file.Write("Actions", "READ,CLOSE,QUIT", "Out Reader");
                    out_param = new KeyenceReaderParam { IP = "192.168.0.19", Port = 9004, Timeout = 10000, Action_Name = new string[] { "READ", "CLOSE", "QUIT" }.ToList() };
                }
                else
                {
                    var ini_file = new IniFile("Reader_Settings.ini");
                    reader_param.IP = ini_file.Read("IP", "Reader");
                    reader_param.Port = int.Parse(ini_file.Read("Port", "Reader"));
                    reader_param.Timeout = int.Parse(ini_file.Read("Timeout", "Reader"));
                    reader_param.Action_Name = ini_file.Read("Actions", "Reader").Split(',').ToList();

                    out_param.IP = ini_file.Read("IP", "Out Reader");
                    out_param.Port = int.Parse(ini_file.Read("Port", "Out Reader"));
                    out_param.Timeout = int.Parse(ini_file.Read("Timeout", "Out Reader"));
                    out_param.Action_Name = ini_file.Read("Actions", "Out Reader").Split(',').ToList();
                }
                reader.SetReaderParam(reader_param);
                out_reader.SetReaderParam(out_param);
                OnPresentResponseEvent("load_reader_param", new LoadReaderParamArgs { reader = reader_param, out_reader = out_param });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, "讀取掃碼資料失敗。");
            }

        }
        private void LoadLaserHeighParam()
        {
            try
            {
                if (!File.Exists("Laser_Heigh_Settings.ini"))
                {
                    var ini_file = new IniFile("Laser_Heigh_Settings.ini");
                    ini_file.Write("IP", "192.168.0.82", "Height");
                    lk_processor.IP = "192.168.0.82";
                }
                else
                {
                    var ini_file = new IniFile("Laser_Heigh_Settings.ini");
                    lk_processor.IP = ini_file.Read("IP", "Height");
                }
                var success = lk_processor.OpenDevice();
                OnPresentResponseEvent("load_laser_heigh_param", new LoadLaserHeightParamArgs { IP = lk_processor.IP, connect_state = lk_processor.ConnectState, message = lk_processor.Message, success = success });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, "讀取雷射測高資料失敗。");
            }
        }
        private void LoadSystemParam()
        {
            try
            {
                if (!File.Exists("System_Settings.ini"))
                {
                    var ini_file = new IniFile("System_Settings.ini");
                    ini_file.Write("CaseCount", "12", "System");
                    ini_file.Write("Recipe", "0", "System");
                    ini_file.Write("TrayCount", "12", "System");
                    ini_file.Write("LidTrayCount", "35", "System");
                    ini_file.Write("NgOutCountLimit", "3", "System");
                    ini_file.Write("DataRecordCount", "180", "System");
                    system_param.CaseCount = 12;
                    system_param.Recipe = 0;
                    system_param.TrayCount = 12;
                    system_param.LidTrayCount = 35;
                    system_param.NgOutCountLimit = 3;
                    system_param.DataRecordCount = 180;
                    ini_file.Write("Enable", "False", "SFIS");
                    ini_file.Write("StationID", "CA01", "SFIS");
                    ini_file.Write("LineID", "PD01", "SFIS");
                    ini_file.Write("TicketID", "W177", "SFIS");
                    ini_file.Write("WorkerID", "M026125", "SFIS");
                    ini_file.Write("LidLotID", "1234", "SFIS");
                    ini_file.Write("NutNo", "2345", "SFIS");
                    ini_file.Write("BarcodeA", "", "SFIS");
                    ini_file.Write("BarcodeB", "", "SFIS");
                    ini_file.Write("InspLevel", "A,B", "SFIS");
                    ini_file.Write("IP", "10.0.4.47", "SFIS");
                    ini_file.Write("Port", "21347", "SFIS");
                    system_param.Sfis.Enable = false;
                    system_param.Sfis.StationID = "CA01";
                    system_param.Sfis.LineID = "PD01";
                    system_param.Sfis.TicketID = "W177";
                    system_param.Sfis.WorkerID = "M026125";
                    system_param.Sfis.LidLotID = "1234";
                    system_param.Sfis.NutNo = "2345";
                    system_param.Sfis.BarcodeA = "";
                    system_param.Sfis.BarcodeB = "";
                    system_param.Sfis.InspLevel = "A";
                    _sfis.IP = "10.0.4.47";
                    _sfis.Port = 21347;
                    ini_file.Write("MeasurePosition", "7700,600;7950,4900;250,4900;700,4000;1300,4000;3300,4000;3900,4000;6800,4000;7400,4000;7100,2200;3600,2200;1000,2200", "Measure");
                    ini_file.Write("FlatnessUpperLimit", "1", "Measure");
                    ini_file.Write("HeightLimit", "9,10;12,14;9,10", "Measure");
                    system_param.MeasurePosition = new List<EstimatePosition>
                    {
                        new EstimatePosition { X = 7700,Y=600 },
                        new EstimatePosition { X = 7950,Y=4900 },
                        new EstimatePosition { X = 250,Y=4900 },
                        new EstimatePosition { X = 700,Y=4000 },
                        new EstimatePosition { X = 1300,Y=4000 },
                        new EstimatePosition { X = 3300,Y=4000 },
                        new EstimatePosition { X = 3900,Y=4000 },
                        new EstimatePosition { X = 6800,Y=4000 },
                        new EstimatePosition { X = 7400,Y=4000 },
                        new EstimatePosition { X = 7100,Y=2200 },
                        new EstimatePosition { X = 3600,Y=2200 },
                        new EstimatePosition { X = 1000,Y=2200 },
                    };
                    system_param.FlatnessUpperLimit = 1;
                    system_param.HeightLimit = new List<Range>
                    {
                        new Range{Lower = 9, Upper=10},
                        new Range{Lower = 12, Upper=14},
                        new Range{Lower = 9, Upper=10},
                    };
                    ini_file.Write("ShiftInArmsPickX", "0", "Shift");
                    ini_file.Write("ShiftInArmsPickY", "0", "Shift");
                    ini_file.Write("ShiftInArmsPickZ", "0", "Shift");
                    ini_file.Write("ShiftInArmsPickU", "0", "Shift");
                    ini_file.Write("ShiftInArmsPutX", "0", "Shift");
                    ini_file.Write("ShiftInArmsPutY", "0", "Shift");
                    ini_file.Write("ShiftInArmsPutZ", "0", "Shift");
                    ini_file.Write("ShiftInArmsPutU", "0", "Shift");

                    ini_file.Write("ShiftOutArmsPickX", "0", "Shift");
                    ini_file.Write("ShiftOutArmsPickY", "0", "Shift");
                    ini_file.Write("ShiftOutArmsPickZ", "0", "Shift");
                    ini_file.Write("ShiftOutArmsPickU", "0", "Shift");
                    ini_file.Write("ShiftOutArmsPutX", "0", "Shift");
                    ini_file.Write("ShiftOutArmsPutY", "0", "Shift");
                    ini_file.Write("ShiftOutArmsPutZ", "0", "Shift");
                    ini_file.Write("ShiftOutArmsPutU", "0", "Shift");
                    system_param.ShiftInArms = new ShiftArms { Pick = new ShiftArmsAxis { X = 0, Y = 0, Z = 0, U = 0 }, Put = new ShiftArmsAxis { X = 0, Y = 0, Z = 0, U = 0 } };
                    system_param.ShiftOutArms = new ShiftArms { Pick = new ShiftArmsAxis { X = 0, Y = 0, Z = 0, U = 0 }, Put = new ShiftArmsAxis { X = 0, Y = 0, Z = 0, U = 0 } };
                    ini_file.Write("ShiftLimitUpper", "100", "Shift");
                    ini_file.Write("ShiftLimitLower", "-100", "Shift");
                    system_param.ShiftLimit = new Range { Lower = -100, Upper = 100 };
                    ini_file.Write("PlateAccuracyXUpper", "100", "Plate");
                    ini_file.Write("PlateAccuracyXLower", "-100", "Plate");
                    ini_file.Write("PlateAccuracyYUpper", "100", "Plate");
                    ini_file.Write("PlateAccuracyYLower", "-100", "Plate");
                    ini_file.Write("PlateAccuracyUUpper", "100", "Plate");
                    ini_file.Write("PlateAccuracyULower", "-100", "Plate");
                    system_param.PlateAccuracy = new PlateAccuracy { X = new Range { Lower = -100, Upper = 100 }, Y = new Range { Lower = -100, Upper = 100 }, U = new Range { Lower = -100, Upper = 100 } };
                    ini_file.Write("CaseAssemble", "True", "Flow");
                    ini_file.Write("CastScan", "True", "Flow");
                    ini_file.Write("CasePutNut", "True", "Flow");
                    
                    ini_file.Write("CastBending", "True", "Flow");
                    ini_file.Write("CasePlate", "True", "Flow");
                    ini_file.Write("CaseEstHeight", "True", "Flow");
                    ini_file.Write("CastNgOut", "True", "Flow");
                    ini_file.Write("CastMarking", "True", "Flow");
                    system_param.Flow = new SystemFlow { CaseAssemble = true, CaseScan = true, CasePutNut = true, CaseBending = true, CasePlate = true, CaseEstHeight = true, CaseNgOut = true, CaseMarking = true };
                    ini_file.Write("EstHeighInLower", "-100", "In");
                    ini_file.Write("EstHeighInUpper", "100", "In");
                    system_param.EstHeighIn = new Range { Lower = -100, Upper = 100 };
                    ini_file.Write("marking_fst_code", "CM_TextObj T1", "Marking");
                    ini_file.Write("marking_fst_txt", "EAB450M12XM35", "Marking");
                    ini_file.Write("marking_snd_code", "CM_TextObj T2", "Marking");
                    ini_file.Write("marking_snd_txt", "HX2220-A1", "Marking");
                    ini_file.Write("marking_snd_index", "0", "Marking");
                    ini_file.Write("marking_2d_code", "CM_2DObj Var2D", "Marking");
                    ini_file.Write("marking_2d_txt", "0000000001", "Marking");
                    ini_file.Write("start_marking_code", "CM_StartMarking", "Marking");
                    ini_file.Write("shift_code", "CM_OffsetExt", "Marking");
                    ini_file.Write("shift_x", "0", "Marking");
                    ini_file.Write("shift_y", "0", "Marking");
                    ini_file.Write("shift_a", "0", "Marking");
                    ini_file.Write("pass_level", "A,B", "Marking");
                    ini_file.Write("MarkingIP", "127.0.0.1", "Marking");
                    ini_file.Write("MarkingPort", "4000", "Marking");
                    system_param.MarkParam = new MarkingParam
                    {
                        marking_fst_code = "CM_TextObj T1",
                        marking_fst_txt = "EAB450M12XM35",
                        marking_snd_code = "CM_TextObj T2",
                        marking_snd_txt = "HX2220-A1",
                        marking_snd_index = 0,
                        marking_2d_code = "CM_2DObj Var2D",
                        marking_2d_txt = "0000000001",
                        start_marking_code = "CM_StartMarking",
                        shift_code = "CM_OffsetExt",
                        shift_x = 0,
                        shift_y = 0,
                        shift_a = 0,
                        pass_level = new List<string> { "A", "B" },
                    };
                    lfj_processor.IP = "127.0.0.1";
                    lfj_processor.Port = 4000;

                }
                else
                {
                    var ini_file = new IniFile("System_Settings.ini");
                    system_param.CaseCount = int.Parse(ini_file.Read("CaseCount", "System"));
                    system_param.Recipe = int.Parse(ini_file.Read("Recipe", "System"));
                    system_param.TrayCount = int.Parse(ini_file.Read("TrayCount", "System"));
                    system_param.LidTrayCount = int.Parse(ini_file.Read("LidTrayCount", "System"));
                    system_param.NgOutCountLimit = int.Parse(ini_file.Read("NgOutCountLimit", "System"));
                    system_param.DataRecordCount = int.Parse(ini_file.Read("DataRecordCount", "System"));

                    system_param.Sfis.Enable = bool.Parse(ini_file.Read("Enable", "SFIS"));
                    system_param.Sfis.StationID = ini_file.Read("StationID", "SFIS");
                    system_param.Sfis.LineID = ini_file.Read("LineID", "SFIS");
                    system_param.Sfis.TicketID = ini_file.Read("TicketID", "SFIS");
                    system_param.Sfis.WorkerID = ini_file.Read("WorkerID", "SFIS");
                    system_param.Sfis.LidLotID = ini_file.Read("LidLotID", "SFIS");
                    system_param.Sfis.NutNo = ini_file.Read("NutNo", "SFIS");
                    system_param.Sfis.BarcodeA = ini_file.Read("BarcodeA", "SFIS");
                    system_param.Sfis.BarcodeB = ini_file.Read("BarcodeB", "SFIS");
                    system_param.Sfis.InspLevel = ini_file.Read("InspLevel", "SFIS");
                    _sfis.IP = ini_file.Read("IP", "SFIS");
                    _sfis.Port = int.Parse(ini_file.Read("Port", "SFIS"));

                    var measure_position = ini_file.Read("MeasurePosition", "Measure").Split(';').ToList();
                    var measure = new List<EstimatePosition>();
                    measure_position.ForEach(x => measure.Add(new EstimatePosition { X = int.Parse(x.Split(',')[0]), Y = int.Parse(x.Split(',')[1]) }));
                    system_param.MeasurePosition = measure;
                    system_param.FlatnessUpperLimit = int.Parse(ini_file.Read("FlatnessUpperLimit", "Measure"));
                    var hei_limit = ini_file.Read("HeightLimit", "Measure").Split(';').ToList();
                    var height = new List<Range>();
                    hei_limit.ForEach(x => height.Add(new Range() { Lower = double.Parse(x.Split(',')[0]), Upper = double.Parse(x.Split(',')[1]) }));
                    system_param.HeightLimit = height;

                    system_param.ShiftInArms = new ShiftArms
                    {
                        Pick = new ShiftArmsAxis
                        {
                            X = double.Parse(ini_file.Read("ShiftInArmsPickX", "Shift")),
                            Y = double.Parse(ini_file.Read("ShiftInArmsPickY", "Shift")),
                            Z = double.Parse(ini_file.Read("ShiftInArmsPickZ", "Shift")),
                            U = double.Parse(ini_file.Read("ShiftInArmsPickU", "Shift")),
                        },
                        Put = new ShiftArmsAxis
                        {
                            X = double.Parse(ini_file.Read("ShiftInArmsPutX", "Shift")),
                            Y = double.Parse(ini_file.Read("ShiftInArmsPutY", "Shift")),
                            Z = double.Parse(ini_file.Read("ShiftInArmsPutZ", "Shift")),
                            U = double.Parse(ini_file.Read("ShiftInArmsPutU", "Shift")),
                        },
                    };
                    system_param.ShiftInArms = new ShiftArms
                    {
                        Pick = new ShiftArmsAxis
                        {
                            X = double.Parse(ini_file.Read("ShiftOutArmsPickX", "Shift")),
                            Y = double.Parse(ini_file.Read("ShiftOutArmsPickY", "Shift")),
                            Z = double.Parse(ini_file.Read("ShiftOutArmsPickZ", "Shift")),
                            U = double.Parse(ini_file.Read("ShiftOutArmsPickU", "Shift")),
                        },
                        Put = new ShiftArmsAxis
                        {
                            X = double.Parse(ini_file.Read("ShiftOutArmsPutX", "Shift")),
                            Y = double.Parse(ini_file.Read("ShiftOutArmsPutY", "Shift")),
                            Z = double.Parse(ini_file.Read("ShiftOutArmsPutZ", "Shift")),
                            U = double.Parse(ini_file.Read("ShiftOutArmsPutU", "Shift")),
                        },
                    };
                    system_param.ShiftLimit = new Range { Lower = double.Parse(ini_file.Read("ShiftLimitLower", "Shift")), Upper = double.Parse(ini_file.Read("ShiftLimitUpper", "Shift")) };

                    system_param.PlateAccuracy = new PlateAccuracy
                    {
                        X = new Range { Lower = double.Parse(ini_file.Read("PlateAccuracyXUpper", "Plate")), Upper = double.Parse(ini_file.Read("PlateAccuracyXLower", "Plate")) },
                        Y = new Range { Lower = double.Parse(ini_file.Read("PlateAccuracyYUpper", "Plate")), Upper = double.Parse(ini_file.Read("PlateAccuracyYLower", "Plate")) },
                        U = new Range { Lower = double.Parse(ini_file.Read("PlateAccuracyUUpper", "Plate")), Upper = double.Parse(ini_file.Read("PlateAccuracyULower", "Plate")) }
                    };
                    system_param.Flow = new SystemFlow
                    {
                        CaseAssemble = bool.Parse(ini_file.Read("CaseAssemble", "Flow")),
                        CaseScan = bool.Parse(ini_file.Read("CastScan", "Flow")),
                        CasePutNut = bool.Parse(ini_file.Read("CasePutNut", "Flow")),
                        CaseBending = bool.Parse(ini_file.Read("CastBending", "Flow")),
                        CasePlate = bool.Parse(ini_file.Read("CasePlate", "Flow")),
                        CaseEstHeight = bool.Parse(ini_file.Read("CaseEstHeight", "Flow")),
                        CaseNgOut = bool.Parse(ini_file.Read("CastNgOut", "Flow")),
                        CaseMarking = bool.Parse(ini_file.Read("CastMarking", "Flow")),
                    };
                    system_param.EstHeighIn = new Range
                    {
                        Lower = double.Parse(ini_file.Read("EstHeighInLower", "In")),
                        Upper = double.Parse(ini_file.Read("EstHeighInUpper", "In"))
                    };
                    system_param.MarkParam.marking_fst_code = ini_file.Read("marking_fst_code", "Marking");
                    system_param.MarkParam.marking_fst_txt = ini_file.Read("marking_fst_txt", "Marking");
                    system_param.MarkParam.marking_snd_code = ini_file.Read("marking_snd_code", "Marking");
                    system_param.MarkParam.marking_snd_txt = ini_file.Read("marking_snd_txt", "Marking");
                    system_param.MarkParam.marking_snd_index = int.Parse(ini_file.Read("marking_snd_index", "Marking"));
                    system_param.MarkParam.marking_2d_code = ini_file.Read("marking_2d_code", "Marking");
                    system_param.MarkParam.marking_2d_txt = ini_file.Read("marking_2d_txt", "Marking");
                    system_param.MarkParam.start_marking_code = ini_file.Read("start_marking_code", "Marking");
                    system_param.MarkParam.shift_code = ini_file.Read("shift_code", "Marking");
                    system_param.MarkParam.shift_x = int.Parse(ini_file.Read("shift_x", "Marking"));
                    system_param.MarkParam.shift_y = int.Parse(ini_file.Read("shift_y", "Marking"));
                    system_param.MarkParam.shift_a = int.Parse(ini_file.Read("shift_a", "Marking"));
                    system_param.MarkParam.pass_level = ini_file.Read("pass_level", "Marking").Split(',').ToList();
                    lfj_processor.IP = ini_file.Read("MarkingIP", "Marking");
                    lfj_processor.Port = int.Parse(ini_file.Read("MarkingPort", "Marking"));
                }
                lfj_processor.StartConnected();
                OnPresentResponseEvent("load_system_param", new LoadSystemParamResponseArgs { IP = lfj_processor.IP, Port = lfj_processor.Port, MarkingStatus = lfj_processor.ConnectedState, Param = system_param });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, "讀取系統參數失敗。");
            }
        }
        private void SaveSystemParam()
        {
            var ini_file = new IniFile("System_Settings.ini");
            ini_file.Write("CaseCount", system_param.CaseCount.ToString(), "System");
            ini_file.Write("Recipe", system_param.Recipe.ToString(), "System");
            ini_file.Write("TrayCount", system_param.TrayCount.ToString(), "System");
            ini_file.Write("LidTrayCount", system_param.LidTrayCount.ToString(), "System");
            ini_file.Write("NgOutCountLimit", system_param.NgOutCountLimit.ToString(), "System");
            ini_file.Write("DataRecordCount", system_param.DataRecordCount.ToString(), "System");

            ini_file.Write("Enable", system_param.Sfis.Enable.ToString(), "SFIS");
            ini_file.Write("StationID", system_param.Sfis.StationID.ToString(), "SFIS");
            ini_file.Write("LineID", system_param.Sfis.LineID.ToString(), "SFIS");
            ini_file.Write("TicketID", system_param.Sfis.TicketID.ToString(), "SFIS");
            ini_file.Write("WorkerID", system_param.Sfis.WorkerID.ToString(), "SFIS");
            ini_file.Write("LidLotID", system_param.Sfis.LidLotID.ToString(), "SFIS");
            ini_file.Write("NutNo", system_param.Sfis.NutNo.ToString(), "SFIS");
            ini_file.Write("BarcodeA", system_param.Sfis.BarcodeA.ToString(), "SFIS");
            ini_file.Write("BarcodeB", system_param.Sfis.BarcodeB.ToString(), "SFIS");
            ini_file.Write("InspLevel", system_param.Sfis.InspLevel.ToString(), "SFIS");
            ini_file.Write("IP", _sfis.IP, "SFIS");
            ini_file.Write("Port", _sfis.Port.ToString(), "SFIS");

            ini_file.Write("MeasurePosition", system_param.MeasurePosition.Aggregate("", (total, next) => total += total.Length == 0 ? $"{next.X},{next.Y}" : $";{next.X},{next.Y}"), "Measure");
            ini_file.Write("FlatnessUpperLimit", system_param.FlatnessUpperLimit.ToString(), "Measure");
            ini_file.Write("HeightLimit", system_param.HeightLimit.Aggregate("", (total, next) => total += total.Length == 0 ? $"{next.Lower},{next.Upper}" : $";{next.Lower},{next.Upper}"), "Measure");

            ini_file.Write("ShiftInArmsPickX", system_param.ShiftInArms.Pick.X.ToString(), "Shift");
            ini_file.Write("ShiftInArmsPickY", system_param.ShiftInArms.Pick.Y.ToString(), "Shift");
            ini_file.Write("ShiftInArmsPickZ", system_param.ShiftInArms.Pick.Z.ToString(), "Shift");
            ini_file.Write("ShiftInArmsPickU", system_param.ShiftInArms.Pick.U.ToString(), "Shift");
            ini_file.Write("ShiftInArmsPutX", system_param.ShiftInArms.Put.X.ToString(), "Shift");
            ini_file.Write("ShiftInArmsPutY", system_param.ShiftInArms.Put.Y.ToString(), "Shift");
            ini_file.Write("ShiftInArmsPutZ", system_param.ShiftInArms.Put.Z.ToString(), "Shift");
            ini_file.Write("ShiftInArmsPutU", system_param.ShiftInArms.Put.U.ToString(), "Shift");
            ini_file.Write("ShiftOutArmsPickX", system_param.ShiftOutArms.Pick.X.ToString(), "Shift");
            ini_file.Write("ShiftOutArmsPickY", system_param.ShiftOutArms.Pick.Y.ToString(), "Shift");
            ini_file.Write("ShiftOutArmsPickZ", system_param.ShiftOutArms.Pick.Z.ToString(), "Shift");
            ini_file.Write("ShiftOutArmsPickU", system_param.ShiftOutArms.Pick.U.ToString(), "Shift");
            ini_file.Write("ShiftOutArmsPutX", system_param.ShiftOutArms.Put.X.ToString(), "Shift");
            ini_file.Write("ShiftOutArmsPutY", system_param.ShiftOutArms.Put.Y.ToString(), "Shift");
            ini_file.Write("ShiftOutArmsPutZ", system_param.ShiftOutArms.Put.Z.ToString(), "Shift");
            ini_file.Write("ShiftOutArmsPutU", system_param.ShiftOutArms.Put.U.ToString(), "Shift");
            ini_file.Write("ShiftLimitUpper", system_param.ShiftLimit.Upper.ToString(), "Shift");
            ini_file.Write("ShiftLimitLower", system_param.ShiftLimit.Lower.ToString(), "Shift");
            ini_file.Write("PlateAccuracyXUpper", system_param.PlateAccuracy.X.Upper.ToString(), "Plate");
            ini_file.Write("PlateAccuracyXLower", system_param.PlateAccuracy.X.Lower.ToString(), "Plate");
            ini_file.Write("PlateAccuracyYUpper", system_param.PlateAccuracy.Y.Upper.ToString(), "Plate");
            ini_file.Write("PlateAccuracyYLower", system_param.PlateAccuracy.Y.Lower.ToString(), "Plate");
            ini_file.Write("PlateAccuracyUUpper", system_param.PlateAccuracy.U.Upper.ToString(), "Plate");
            ini_file.Write("PlateAccuracyULower", system_param.PlateAccuracy.U.Lower.ToString(), "Plate");

            ini_file.Write("CaseAssemble", system_param.Flow.CaseAssemble.ToString(), "Flow");
            ini_file.Write("CastScan", system_param.Flow.CaseScan.ToString(), "Flow");
            ini_file.Write("CasePutNut", system_param.Flow.CasePutNut.ToString(), "Flow");
            ini_file.Write("CastBending", system_param.Flow.CaseBending.ToString(), "Flow");
            ini_file.Write("CasePlate", system_param.Flow.CasePlate.ToString(), "Flow");
            ini_file.Write("CaseEstHeight", system_param.Flow.CaseEstHeight.ToString(), "Flow");
            ini_file.Write("CastNgOut", system_param.Flow.CaseNgOut.ToString(), "Flow");
            ini_file.Write("CastMarking", system_param.Flow.CaseMarking.ToString(), "Flow");

            ini_file.Write("EstHeighInLower", system_param.EstHeighIn.Upper.ToString(), "In");
            ini_file.Write("EstHeighInUpper", system_param.EstHeighIn.Lower.ToString(), "In");

            ini_file.Write("marking_fst_code", system_param.MarkParam.marking_fst_code.ToString(), "Marking");
            ini_file.Write("marking_fst_txt", system_param.MarkParam.marking_fst_txt.ToString(), "Marking");
            ini_file.Write("marking_snd_code", system_param.MarkParam.marking_snd_code.ToString(), "Marking");
            ini_file.Write("marking_snd_txt", system_param.MarkParam.marking_snd_txt.ToString(), "Marking");
            ini_file.Write("marking_snd_index", system_param.MarkParam.marking_snd_index.ToString(), "Marking");
            ini_file.Write("marking_2d_code", system_param.MarkParam.marking_2d_code.ToString(), "Marking");
            ini_file.Write("marking_2d_txt", system_param.MarkParam.marking_2d_txt.ToString(), "Marking");
            ini_file.Write("start_marking_code", system_param.MarkParam.start_marking_code.ToString(), "Marking");
            ini_file.Write("shift_code", system_param.MarkParam.shift_code.ToString(), "Marking");
            ini_file.Write("shift_x", system_param.MarkParam.shift_x.ToString(), "Marking");
            ini_file.Write("shift_y", system_param.MarkParam.shift_y.ToString(), "Marking");
            ini_file.Write("shift_a", system_param.MarkParam.shift_a.ToString(), "Marking");
            ini_file.Write("pass_level", system_param.MarkParam.pass_level.Aggregate("", (total, next) => total += total.Length == 0 ? $"{next}" : $",{next}"), "Marking");
            ini_file.Write("MarkingIP", lfj_processor.IP, "Marking");
            ini_file.Write("MarkingPort", lfj_processor.Port.ToString(), "Marking");
        }
        private void LoadDefectInfo()
        {
            try
            {
                if (!File.Exists("DefectList.txt"))
                {
                    using (FileStream fs = new FileStream("DefectList.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.WriteLine("高度異常,2D01");
                            sw.WriteLine("平整度異常,2D02");
                            sw.WriteLine("BasePlate 2D Code,2D03");
                            sw.WriteLine("BasePlate/ Lid 2D code異常,2D04");
                            sw.WriteLine("鐳碼等級異常,2D05");
                            system_param.DefectMapping.Add("高度異常", "2D01");
                            system_param.DefectMapping.Add("平整度異常", "2D02");
                            system_param.DefectMapping.Add("BasePlate 2D Code", "2D03");
                            system_param.DefectMapping.Add("BasePlate/ Lid 2D code異常", "2D04");
                            system_param.DefectMapping.Add("鐳碼等級異常", "2D05");
                        }
                    }
                }
                else
                {
                    using (FileStream fs = new FileStream("DefectList.txt", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            while (!sr.EndOfStream)
                            {
                                var line = sr.ReadLine().Split(',');

                                if (!system_param.DefectMapping.ContainsKey(line[0]))
                                    system_param.DefectMapping.Add(line[0], line[1]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, "讀取Defect列表失敗。");
            }
        }
        #endregion 系統參數功能
        #endregion 實作功能
        #region 靜態動作
        internal static Action<bool> SetRunFlow;
        internal static Action<bool> SetRunSingleStep;
        internal static Action<bool> SetRunSingleFlow;
        #endregion 靜態動作
        #region 靜態功能
        internal static Func<List<LogData>> LogDatas;
        internal static Func<SystemParam> SystemParam;
        
        internal static Func<bool> GetInitRun;
        internal static Func<bool> GetManualPause;
        internal static Func<bool> GetRunSingleStep;
        internal static Func<bool> GetRunSingleFlow;
        internal static Func<EsponArmsProcessor_In> CaseInArms;
        internal static Func<EsponArmsProcessor> CaseLidArms;
        internal static Func<EsponArmsProcessor> CaseOutArms;
        internal static Func<KeyencePLCProcessor> MainPLC;
        internal static Func<KeyencePLCProcessor> NutPLC;
        internal static Func<KeyencePLCProcessor> BendPLC;
        internal static Func<KeyencePLCProcessor> PlatePLC;
        internal static Func<KeyencePLCProcessor_Height> HeightPLC;
        internal static Func<KeyencePLCProcessor> NgPLC;
        internal static Func<KeyenceLoadPLCProcessor> InPLC;
        internal static Func<KeyenceLoadPLCProcessor> LidPLC;
        internal static Func<KeyenceLoadPLCProcessor> OutPLC;
        internal static Func<KeyenceReaderProcessor> Reader;
        internal static Func<KeyenceReaderProcessor> OutReader;
        internal static Func<LKIFProcessor> LKProcessor;
        internal static Func<LFJProcessor> LFJProcessor;
        internal static Func<SfisProcessor> SFIS;

        #endregion 靜態功能
        public MainPresenter(MainView _view)
        {
            try
            {
                view = _view;
                view.PresenterSendEvent += PresenterSendEvent;
                #region 靜態動作
                SetRunFlow = value => run_flow = value;
                SetRunSingleStep = data => run_single_step = data;
                SetRunSingleFlow = data => run_single_flow = data;
                #endregion 靜態動作

                #region 靜態功能
                LogDatas = () => logger;
                SystemParam = () => system_param;
                GetInitRun = () => init_run;
                GetManualPause = () => manual_pause;
                GetRunSingleStep = () => run_single_step;
                GetRunSingleFlow = () => run_single_flow;
                CaseInArms = () => in_arms;
                CaseLidArms = () => lid_arms;
                CaseOutArms = () => out_arms;
                MainPLC = () => main_plc;
                NutPLC = () => nut_plc;
                BendPLC = () => bend_plc;
                PlatePLC = () => plate_plc;
                HeightPLC = () => height_plc;
                NgPLC = () => ng_plc;
                InPLC = () => in_station_plc;
                LidPLC = () => lid_station_plc;
                OutPLC = () => out_station_plc;
                Reader = () => reader;
                OutReader = () => out_reader;
                LKProcessor = () => lk_processor;
                LFJProcessor = () => lfj_processor;
                SFIS = () => _sfis;
                #endregion 靜態功能
                #region 流程錯誤訊息串接
                CaseInTask.GetEntity().ShowErrorMessageEvent += MainPresenter_ShowFlowErrorEvent;
                CaseLidTask.GetEntity().ShowErrorMessageEvent += MainPresenter_ShowFlowErrorEvent;
                CaseScanCodeTask.GetEntity().ShowErrorMessageEvent += MainPresenter_ShowFlowErrorEvent;
                CasePutNutTask.GetEntity().ShowErrorMessageEvent += MainPresenter_ShowFlowErrorEvent;
                CaseBendTask.GetEntity().ShowErrorMessageEvent += MainPresenter_ShowFlowErrorEvent;
                CasePlateTask.GetEntity().ShowErrorMessageEvent += MainPresenter_ShowFlowErrorEvent;
                CaseEstHeightTask.GetEntity().ShowErrorMessageEvent += MainPresenter_ShowFlowErrorEvent;
                CaseNgOutTask.GetEntity().ShowErrorMessageEvent += MainPresenter_ShowFlowErrorEvent;
                CaseMarkingTask.GetEntity().ShowErrorMessageEvent += MainPresenter_ShowFlowErrorEvent;
                CaseOutTask.GetEntity().ShowErrorMessageEvent += MainPresenter_ShowFlowErrorEvent;
                CaseAllTask.GetEntity().UpdateCaseDataEvent += MainPresenter_UpdateCaseDataEvent;
                CaseAllTask.GetEntity().ShowErrorMessageEvent += MainPresenter_ShowMessageEvent;
                CaseAllTask.GetEntity().FinishCaseEvent += MainPresenter_FinishCaseEvent;
                CaseDoorCheckTask.GetEntity().ShowErrorMessageEvent += MainPresenter_ShowFlowErrorEvent;
                CaseInStationTask.GetEntity().ShowErrorMessageEvent += MainPresenter_ShowFlowErrorEvent;
                CaseLidStationTask.GetEntity().ShowErrorMessageEvent += MainPresenter_ShowFlowErrorEvent;
                CaseOutStationTask.GetEntity().ShowErrorMessageEvent += MainPresenter_ShowFlowErrorEvent;
                #endregion 流程錯誤訊息串接
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, "初始化Presenter失敗。");
            }

        }
        
        private void SendArmsAction(SendArmsActionArgs e)
        {
            try
            {

                string arms = e.arms.ToUpper();
                string action = e.action;
                EsponArmsProcessor processor = arms == "IN" ? in_arms : arms == "OUT" ? out_arms : lid_arms;
                if (is_run)
                {
                    OnPresentResponseEvent("send_arms_action", new SendArmsActionResponseArgs { arms = arms, message = "系統執行中，無法動作。", success = false, show_message = true });
                    return;
                }
                bool success = false;
                is_run = true;
                success = processor.SendActionFunction(action);
                is_run = false;
                OnPresentResponseEvent("send_arms_action", new SendArmsActionResponseArgs { arms = arms, message = processor.Message, success = success, show_message = !success });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"手臂動作失敗，手臂：{e.arms}、動作：{e.action}。");
            }
        }
        private void SendPLCAction(SendPLCActionArgs e)
        {
            try
            {


                string station = e.Station;
                string action_detail = e.ActionDetail;
                string message = "";
                bool success = false;
                switch (station)
                {
                    case "主流道":
                        switch (action_detail)
                        {
                            case "臺車移動":
                                success = main_plc.RunToNextStep();
                                break;
                            case "單站初始化":
                                success = main_plc.RunStationInitialized();
                                break;
                            case "全機初始化":
                                success = main_plc.RunAllInitialized();
                                break;
                            case "關閉通訊":
                                success = main_plc.SendIdle();
                                break;
                        }
                        message = main_plc.Message;
                        break;
                   
                    case "螺帽站":
                        switch (action_detail)
                        {
                            case "放置螺帽":
                                success = nut_plc.RunPutNut();
                                break;
                            case "單站初始化":
                                success = nut_plc.RunStationInitialized();
                                break;
                            case "關閉通訊":
                                success = nut_plc.SendIdle();
                                break;
                        }
                        message = nut_plc.Message;
                        break;
                    case "折彎站":
                        switch (action_detail)
                        {
                            case "折彎":
                                success = bend_plc.RunBending();
                                break;
                            case "單站初始化":
                                success = bend_plc.RunStationInitialized();
                                break;
                            case "關閉通訊":
                                success = bend_plc.SendIdle();
                                break;
                        }
                        message = bend_plc.Message;
                        break;
                    case "壓平站":
                        switch (action_detail)
                        {
                            case "壓平":
                                success = plate_plc.RunPlate();
                                break;
                            case "單站初始化":
                                success = plate_plc.RunStationInitialized();
                                break;
                            case "關閉通訊":
                                success = plate_plc.SendIdle();
                                break;
                        }
                        message = plate_plc.Message;
                        break;
                    case "測高站":
                        switch (action_detail)
                        {
                            case "測高":
                                height_plc.PosX = e.PosX;
                                height_plc.PosY = e.PosY;
                                success = height_plc.RunMovePos();
                                break;
                            case "單站初始化":
                                success = height_plc.RunStationInitialized();
                                break;
                            case "關閉通訊":
                                success = height_plc.SendIdle();
                                break;
                        }
                        message = height_plc.Message;
                        break;
                    case "NG站":
                        switch (action_detail)
                        {
                            case "NG出料":
                                success = ng_plc.RunNGOut();
                                break;
                            case "單站初始化":
                                success = ng_plc.RunStationInitialized();
                                break;
                            case "關閉通訊":
                                success = ng_plc.SendIdle();
                                break;
                        }
                        message = ng_plc.Message;
                        break;
                    case "上蓋站":
                        switch (action_detail)
                        {
                            case "換盤":
                                success = lid_station_plc.RunOneStepFlow();
                                break;
                            case "關閉通訊":
                                success = lid_station_plc.SendIdle();
                                break;
                        }
                        message = lid_station_plc.Message;
                        break;
                    case "入料站":
                        switch (action_detail)
                        {
                            case "換盤":
                                success = in_station_plc.RunOneStepFlow();
                                break;
                            case "關閉通訊":
                                success = in_station_plc.SendIdle();
                                break;
                        }
                        message = in_station_plc.Message;
                        break;
                    case "出料站":
                        switch (action_detail)
                        {
                            case "換盤":
                                success = out_station_plc.RunOneStepFlow();
                                break;
                            case "關閉通訊":
                                success = out_station_plc.SendIdle();
                                break;
                        }
                        message = out_station_plc.Message;
                        break;
                }
                OnPresentResponseEvent("send_plc_action", new SendPLCActionResponseArgs { message = message, success = success, show_message = !success });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"PLC動作失敗，PLC:{e.Station}、動作:{e.ActionDetail}。");
            }
        }
        private void SendReaderAction(SendReaderActionArgs e)
        {
            try
            {
                string station = e.Station;
                string action = e.Action;
                KeyenceReaderProcessor processor = null;
                if (station.ToLower().Contains("reader"))
                    processor = reader;
                else if (station.ToLower().Contains("out"))
                    processor = out_reader;
                bool success = processor.SendActionFunction(action);
                OnPresentResponseEvent("send_reader_action", new SendReaderActionResponseArgs { station = station, message = processor.Message, success = success, show_message = !success });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"條碼機動作失敗，條碼機{e.Station}、動作{e.Action}。");
            }
        }
        private void SendLaserHeightAction(SendLaserHeightActionsArgs e)
        {
            try
            {
                var channel = e.Channel;
                string action = e.Action;
               
                bool success = lk_processor.SendActionFunction(channel, action);
                OnPresentResponseEvent("send_height_action", new SendLaserHeightActionsResponseArgs { channel = channel, message = lk_processor.Message, value = lk_processor.Value, connect_state = lk_processor.ConnectState, success = success, show_message = !success });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"測高機動作失敗，測高機 {e.Channel}、動作{e.Action}。");
            }
        }
        private void SendMarkingAction(SendMarkingActionsArgs e)
        {
            try
            {
                var Message = "";
                var success = false;
                switch (e.Action)
                {
                    case "SettingParam":
                        system_param.MarkParam.marking_fst_txt = e.FirstText;
                        system_param.MarkParam.marking_snd_txt = e.SecondText;
                        system_param.MarkParam.marking_snd_index = e.SecondIndex;
                        system_param.MarkParam.marking_2d_txt = e.CodeText;
                        SaveSystemParam();
                        success = lfj_processor.SetTextParam(system_param.MarkParam);
                        Message = success ? $"設定成功。回傳結果:{lfj_processor.Message}" : $"設定失敗。回傳結果:{lfj_processor.Message}";
                        break;
                    case "Marking":
                        success = lfj_processor.StartMarking(system_param.MarkParam.start_marking_code);
                        Message = success ? $"雷雕成功。回傳結果:{lfj_processor.Message}" : $"雷雕失敗。回傳結果:{lfj_processor.Message}";
                        break;
                    case "Connect":
                        success = lfj_processor.StartConnected();
                        Message = success ? $"連線成功。回傳結果:{lfj_processor.Message}" : $"連線失敗。回傳結果:{lfj_processor.Message}";
                        break;
                    case "Disconnect":
                        success = lfj_processor.ConnectedClose();
                        Message = success ? $"關閉連線成功。回傳結果:{lfj_processor.Message}" : $"關閉連線失敗。回傳結果:{lfj_processor.Message}";
                        break;
                    case "SetShift":
                        system_param.MarkParam.shift_x = e.OffsetX;
                        system_param.MarkParam.shift_y = e.OffsetY;
                        system_param.MarkParam.shift_a = e.OffsetA;
                        SaveSystemParam();
                        success = lfj_processor.SetShiftData(system_param.MarkParam.shift_code, system_param.MarkParam.shift_x, system_param.MarkParam.shift_y, system_param.MarkParam.shift_a);
                        Message = success ? $"設定偏移成功。回傳結果:{lfj_processor.Message}" : $"設定偏移失敗。回傳結果:{lfj_processor.Message}";
                        break;
                    case "SetLevel":
                        system_param.MarkParam.pass_level = e.PassLevel;
                        Message = $"設定等級成功。";
                        success = true;
                        break;
                }
                OnPresentResponseEvent("send_marking_action", new SendMarkingActionResponseArgs { success = success, message = Message, connection = lfj_processor.ConnectedState });

            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"雷雕機動作失敗，動作{e.Action}。");
            }
        }
        public void RunSfisStepAction(SfisStepArgs e)
        {
            try
            {
                var success = false;
                if (system_param.Sfis.Enable)
                    success = _sfis.SendStep(e.Step, e.Param, new CaseData { ReaderResult1 = e.Param.BarcodeA, ReaderResult2 = e.Param.BarcodeB });
                OnPresentResponseEvent("show_message", new SendMessageBoxArgs { Name = "SFIS訊息", Message = success ? "模擬發送成功。" : $"模擬發送訊號失敗。錯誤訊息:{_sfis.Message}", Image = success ? System.Windows.MessageBoxImage.Information : System.Windows.MessageBoxImage.Error });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"發送SFIS測試動作錯誤。");
            }
        }
        public void SetArmsShiftAction(ArmsShiftArgs e)
        {
            try
            {
                var processor = e.Arms.ToLower() == "in" ? in_arms : lid_arms;
                if (e.Arms.ToLower() == "in")
                    system_param.ShiftInArms = e.Value;
                else
                    system_param.ShiftOutArms = e.Value;
                processor.SetShiftValue(e.Value);
                OnPresentResponseEvent("show_message", new SendMessageBoxArgs { Name = "手臂訊息", Message = "設定完成。", Image = System.Windows.MessageBoxImage.Information });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"設定手臂偏移錯誤。");
            }
        }
        public void GetArmsShiftAction(ArmsShiftArgs e)
        {
            try
            {
                var processor = e.Arms.ToLower() == "in" ? in_arms : lid_arms;
                var val = processor.GetShiftValue();
                if (e.Arms.ToLower() == "in")
                    system_param.ShiftInArms = val;
                else
                    system_param.ShiftOutArms = val;
                OnPresentResponseEvent("get_arms_shift_response", new ArmsShiftArgs { Arms = e.Arms, Value = val });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"取得手臂偏移參數錯誤。");
            }
        }
        public void SetPlateAccuracyAction(SetPlateAccuracyArgs e)
        {
            try
            {
                system_param.PlateAccuracy = e.Accuracy;
                lid_arms.SetPlateAccuracy(system_param.PlateAccuracy);
                OnPresentResponseEvent("show_message", new SendMessageBoxArgs { Name = "出料手臂訊息", Message = "設定完成。", Image = System.Windows.MessageBoxImage.Information });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"設定組裝精度參數錯誤。");
            }
        }
        public void SaveSystemParamAction(SystemParamArgs e)
        {
            try
            {
                CaseNgOutTask.GetEntity().NgCountLimit = system_param.NgOutCountLimit = e.Param.NgOutCountLimit;
                system_param.DataRecordCount = e.Param.DataRecordCount;
                system_param.Sfis.StationID = e.Param.Sfis.StationID;
                system_param.Sfis.LineID = e.Param.Sfis.LineID;
                system_param.Sfis.BarcodeA = e.Param.Sfis.BarcodeA;
                system_param.Sfis.BarcodeB = e.Param.Sfis.BarcodeB;
                system_param.Sfis.InspLevel = e.Param.Sfis.InspLevel;
                system_param.Sfis.Enable = e.Param.Sfis.Enable;
                system_param.MeasurePosition = e.Param.MeasurePosition;
                system_param.FlatnessUpperLimit = e.Param.FlatnessUpperLimit;
                system_param.HeightLimit = e.Param.HeightLimit;
                SaveSystemParam();
                OnPresentResponseEvent("show_message", new SendMessageBoxArgs { Name = "參數設定", Message = "系統參數儲存成功。", Image = System.Windows.MessageBoxImage.Information });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"儲存參數動作錯誤。");
            }
        }
        public void EstHeighAction()
        {
            try
            {
                var height_val = height_plc.GetHeightVal();
                var base_param = new List<float>();
                var plane_func = CalcFunc.CalBasePlane(system_param.MeasurePosition, height_val, out base_param);
                var dist = CalcFunc.CalDist(system_param.MeasurePosition, height_val, base_param);
                var flatness = CalcFunc.CalFlatness(dist);
                OnPresentResponseEvent("estimate_height_response_action", new EstimateHeightResponseActionArgs { HeightValue = height_val, PlaneFunc = plane_func, Distance = dist, Flatness = flatness });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"測高機單測功能錯誤。");
            }
        }
        public void SetWorksheetAction(SetWorkSheetArgs e)
        {
            try
            {
                system_param.Sfis.WorkerID = e.WorkerID;
                system_param.Sfis.TicketID = e.TicketID;
                system_param.Sfis.LidLotID = e.LidLotID;
                system_param.Sfis.NutNo = e.NutLotID;
                system_param.CaseCount = e.RunCount;
                system_param.Recipe = e.Recipe;
                SaveSystemParam();
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"儲存工單資料錯誤。");
            }
        }
        public void RunMainFlowAction(RunMainFlowArgs e)
        {
            try
            {
                if (e.Action.ToLower().Contains("start") || e.Action.ToLower().Contains("resume"))
                {
                    main_plc.PCErrorSet(0);

                    #region 檢查硬體狀態
                    var restart = false;
                    var message = "Start All Cases.";
                    main_plc.GetStatus();
                    if (main_plc.IsError || main_plc.IsInitialized || main_plc.IsSend)
                    {
                        restart = true;
                        message = "主流道PLC錯誤，請確認。";
                    }
                    nut_plc.GetStatus();
                    if (!(nut_plc.IsIdle || nut_plc.IsFinish))
                    {
                        restart = true;
                        message = "螺帽站PLC錯誤，請確認。";
                    }
                    bend_plc.GetStatus();
                    if (!(bend_plc.IsIdle || bend_plc.IsFinish))
                    {
                        restart = true;
                        message = "折彎站PLC錯誤，請確認。";
                    }
                    plate_plc.GetStatus();
                    if (!(plate_plc.IsIdle || plate_plc.IsFinish))
                    {
                        restart = true;
                        message = "壓平站PLC錯誤，請確認。";
                    }
                    height_plc.GetStatus();
                    if (!(height_plc.IsIdle || height_plc.IsFinish))
                    {
                        restart = true;
                        message = "測高站PLC錯誤，請確認。";
                    }
                    ng_plc.GetStatus();
                    if (ng_plc.IsError || ng_plc.IsInitialized || ng_plc.IsSend)
                    {
                        restart = true;
                        message = "NG站PLC錯誤，請確認。";
                    }
                    
                    in_station_plc.GetStatus();
                    if (in_station_plc.IsError)
                    {
                        restart = true;
                        message = "入料站PLC錯誤，請確認。";
                    }
                    lid_station_plc.GetStatus();
                    if (lid_station_plc.IsError)
                    {
                        restart = true;
                        message = "組裝站PLC錯誤，請確認。";
                    }
                    out_station_plc.GetStatus();
                    if (out_station_plc.IsError)
                    {
                        restart = true;
                        message = "出料站PLC錯誤，請確認。";
                    }
                    if (!in_arms.IsLogin)
                    {
                        restart = true;
                        message = "入料手臂尚未登入遠端控制，請確認。";
                    }
                    if (!lid_arms.IsLogin)
                    {
                        restart = true;
                        message = "組裝手臂尚未登入遠端控制，請確認。";
                    }
                    if (!out_arms.IsLogin)
                    {
                        restart = true;
                        message = "出料手臂尚未登入遠端控制，請確認。";
                    }
                    if (!lfj_processor.ConnectedState)
                    {
                        restart = true;
                        message = "雷射尚未登入連線，請確認。";
                    }
                    if (!lk_processor.ConnectState)
                    {
                        restart = true;
                        message = "測高機尚未登入連線，請確認。";
                    }
                    #endregion 檢查硬體狀態
                    #region 卡控檢查

                    #region 檢查NG數量
                    if (CaseNgOutTask.GetEntity().NgCount >= system_param.NgOutCountLimit)
                    {
                        restart = true;
                        message = "NG計數尚未重置，請重置NG數量。";
                    }
                    if (CaseOutTask.GetEntity().NGCount >= CaseOutTask.GetEntity().NGCountLimit)
                    {
                        restart = true;
                        message = "NG計數尚未重置，請重置NG數量。";
                    }
                    #endregion 檢查NG數量
                    #endregion 卡控檢查
                    if (restart)
                    {
                        OnPresentResponseEvent("run_main_flow", new RunMainFlowArgs { ReStartFlow = e.Action.ToLower().Contains("resume") ? false : restart, Message = message });
                        return;
                    }
                }
                if (e.Action.ToLower().Contains("start"))
                {
                    #region 檢查Load/Unload
                    in_station_plc.GetStatus();
                    if (in_station_plc.IsChangedCassette)
                    {
                        OnPresentResponseEvent("run_main_flow", new RunMainFlowArgs { ReStartFlow = true, Message = "入料站尚未入盤，請先入盤，再按下「Start」按鈕。" });
                        return;
                    }
                    lid_station_plc.GetStatus();
                    if (lid_station_plc.IsChangedCassette)
                    {
                        OnPresentResponseEvent("run_main_flow", new RunMainFlowArgs { ReStartFlow = true, Message = "組裝站尚未入盤，請先入盤，再按下「Start」按鈕。" });
                        return;
                    }
                    out_station_plc.GetStatus();
                    if (out_station_plc.IsChangedCassette)
                    {
                        OnPresentResponseEvent("run_main_flow", new RunMainFlowArgs { ReStartFlow = true, Message = "出料站尚未入盤，請先入盤，再按下「Start」按鈕。" });
                        return;
                    }
                    #endregion 檢查Load/Unload
                    #region 初始化手臂並將PLC設定成自動模式
                    Task.WaitAll(new Task[] {
                        Task.Run(()=> in_arms.Home()),
                        Task.Run(()=> lid_arms.Home()),
                        Task.Run(()=> out_arms.Home()),
                        Task.Run(()=>main_plc.SwitchAutoMode(1)),
                        });
                    Task.WaitAll(new Task[] {
                        Task.Run(()=> in_arms.SetRecipe(system_param.Recipe)),
                        Task.Run(() => lid_arms.SetRecipe(system_param.Recipe)),
                        Task.Run(() => out_arms.SetRecipe(system_param.Recipe)),
                    });
                    #endregion 初始化手臂並將PLC設定成自動模式

                    #region 重置計數
                    CaseNgOutTask.GetEntity().NgCount = 0;
                    CaseOutTask.GetEntity().NGCount = 0;
                    #endregion 重置計數
                    Thread.Sleep(500);
                    if (CaseAllTask.GetEntity().IsRunning)
                    {
                        CaseAllTask.GetEntity().Stop();
                        Thread.Sleep(1000);
                    }
                    if (!main_plc.RunAllInitialized())
                    {
                        OnPresentResponseEvent("run_main_flow", new RunMainFlowArgs { ReStartFlow = true, Message = "All PLC Initialize Fail." });
                        return;
                    }
                    CaseAllTask.GetEntity().StartTask();
                    is_run = true;

                }
                else if (e.Action.ToLower().Contains("stop"))
                {
                    main_plc.PCErrorSet(0);
                    CaseAllTask.GetEntity().Stop();
                    is_run = false;
                }
                else if (e.Action.ToLower().Contains("resume"))
                {
                    main_plc.PCErrorSet(0);
                    CaseAllTask.GetEntity().Resume();
                    is_run = true;
                }
                else if (e.Action.ToLower().Contains("pause"))
                {
                    CaseAllTask.GetEntity().Pause();
                    is_run = false;
                }
            }
            catch (Exception ex)
            {
                OnPresentResponseEvent("run_main_flow", new RunMainFlowArgs { ReStartFlow = true, Message = $"執行主流程動作錯誤，動作{e.Action}。" });
                logger = LoggerData.Error(ex, $"執行主流程動作錯誤，動作{e.Action}。");
            }
        }
        public void ManualNgSettingAction(ManualNGSettingArgs e)
        {
            try
            {
                switch (e.station_index)
                {
                    case 1:
                        var case_data = CaseInTask.GetEntity().case_data;
                        case_data.IsRun = false;
                        case_data.ManualNG = true;
                        case_data.NGPosition.Add(e.station_index);
                        CaseInTask.GetEntity().SetStep(0);
                        break;
                    case 5:
                        case_data = CaseLidTask.GetEntity().case_data;
                        case_data.IsRun = false;
                        case_data.ManualNG = true;
                        CaseLidTask.GetEntity().PutCaseFinish = true;
                        case_data.NGPosition.Add(e.station_index);
                        CaseLidTask.GetEntity().SetStep(6);
                        break;
                    case 7:
                        case_data = CaseScanCodeTask.GetEntity().case_data;
                        case_data.IsRun = false;
                        case_data.ManualNG = true;
                        case_data.NGPosition.Add(e.station_index);
                        CaseScanCodeTask.GetEntity().Status = EnumData.TaskStatus.Done;
                        break;
                    case 8:
                        case_data = CasePutNutTask.GetEntity().case_data;
                        case_data.IsRun = false;
                        case_data.ManualNG = true;
                        case_data.NGPosition.Add(e.station_index);
                        CasePutNutTask.GetEntity().Status = EnumData.TaskStatus.Done;
                        break;
                    case 9:
                    case 10:
                        case_data = CaseBendTask.GetEntity().case_data;
                        case_data.IsRun = false;
                        case_data.ManualNG = true;
                        case_data.NGPosition.Add(e.station_index);
                        CaseBendTask.GetEntity().Status = EnumData.TaskStatus.Done;
                        break;
                    case 11:
                        case_data = CasePlateTask.GetEntity().case_data;
                        case_data.IsRun = false;
                        case_data.ManualNG = true;
                        case_data.NGPosition.Add(e.station_index);
                        CasePlateTask.GetEntity().Status = EnumData.TaskStatus.Done;
                        break;
                    case 13:
                        case_data = CaseEstHeightTask.GetEntity().case_data;
                        case_data.IsRun = false;
                        case_data.ManualNG = true;
                        case_data.NGPosition.Add(e.station_index);
                        CaseEstHeightTask.GetEntity().Status = EnumData.TaskStatus.Done;
                        break;
                    case 14:
                        case_data = CaseNgOutTask.GetEntity().case_data;
                        case_data.IsRun = false;
                        case_data.ManualNG = true;
                        case_data.NGPosition.Add(e.station_index);
                        CaseNgOutTask.GetEntity().Status = EnumData.TaskStatus.Done;
                        break;
                    case 15:
                        case_data = CaseMarkingTask.GetEntity().case_data;
                        case_data.IsRun = false;
                        case_data.ManualNG = true;
                        case_data.NGPosition.Add(e.station_index);
                        CaseMarkingTask.GetEntity().Status = EnumData.TaskStatus.Done;
                        break;
                    case 20:
                        case_data = CaseOutTask.GetEntity().case_data;
                        case_data.IsRun = false;
                        case_data.ManualNG = true;
                        case_data.NGPosition.Add(e.station_index);
                        if (system_param.Sfis.Enable)
                        {
                            _sfis.SendStep(3, MainPresenter.SystemParam().Sfis, case_data);
                        }
                        CaseOutTask.GetEntity().Status = EnumData.TaskStatus.Done;
                        break;
                }
                OnPresentResponseEvent("show_message", new SendMessageBoxArgs { Name = "流程訊息", Message = $"手動NG設定完成，請確認是否已將物料移除。", Image = System.Windows.MessageBoxImage.Information });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"手動設定NG站別失敗。");
            }
        }
        public void ResetNgCountAction()
        {
            try
            {
                CaseNgOutTask.GetEntity().NgCount = 0;
                OnPresentResponseEvent("show_message", new SendMessageBoxArgs { Name = "流程訊息", Message = "重置成功。", Image = System.Windows.MessageBoxImage.Information });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"重置NG計數失敗。");
            }
        }
        public void ResetOutNgCountAction()
        {
            try
            {
                CaseOutTask.GetEntity().NGCount = 0;
                OnPresentResponseEvent("show_message", new SendMessageBoxArgs { Name = "流程訊息", Message = "重置成功。", Image = System.Windows.MessageBoxImage.Information });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"重置出料NG計數失敗。");
            }
        }
        public void ResetPCErrorAction()
        {
            try
            {
                var success = main_plc.PCErrorSet(0);
                var message = success ? "成功" : "失敗";
                OnPresentResponseEvent("show_message", new SendMessageBoxArgs { Name = "流程訊息", Message = $"重置{message}。", Image = success ? System.Windows.MessageBoxImage.Information : System.Windows.MessageBoxImage.Error });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"重置NG計數失敗。");
            }
        }
        public void SetSystemFlowAction(SetSystemFlowArgs e)
        {
            try
            {
                LoggerData.Info($"儲存系統流程參數開始。");
                system_param.Flow = e.Flow;
                SaveSystemParam();
                LoggerData.Info($"儲存系統流程參數完成，參數:{e.Flow.CaseAssemble},{e.Flow.CaseScan},{e.Flow.CasePutNut},{e.Flow.CaseBending},{e.Flow.CasePlate},{e.Flow.CaseEstHeight},{e.Flow.CaseNgOut},{e.Flow.CaseMarking}。");
                OnPresentResponseEvent("show_message", new SendMessageBoxArgs { Name = "參數設定", Message = "系統參數儲存成功。", Image = System.Windows.MessageBoxImage.Information });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"儲存系統流程參數錯誤。");
            }
        }
        private void SendSingleStationFlow(SendSingleStationFlowArgs e)
        {
            try
            {
                if (run_single_flow)
                    OnPresentResponseEvent("send_flow_error", new SendMessageBoxArgs { Message = "流程執行中，請等待流程結束後，再行後續動作。" });
                else
                {
                    run_single_flow = run_flow = true;
                    if (e.Station == "In")
                    {
                        CaseInTask.GetEntity().case_data = e.CaseDatas[0]; CaseInTask.GetEntity().StartTask();
                    }
                    else if (e.Station == "Lid")
                    {
                        CaseLidTask.GetEntity().case_data = e.CaseDatas[0]; CaseLidTask.GetEntity().StartTask();
                    }
                    else if (e.Station == "Scan")
                    {
                        CaseScanCodeTask.GetEntity().case_data = e.CaseDatas[0]; CaseScanCodeTask.GetEntity().StartTask();
                    }
                    else if (e.Station == "PutNut")
                    {
                        CasePutNutTask.GetEntity().case_data = e.CaseDatas[0]; CasePutNutTask.GetEntity().StartTask();
                    }
                    else if (e.Station == "Bend")
                    {
                        CaseBendTask.GetEntity().case_data = e.CaseDatas[0]; CaseBendTask.GetEntity().StartTask();
                    }
                    else if (e.Station == "Plate")
                    {
                        CasePlateTask.GetEntity().case_data = e.CaseDatas[0]; CasePlateTask.GetEntity().StartTask();
                    }
                    else if (e.Station == "EstHeight")
                    {
                        CasePlateTask.GetEntity().case_data = e.CaseDatas[0]; CaseEstHeightTask.GetEntity().StartTask();
                    }
                    else if (e.Station == "NgOut")
                    {
                        CaseNgOutTask.GetEntity().case_data = e.CaseDatas[0]; CaseNgOutTask.GetEntity().StartTask();
                    }
                    else if (e.Station == "Marking")
                    {
                        CaseMarkingTask.GetEntity().case_data = e.CaseDatas[0]; CaseMarkingTask.GetEntity().StartTask();
                    }
                    else if (e.Station == "Out")
                    {
                        CaseOutTask.GetEntity().case_data = e.CaseDatas[0]; CaseOutTask.GetEntity().StartTask();
                    }
                    else if (e.Station == "InStation")
                    {
                        CaseInStationTask.GetEntity().StartTask();
                    }
                    else if (e.Station == "LidStation")
                    {
                        CaseLidStationTask.GetEntity().StartTask();
                    }
                    else if (e.Station == "OutStation")
                    {
                        CaseOutStationTask.GetEntity().StartTask();
                    }
                    else if (e.Station == "NextStep")
                    {
                        main_plc.RunToNextStep();
                        run_flow = false;
                    }
                    Task.Run(() =>
                    {
                        while (run_single_flow)
                        {
                            Thread.Sleep(200);
                            OnPresentResponseEvent("send_single_station_response", new SendSingleStationFlowArgs { Station = e.Station, CaseDatas = e.CaseDatas });
                        };
                    });
                }
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"執行單動流程失敗。");
            }
        }
        private void SendSingleStationControlFlow(SendSingleStationFlowArgs e)
        {
            try
            {
                BaseTask flow_task = null;
                if (CaseInTask.GetEntity().ThreadState == System.Threading.ThreadState.Running || CaseInTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                    flow_task = CaseInTask.GetEntity();
                else if (CaseLidTask.GetEntity().ThreadState == System.Threading.ThreadState.Running || CaseLidTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                    flow_task = CaseLidTask.GetEntity();
                else if (CasePutNutTask.GetEntity().ThreadState == System.Threading.ThreadState.Running || CasePutNutTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                    flow_task = CasePutNutTask.GetEntity();
                else if (CaseBendTask.GetEntity().ThreadState == System.Threading.ThreadState.Running || CaseBendTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                    flow_task = CaseBendTask.GetEntity();
                else if (CasePlateTask.GetEntity().ThreadState == System.Threading.ThreadState.Running || CasePlateTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                    flow_task = CasePlateTask.GetEntity();
                else if (CaseEstHeightTask.GetEntity().ThreadState == System.Threading.ThreadState.Running || CaseEstHeightTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                    flow_task = CaseEstHeightTask.GetEntity();
                else if (CaseNgOutTask.GetEntity().ThreadState == System.Threading.ThreadState.Running || CaseNgOutTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                    flow_task = CaseNgOutTask.GetEntity();
                else if (CaseMarkingTask.GetEntity().ThreadState == System.Threading.ThreadState.Running || CaseMarkingTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                    flow_task = CaseMarkingTask.GetEntity();
                else if (CaseOutTask.GetEntity().ThreadState == System.Threading.ThreadState.Running || CaseOutTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                    flow_task = CaseOutTask.GetEntity();
                else if (CaseInStationTask.GetEntity().ThreadState == System.Threading.ThreadState.Running || CaseInStationTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                    flow_task = CaseInStationTask.GetEntity();
                else if (CaseLidStationTask.GetEntity().ThreadState == System.Threading.ThreadState.Running || CaseLidStationTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                    flow_task = CaseLidStationTask.GetEntity();
                else if (CaseOutStationTask.GetEntity().ThreadState == System.Threading.ThreadState.Running || CaseOutStationTask.GetEntity().ThreadState == System.Threading.ThreadState.Suspended)
                    flow_task = CaseOutStationTask.GetEntity();
                if (flow_task == null)
                    return;
                if (e.Station == "Pause")
                    flow_task.PauseTaskWithoutWait();
                else if (e.Station == "Resume")
                    flow_task.ResumeTask();
                else if (e.Station == "Stop")
                {
                    run_single_flow = run_flow = false;
                    flow_task.SetStep(0);
                    flow_task.StopTask();
                }
                OnPresentResponseEvent("send_flow_error", new SendMessageBoxArgs { Message = $"執行 {e.Station} 動作完成。", Image = System.Windows.MessageBoxImage.Information });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"執行單動控制流程錯誤，動作{e.Station}。");
            }
        }
        public void LoadLogDatasAction()
        {
            try
            {
                OnPresentResponseEvent("load_log_datas", new UpdateLoggerArgs { Datas = _logger });
            }
            catch (Exception ex)
            {
                logger = LoggerData.Error(ex, $"讀取檢測膠量設定資料失敗。");
            }
        }
    }
}
