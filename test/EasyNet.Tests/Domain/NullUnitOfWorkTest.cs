using System.Threading.Tasks;
using EasyNet.DependencyInjection;
using EasyNet.Domain.Uow;
using EasyNet.Extensions;
using EasyNet.Runtime.Session;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace EasyNet.Tests.Domain
{
	public class NullUnitOfWorkTest
	{
		[Fact]
		public void TestInit()
		{
			// Arrange
			var nullUnitOfWork = GetNullUnitOfWork();

			// Assert
			Assert.False(string.IsNullOrEmpty(nullUnitOfWork.Id));
			Assert.False(nullUnitOfWork.IsDisposed);
			Assert.Null(nullUnitOfWork.Outer);
		}

		[Fact]
		public void TestBegin()
		{
			// Arrange
			var nullUnitOfWork = GetNullUnitOfWork();

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
			var nullUnitOfWork = GetNullUnitOfWork();

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
			var nullUnitOfWork = GetNullUnitOfWork();

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
			var nullUnitOfWork = GetNullUnitOfWork();

			// Act
			nullUnitOfWork.Begin(new UnitOfWorkOptions());
			nullUnitOfWork.Complete();
			nullUnitOfWork.Dispose();

			// Assert
			Assert.True(nullUnitOfWork.IsDisposed);
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
