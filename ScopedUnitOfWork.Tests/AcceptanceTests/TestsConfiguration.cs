using Autofac;
using ScopedUnitOfWork.Tests.AcceptanceTests.SampleApplication.Infrastructure;

namespace ScopedUnitOfWork.Tests.AcceptanceTests
{
    public class TestsConfiguration
    {
        public static IContainer Container { get; private set; }

        public static void CreateApplication()
        {
            Container = new ContainerSetup().Setup();

            // always make sure we have a fresh database
            using (var context = Container.Resolve<SampleContext>())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }
    }
}
