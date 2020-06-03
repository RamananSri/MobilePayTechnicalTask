using System;
using System.Collections;
using LogComponent.Models;
using LogTest;
using Moq;
using NUnit.Framework;

namespace LogComponent.UnitTests
{
    [TestFixture]
    public class AsyncFileLoggerTests : LoggerTests<AsyncFileLogger>
    {
        private readonly Mock<IDateTimeProvider> _datetimeProviderMock = new Mock<IDateTimeProvider>();

        protected override AsyncFileLogger CreateLogger()
        {
            var loggerConfig = new LoggerConfiguration
            {
                Filepath = DirectoryInformation.FullName,
            };

            return new AsyncFileLogger(loggerConfig, _datetimeProviderMock.Object);
        }

        private static IEnumerable DateValuesData()
        {
            yield return new TestCaseData(
                new DateTime(2020, 01, 01,23,00,00), 
                new DateTime(2020,01,02,01,00,00)).Returns(2);
            yield return new TestCaseData(
                new DateTime(2020, 12, 12,01,01,01),
                new DateTime(2020, 12,12,01,02,00)).Returns(1);
        }

        [TestCaseSource(typeof(AsyncFileLoggerTests), nameof(DateValuesData))]
        public override int WriteToLog_PassingMidnight_ReturnsExpectedAmountOfLogFiles(DateTime first, DateTime second)
        {
            _datetimeProviderMock.SetupSequence(e => e.Now)
                .Returns(first)
                .Returns(second);

            var logger = CreateLogger();

            logger.WriteToLog("HelloWorld");
            logger.WriteToLog("HelloWorld");

            logger.StopWithFlush();

            return DirectoryInformation.GetFiles().Length;
        }
    }
}
