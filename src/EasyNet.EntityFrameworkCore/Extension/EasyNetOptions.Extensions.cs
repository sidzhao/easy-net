using System;
using System.Reflection;
using EasyNet.Data;
using EasyNet.EntityFrameworkCore;
using EasyNet.EntityFrameworkCore.Data;
using EasyNet.EntityFrameworkCore.Domain.Repositories;
using EasyNet.EntityFrameworkCore.Domain.Uow;
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
        public static void UseEfCore<TDbContext>(this EasyNetOptions options, Action<DbContextOptionsBuilder> setupAction)
            where TDbContext : EasyNetDbContext
        {
            Check.NotNull(options, nameof(options));
            Check.NotNull(setupAction, nameof(setupAction));

            options.AddRegisterServicesAction(services =>
            {
                // DbContext
                services.AddDbContext<TDbContext>(setupAction);
                services.AddScoped<IDbContextProvider, UnitOfWorkDbContextProvider>();
                services.AddTransient<IDbContextCreator, DbContextCreator<TDbContext>>();

                // Data
                services.TryAddScoped<IActiveDbTransactionProvider, EfCoreActiveDbTransactionProvider>();


                // Uow
                services.TryAddTransient<IUnitOfWork, EfCoreUnitOfWork>();

                // Repository
                RegisterRepositories<TDbContext>(services);

            });
        }

        /// <summary>
        /// Add all repositories service to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TDbContext">The context associated with the application.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        private static void RegisterRepositories<TDbContext>(IServiceCollection services) where TDbContext : EasyNetDbContext
        {
            var dbContextType = typeof(TDbContext);
            var properties = dbContextType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                // Try to get DbSet<> type collection
                if (property.PropertyType.IsGenericType &&
                    string.Equals(property.PropertyType.Name, typeof(DbSet<>).Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    // Try to get entity type
                    if (property.PropertyType.GenericTypeArguments.Length == 1)
                    {
                        var entityType = property.PropertyType.GenericTypeArguments[0];

                        // Try to get id property
                        var idProperty = entityType.GetProperty("Id");
                        if (idProperty != null)
                        {
                            // Add short service IRepository<TEntity> if the id property type is int.
                            if (idProperty.PropertyType == typeof(int))
                            {
                                services.TryAddTransient(
                                    typeof(IRepository<>).MakeGenericType(entityType),
                                    typeof(EfCoreRepositoryBase<,>).MakeGenericType(dbContextType, entityType));
                            }

                            // Add service IRepository<TEntity,TPrimaryKey>
                            services.TryAddTransient(
                                typeof(IRepository<,>).MakeGenericType(entityType, idProperty.PropertyType),
                                typeof(EfCoreRepositoryBase<,,>).MakeGenericType(dbContextType, entityType, idProperty.PropertyType));
                        }
                    }
                }
            }
        }
    }
}
