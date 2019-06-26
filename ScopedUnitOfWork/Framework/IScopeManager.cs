using ScopedUnitOfWork.Interfaces;

namespace ScopedUnitOfWork.Framework
{
    public interface IScopeManager
    {
        void Complete(IUnitOfWork unitOfWork);
        void Remove(IUnitOfWork unitOfWork);
    }
}