using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4DMEN_Library.Model
{
    internal class LoggerData
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        internal static List<LogData> LoadLogData(string init_folder_path = @"C:/ProgramData/4DMEN/Logs")
        {
            var result = new List<LogData>();
            var folder_path1 = $"{init_folder_path}/{DateTime.Now.Year}-{(DateTime.Now.Month - 1).ToString().PadLeft(2, '0')}";
            var folder_path2 = $"{init_folder_path}/{DateTime.Now.Year}-{DateTime.Now.Month.ToString().PadLeft(2, '0')}";
            if (Directory.Exists(folder_path1))
            {
                foreach (var file_name in Directory.GetFiles(folder_path1))
                {
                    using (StreamReader sr = new StreamReader(new FileStream(file_name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    {
                        LogData _data = null;
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            var line_seprate = line.Split('|');
                            if (line_seprate.Length > 1)
                            {
                                if (_data != null)
                                    result.Add(_data);
                                if (line_seprate.Length == 4)
                                    _data = new LogData
                                    {
                                        DateTime = line_seprate[0],
                                        LogLevel = line_seprate[1],
                                        LogClass = line_seprate[2],
                                        LogMessage = line_seprate[3]
                                    };
                                else if (line_seprate.Length == 5)
                                    _data = new LogData
                                    {
                                        DateTime = line_seprate[0],
                                        LogLevel = line_seprate[1],
                                        LogClass = line_seprate[2],
                                        LogMessage = line_seprate[3],
                                        LogException = line_seprate[4]
                                    };
                            }
                            else
                            {
                                if (_data != null) _data.LogException += line;
                            }
                        }
                        result.Add(_data);
                    }
                }
            }
            if (Directory.Exists(folder_path2))
            {
                foreach (var file_name in Directory.GetFiles(folder_path2))
                {
                    using (StreamReader sr = new StreamReader(new FileStream(file_name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    {
                        LogData _data = null;
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            var line_seprate = line.Split('|');
                            if (line_seprate.Length > 1)
                            {
                                if (_data != null)
                                    result.Add(_data);
                                if (line_seprate.Length == 4)
                                    _data = new LogData
                                    {
                                        DateTime = line_seprate[0],
                                        LogLevel = line_seprate[1],
                                        LogClass = line_seprate[2],
                                        LogMessage = line_seprate[3]
                                    };
                                else if (line_seprate.Length == 5)
                                    _data = new LogData
                                    {
                                        DateTime = line_seprate[0],
                                        LogLevel = line_seprate[1],
                                        LogClass = line_seprate[2],
                                        LogMessage = line_seprate[3],
                                        LogException = line_seprate[4]
                                    };
                            }
                            else
                            {
                                if (_data != null) _data.LogException += line;
                            }
                        }
                        result.Add(_data);
                    }
                }
            }
            return result;
        }
        internal static List<LogData> Info(string message)
        {
            var _datas = MainPresenter.LogDatas();
            _datas.Add(new LogData { DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), LogLevel = "Info", LogMessage = message });
            logger.Info(message);
            return _datas;
        }
        internal static List<LogData> Error(Exception ex, string message)
        {
            var _datas = MainPresenter.LogDatas();
            _datas.Add(new LogData { DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), LogLevel = "Error", LogMessage = message, LogException = ex.Message });
            logger.Error(ex, message);
            return _datas;
        }
        internal static List<LogData> Fatal(Exception ex, string message)
        {
            var _datas = MainPresenter.LogDatas();
            _datas.Add(new LogData { DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), LogLevel = "Fatal", LogMessage = message, LogException = ex.Message });
            logger.Fatal(ex, message);
            return _datas;
        }
    }
}
