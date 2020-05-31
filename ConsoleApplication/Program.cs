using System;
using LogTest;
using System.Threading;

namespace LogUsers
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger  logger = new AsyncLogger();

            for (int i = 0; i < 15; i++)
            {
                logger.WriteToLog("Number with Flush: " + i);
                Thread.Sleep(50);
            }

            logger.StopWithFlush();

            ILogger logger2 = new AsyncLogger();

            for (int i = 50; i > 0; i--)
            {
                logger2.WriteToLog("Number with No flush: " + i);
                Thread.Sleep(20);
            }

            logger2.StopWithoutFlush();

            Console.ReadLine();
        }
    }
}
