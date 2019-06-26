using System;

namespace ScopedUnitOfWork.Framework
{
    /// <summary>
    /// Simple interface to wrap platform dependent transactions 
    /// which vary with each ORM implementation.
    /// </summary>
    public interface ITransactionWrapper : IDisposable
    {
        void Commit();
        void Rollback();
    }
}