using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    public class CaseData
    {
        #region 參數
        private double _afterWeight = 0;
        private double _beforeWeight = 0;
        private double _glueWeight = 0;
        private string _stationName = "";
        private string _stepName = "";
        #endregion 參數
        #region 屬性
        /// <summary>
        /// 是否執行
        /// </summary>
        public bool IsRun { get; set; } = true;
        /// <summary>
        /// 是否完成取料到台車
        /// </summary>
        public bool PickToCast { get; set; } = true;
        /// <summary>
        /// 當前站號
        /// </summary>
        public int Station { get; set; }
        public string StationName
        {
            get
            {
                if (Station == 1)
                    _stationName = "入料站";
                else if(Station == 2)
                    _stationName = "入料->組裝(2)";
                else if (Station == 3)
                    _stationName = "入料->組裝(3)";
                else if (Station == 4)
                    _stationName = "入料->組裝(4)";
                else if (Station == 5)
                    _stationName = "組裝站";
                else if (Station == 6)
                    _stationName = "組裝->掃碼(6)";
                else if (Station == 7)
                    _stationName = "掃碼站";
                else if (Station == 8)
                    _stationName = "螺帽站";
                else if (Station == 9)
                    _stationName = "折彎站(9)";
                else if (Station == 10)
                    _stationName = "折彎站(10)";
                else if (Station == 11)
                    _stationName = "壓平站";
                else if (Station == 12)
                    _stationName = "壓平->測高(11)";
                else if (Station == 13)
                    _stationName = "測高站";
                else if (Station == 14)
                    _stationName = "NG站";
                else if (Station == 15)
                    _stationName = "雷雕站";
                else if (Station == 16)
                    _stationName = "雷雕->出料(16)";
                else if (Station == 17)
                    _stationName = "雷雕->出料(17)";
                else if (Station == 18)
                    _stationName = "雷雕->出料(18)";
                else if (Station == 19)
                    _stationName = "雷雕->出料(19)";
                else if (Station == 20)
                    _stationName = "出料站";
                return _stationName;
            }
            set => _stationName = value;
        }
        /// <summary>
        /// 當前流程對應步驟
        /// </summary>
        public int Step { get; set; }
        /// <summary>
        /// 步驟名稱
        /// </summary>
        public string StepName
        {
            get
            {
                switch (Station)
                {
                    case 1:
                        switch (Step)
                        {
                            case 0:
                                _stepName = "等待流程開始";
                                break;
                            case 1:
                                _stepName = "設定手臂取料順序";
                                break;
                            case 2:
                                _stepName = "判斷換Tray是否完成";
                                break;
                            case 3:
                                _stepName = "手臂取料";
                                break;
                            case 4:
                                _stepName = "判斷是否可以進行放料";
                                break;
                            case 5:
                                _stepName = "手臂放料到台車上";
                                break;
                            case 6:
                                _stepName = "判斷入料平台是否要換Tray";
                                break;
                            case 7:
                                _stepName = "完成流程";
                                break;
                        }
                        break;
                    case 5:
                        switch (Step)
                        {
                            case 0:
                                _stepName = "等待流程開始";
                                break;
                            case 1:
                                _stepName = "設定手臂取料順序";
                                break;
                            case 2:
                                _stepName = "判斷換Tray是否完成";
                                break;
                            case 3:
                                _stepName = "手臂取料";
                                break;
                            case 4:
                                _stepName = "判斷是否可以進行放料";
                                break;
                            case 5:
                                _stepName = "手臂放料台車上";
                                break;
                            case 6:
                                _stepName = "判斷入料平台是否要換Tray";
                                break;
                            case 7:
                                _stepName = "完成流程";
                                break;
                        }
                        break;

                    case 7:
                        switch (Step)
                        {
                            case 0:
                                _stepName = "等待流程開始";
                                break;
                            case 1:
                                _stepName = "執行掃碼";
                                break;
                            case 2:
                                _stepName = "完成流程";
                                break;
                        }
                        break;
                    case 8:
                        switch (Step)
                        {
                            case 0:
                                _stepName = "等待流程開始";
                                break;
                            case 1:
                                _stepName = "執行放置螺帽";
                                break;
                            case 2:
                                _stepName = "完成流程";
                                break;
                        }
                        break;
                    case 9:
                    case 10:
                        switch (Step)
                        {
                            case 0:
                                _stepName = "等待流程開始";
                                break;
                            case 1:
                                _stepName = "執行折彎";
                                break;
                            case 2:
                                _stepName = "完成流程";
                                break;
                        }
                        break;
                    case 11:
                        switch (Step)
                        {
                            case 0:
                                _stepName = "等待流程開始";
                                break;
                            case 1:
                                _stepName = "執行壓平";
                                break;
                            case 2:
                                _stepName = "完成流程";
                                break;
                        }
                        break;
                    case 13:
                        switch (Step)
                        {
                            case 0:
                                _stepName = "等待流程開始";
                                break;
                            case 1:
                                _stepName = "執行測高";
                                break;
                            case 2:
                                _stepName = "計算公式";
                                break;
                            case 3:
                                _stepName = "完成流程";
                                break;
                        }
                        break;
                    case 14:
                        switch (Step)
                        {
                            case 0:
                                _stepName = "等待流程開始";
                                break;
                            case 1:
                                _stepName = "檢查平整度";
                                break;
                            case 2:
                                _stepName = "檢查高度";
                                break;
                            case 3:
                                _stepName = "NG出料";
                                break;
                            case 4:
                                _stepName = "完成流程";
                                break;
                        }
                        break;
                    case 15:
                        switch (Step)
                        {
                            case 0:
                                _stepName = "等待流程開始";
                                break;
                            case 1:
                                _stepName = "檢查雷雕訊號是否正常";
                                break;
                            case 2:
                                _stepName = "設定雷射雕刻文字";
                                break;
                            case 3:
                                _stepName = "執行雷雕";
                                break;
                            case 4:
                                _stepName = "確認雷雕是否結束";
                                break;
                            case 5:
                                _stepName = "完成流程";
                                break;
                        }
                        break;
                    case 20:
                        switch (Step)
                        {
                            case 0:
                                _stepName = "等待流程開始";
                                break;
                            case 1:
                                _stepName = "手臂移到雷雕掃碼處";
                                break;
                            case 2:
                                _stepName = "執行雷雕掃碼辨識";
                                break;
                            case 3:
                                _stepName = "判斷雷雕品質";
                                break;
                            case 4:
                                _stepName = "手臂移到底板掃碼處";
                                break;
                            case 5:
                                _stepName = "進行底板掃碼並判斷是否正確";
                                break;
                            case 6:
                                _stepName = "手臂取料";
                                break;
                            case 7:
                                _stepName = "設定手臂放料順序";
                                break;
                            case 8:
                                _stepName = "判斷換Tray是否完成";
                                break;
                            case 9:
                                _stepName = "手臂放料台車上";
                                break;
                            case 10:
                                _stepName = "判斷NG是否超過上限";
                                break;
                            case 11:
                                _stepName = "發送SFIS資料";
                                break;
                            case 12:
                                _stepName = "判斷出料平台是否要換Tray";
                                break;
                            case 13:
                                _stepName = "完成流程";
                                break;
                                
                        }
                        break;
                    
                    case 21:
                        _stepName = "動作完成";
                        break;
                    case 2:
                    case 3:
                    case 4:
                    case 6:
                    case 12:
                    case 16:
                    case 17:
                    case 18:
                    case 19:
                        _stepName = "完成上一站流程，等待下一站開始";
                        break;
                }
                return _stepName;
            }
            set => _stepName = value;
        }
        /// <summary>
        /// 編號
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 手動設定NG站別
        /// </summary>
        public List<int> NGPosition { get; set; } = new List<int>();
        /// <summary>
        /// 手動設定NG
        /// </summary>
        public bool ManualNG { get; set; } = false;
        /// <summary>
        /// 設定NG代碼
        /// </summary>
        public List<string> DefectCode { get; set; } = new List<string>();
        /// <summary>
        /// 量測結果
        /// </summary>
        public List<float> EstResult { get; set; } = new List<float>();
        /// <summary>
        /// 量測高度(顯示)
        /// </summary>
        public string EstResultString { get => EstResult.Aggregate("", (total, next) => total += total.Length == 0 ? $"{next}" : $",{next}"); }
        /// <summary>
        /// 基準面位置公式
        /// </summary>
        public string BasePosFunc { get; set; } = "";
        /// <summary>
        /// 平面參數
        /// </summary>
        public List<float> BaseParam { get; set; } = new List<float>();
        /// <summary>
        /// 平面參數(顯示)
        /// </summary>
        public string BaseParamString { get => BaseParam.Aggregate("", (total, next) => total += total.Length == 0 ? $"{next}" : $",{next}"); }
        /// <summary>
        /// 平面距離
        /// </summary>
        public List<float> PlaneDist { get; set; } = new List<float>();
        /// <summary>
        /// 平面距離(顯示)
        /// </summary>
        public string PlaneDistString { get => PlaneDist.Aggregate("", (total, next) => total += total.Length == 0 ? $"{next}" : $",{next}"); }
        /// <summary>
        /// 平整度
        /// </summary>
        public List<float> Flatness { get; set; } = new List<float>();
        /// <summary>
        /// 平整度(顯示)
        /// </summary>
        public string FlatnessString { get => Flatness.Aggregate("", (total, next) => total += total.Length == 0 ? $"{next}" : $",{next}"); }
        /// <summary>
        /// 量測NG
        /// </summary>
        public bool MeasureNG { get; set; } = false;
        /// <summary>
        /// 掃碼結果1NG
        /// </summary>
        public bool Reader1NG { get; set; } = false;
        /// <summary>
        /// 掃碼結果2NG
        /// </summary>
        public bool Reader2NG { get; set; } = false;
        /// <summary>
        /// 掃碼結果1
        /// </summary>
        public string ReaderResult1 { get; set; }
        /// <summary>
        /// 掃碼結果2
        /// </summary>
        public string ReaderResult2 { get; set; }
        /// <summary>
        /// 雷雕等級
        /// </summary>
        public string MarkingLevel { get; set; }
        /// <summary>
        /// 入料時間
        /// </summary>
        public Stopwatch CaseInTime { get; set; } = new Stopwatch();
        /// <summary>
        /// 組裝上蓋時間
        /// </summary>
        public Stopwatch CaseAssembleTime { get; set; } = new Stopwatch();
        /// <summary>
        /// 掃碼時間
        /// </summary>
        public Stopwatch CaseReaderTime { get; set; } = new Stopwatch();

        /// <summary>
        /// 螺帽放置時間
        /// </summary>
        public Stopwatch CasePutNutTime { get; set; } = new Stopwatch();
        /// <summary>
        /// 折彎時間
        /// </summary>
        public Stopwatch CaseBendTime { get; set; } = new Stopwatch();
        /// <summary>
        /// 下壓時間
        /// </summary>
        public Stopwatch CasePlateTime { get; set; } = new Stopwatch();
        /// <summary>
        /// 測高時間
        /// </summary>
        public Stopwatch CaseEstHeiTime { get; set; } = new Stopwatch();
        /// <summary>
        /// NG出料時間
        /// </summary>
        public Stopwatch CaseNgTime { get; set; } = new Stopwatch();
        /// <summary>
        /// 雷雕時間
        /// </summary>
        public Stopwatch CaseMarkingTime { get; set; } = new Stopwatch();
        /// <summary>
        /// 出料時間
        /// </summary>
        public Stopwatch CaseOutTime { get; set; } = new Stopwatch();
        /// <summary>
        /// 單顆總耗時
        /// </summary>
        public Stopwatch CaseTotalTime { get; set; } = new Stopwatch();
        public double CTTime { get; set; } = 0;
        #endregion 屬性
    }
}
