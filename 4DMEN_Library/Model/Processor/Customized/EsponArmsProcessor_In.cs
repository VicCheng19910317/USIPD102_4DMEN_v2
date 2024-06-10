using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class EsponArmsProcessor_In : EsponArmsProcessor
    {
        #region 覆寫功能
        /// <summary>
        /// 手臂取料
        /// </summary>
        /// <returns>成功/失敗</returns>
        internal override bool Pick(bool by_pass = false)
        {
            var logger = MainPresenter.LogDatas();
            logger = LoggerData.Info($"手臂 {Name} 手臂取料");
            var _timeout = Timeout;
            _timeout = 10000;
            var success = SendAction($"PICK", $"ONPOSGAUGE");
            if (success)
            {
                var val = MainPresenter.LKProcessor().GetEstimateHieghtValue(0);
                var gauge_result = (val == float.NaN || val < MainPresenter.SystemParam().EstHeighIn.Lower || val > MainPresenter.SystemParam().EstHeighIn.Upper) ? "NG" : "OK";
                if (by_pass) gauge_result = "OK";
                success = SendAction($"GaugeResult;{gauge_result}");
            }
            logger = LoggerData.Info($"手臂 {Name} 手臂取料完成");
            timeout = Timeout;
            return success;
        }
        
        #endregion 覆寫功能
    }
}
