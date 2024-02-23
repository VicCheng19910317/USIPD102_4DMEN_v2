using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class KeyencePLCProcessor_Height : KeyencePLCProcessor
    {
        #region 屬性參數
        internal int PosX { get; set; } = 0;
        internal int PosY { get; set; } = 0;
        #endregion 屬性參數
        #region 實作功能
        /// <summary>
        /// 測高位置設定
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool SetPos()
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info("PLC測高位置設定");
            string send_data = $"WRS DM150 4 {PosX % 65535} {PosX / 65535} {PosY % 65535} {PosY / 65535}";
            string status = plcNet.WriteData(send_data).Replace("E", "").Replace("\r\n", "");
            if (!status.Contains(""))
            {
                Message = $"發送回傳訊號比對錯誤，發送訊號:{send_data}，比對訊號:OK";
                logger = LoggerData.Error(new Exception("錯誤"), $"發送回傳訊號比對錯誤，發送訊號:{send_data}，比對訊號:");
                return false;
            }
            logger = LoggerData.Info("PLC測高位置設定完成");
            return true;
        }
        /// <summary>
        /// 移動至測高位置
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal virtual bool RunMovePos()
        {
            if (SetPos())
            {
                var logger = MainPresenter.LogDatas();
                logger = LoggerData.Info("PLC移動至測高位置");
                var result = RunOneStepFlow();
                logger = LoggerData.Info("PLC移動至測高位置完成");
                return result;
            }
            else return false;
        }
        /// <summary>
        /// 取得測高量測值
        /// </summary>
        /// <returns></returns>
        public virtual List<float> GetHeightVal()
        {
            List<float> result = new List<float>();
            var processor = MainPresenter.LKProcessor();
            var pos_list = MainPresenter.SystemParam().MeasurePosition;
            pos_list.ForEach(x =>
            {
                PosX = x.X;
                PosY = x.Y;
                var success = RunMovePos();
                result.Add(success ? processor.GetEstimateHieghtValue(1) : float.NaN);
            });
            return result;
        }
        public virtual bool GetHeightVal(out List<float> HeightVal)
        {
            HeightVal = new List<float>();
            var result = true;
            var processor = MainPresenter.LKProcessor();
            var pos_list = MainPresenter.SystemParam().MeasurePosition;
            List<float> tmp = new List<float>();
            pos_list.ForEach(x =>
            {
                PosX = x.X;
                PosY = x.Y;
                var success = RunMovePos();
                tmp.Add(success ? processor.GetEstimateHieghtValue(1) : float.NaN);
            });
            HeightVal = tmp;
            if (HeightVal.Contains(float.NaN)) result = false;
            return result;
        }
        #endregion 實作功能
    }
}
