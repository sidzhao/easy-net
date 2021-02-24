using System.Threading.Tasks;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.Runtime.Session;
using EasyNet.Uow;
using Microsoft.Extensions.Options;
using Xunit;

namespace EasyNet.Tests.Uow
{
    public class UnitOfWorkBaseTest
    {
        [Fact]
        public void TestInit()
        {
            // Arrange
            var nullUnitOfWork = GetUnitOfWork();

            // Assert
            Assert.False(string.IsNullOrEmpty(nullUnitOfWork.Id));
            Assert.False(nullUnitOfWork.IsDisposed);
            Assert.Null(nullUnitOfWork.Outer);
        }

        [Fact]
        public void TestBegin()
        {
            // Arrange
            var nullUnitOfWork = GetUnitOfWork();

            // Act
            var unitOfWorkOptions = new UnitOfWorkOptions();
            nullUnitOfWork.Begin(unitOfWorkOptions);

            // Assert
            Assert.True(nullUnitOfWork.GetPrivateField<bool>("_isBeginCalledBefore"));
            Assert.Same(unitOfWorkOptions, nullUnitOfWork.Options);
            Assert.Throws<EasyNetException>(() => nullUnitOfWork.Begin(unitOfWorkOptions));
        }

        [Fact]
        public void TestComplete()
        {
            // Arrange
            var nullUnitOfWork = GetUnitOfWork();

            // Act
            nullUnitOfWork.Begin(new UnitOfWorkOptions());
            nullUnitOfWork.Complete();

            // Assert
            Assert.True(nullUnitOfWork.GetPrivateField<bool>("_isCompleteCalledBefore"));
            Assert.True(nullUnitOfWork.GetPrivateField<bool>("_succeed"));
            Assert.Throws<EasyNetException>(() => nullUnitOfWork.Complete());
        }

        [Fact]
        public async Task TestCompleteAsync()
        {
            // Arrange
            var nullUnitOfWork = GetUnitOfWork();

            // Act
            nullUnitOfWork.Begin(new UnitOfWorkOptions());
            await nullUnitOfWork.CompleteAsync();

            // Assert
            Assert.True(nullUnitOfWork.GetPrivateField<bool>("_isCompleteCalledBefore"));
            Assert.True(nullUnitOfWork.GetPrivateField<bool>("_succeed"));
            await Assert.ThrowsAsync<EasyNetException>(async () => await nullUnitOfWork.CompleteAsync());
        }

        [Fact]
        public void TestDispose()
        {
            // Arrange
            var nullUnitOfWork = GetUnitOfWork();

            // Act
            nullUnitOfWork.Begin(new UnitOfWorkOptions());
            nullUnitOfWork.Complete();
            nullUnitOfWork.Dispose();

            // Assert
            Assert.True(nullUnitOfWork.IsDisposed);
        }

        private DefaultUnitOfWork GetUnitOfWork()
        {
            return new DefaultUnitOfWork(NullEasyNetSession.Instance, new OptionsWrapper<UnitOfWorkDefaultOptions>(new UnitOfWorkDefaultOptions()));
        }
    }
}
