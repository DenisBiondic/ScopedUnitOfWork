using ScopedUnitOfWork.Interfaces;

namespace ScopedUnitOfWork.Framework
{
    public interface IContextAwareUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets the underlying database context
        /// </summary>
        object GetContext();
    }
}