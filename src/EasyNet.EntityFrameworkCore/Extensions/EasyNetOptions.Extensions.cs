using System;
using EasyNet.Data;
using EasyNet.EntityFrameworkCore;
using EasyNet.EntityFrameworkCore.Data;
using EasyNet.EntityFrameworkCore.Uow;
using EasyNet.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace EasyNet.Extensions.DependencyInjection
{
    /// <summary>
    /// Extensions for configuring EasyNet using an <see cref="EasyNetOptions"/>.
    /// </summary>
    public static class EasyNetOptionsExtensions
    {
        /// <summary>
        /// Add specified services to let the system support EntityFrameworkCore.
        /// </summary>
        /// <typeparam name="TDbContext">The context associated with the application.</typeparam>
        /// <param name="options">The <see cref="EasyNetOptions"/>.</param>
        /// <param name="setupAction">An <see cref="Action{DbContextOptionsBuilder}"/> to configure the provided <see cref="DbContextOptionsBuilder"/>.</param>
        /// <returns></returns>
        public static EasyNetRepositoryBuilder UseEfCore<TDbContext>(this EasyNetOptions options, Action<DbContextOptionsBuilder> setupAction)
            where TDbContext : EasyNetDbContext
        {
            Check.NotNull(options, nameof(options));
            Check.NotNull(setupAction, nameof(setupAction));

            options.AddRegisterServicesAction(services =>
            {
                // DbContext
                services.AddDbContext<TDbContext>(setupAction, ServiceLifetime.Transient);

                // Data
                services.TryAddScoped<IDbConnectorCreator, EfCoreDbConnectorCreator<TDbContext>>();

                // Uow
                services.TryAddTransient<IUnitOfWork, EfCoreUnitOfWork>();
            });

            return new EasyNetRepositoryBuilder(options);
        }
    }
}
