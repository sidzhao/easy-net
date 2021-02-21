using System;
using System.Threading.Tasks;
using EasyNet.CommonTests;
using EasyNet.Domain.Uow;
using EasyNet.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace EasyNet.Tests.Mvc
{
    public class EasyNetUowActionFilterTest
    {
        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task TestEasyNetUowActionFilter(bool isControllerAction, bool suppressAutoBeginUnitOfWork)
        {
            // Arrange
            var completeHandleMock = new Mock<IUnitOfWorkCompleteHandle>();

            var uowMock = new Mock<IUnitOfWorkManager>();
            uowMock
                .Setup(f => f.Begin(It.IsAny<IServiceProvider>(), It.IsAny<UnitOfWorkOptions>()))
                .Returns(() => completeHandleMock.Object);

            var optionsMock = new Mock<IOptions<EasyNetOptions>>();
            optionsMock
                .Setup(f => f.Value)
                .Returns(() => new EasyNetOptions { SuppressAutoBeginUnitOfWork = suppressAutoBeginUnitOfWork });

            var uowActionFilterMock = new Mock<EasyNetUowActionFilter>(uowMock.Object, optionsMock.Object);
            uowActionFilterMock.As<IAsyncActionFilter>()
                .Setup(f => f.OnActionExecutionAsync(
                    It.IsAny<ActionExecutingContext>(),
                    It.IsAny<ActionExecutionDelegate>()))
                .CallBase();

            var context = isControllerAction
                ? CommonTest.CreateControllerActionExecutingContext(uowActionFilterMock.As<IFilterMetadata>().Object)
                : CommonTest.CreateActionExecutingContext(uowActionFilterMock.As<IFilterMetadata>().Object);
            var next = new ActionExecutionDelegate(() =>
                Task.FromResult(CommonTest.CreateActionExecutedContext(context)));

            // Act
            await uowActionFilterMock.As<IAsyncActionFilter>().Object.OnActionExecutionAsync(context, next);

            // Assert
            var times = !isControllerAction || suppressAutoBeginUnitOfWork ? Times.Never() : Times.Once();
            uowMock.Verify(f => f.Begin(It.IsAny<IServiceProvider>(), It.IsAny<UnitOfWorkOptions>()), times);
            completeHandleMock.Verify(f => f.CompleteAsync(), times);
        }
    }
}
