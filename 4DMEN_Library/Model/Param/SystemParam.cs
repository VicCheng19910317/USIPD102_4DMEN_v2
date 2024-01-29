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
        public int CastCount { get; set; } = 12;
        /// <summary>
        /// 膠材壽命檢測
        /// </summary>
        public bool GlueLifeStart { get; set; } = false;
        /// <summary>
        /// 膠材壽命更新時間
        /// </summary>
        public DateTime GlueLifeTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 膠材壽命逾期時間(小時)
        /// </summary>
        public double GlueLifeAlarm { get; set; } = 3;
        /// <summary>
        /// 膠材壽命檢測區間(分鐘)
        /// </summary>
        public double GlueLifeInterval { get; set; } = 2;
        /// <summary>
        /// 組裝逾期時間(秒)
        /// </summary>
        public double PlateInterval { get; set; } = 180;
        /// <summary>
        /// 吐膠時間(秒)
        /// </summary>
        public double GluePurgeTime { get; set; } = 2;
        /// <summary>
        /// 膠重上下限
        /// </summary>
        public Range GlueWeight { get; set; } = new Range { Lower = 0.2, Upper = 0.9 };
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
        /// 校針上下限數值設定
        /// </summary>
        public NeedleAlignLimit NeedleAlignLimit { get; set; } = new NeedleAlignLimit();
        /// <summary>
        /// 校針間隔差異數值設定
        /// </summary>
        public NeedleAlignAxis NeedleAlignInterval { get; set; } = new NeedleAlignAxis();
        /// <summary>
        /// 是否完成校針
        /// </summary>
        public bool NeedleTeachFinish { get; set; } = false;
        /// <summary>
        /// 校針數值結果
        /// </summary>
        public NeedleAlignAxis NeedleTeachValue { get; set; } = new NeedleAlignAxis();
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
        /// 手動塗膠Recipe
        /// </summary>
        public int ManualGlueRecipe { get; set; } = 0;
        /// <summary>
        /// 手動塗膠Recipe列表
        /// </summary>
        public List<string> RecipeList { get; set; } = new List<string> { "XM3", "Infineon1B" };

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
        /// 膠材Lot
        /// </summary>
        public string GlueLotID { get; set; } = "1234";
        /// <summary>
        /// 秤重站條碼
        /// </summary>
        public string BarcodeA { get; set; } = "";
        /// <summary>
        /// 出料站條碼
        /// </summary>
        public string BarcodeB { get; set; } = "";
        /// <summary>
        /// 膠重
        /// </summary>
        public string GlueWeight { get; set; }
    }
    public class SystemFlow
    {
        /// <summary>
        /// 入料換盤
        /// </summary>
        public bool CastIn { get; set; } = true;
        /// <summary>
        /// 下壓壓平
        /// </summary>
        public bool CasePlate { get; set; } = true;
        /// <summary>
        /// 塗膠製作
        /// </summary>
        public bool CastGlue { get; set; } = true;
        /// <summary>
        /// 條碼掃描
        /// </summary>
        public bool CastScan { get; set; } = true;
        /// <summary>
        /// 塗膠檢測
        /// </summary>
        public bool CastInsp { get; set; } = true;
        /// <summary>
        /// NG排料
        /// </summary>
        public bool CastNgOut { get; set; } = true;
        /// <summary>
        /// 出料換盤
        /// </summary>
        public bool CastOut { get; set; }
    }
}
