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
        private BlockingCollection<FileLogLine> _loqQueue;
        private DateTime _latestLogTimestamp;

        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly string _logPath;
        private readonly CancellationTokenSource cancellationTokenSource;

        public AsyncFileLogger(LoggerConfiguration loggerConfiguration, IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _logPath = loggerConfiguration.Filepath ?? throw new ArgumentNullException(nameof(loggerConfiguration.Filepath));

            _loqQueue = new BlockingCollection<FileLogLine>();
            cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => StartLogging(cancellationTokenSource.Token));
        }

        public void StopWithoutFlush()
        {
            cancellationTokenSource.Cancel();
        }

        public void StopWithFlush()
        {
            _loqQueue.CompleteAdding();
        }

        public void WriteToLog(string s)
        {
            var newLine = new FileLogLine(s, _dateTimeProvider.Now);
            _loqQueue.Add(newLine);
        }

        private void StartLogging(CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(_logPath);

            _latestLogTimestamp = _dateTimeProvider.Now;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    FileLogLine line = _loqQueue.Take();
                    Log(line);

                }
                catch (TaskCanceledException ex)
                {
                    break;
                }
                catch (Exception e) //Reset the log queue to keep going
                {
                    _loqQueue = new BlockingCollection<FileLogLine>();
                    continue;
                }
                finally
                {
                    cancellationTokenSource.Dispose();
                }
            }

            Thread.Sleep(50);
        }

        private void Log(FileLogLine line)
        {
            if (IsNewDay(_latestLogTimestamp))
            {
                _latestLogTimestamp = _dateTimeProvider.Now;
                WriteLogLine(line, true);
            }
            else
            {
                WriteLogLine(line);
            }
        }

        private void WriteLogLine(FileLogLine line, bool includeHeader = false)
        {
            var filepath = GetLogFilepath(_latestLogTimestamp);

            using (StreamWriter streamWriter = new StreamWriter(filepath, true, Encoding.UTF8, 65536))
            {
                if (includeHeader)
                {
                    streamWriter.WriteLine(FileLogLine.GetFormattedFileHeader());
                }

                streamWriter.WriteLine(line.GetLineText());
            }
        }

        private string GetLogFilepath(DateTime timestamp)
        {
            return Path.Combine(_logPath, $@"Log{timestamp:yyyyMMdd HHmmss fff}.log");
        }

        private bool IsNewDay(DateTime latestLog)
        {
            return latestLog.Date == _dateTimeProvider.Now.Date;
        }
    }
}