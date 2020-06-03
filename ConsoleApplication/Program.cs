using System;
using System.Diagnostics;
using Autofac;
using ConsoleApplication.DI;
using LogTest;
using System.Threading;

namespace LogUsers
{
    class Program
    {
        static void Main(string[] args)
        {
            IContainer container = ContainerConfig.CreateContainer();

            var watch = Stopwatch.StartNew();

            var logger = container.Resolve<ILogger>();

            for (int i = 0; i < 15; i++)
            {
                logger.WriteToLog("Number with Flush: " + i);
                Thread.Sleep(50);
            }

            logger.StopWithFlush();

            var logger2 = container.Resolve<ILogger>();

            for (int i = 50; i > 0; i--)
            {
                logger2.WriteToLog("Number with No flush: " + i);
                Thread.Sleep(20);
            }

            logger2.StopWithoutFlush();

            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
            
            Console.ReadLine();
        }
    }
}
