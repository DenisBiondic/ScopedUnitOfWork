using System.Linq;
using ScopedUnitOfWork.EntityFrameworkCore;

namespace ScopedUnitOfWork.Tests.AcceptanceTests.SampleApplication.SimpleDomain
{
    public class CustomerRepository : GenericRepository<Customer, int>, ICustomerRepository
    {
        public Customer FindByName(string name)
        {
            return Set.SingleOrDefault(x => x.Name == name);
        }
    }
}