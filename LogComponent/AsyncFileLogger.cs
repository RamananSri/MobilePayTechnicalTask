using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogComponent;
using LogComponent.Models;

namespace LogTest
{
    public class AsyncFileLogger : ILogger
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly string _logPath;
        private readonly ConcurrentQueue<FileLogLine> _lines; 
        
        private bool _exit;
        private bool _quitWithFlush;

        public AsyncFileLogger(LoggerConfiguration loggerConfiguration, IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _logPath = loggerConfiguration.Filepath ?? throw new ArgumentNullException(nameof(loggerConfiguration.Filepath));

            _lines = new ConcurrentQueue<FileLogLine>();

            Task.Run(MainLoop);
        }

        public void StopWithoutFlush()
        {
            _exit = true;
        }

        public void StopWithFlush()
        {
            _quitWithFlush = true;
        }

        public void WriteToLog(string s)
        {
            Console.WriteLine(new FileLogLine(s, _dateTimeProvider.Now).GetLineText());
            _lines.Enqueue(new FileLogLine(s, _dateTimeProvider.Now));
        }

        private void MainLoop()
        {
            Directory.CreateDirectory(_logPath);

            var latestLogFileTimestamp = _dateTimeProvider.Now;
            var isNewLog = true;

            while (!_exit)
            {
                if (_lines.Count > 0)
                {
                    using (StreamWriter streamWriter = new StreamWriter(GetLogFilepath(latestLogFileTimestamp),true, Encoding.UTF8, 65536))
                    {
                        if (isNewLog)
                        {
                            streamWriter.WriteLine(FileLogLine.GetFormattedFileHeader());
                            isNewLog = false;
                        } 

                        foreach (FileLogLine logLine in _lines)
                        {
                            if (!_exit || _quitWithFlush)
                            {


                                //if ((DateTime.Now - _curDate).Days != 0)
                                //{
                                //    _curDate = DateTime.Now;

                                //    _writer = File.AppendText(@"C:\LogTest\Log" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + ".log");

                                //    _writer.Write("Timestamp".PadRight(25, ' ') + "\t" + "Data".PadRight(15, ' ') + "\t" + Environment.NewLine);

                                //    stringBuilder.Append(Environment.NewLine);

                                //    _writer.Write(stringBuilder.ToString());

                                //    _writer.AutoFlush = true;
                                //}

                                streamWriter.WriteLine(logLine.GetLineText());

                                if (!_lines.TryDequeue(out _))
                                {
                                    throw new ArgumentException("Failed to dequeue queue");
                                }
                            }
                        }
                    }

                    if (_quitWithFlush && _lines.Count == 0)
                    {
                        _exit = true;
                    }
                        
                    Thread.Sleep(50);
                }
            }
        }

        //private void CreateNewLog()
        //{
        //    _writer = File.AppendText($@"{_logPath}\Log{_dateTimeProvider.Now:yyyyMMdd HHmmss fff}.log");
        //    _writer.WriteLineAsync(FileLogLine.GetFormattedFileHeader());
        //    _writer.AutoFlush = true;
        //}

        private string GetLogFilepath(DateTime timestamp)
        {
            return Path.Combine(_logPath, $@"Log{timestamp:yyyyMMdd HHmmss fff}.log");
        }

    }
}