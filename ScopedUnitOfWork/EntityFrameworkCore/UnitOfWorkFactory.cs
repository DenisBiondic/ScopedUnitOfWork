using CommonServiceLocator;
using Microsoft.EntityFrameworkCore;
using ScopedUnitOfWork.Framework;

namespace ScopedUnitOfWork.EntityFrameworkCore
{
    /// <summary>
    /// Factory ideally injected into services or other objects and used for UoW creation.
    /// </summary>
    /// <typeparam name="TContext">
    /// Concrete EF context type which will be used for created units of work.
    /// </typeparam>
    public class UnitOfWorkFactory<TContext> : UnitOfWorkFactoryBase<TContext> where TContext : DbContext
    {
        public UnitOfWorkFactory(IServiceLocator serviceLocator) : 
            base(new UnitOfWorkScopeManager<TContext>(serviceLocator))
        {
        }
    }
}