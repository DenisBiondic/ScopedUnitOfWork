using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Autofac.Features.ResolveAnything;
using CommonServiceLocator;
using ScopedUnitOfWork.EntityFrameworkCore;
using ScopedUnitOfWork.Interfaces;
using ScopedUnitOfWork.Tests.AcceptanceTests.SampleApplication.SimpleDomain;

namespace ScopedUnitOfWork.Tests.AcceptanceTests.SampleApplication.Infrastructure
{
    public class ContainerSetup
    {
        public IContainer Setup()
        {
            var builder = new ContainerBuilder();

            builder.Register(x => new SampleContext())
                .As<SampleContext>()
                .InstancePerDependency();

            builder.RegisterType<UnitOfWorkFactory<SampleContext>>().As<IUnitOfWorkFactory>();
            builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            var container = builder.Build();

            // update the container with "self" registation (so that no static IoC or 
            // similar interfaces are used)
            var serviceLocator = new AutofacServiceLocator(container);
            builder = new ContainerBuilder();
            builder.RegisterInstance(serviceLocator).As<IServiceLocator>();
            builder.Update(container);

            return container;
        }
    }
}
