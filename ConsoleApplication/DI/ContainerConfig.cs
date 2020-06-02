using System;
using System.Reflection;
using Autofac;
using LogComponent.Models;
using LogTest;
using Microsoft.Extensions.Configuration;

namespace ConsoleApplication.DI
{
    public static class ContainerConfig
    {
        public static IContainer CreateContainer()
        {
            var containerBuilder = new ContainerBuilder();
            var configuration = GetConfiguration();
            
            RegisterDependencies(containerBuilder, configuration);

            return containerBuilder.Build();
        }

        private static IConfigurationRoot GetConfiguration()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", true, true);

            return configurationBuilder.Build();
        }

        private static void RegisterDependencies(ContainerBuilder containerBuilder, IConfigurationRoot configuration)
        {
            string logFilepath = configuration["Paths:Log"];

            containerBuilder.Register(config => new LoggerConfiguration {Filepath = logFilepath});

            containerBuilder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(AsyncLogger)))
                .Where(type => type.Name.EndsWith("Logger"))
                .InstancePerDependency()
                .AsImplementedInterfaces();
        }
    }
}
