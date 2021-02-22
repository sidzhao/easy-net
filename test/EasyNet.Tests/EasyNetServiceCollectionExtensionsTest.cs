using EasyNet.CommonTests;
using EasyNet.Data;
using EasyNet.Mvc;
using EasyNet.Runtime.Initialization;
using EasyNet.Runtime.Session;
using EasyNet.Uow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace EasyNet.Tests
{
	public class EasyNetServiceCollectionExtensionsTest : DependencyInjectionTest
	{
		[Fact]
		public void TestAddEasyNet()
		{
			// Arrange
			var services = new ServiceCollection();
			services.AddSingleton(CommonTest.GetHostingEnvironment());

			// Act
			services.AddEasyNet();
            var serviceProvider = services.BuildServiceProvider();

			// Assert
			AssertSpecifiedServiceTypeAndImplementationType<IHttpContextAccessor, HttpContextAccessor>(services, ServiceLifetime.Singleton);
			
			// Session
			AssertSpecifiedServiceTypeAndImplementationType<IPrincipalAccessor, DefaultPrincipalAccessor>(services, ServiceLifetime.Singleton);
			AssertSpecifiedServiceTypeAndImplementationType<IEasyNetSession, ClaimsEasyNetSession>(services, ServiceLifetime.Scoped);

			// Exception
            AssertSpecifiedServiceTypeAndImplementationType<IEasyNetExceptionHandler, EasyNetExceptionHandler>(services, ServiceLifetime.Transient);

			// Unit of work
			AssertSpecifiedServiceTypeAndImplementationType<IUnitOfWorkManager, UnitOfWorkManager>(services, ServiceLifetime.Singleton);
            AssertSpecifiedServiceTypeAndImplementationType<ICurrentUnitOfWorkProvider, AsyncLocalCurrentUnitOfWorkProvider>(services, ServiceLifetime.Singleton);
			AssertSpecifiedServiceTypeAndImplementationType<IUnitOfWork, NullUnitOfWork>(services, ServiceLifetime.Transient);

			// Data
            AssertSpecifiedServiceTypeAndImplementationType<ICurrentDbConnectorProvider, AsyncLocalCurrentDbConnectorProvider>(services, ServiceLifetime.Scoped);

			// Initializer
            AssertSpecifiedServiceTypeAndImplementationType<IEasyNetInitializer, EasyNetInitializer>(services, ServiceLifetime.Transient);

			// Mvc
			AssertSpecifiedServiceTypeAndImplementationType<EasyNetUowActionFilter, EasyNetUowActionFilter>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<EasyNetExceptionFilter, EasyNetExceptionFilter>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<EasyNetResultFilter, EasyNetResultFilter>(services, ServiceLifetime.Transient);
            AssertSpecifiedServiceTypeAndImplementationType<EasyNetPageFilter, EasyNetPageFilter>(services, ServiceLifetime.Transient);
			var mvcOptions = serviceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;
			Assert.Contains(mvcOptions.Filters, p => ((ServiceFilterAttribute)p).ServiceType == typeof(EasyNetUowActionFilter));
            Assert.Contains(mvcOptions.Filters, p => ((ServiceFilterAttribute)p).ServiceType == typeof(EasyNetExceptionFilter));
            Assert.Contains(mvcOptions.Filters, p => ((ServiceFilterAttribute)p).ServiceType == typeof(EasyNetResultFilter));
            Assert.Contains(mvcOptions.Filters, p => ((ServiceFilterAttribute)p).ServiceType == typeof(EasyNetPageFilter));
		}
	}
}
