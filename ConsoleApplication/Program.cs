using System;
using System.Diagnostics;
using LogTest;
using System.Threading;
using Autofac;
using ConsoleApplication.DI;
using System.Collections;
using System.Collections.Generic;
using ConsoleApplication;

namespace LogUsers
{
    class Program
    {
        static void Main(string[] args)
        {
            IContainer container = ContainerConfig.CreateContainer();
            IEnumerable<IProducer> producers = container.Resolve<IEnumerable<IProducer>>();

            var watch = Stopwatch.StartNew();

            foreach (var producer in producers)
            {
                producer.Produce();
            }            

            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
            
            Console.ReadLine();
        }
    }
}
