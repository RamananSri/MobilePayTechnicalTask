using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace LogTest
{
    public class AsyncLogger : ILogger
    {
        private Thread _runThread;
        private List<LogLine> _lines = new List<LogLine>();
        private StreamWriter _writer; 
        private bool _exit;
        private bool _quitWithFlush = false;
        private DateTime _curDate = DateTime.Now;

        public AsyncLogger()
        {
            if (!Directory.Exists(@"C:\LogTest")) 
                Directory.CreateDirectory(@"C:\LogTest");

            _writer = File.AppendText(@"C:\LogTest\Log" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + ".log");
            
            _writer.Write("Timestamp".PadRight(25, ' ') + "\t" + "Data".PadRight(15, ' ') + "\t" + Environment.NewLine);

            _writer.AutoFlush = true;

            _runThread = new Thread(MainLoop);
            _runThread.Start();
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
            _lines.Add(new LogLine() { Text = s, Timestamp = DateTime.Now });
        }

        private void MainLoop()
        {
            while (!_exit)
            {
                if (_lines.Count > 0)
                {
                    int f = 0;
                    List<LogLine> _handled = new List<LogLine>();

                    foreach (LogLine logLine in _lines)
                    {
                        f++;

                        if (f > 5)
                            continue;

                        if (!_exit || _quitWithFlush)
                        {
                            _handled.Add(logLine);

                            StringBuilder stringBuilder = new StringBuilder();

                            if ((DateTime.Now - _curDate).Days != 0)
                            {
                                _curDate = DateTime.Now;

                                _writer = File.AppendText(@"C:\LogTest\Log" + DateTime.Now.ToString("yyyyMMdd HHmmss fff") + ".log");

                                _writer.Write("Timestamp".PadRight(25, ' ') + "\t" + "Data".PadRight(15, ' ') + "\t" + Environment.NewLine);

                                stringBuilder.Append(Environment.NewLine);

                                _writer.Write(stringBuilder.ToString());

                                _writer.AutoFlush = true;
                            }

                            stringBuilder.Append(logLine.Timestamp.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                            stringBuilder.Append("\t");
                            stringBuilder.Append(logLine.LineText());
                            stringBuilder.Append("\t");

                            stringBuilder.Append(Environment.NewLine);

                            _writer.Write(stringBuilder.ToString());
                        }
                    }

                    for (int y = 0; y < _handled.Count; y++)
                    {
                        _lines.Remove(_handled[y]);
                    }

                    if (_quitWithFlush == true && _lines.Count == 0)
                        _exit = true;

                    Thread.Sleep(50);
                }
            }
        }

    }
}