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
                    _stationName = "入料->塗膠(2)";
                else if (Station == 3)
                    _stationName = "下壓站";
                else if (Station == 4)
                    _stationName = "塗膠站";
                else if (Station == 5)
                    _stationName = "塗膠->秤重(5)";
                else if (Station == 6)
                    _stationName = "秤重站";
                else if (Station == 7)
                    _stationName = "檢測站";
                else if (Station == 8)
                    _stationName = "NG出料站";
                else if (Station == 9)
                    _stationName = "NG->翻轉(9)";
                else if (Station == 10)
                    _stationName = "翻轉站";
                else if (Station == 11)
                    _stationName = "出料站";
                else if (Station == 12)
                    _stationName = "出料完成";
                return _stationName;
            }
            set => _stationName = value;
        }
        public int Step { get; set; }
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
                                _stepName = "手臂取料置秤重站";
                                break;
                            case 4:
                                _stepName = "讀取秤重資料";
                                break;
                            case 5:
                                _stepName = "手臂取料置台車上";
                                break;
                            case 6:
                                _stepName = "判斷入料平台是否要換Tray";
                                break;
                            case 7:
                                _stepName = "完成流程";
                                break;
                        }
                        break;
                    case 3:
                        switch (Step)
                        {
                            case 0:
                                _stepName = "等待流程開始";
                                break;
                            case 1:
                                _stepName = "執行下壓動作";
                                break;
                            case 2:
                                _stepName = "檢查下壓是否完成";
                                break;
                            case 3:
                                _stepName = "下壓回原點";
                                break;
                            case 4:
                                _stepName = "檢查下壓回原點是否完成";
                                break;
                            case 5:
                                _stepName = "完成流程";
                                break;
                        }
                        break;

                    case 4:
                        switch (Step)
                        {
                            case 0:
                                _stepName = "等待流程開始";
                                break;
                            case 1:
                                _stepName = "開始進行塗膠";
                                break;
                            case 2:
                                _stepName = "完成流程";
                                break;
                        }
                        break;
                    case 6:
                        switch (Step)
                        {
                            case 0:
                                _stepName = "等待流程開始";
                                break;
                            case 1:
                                _stepName = "取料到QR Code讀碼器上";
                                break;
                            case 2:
                                _stepName = "讀碼器讀碼";
                                break;
                            case 3:
                                _stepName = "取料到秤重機上";
                                break;
                            case 4:
                                _stepName = "讀取秤重資料";
                                break;
                            case 5:
                                _stepName = "發送SFIS資料";
                                break;
                            case 6:
                                _stepName = "取料至台車上";
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
                                _stepName = "設定檢測模式";
                                break;
                            case 2:
                                _stepName = "進行檢測";
                                break;
                            case 3:
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
                                _stepName = "檢查重量";
                                break;
                            case 2:
                                _stepName = "檢查檢測結果";
                                break;
                            case 3:
                                _stepName = "NG出料";
                                break;
                            case 4:
                                _stepName = "完成流程";
                                break;
                        }
                        break;
                    case 10:
                        switch (Step)
                        {
                            case 0:
                                _stepName = "等待流程開始";
                                break;
                            case 1:
                                _stepName = "PLC翻料";
                                break;
                            case 2:
                                _stepName = "是否需要更換料槽";
                                break;
                            case 3:
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
                                _stepName = "確認翻轉完成";
                                break;
                            case 2:
                                _stepName = "手臂取料";
                                break;
                            case 3:
                                _stepName = "手臂組裝並取置掃碼器上";
                                break;
                            case 4:
                                _stepName = "掃碼器掃描條碼";
                                break;
                            case 5:
                                _stepName = "發送SFIS資料";
                                break;
                            case 6:
                                _stepName = "設定放料位置";
                                break;
                            case 7:
                                _stepName = "判斷換Tray是否完成";
                                break;
                            case 8:
                                _stepName = "執行放料至Case上";
                                break;
                            case 9:
                                _stepName = "判斷出料平台是否要換Tray";
                                break;
                            case 10:
                                _stepName = "完成流程";
                                break;
                        }
                        break;
                    case 12:
                        _stepName = "動作完成";
                        break;
                    case 2:
                    case 5:
                    case 9:
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
        public int ManualNGPosition { get; set; } = 0;
        /// <summary>
        /// 手動設定NG
        /// </summary>
        public bool ManualNG { get; set; } = false;
        /// <summary>
        /// 量測結果
        /// </summary>
        public List<float> EstResult { get; set; } = new List<float>();
        /// <summary>
        /// 基準面位置
        /// </summary>
        public string BasePost { get; set; } = "";
        /// <summary>
        /// 平面參數
        /// </summary>
        public List<float> BaseParam { get; set; } = new List<float>();
        /// <summary>
        /// 平面距離
        /// </summary>
        public List<float> PlaneDist { get; set; } = new List<float>();
        /// <summary>
        /// 平整度
        /// </summary>
        public List<float> Flatness { get; set; } = new List<float>();
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
        /// NG初料時間
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
