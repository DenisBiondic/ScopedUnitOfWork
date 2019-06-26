using System.Linq;
using Microsoft.EntityFrameworkCore;
using ScopedUnitOfWork.Framework;
using ScopedUnitOfWork.Interfaces;

namespace ScopedUnitOfWork.EntityFrameworkCore
{
    public abstract class GenericRepository<TEntity, TEntityIdentifier> : 
        UnitOfWorkAwareRepository, IRepository<TEntity, TEntityIdentifier>
        where TEntity : class
    {
        protected DbSet<TEntity> Set 
        {
            get
            {
                var context = UnitOfWork.GetContext() as DbContext;

                if (context == null)
                    throw CreateIncorrectContextTypeException();

                return context.Set<TEntity>();
            }
        }

        public virtual TEntity Get(TEntityIdentifier id)
        {
            return Set.Find(id);
        }

        public virtual void Add(TEntity entity)
        {
            Set.Add(entity);
        }

        public void Remove(TEntity entity)
        {
            Set.Remove(entity);
        }

        protected virtual IQueryable<TEntity> GetCompleteEntities(IQueryable<TEntity> source)
        {
            return source;
        }
    }
}