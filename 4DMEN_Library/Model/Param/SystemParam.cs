using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class SystemParam
    {
        /// <summary>
        /// 執行數量
        /// </summary>
        public int CaseCount { get; set; } = 12;
        /// <summary>
        /// 產品編號
        /// </summary>
        public int Recipe { get; set; } = 0;
        /// <summary>
        /// 出入料Tray上限數
        /// </summary>
        public int TrayCount { get; set; } = 12;
        /// <summary>
        /// 組裝Tray上限數
        /// </summary>
        public int LidTrayCount { get; set; } = 35;
        /// <summary>
        /// NG數量上限
        /// </summary>
        public int NgOutCountLimit { get; set; } = 3;
        /// <summary>
        /// NG數量計數
        /// </summary>
        public int NgOutCount { get; set; } = 0;
        /// <summary>
        /// 資料保存時間(天)
        /// </summary>
        public int DataRecordCount { get; set; } = 180;
        /// <summary>
        /// 上傳SFIS資料
        /// </summary>
        public SfisParam Sfis { get; set; } = new SfisParam();
        /// <summary>
        /// 測高位置
        /// </summary>
        public List<EstimatePosition> MeasurePosition { get; set; } = new List<EstimatePosition>();
        /// <summary>
        /// 平整度上限
        /// </summary>
        public float FlatnessUpperLimit { get; set; } = 1;
        /// <summary>
        /// 高度上下限範圍
        /// </summary>
        public List<Range> HeightLimit { get; set; } = new List<Range>();
        /// <summary>
        /// 入料手臂偏移參數
        /// </summary>
        public ShiftArms ShiftInArms { get; set; } = new ShiftArms();
        /// <summary>
        /// 出料手臂偏移參數
        /// </summary>
        public ShiftArms ShiftOutArms { get; set; } = new ShiftArms();
        /// <summary>
        /// 偏移上下限卡控值(mm)
        /// </summary>
        public Range ShiftLimit { get; set; } = new Range();
        /// <summary>
        /// 出料手臂組裝精度卡控數值
        /// </summary>
        public PlateAccuracy PlateAccuracy { get; set; } = new PlateAccuracy();
        /// <summary>
        /// 系統執行流程
        /// </summary>
        public SystemFlow Flow { get; set; } = new SystemFlow();
        /// <summary>
        /// 系統狀態
        /// </summary>
        public string SystemStatus => "Idle";//CaseAllTask.GetEntity().Status == EnumData.TaskStatus.Idle ? "Idle" : CaseAllTask.GetEntity().Status == EnumData.TaskStatus.Pause ? "Pause" : "Runing";
        /// <summary>
        /// 入料手臂高度檢測
        /// </summary>
        public Range EstHeighIn { get; set; } = new Range { Lower = -100, Upper = 100 };
        /// <summary>
        /// 雷射設定參數
        /// </summary>
        public MarkingParam MarkParam { get; set; } = new MarkingParam();
        /// <summary>
        /// 雷雕IP
        /// </summary>
        public string MarkingIP { get; set; } = "127.0.0.1";
        /// <summary>
        /// 雷雕Port
        /// </summary>
        public int MarkingPort { get; set; } = 4000;

    }
    public class Range
    {
        public double Upper { get; set; }
        public double Lower { get; set; }
    }
    public class ShiftArms
    {
        public ShiftArmsAxis Pick { get; set; } = new ShiftArmsAxis();
        public ShiftArmsAxis Put { get; set; } = new ShiftArmsAxis();
    }
    public class EstimatePosition
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
    }
    public class ShiftArmsAxis
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double U { get; set; }
    }
    public class NeedleAlignAxis
    {
        public double X { get; set; } = double.NaN;
        public double Y { get; set; } = double.NaN;
        public double Z { get; set; } = double.NaN;
    }
   
    public class NeedleAlignLimit
    {
        public Range X { get; set; } = new Range();
        public Range Y { get; set; } = new Range();
        public Range Z { get; set; } = new Range();
    }
    public class PlateAccuracy
    {
        public Range X { get; set; } = new Range();
        public Range Y { get; set; } = new Range();
        public Range U { get; set; } = new Range();
    }
    public class SfisParam
    {
        /// <summary>
        /// SFIS開關
        /// </summary>
        public bool Enable { get; set; } = false;
        /// <summary>
        /// 站別
        /// </summary>
        public string StationID { get; set; } = "CA01";
        /// <summary>
        /// 線別
        /// </summary>
        public string LineID { get; set; } = "PD01";
        /// <summary>
        /// 工單
        /// </summary>
        public string TicketID { get; set; } = "W177";
        /// <summary>
        /// 操作員
        /// </summary>
        public string WorkerID { get; set; } = "M026125";
        /// <summary>
        /// 組裝上蓋編號
        /// </summary>
        public string LidLotID { get; set; } = "1234";
        /// <summary>
        /// 螺帽編號
        /// </summary>
        public string NutNo { get; set; } = "2345";
        /// <summary>
        /// 秤重站條碼
        /// </summary>
        public string BarcodeA { get; set; } = "";
        /// <summary>
        /// 出料站條碼
        /// </summary>
        public string BarcodeB { get; set; } = "";
        /// <summary>
        /// 條碼檢測等級
        /// </summary>
        public string InspLevel { get; set; }
    }
    public class SystemFlow
    {
        /// <summary>
        /// 上蓋組裝
        /// </summary>
        public bool CaseAssemble { get; set; } = true;
        /// <summary>
        /// 條碼掃描
        /// </summary>
        public bool CaseScan { get; set; } = true;
        /// <summary>
        /// 底板折彎
        /// </summary>
        public bool CaseBending { get; set; } = true;
        /// <summary>
        /// 底板壓平
        /// </summary>
        public bool CasePlate { get; set; } = true;
        /// <summary>
        /// 高度測試
        /// </summary>
        public bool CaseEstHeight { get; set; } = true;
        /// <summary>
        /// NG排料
        /// </summary>
        public bool CaseNgOut { get; set; } = true;
        /// <summary>
        /// 雷射雕刻
        /// </summary>
        public bool CaseMarking { get; set; } = true;
    }
    public class MarkingParam
    {
        public string marking_fst_code { get; set; } = "CM_TextObj T1";
        public string marking_fst_txt { get; set; } = "EAB450M12XM35";
        public string marking_snd_code { get; set; } = "CM_TextObj T2";
        public string marking_snd_txt { get; set; } = "HX2220-A1";
        public int marking_snd_index { get; set; } = 0;
        public string marking_snd_index_txt { get => marking_snd_index.ToString().PadLeft(3,'0'); }
        public string marking_2d_code { get; set; } = "CM_2DObj Var2D";
        public string marking_2d_txt { get; set; } = "0000000001";
        public string marking_2d_result { get => $"{marking_fst_txt}-{marking_snd_txt}{marking_snd_index_txt}-{marking_2d_txt}"; }
        public string start_marking_code { get; set; } = "CM_StartMarking";
        public string shift_code { get; set; } = "CM_OffsetExt";
        public int shift_x { get; set; } = 0;
        public int shift_y { get; set; } = 0;
        public int shift_a { get; set; } = 0;
        public List<string> pass_level { get; set; } = new List<string>();
    }
}
