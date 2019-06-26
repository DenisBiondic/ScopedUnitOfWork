using Autofac;
using ScopedUnitOfWork.Interfaces;

namespace ScopedUnitOfWork.Tests.AcceptanceTests
{
    public abstract class AcceptanceTestBase
    {
        protected abstract IContainer Container { get; }

        protected IUnitOfWorkFactory GetFactory()
        {
            return Container.Resolve<IUnitOfWorkFactory>();
        }
    }
}