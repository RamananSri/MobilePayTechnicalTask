using System;
using System.Threading;
using LogTest;

namespace ConsoleApplication
{
    public class ProducerWithFlush : IProducer
    {
        private readonly ILogger logger;

        public ProducerWithFlush(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Produce()
        {
            for (int i = 0; i < 15; i++)
            {
                logger.WriteToLog("Number with Flush: " + i);
                Thread.Sleep(2);
            }

            logger.StopWithFlush();
        }
    }
}
