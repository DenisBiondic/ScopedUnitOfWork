namespace ScopedUnitOfWork.Interfaces
{
    /// <summary>
    /// Simple marker interface for cases where generic version is not needed.
    /// </summary>
    public interface IRepository
    {
        void SetUnitOfWork(IUnitOfWork unitOfWork);
    }

    /// <summary>
    /// Strongly typed repository interface.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity for this repository.</typeparam>
    /// <typeparam name="TEntityIdentifier">
    /// Type of the identifier of the entity used for this repository. 
    /// Will be enforced on operations such as Get() where identifier is needed.
    /// </typeparam>
    public interface IRepository<TEntity, in TEntityIdentifier> : IRepository
        where TEntity : class
    {
        TEntity Get(TEntityIdentifier id);

        void Add(TEntity entity);

        void Remove(TEntity entity);
    }
}