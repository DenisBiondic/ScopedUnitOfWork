using ScopedUnitOfWork.Interfaces;

namespace ScopedUnitOfWork.Tests.AcceptanceTests.SampleApplication.SimpleDomain
{
    public interface ICustomerRepository : IRepository<Customer, int>
    {
        Customer FindByName(string name);
    }
}