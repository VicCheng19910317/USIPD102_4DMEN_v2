using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace _4DMEN_Library.Model
{
    public class UpdateLoggerArgs : EventArgs
    {
        public List<LogData> Datas { get; set; }
    }

    public class SendMessageBoxArgs : EventArgs
    {
        public string Name { get; set; }
        public MessageBoxButton Button { get; set; } = MessageBoxButton.OK;
        public MessageBoxImage Image { get; set; } = MessageBoxImage.Error;
        public string Message { get; set; }
    }
    public class SendInitResponseArgs : EventArgs
    {
        public bool success { get; set; }
        public string message { get; set; }
    }

    #region 手臂
    public class SendArmsActionArgs : EventArgs
    {
        public string arms { get; set; }
        public string action { get; set; }
    }
    public class SendArmsActionResponseArgs : EventArgs
    {
        public string arms { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public bool show_message { get; set; } = false;
    }
    public class LoadArmsParamArgs : EventArgs
    {
        public List<EsponArmsParam> arms_param { get; set; }
    }
    #endregion 手臂
    #region PLC
    public class SendPLCActionArgs : EventArgs
    {
        public string Station { get; set; }
        public string ActionDetail { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
    }
    public class SendPLCActionResponseArgs : EventArgs
    {
        public bool success { get; set; }
        public string message { get; set; }
        public bool show_message { get; set; } = false;
    }
    public class DisplayPLCActionArgs : EventArgs
    {
        public KeyencePLCNetParam plc_param { get; set; }
        public List<DisplayPLCAction> plc_action { get; set; }
    }
    #endregion PLC
    #region 條碼機
    public class SendReaderActionArgs : EventArgs
    {
        public string Station { get; set; }
        public string Action { get; set; }
    }
    
    public class SendReaderActionResponseArgs : EventArgs
    {
        public string station { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public bool show_message { get; set; } = false;
    }
    public class LoadReaderParamArgs : EventArgs
    {
        public KeyenceReaderParam reader { get; set; }
        public KeyenceReaderParam out_reader { get; set; }
    }
    #endregion 條碼機
    #region 測高機
    public class SendLaserHeightActionsArgs : EventArgs
    {
        public int Channel { get; set; }
        public string Action { get; set; }
    }

    public class SendLaserHeightActionsResponseArgs : EventArgs
    {
        public int channel { get; set; }
        public float value { get; set; }
        public bool connect_state { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public bool show_message { get; set; } = false;
    }
    public class LoadLaserHeightParamArgs : EventArgs
    {
        public string IP { get; set; }
        public bool connect_state { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
    }
    #endregion 測高機
    #region 雷雕機
    public class SendMarkingActionsArgs : EventArgs
    {
        public string FirstText { get; set; }
        public string SecondText { get; set; }
        public int SecondIndex { get; set; }
        public string CodeText { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int OffsetA { get; set; }
        public List<string> PassLevel { get; set; }
        public string Action { get; set; }
    }
    public class SendMarkingActionResponseArgs : EventArgs
    {
        public bool connection { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
    }
    #endregion 雷雕機
    #region 系統設定
    public class SfisStepArgs : EventArgs
    {
        public int Step { get; set; }
        public SfisParam Param { get; set; }
    }
    public class ArmsShiftArgs : EventArgs
    {
        public string Arms { get; set; }
        public ShiftArms Value { get; set; }
    }
    public class SetPlateAccuracyArgs : EventArgs
    {
        public PlateAccuracy Accuracy { get; set; }
    }
    public class LoadSystemParamResponseArgs : EventArgs
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public bool MarkingStatus { get; set; } = false;
        public SystemParam Param { get; set; }
    }
    public class SystemParamArgs : EventArgs
    {
        public SystemParam Param { get; set; }
    }
    public class EstimateHeightResponseActionArgs : EventArgs
    {
        public List<float> HeightValue { get; set; } 
        public string PlaneFunc { get; set; }
        public List<float> Distance { get; set; }
        public List<float> Flatness { get; set; }
    }
    public class SetWorkSheetArgs : EventArgs
    {
        public string WorkerID { get; set; }
        public string TicketID { get; set; }
        public string LidLotID { get; set; }
        public string NutLotID { get; set; }
        public int RunCount { get; set; }
        public int Recipe { get; set; }
    }
    public class SetSystemFlowArgs : EventArgs
    {
        public SystemFlow Flow { get; set; }
    }
    #endregion 系統設定
    #region 流程
    public class SendSingleStationFlowArgs : EventArgs
    {
        public string Station { get; set; }
        public List<CaseData> CaseDatas { get; set; }

    }
    public class UpdateCaseDatasArgs : EventArgs
    {
        public List<CaseData> CaseDatas { get; set; }
    }
    public class RunMainFlowArgs : EventArgs
    {
        public string Action { get; set; }
        public bool ReStartFlow { get; set; } = false;
        public string Message { get; set; } = "";
        public string Title { get; set; } = "Run All Cases Flow Infromation";
        public System.Windows.MessageBoxImage Image { get; set; } = System.Windows.MessageBoxImage.Error;
    }
    public class ManualNGSettingArgs : EventArgs
    {
        public int station_index { get; set; }
    }
    #endregion 流程
}
