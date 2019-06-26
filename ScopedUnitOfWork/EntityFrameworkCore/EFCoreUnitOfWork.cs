using CommonServiceLocator;
using Microsoft.EntityFrameworkCore;
using ScopedUnitOfWork.Framework;
using ScopedUnitOfWork.Interfaces;
using ScopedUnitOfWork.Interfaces.Exceptions;

namespace ScopedUnitOfWork.EntityFrameworkCore
{
    public class EntityFrameworkCoreUnitOfWork<TContext> : UnitOfWork<TContext> where TContext : DbContext
    {
        public EntityFrameworkCoreUnitOfWork(TContext context, IScopeManager scopeManager, IServiceLocator serviceLocator, ScopeType scopeType) 
            : base(context, scopeManager, serviceLocator, scopeType)
        {
        }

        protected override void SaveContextChanges()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException exception)
            {
                throw new ConcurrentModificationException(
                    "The record you attempted to edit was modified by another " +
                    "user after you loaded it. The edit operation was cancelled and the " +
                    "correct values in the database are displayed. Please try again.", exception);
            }
        }
    }
}