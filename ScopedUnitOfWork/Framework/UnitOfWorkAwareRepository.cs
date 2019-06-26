using System;
using ScopedUnitOfWork.Interfaces;
using ScopedUnitOfWork.Interfaces.Exceptions;

namespace ScopedUnitOfWork.Framework
{
    public abstract class UnitOfWorkAwareRepository : IRepository
    {
        protected IContextAwareUnitOfWork UnitOfWork { get; private set; }

        protected Exception CreateIncorrectContextTypeException()
        {
            return new IncorrectUnitOfWorkUsageException(
                        "Unit of work registered context: " + UnitOfWork.GetContext() +
                        ", type: " + UnitOfWork.GetContext()?.GetType() +
                        " could not be cast to expected EF DbContext base class. " +
                        "Make sure the context you registered the unit of work factory with " +
                        "is correctly registed with IoC container (as self) and that it inherits " +
                        "from EF DbContext.");
        }

        public void SetUnitOfWork(IUnitOfWork unitOfWork)
        {
            UnitOfWork = (IContextAwareUnitOfWork) unitOfWork;
        }
    }
}