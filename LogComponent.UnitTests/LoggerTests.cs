using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogTest;
using NUnit.Framework;

namespace LogComponent.UnitTests
{
    [TestFixture]
    public abstract class LoggerTests<TLogger> where TLogger : ILogger
    {
        protected abstract TLogger CreateLogger();

        protected DirectoryInfo DirectoryInformation;

        [SetUp]
        public void Setup()
        {
            DirectoryInformation = new DirectoryInfo("TestLogs");
        }

        [TearDown]
        public void TearDown()
        {
            DirectoryInformation.Delete(true);
        }

        [TestCase("HelloWorld")]
        public virtual void WriteToLog_WriteMessageToFile_LogContainsMessage(string message)
        {
            TLogger logger = CreateLogger();

            logger.WriteToLog(message);
            logger.StopWithFlush();
            
            var log = ReadLogLines().ToList();
            Assert.That(log.Any(e => e.Contains(message)));
        }

        [Test]
        public abstract int WriteToLog_PassingMidnight_ReturnsExpectedAmountOfLogFiles(DateTime first, DateTime second);

        [Test]
        public virtual void StopWithFlush_OutstandingLogEntries_Returns()
        {
            var iterations = 500;
            var logger = CreateLogger();

            for (int i = 0; i < iterations; i++)
            {
                logger.WriteToLog($"Writing {i}");
            }

            logger.StopWithFlush();

            var log = ReadLogLines().ToList().Count;

            Assert.That(log == iterations+1);
        }

        [Test]
        public virtual void StopWithoutFlush_WithOutstandingLogentries_Returns()
        {
            var iterations = 500;
            var logger = CreateLogger();

            for (int i = 0; i < iterations; i++)
            {
                logger.WriteToLog($"Writing {i}");
            }

            logger.StopWithoutFlush();

            var log = ReadLogLines();

            Assert.That(log.Count() != iterations+1);
        }

        protected IEnumerable<string> ReadLogLines()
        {
            var logFilename = DirectoryInformation.GetFiles();
            return File.ReadLines(logFilename.First().FullName);
        }
    }
}
