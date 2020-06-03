using System;
using System.Threading;
using LogTest;

namespace ConsoleApplication
{
    public class ProducerWithoutFlush : IProducer
    {
        private readonly ILogger logger;

        public ProducerWithoutFlush(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Produce()
        {
            for (int i = 50; i > 0; i--)
            {
                logger.WriteToLog("Number with No flush: " + i);
                Thread.Sleep(2);
            }

            logger.StopWithoutFlush();
        }
    }
}
