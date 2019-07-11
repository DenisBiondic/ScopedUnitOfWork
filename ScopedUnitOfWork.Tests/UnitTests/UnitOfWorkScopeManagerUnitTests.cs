using System;
using CommonServiceLocator;
using FluentAssertions;
using NSubstitute;
using ScopedUnitOfWork.Framework;
using ScopedUnitOfWork.Interfaces;
using Xunit;

namespace ScopedUnitOfWork.Tests.UnitTests
{
    public class UnitOfWorkScopeManagerUnitTests
    {
        private readonly UnitOfWorkScopeManagerBase<DummyContext> _manager;

        public UnitOfWorkScopeManagerUnitTests()
        {
            var resolver = Substitute.For<IServiceLocator>();
            resolver.GetInstance<DummyContext>().Returns(new DummyContext());
            _manager = new DummyScopeManager(resolver);
        }

        [Fact]
        public void ScopeStack_ShouldBeProperlyFilledAndEmptiedAfterAllUoWsFinish()
        {
            using (_manager.CreateNew(ScopeType.Default))
            {
                using (_manager.CreateNew(ScopeType.Transactional))
                {
                    using (_manager.CreateNew(ScopeType.Default))
                    {
                        CheckStackSizeAndExistenceOfContextAndTransaction(3, true, true);
                    }

                    // we throw out one uow but everything else should be there
                    CheckStackSizeAndExistenceOfContextAndTransaction(2, true, true);
                }

                // transaction should be gone here, as well as another uow
                CheckStackSizeAndExistenceOfContextAndTransaction(1, true, false);
            }

            // everything should be gone here
            UnitOfWorkScopeManagerBase<DummyContext>.ScopeStack.Should().BeNull();
        }

        private static void CheckStackSizeAndExistenceOfContextAndTransaction(int stackSize, bool shouldContainContext, bool shouldContainTransaction)
        {
            UnitOfWorkScopeManagerBase<DummyContext>.ScopeStack.Stack.Count.Should().Be(stackSize);

            if (shouldContainContext)
                UnitOfWorkScopeManagerBase<DummyContext>.ScopeStack.Context.Should().NotBeNull();
            else
                UnitOfWorkScopeManagerBase<DummyContext>.ScopeStack.Context.Should().BeNull();

            if (shouldContainTransaction)
                UnitOfWorkScopeManagerBase<DummyContext>.ScopeStack.Transaction.Should().NotBeNull();
            else
                UnitOfWorkScopeManagerBase<DummyContext>.ScopeStack.Transaction.Should().BeNull();
        }

        public class DummyContext : IDisposable
        {
            public void Dispose()
            {
            }
        }

        public class DummyScopeManager : UnitOfWorkScopeManagerBase<DummyContext>
        {
            public DummyScopeManager(IServiceLocator serviceLocator) : base(serviceLocator)
            {
            }

            protected override IUnitOfWork CreateUnitOfWork(ScopeType scopeType)
            {
                var unitOfWork = Substitute.For<IUnitOfWork>();
                unitOfWork.ScopeType.Returns(scopeType);
                unitOfWork.When(x => x.Dispose())
                    .Do(x => Remove(unitOfWork));
                unitOfWork.When(x => x.Commit())
                    .Do(x => Complete(unitOfWork));
                return unitOfWork;
            }

            protected override ITransactionWrapper CreateAndStartTransaction()
            {
                return Substitute.For<ITransactionWrapper>();
            }
        }
    }
}