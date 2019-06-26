using System;

namespace ScopedUnitOfWork.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Strongly typed repository resolution method using the IoC container
        /// </summary>
        /// <typeparam name="TRepository">
        /// Concrete repository type. 
        /// Should normally be an strong interface like ICustomerRepository.
        /// </typeparam>
        /// <returns>Requested repository resolved from IoC container.</returns>
        TRepository GetRepository<TRepository>() where TRepository : class, IRepository;

        /// <summary>
        /// ScopeType with which this unit of work was started by the factory.
        /// </summary>
        ScopeType ScopeType { get; }

        /// <summary>
        /// Indicated that unit of work finished, either by calling Commit or Dispose.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        /// Persist all changes for given unit of work, also commiting the transaction
        /// if one is open and if this is the topmost transactional unit of work
        /// in the stack.
        /// </summary>
        void Commit();
    }
}