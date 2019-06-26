using System;
using ScopedUnitOfWork.Interfaces;

namespace ScopedUnitOfWork.Framework
{
    public abstract class UnitOfWorkFactoryBase<TContext> : IUnitOfWorkFactory where TContext : class, IDisposable
    {
        protected UnitOfWorkFactoryBase(UnitOfWorkScopeManagerBase<TContext> scopeManager)
        {
            ScopeManager = scopeManager;
        }

        public IUnitOfWork Create()
        {
            return ScopeManager.CreateNew(ScopeType.Default);
        }

        public IUnitOfWork Create(ScopeType scopeType)
        {
            return ScopeManager.CreateNew(scopeType);
        }

        internal UnitOfWorkScopeManagerBase<TContext> ScopeManager { get; }
    }
}