using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class SfisProcessor : ClientTCP
    {
        #region 參數屬性
        public string Name { get; set; } = "SFIS";
        public string IP { get => base.ip; set => base.ip = value; }
        public int Port { get => port; set => port = value; }
        public bool IsConnected { get => is_connected; set => is_connected = value; }
        public string Message { get => message; set => message = value; }
        public int Timeout { get => timeout; set => timeout = value; }
        public List<string> Action_Name { get; set; }
        #endregion 參數屬性
        #region 虛擬方法
        /// <summary>
        /// 自定義Reader動作
        /// </summary>
        /// <param name="action">輸入動作</param>
        /// <returns>成功/失敗</returns>
        internal virtual bool SendAction(string action)
        {
            var result = false;
            string message = SendMessage(action);
            if (message.ToUpper().Contains("OK"))
            {
                result = true;
            }
            Message = message;
            return result;
        }
        /// <summary>
        /// 自定義Reader動作
        /// </summary>
        /// <param name="action">輸入動作</param>
        /// <param name="match_data">比對結果</param>
        /// <returns>成功/失敗</returns>
        internal virtual bool SendAction(string action, string match_data)
        {
            var result = false;
            string message = SendMessage(action);
            RecordData.RecordSFISData(MainPresenter.SystemParam(), $"SFIS回傳:{message}");
            if (message.Contains("超時") || message.Contains("未連線"))
            {
                Message = $"發送SFIS訊號失敗，失敗訊息：{Message}";
            }
            else if (message.ToUpper().Contains(match_data))
            {
                Message = $"發送SFIS訊號成功";
                result = true;
            }
            else
                Message = $"發送SFIS訊號失敗，失敗訊息：{Message}";
            return result;
        }
        #endregion 虛擬方法
        #region 覆寫方法
        /// <summary>
        /// 覆寫發送方法(取消CrLf功能)
        /// </summary>
        /// <param name="message">訊息內容</param>
        /// <returns>成功/失敗</returns>
        protected override string SendMessage(string message)
        {
            string resMessage = "";
            if (StartConnected())
            {
                NetworkStream networkStream = tcpClient.GetStream();
                byte[] data = Encoding.ASCII.GetBytes($"{message}");
                if (networkStream.WriteAsync(data, 0, data.Length).Wait(timeout))
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = buffer.Length;
                    if (networkStream.ReadAsync(buffer, 0, buffer.Length).Wait(timeout))
                    {
                        resMessage = Encoding.ASCII.GetString(buffer).Replace("\r\n", "").Replace("\0", "");
                    }
                    else
                    {
                        resMessage = "讀取回傳訊號超時";
                    }
                }
                else
                {
                    resMessage = "發送訊號超時";
                }
            }
            else
            {
                resMessage = "系統未連線";
            }
            ConnectedClose();
            return resMessage;
        }
        #endregion 覆寫方法
        #region 實作方法
        internal bool SendStep(int step, SfisParam param, CaseData data)
        {
            var send_data = "";
            var success = false;
           
            switch (step)
            {
                case 1:
                    send_data = $"{param.StationID},{data.ReaderResult1},1,{param.WorkerID},{param.LineID},,OK,,";
                    break;
                case 2:
                    var result = data.DefectCode.Count() > 0 || data.ManualNG ? $"NG" : $"OK";
                    var defect_code = data.DefectCode.Aggregate("", (total, next) => total += total.Length == 0 ? $"{next}" : $"+{next}");
                    var manual_defect = data.ManualNG ? "Manual" : "Auto";
                    var position = data.NGPosition.Count == 0 ? "" : data.NGPosition.Aggregate("", (total, next) => total += total.Length == 0 ? $"{next}" : $"+{next}");
                    defect_code += defect_code.Length == 0 ? $"{manual_defect}" : $"+{manual_defect}";
                    send_data = $"{param.StationID},{data.ReaderResult1},3,{param.WorkerID},{param.LineID},,{result},{defect_code},{position},{data.ReaderResult2},,{data.MarkingLevel},,";
                    break;
                case 3:
                    result = data.DefectCode.Count() > 0 || data.ManualNG ? $"FAIL" : $"OK";
                    defect_code = data.DefectCode.GroupBy(x=>x).Aggregate("", (total, next) => total += total.Length == 0 ? $"{next.Key}" : $"+{next.Key}");
                    manual_defect = data.ManualNG ? "Manual" : "Auto";
                    defect_code += defect_code.Length == 0 ? (data.NGPosition.Count > 0 ? $"{manual_defect}" : $"") : $"+{manual_defect}";
                    position = data.NGPosition.Count == 0 ? "" : data.NGPosition.GroupBy(x=>x).Aggregate("", (total, next) => total += total.Length == 0 ? $"{next.Key}" : $"+{next.Key}");
                    var defect_position = defect_code.Length == 0 && position.Length == 0 ? "" : $"{defect_code}:{position}";

                    var height = $"\"[VR]LH1=\'N/A\'\" \"[VR]LH2=\'N/A\'\" \"[VR]LH3=\'N/A\'\" \"[VR]LHA=\'N/A\'\" \"[VR]MH1=\'N/A\'\" \"[VR]MH2=\'N/A\'\" \"[VR]MH3=\'N/A\'\" \"[VR]MHA=\'N/A\'\" \"[VR]RH1=\'N/A\'\" \"[VR]RH2=\'N/A\'\" \"[VR]RH3=\'N/A\'\" \"[VR]RHA=\'N/A\'\"";
                    if (data.PlaneDist.Count >= 9)
                    {
                        height = "";
                        for (int i = 0; i < 3; i++)
                        {
                            var avg = Math.Round((data.PlaneDist[3 * i + 0] + data.PlaneDist[3 * i + 1] + data.PlaneDist[3 * i + 2]) / 3, 2);
                            var tag = i == 0 ? "L" : i == 1 ? "M" : "R";
                            height += $"\"[VR]{tag}H1=\'{data.PlaneDist[3 * i + 0]} mm\'\" \"[VR]{tag}H2=\'{data.PlaneDist[3 * i + 1]} mm\'\" \"[VR]{tag}H3=\'{data.PlaneDist[3 * i + 2]} mm\'\" \"[VR]{tag}HA=\'{avg} mm\'\"";
                        }
                    }
                    var flatness = $"\"[VR]LF=\'N/A\'\" \"[VR]MF=\'N/A\'\" \"[VR]RF=\'N/A\'\"";
                    if(data.Flatness.Count >= 3)
                        flatness = $"\"[VR]LF=\'{data.Flatness[0]} mm\'\" \"[VR]MF=\'{data.Flatness[1]} mm\'\" \"[VR]RF=\'{data.Flatness[2]} mm\'\"";
                    var _grade = data.MarkingLevel == null ? "N/A" : data.MarkingLevel;
                    var grade = $"\"[VR]Grade=\'{_grade}\'\"";
                    var base_plate_lid_id = $"\"[VR]BASEPLATE LID ID=\'{data.ReaderResult2}\'\"";
                    send_data = $"{param.StationID},{data.ReaderResult1},2,{param.WorkerID},{param.LineID},,{result},,{defect_position},{grade} {height} {flatness} {base_plate_lid_id}";
                    break;
            }
            RecordData.RecordSFISData(MainPresenter.SystemParam(), send_data);
            success = SendAction(send_data, "");
            
            return success;
        }
        #endregion 實作方法
    }
}
