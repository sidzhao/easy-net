using System.Threading.Tasks;
using EasyNet.DependencyInjection;
using EasyNet.Ioc;
using EasyNet.Runtime.Session;
using EasyNet.Uow;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace EasyNet.Tests.Domain
{
    public class AsyncLocalCurrentUnitOfWorkProviderTest
    {
        [Fact]
        public void TestInit()
        {
            // Arrange
            var currentUnitOfWorkProvider = new AsyncLocalCurrentUnitOfWorkProvider();

            // Assert
            Assert.Null(currentUnitOfWorkProvider.Current);
        }

        [Fact]
        public void TestSetCurrentUnitOfWork()
        {
            // Arrange
            var currentUnitOfWorkProvider = new AsyncLocalCurrentUnitOfWorkProvider();

            // Act
            var unitOfWork = GetNullUnitOfWork();
            currentUnitOfWorkProvider.Current = unitOfWork;

            // Assert
            Assert.Same(unitOfWork, currentUnitOfWorkProvider.Current);
        }

        /// <summary>
        /// Test to use AsyncLocal to keep UnitOfWork during AsyncAction.
        /// If use ThreadLocal to keep UnitOfWork, it cannot keep it in the whole process.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestAsyncAction()
        {
            // Arrange
            var currentUnitOfWorkProvider = new AsyncLocalCurrentUnitOfWorkProvider();

            // Act
            var unitOfWork = GetNullUnitOfWork();
            currentUnitOfWorkProvider.Current = unitOfWork;
            await Task.Delay(500);

            // Assert
            Assert.Same(unitOfWork, currentUnitOfWorkProvider.Current);
        }

        [Fact]
        public void TestSetSecondUnitOfWork()
        {
            // Arrange
            var currentUnitOfWorkProvider = new AsyncLocalCurrentUnitOfWorkProvider();


            // Act
            var unitOfWork1 = GetNullUnitOfWork();
            currentUnitOfWorkProvider.Current = unitOfWork1;

            var unitOfWork2 = GetNullUnitOfWork();
            currentUnitOfWorkProvider.Current = unitOfWork2;

            // Assert
            Assert.Same(unitOfWork2, currentUnitOfWorkProvider.Current);
            Assert.Same(unitOfWork1, currentUnitOfWorkProvider.Current.Outer);
        }

        /// <summary>
        /// If current unit of work has outer and then set current unit of work is null,
        /// it will set outer as current unit of work.
        /// </summary>
        [Fact]
        public void TestSetUnitOfWorkIsNullAndHasOuter()
        {
            // Arrange
            var currentUnitOfWorkProvider = new AsyncLocalCurrentUnitOfWorkProvider();

            // Act
            var unitOfWork1 = GetNullUnitOfWork();
            currentUnitOfWorkProvider.Current = unitOfWork1;

            var unitOfWork2 = GetNullUnitOfWork();
            currentUnitOfWorkProvider.Current = unitOfWork2;

            currentUnitOfWorkProvider.Current = null;

            // Assert
            Assert.Same(unitOfWork1, currentUnitOfWorkProvider.Current);
        }

        private NullUnitOfWork GetNullUnitOfWork()
        {
            var iocMoq = new Mock<IIocResolver>();
            iocMoq.Setup(p => p.GetService<IEasyNetSession>(It.IsAny<bool>()))
                .Returns(NullEasyNetSession.Instance);

            return new NullUnitOfWork(iocMoq.Object, new OptionsWrapper<UnitOfWorkDefaultOptions>(new UnitOfWorkDefaultOptions()));
        }
    }
}
