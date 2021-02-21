using System;
using EasyNet.Data;
using EasyNet.Ioc;
using EasyNet.Mvc;
using EasyNet.Runtime.Initialization;
using EasyNet.Runtime.Session;
using EasyNet.Uow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace EasyNet.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up EasyNet services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class EasyNetServiceCollectionExtensions
    {
        /// <summary>
        /// Adds EasyNet services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>An <see cref="EasyNetBuilder"/> that can be used to further configure the EasyNet services.</returns>
        public static EasyNetBuilder AddEasyNet(this IServiceCollection services)
        {
            Check.NotNull(services, nameof(services));

            return services.AddEasyNet(o => { });
        }

        /// <summary>
        /// Adds EasyNet services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="setupAction">An <see cref="Action{EasyNetOptions}"/> to configure the provided <see cref="EasyNetOptions"/>.</param>
        /// <returns>An <see cref="EasyNetBuilder"/> that can be used to further configure the EasyNet services.</returns>
        public static EasyNetBuilder AddEasyNet(this IServiceCollection services, Action<EasyNetOptions> setupAction)
        {
            Check.NotNull(services, nameof(services));

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            // See https://github.com/aspnet/Mvc/issues/3936 to know why we added these services.
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services
                .AddScoped<IIocResolver, AspNetCoreIocResolver>()
                .AddTransient<IEasyNetInitializer, EasyNetInitializer>()
                .AddTransient<IEasyNetExceptionHandler, EasyNetExceptionHandler>();

            // Mvc
            services
                .AddTransient<EasyNetUowActionFilter>()
                .AddTransient<EasyNetResultFilter>()
                .AddTransient<EasyNetExceptionFilter>()
                .AddTransient<EasyNetPageFilter>()
                .Configure<MvcOptions>(mvcOptions =>
                {
                    mvcOptions.Filters.AddService<EasyNetUowActionFilter>();
                    mvcOptions.Filters.AddService<EasyNetResultFilter>();
                    mvcOptions.Filters.AddService<EasyNetExceptionFilter>();
                    mvcOptions.Filters.AddService<EasyNetPageFilter>();
                });

            // Session
            services
                .AddSingleton<IPrincipalAccessor, DefaultPrincipalAccessor>()
                .AddScoped<IEasyNetSession, ClaimsEasyNetSession>();

            // Options and extension service
            if (setupAction != null)
            {
                var options = new EasyNetOptions();
                setupAction(options);
                foreach (var serviceAction in options.RegisterServicesActions)
                {
                    serviceAction(services);
                }
                services.Configure(setupAction);
            }
            
            // Unit of work
            services.TryAddSingleton<IUnitOfWorkManager, UnitOfWorkManager>();
            services.TryAddSingleton<ICurrentUnitOfWorkProvider, AsyncLocalCurrentUnitOfWorkProvider>();
            services.TryAddTransient<IUnitOfWork, NullUnitOfWork>();

            // Data
            services.TryAddScoped<IActiveDbTransactionProvider, NullDbTransactionProvider>();

            return new EasyNetBuilder(services);
        }
    }
}
