﻿using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal static class RecordData
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        internal static void RecordNeedleData(string folder, string times, DateTime time, NeedleAlignAxis val, bool is_finish)
        {
            Directory.CreateDirectory($"{folder}/{time.ToString("yyyyMM")}");
            var file_name = $"{folder}/{time.ToString("yyyyMM")}/{time.ToString("yyyyMMdd")}.csv";
            if (!File.Exists(file_name))
            {
                using (StreamWriter sw = new StreamWriter(new FileStream(file_name, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8))
                {
                    sw.WriteLine($"校正次數,校正時間,校正數值X,校正數值Y,校正數值Z,是否完成校正");
                }
            }
                File.WriteAllText(file_name, $"校正次數,校正時間,校正數值X,校正數值Y,校正數值Z,是否完成校正\n");
            using (StreamWriter sw = new StreamWriter(new FileStream(file_name, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
            {
                sw.WriteLine($"{times},{time.ToString("HH: mm:ss.ffff")},{val.X},{val.Y},{val.Z},{is_finish}");
            }
        }
        internal static void RecordProcessData(SystemParam param, string data, string folder = @"C:/4DMEN/ProcessData")
        {
            try
            {
                var logger = MainPresenter.LogDatas();
                logger = data.Contains("錯誤") ? LoggerData.Error(new Exception("錯誤"), data) : LoggerData.Info(data);
                var date_time = DateTime.Now;
                Directory.CreateDirectory($"{folder}/{date_time.ToString("yyyyMM")}/{date_time.ToString("yyyyMMdd")}");
                var file_name = $"{folder}/{date_time.ToString("yyyyMM")}/{date_time.ToString("yyyyMMdd")}/{param.Sfis.TicketID}.csv";


                if (!File.Exists(file_name))
                {
                    using (StreamWriter sw = new StreamWriter(new FileStream(file_name, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8))
                    {
                        sw.WriteLine($"站別：{param.Sfis.StationID}");
                        sw.WriteLine($"線別：{param.Sfis.LineID}");
                        sw.WriteLine($"Lid編號：{param.Sfis.LidLotID}");
                        sw.WriteLine($"Nut編號：{param.Sfis.NutNo}");
                        sw.WriteLine($"工單：{param.Sfis.TicketID}");
                        sw.WriteLine($"操作員：{param.Sfis.WorkerID}");
                        sw.WriteLine($"=============================");
                    }
                }
                using (StreamWriter sw = new StreamWriter(new FileStream(file_name, FileMode.Append, FileAccess.Write,FileShare.ReadWrite), Encoding.UTF8))
                {
                    sw.WriteLine($"{date_time.ToString("HH:mm:ss.ffff")},{data}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "儲存資料錯誤。");
            }

        }
        internal static void RecordSFISData(SystemParam param, string data, string folder = @"C:/4DMEN/SFISData")
        {
            try
            {
                var logger = MainPresenter.LogDatas();
                var date_time = DateTime.Now;
                Directory.CreateDirectory($"{folder}/{date_time.ToString("yyyyMM")}/{date_time.ToString("yyyyMMdd")}");
                var file_name = $"{folder}/{date_time.ToString("yyyyMM")}/{date_time.ToString("yyyyMMdd")}/{param.Sfis.TicketID}.csv";


                if (!File.Exists(file_name))
                {
                    using (StreamWriter sw = new StreamWriter(new FileStream(file_name, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8))
                    {
                        sw.WriteLine($"站別：{param.Sfis.StationID}");
                        sw.WriteLine($"線別：{param.Sfis.LineID}");
                        sw.WriteLine($"Lid編號：{param.Sfis.LidLotID}");
                        sw.WriteLine($"Nut編號：{param.Sfis.NutNo}");
                        sw.WriteLine($"工單：{param.Sfis.TicketID}");
                        sw.WriteLine($"操作員：{param.Sfis.WorkerID}");
                        sw.WriteLine($"=============================");
                    }
                }
                using (StreamWriter sw = new StreamWriter(new FileStream(file_name, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8))
                {
                    sw.WriteLine($"{date_time.ToString("HH:mm:ss.ffff")},{data}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "儲存資料錯誤。");
            }

        }
        internal static void RecordDataResult(SystemParam param, List<CaseData> datas, string folder = @"C:/4DMEN/DataResult")
        {
            var date_time = CaseAllTask.GetEntity().run_record_time;
            Directory.CreateDirectory($"{folder}/{date_time.ToString("yyyyMM")}/{date_time.ToString("yyyyMMdd")}");
            var file_name = $"{folder}/{date_time.ToString("yyyyMM")}/{date_time.ToString("yyyyMMdd")}/{param.Sfis.TicketID}_{date_time:HHmmss}.csv";

            if (File.Exists(file_name)) File.Delete(file_name);

            if (!File.Exists(file_name))
            {
                using (StreamWriter sw = new StreamWriter(new FileStream(file_name, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8))
                {
                    sw.WriteLine($"站別：{param.Sfis.StationID}");
                    sw.WriteLine($"線別：{param.Sfis.LineID}");
                    sw.WriteLine($"Lid編號：{param.Sfis.LidLotID}");
                    sw.WriteLine($"Nut編號：{param.Sfis.NutNo}");
                    sw.WriteLine($"工單：{param.Sfis.TicketID}");
                    sw.WriteLine($"操作員：{param.Sfis.WorkerID}");
                    sw.WriteLine($"==============================================");
                    sw.WriteLine($"時間,編號,掃描等級,量測結果,基準面位置公式,平面距離,平整度,條碼結果1,條碼結果2,NG位置,入料站耗時,組裝站耗時,掃碼站耗時,螺帽站耗時,折彎站耗時,下壓站耗時,測高站耗時,NG站耗時,雷雕站耗時,出料站耗時,CT時間,總耗時");
                }
            }
            using (StreamWriter sw = new StreamWriter(new FileStream(file_name, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8))
            {
                datas.ForEach(x =>
                {
                    sw.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.ffff")},{x.Index},{x.MarkingLevel},{x.EstResult.Aggregate("", (total, next) => total += total.Length == 0 ? $"{next}" : $":{next}")},{x.BasePosFunc},{x.PlaneDist.Aggregate("",(total,next) => total += total.Length == 0 ? $"{next}" : $":{next}")},{x.Flatness.Aggregate("", (total, next) => total += total.Length == 0 ? $"{next}" : $":{next}")},{x.ReaderResult1},{x.ReaderResult2},{x.NGPosition.Aggregate("",(total,next)=> total += total.Length == 0 ? $"{next}" : $":{next}")},{x.CaseInTime.Elapsed.TotalSeconds},{x.CaseAssembleTime.Elapsed.TotalSeconds},{x.CaseReaderTime.Elapsed.TotalSeconds},{x.CasePutNutTime.Elapsed.TotalSeconds},{x.CaseBendTime.Elapsed.TotalSeconds},{x.CasePlateTime.Elapsed.TotalSeconds},{x.CaseEstHeiTime.Elapsed.TotalSeconds},{x.CaseNgTime.Elapsed.TotalSeconds},{x.CaseMarkingTime.Elapsed.TotalSeconds},{x.CaseOutTime.Elapsed.TotalSeconds},{x.CTTime},{x.CaseTotalTime.Elapsed.TotalSeconds}");
                });
            }

        }
        
    }
}
