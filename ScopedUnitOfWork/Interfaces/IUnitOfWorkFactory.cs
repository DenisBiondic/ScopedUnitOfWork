namespace ScopedUnitOfWork.Interfaces
{
    /// <summary>
    /// Interface meant to be registered with IoC container and then used
    /// across the application (injected into objects) for unit of work 
    /// instantiations.
    /// </summary>
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
        IUnitOfWork Create(ScopeType scopeType);
    }
}