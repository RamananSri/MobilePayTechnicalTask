using Autofac;

namespace ConsoleApplication.DI
{
    public static class ContainerConfig
    {
        public static IContainer Container { get; private set; }

        public static void Register()
        {
            var containerBuilder = new ContainerBuilder();

            //Registrations

            Container = containerBuilder.Build();
        }
    }
}
