using System;
using CommonServiceLocator;
using ScopedUnitOfWork.Interfaces;

namespace ScopedUnitOfWork.Framework
{
    public abstract class UnitOfWork<TContext> : IContextAwareUnitOfWork where TContext : class, IDisposable
    {
        protected readonly TContext Context;
        private readonly IServiceLocator _serviceLocator;
        private readonly IScopeManager _scopeManager;

        protected UnitOfWork(TContext context, IScopeManager scopeManager, IServiceLocator serviceLocator, ScopeType scopeType)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _scopeManager = scopeManager ?? throw new ArgumentNullException(nameof(scopeManager));
            _serviceLocator = serviceLocator ?? throw new ArgumentNullException(nameof(serviceLocator));

            Id = UniqueIdGenerator.Generate();

            ScopeType = scopeType;
        }

        public object GetContext()
        {
            return Context;
        }

        public TRepository GetRepository<TRepository>() where TRepository : class, IRepository
        {
            var repository = _serviceLocator.GetInstance<TRepository>();
            repository.SetUnitOfWork(this);
            return repository;
        }

        public string Id { get; private set; }
        
        public ScopeType ScopeType { get; private set; }
        
        public bool IsFinished { get; private set; }

        public void Commit()
        {
            if (IsFinished)
                throw new InvalidOperationException("Unit of work could not be committed either because it was " +
                                                    "already committed or it was disposed.");

            try
            {
                ScopedUnitOfWorkConfiguration.LoggingAction(this + "Persisting context changes...");
                SaveContextChanges();

                _scopeManager.Complete(this);
            }
            finally
            {
                IsFinished = true;
            }
        }

        public override string ToString()
        {
            return $"[UnitOfWork {Id}, {ScopeType} ] ";
        }

        protected abstract void SaveContextChanges();

        public void Dispose()
        {
            ScopedUnitOfWorkConfiguration.LoggingAction(this + "Disposing...");
            _scopeManager.Remove(this);

            IsFinished = true;
        }
    }
}