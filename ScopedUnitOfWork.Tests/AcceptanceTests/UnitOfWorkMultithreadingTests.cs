using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using ScopedUnitOfWork.Framework;
using ScopedUnitOfWork.Interfaces;
using ScopedUnitOfWork.Tests.AcceptanceTests.SampleApplication.SimpleDomain;
using Xunit;
using Customer = ScopedUnitOfWork.Tests.AcceptanceTests.SampleApplication.SimpleDomain.Customer;

namespace ScopedUnitOfWork.Tests.AcceptanceTests
{
    [Collection("SequentialTests")]
    public class UnitOfWorkMultithreadingTests : AcceptanceTestBase
    {
        public UnitOfWorkMultithreadingTests()
        {
            TestsConfiguration.CreateApplication();
        }

        protected override IContainer Container => TestsConfiguration.Container;

        // this shows the thread-safe nature of uow implementation which is achieved
        // by sharing nothing at all :)
        [Fact]
        public void UnitOfWorksOnDifferentThreadsShouldNotShareContexts()
        {
            object nestedContext = null;

            using (var outterUow = GetFactory().Create())
            {
                var task = Task.Factory.StartNew(() =>
                {
                    using (var nestedUow = GetFactory().Create())
                    {
                        nestedContext = ((IContextAwareUnitOfWork) nestedUow).GetContext();
                    }
                });

                task.Wait(2500);

                var outterContext = ((IContextAwareUnitOfWork) outterUow).GetContext();

                nestedContext.Should().NotBeSameAs(outterContext);
            }
        }
        
        [Fact]
        public void ParallelUnitOfWorkStacksShouldHaveNoIssues()
        {
            IList<Task> tasks = new List<Task>();
            IList<Customer> customers = new List<Customer>();
            
            using (var outterUow = GetFactory().Create())
            {
                Action action = () =>
                {
                    var customer = CustomerGenerator.Generate();

                    using (var uow1 = GetFactory().Create(ScopeType.Transactional))
                    {
                        using (var uow2 = GetFactory().Create())
                        {
                            uow2.GetRepository<ICustomerRepository>().Add(customer);
                            uow2.Commit();
                        }
                        uow1.Commit();
                    }

                    lock(customers)
                        customers.Add(customer);
                };

                for (int i = 0; i < 25; i++)
                {
                    tasks.Add(Task.Factory.StartNew(action));
                }

                var singleCustomer = CustomerGenerator.Generate();
                outterUow.GetRepository<ICustomerRepository>().Add(singleCustomer);

                outterUow.Commit();

                customers.Add(singleCustomer);
            }

            Task.WaitAll(tasks.ToArray());

            customers.Should().HaveCount(26);
        }
    }
}