using Autofac;
using FluentAssertions;
using ScopedUnitOfWork.Framework;
using ScopedUnitOfWork.Tests.AcceptanceTests.SampleApplication.SimpleDomain;
using Xunit;

namespace ScopedUnitOfWork.Tests.AcceptanceTests
{
    [Collection("SequentialTests")]
    public class UnitOfWorkCachingTests : AcceptanceTestBase
    {
        public UnitOfWorkCachingTests()
        {
            TestsConfiguration.CreateApplication();
        }

        protected override IContainer Container => TestsConfiguration.Container;

        [Fact]
        public void NestedUnitsOfWorkShouldShareSameContext()
        {
            using (var unitOfWork1 = GetFactory().Create())
            {
                using (var unitOfWork2 = GetFactory().Create())
                {
                    using (var unitOfWork3 = GetFactory().Create())
                    {
                        var context1 = ((IContextAwareUnitOfWork) unitOfWork1).GetContext();
                        var context2 = ((IContextAwareUnitOfWork) unitOfWork2).GetContext();
                        var context3 = ((IContextAwareUnitOfWork) unitOfWork3).GetContext();

                        context1.Should().BeSameAs(context2);
                        context2.Should().BeSameAs(context3);
                    }
                }
            }
        }

        // this is testing proper L1 Identity Map caching of unit of work
        [Fact]
        public void GettingSimpleDataFromNestedUnitsOfWorkShouldGetCachedData()
        {
            // Arrange
            using (var unitOfWork1 = GetFactory().Create())
            {
                var repository1 = unitOfWork1.GetRepository<ICustomerRepository>();
                var customer1 = CustomerGenerator.Generate();
                
                repository1.Add(customer1);
                unitOfWork1.Commit();

                var generatedId = customer1.Id;

                using (var unitOfWork2 = GetFactory().Create())
                {
                    var repository2 = unitOfWork2.GetRepository<ICustomerRepository>();
                    var customer2 = repository2.Get(generatedId);

                    using (var unitOfWork3 = GetFactory().Create())
                    {
                        var repository3 = unitOfWork3.GetRepository<ICustomerRepository>();
                        var customer3 = repository3.Get(generatedId);

                        // Assert
                        customer1.Should().BeSameAs(customer2);
                        customer2.Should().BeSameAs(customer3);
                    }
                }
            }
        }

        [Fact]
        public void NonNestedUnitsOfWorkShouldNotShareContexts()
        {
            // Arrange
            object nestedContext;

            using (var unitOfWork1 = GetFactory().Create())
            {
                using (var unitOfWork2 = GetFactory().Create())
                {
                    nestedContext = ((IContextAwareUnitOfWork) unitOfWork1).GetContext();
                    var context2 = ((IContextAwareUnitOfWork) unitOfWork2).GetContext();

                    nestedContext.Should().BeSameAs(context2);
                }
            }

            using (var unitOfWork3 = GetFactory().Create())
            {
                var context3 = ((IContextAwareUnitOfWork) unitOfWork3).GetContext();

                // Assert
                nestedContext.Should().NotBeSameAs(context3);
            }
        }
    }
}
