using System;
using System.Reflection;
using EasyNet.Data.Repositories;
using EasyNet.EntityFrameworkCore;
using EasyNet.EntityFrameworkCore.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace EasyNet.Extensions.DependencyInjection
{
    public static class EasyNetRepositoryBuilderExtensions
    {
        /// <summary>
        /// Use <see cref="IRepository"/> to access repository implementation class.
        /// </summary>
        public static EasyNetRepositoryBuilder AsDefault<TDbContext>(this EasyNetRepositoryBuilder builder)
            where TDbContext : EasyNetDbContext
        {
            Check.NotNull(builder, nameof(builder));

            builder.Options.AddRegisterServicesAction(services =>
            {
                RegisterRepositories<TDbContext>(
                    services,
                    typeof(IRepository<>),
                    typeof(IRepository<,>));
            });

            return builder;
        }

        /// <summary>
        /// Use <see cref="IEfCoreRepository{TEntity}"/> to access repository implementation class.
        /// </summary>
        public static EasyNetRepositoryBuilder AsIEfCoreRepository<TDbContext>(this EasyNetRepositoryBuilder builder)
            where TDbContext : EasyNetDbContext
        {
            Check.NotNull(builder, nameof(builder));

            builder.Options.AddRegisterServicesAction(services =>
            {
                RegisterRepositories<TDbContext>(
                    services,
                    typeof(IEfCoreRepository<>),
                    typeof(IEfCoreRepository<,>));
            });

            return builder;
        }

        private static void RegisterRepositories<TDbContext>(
            IServiceCollection services,
            Type repositoryOfEntityServiceType,
            Type repositoryOfEntityAndPrimaryKeyServiceType) where TDbContext : EasyNetDbContext
        {
            var dbContextType = typeof(TDbContext);
            var properties = dbContextType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                // Try to get DbSet<> type collection
                if (property.PropertyType.IsGenericType &&
                    string.Equals(property.PropertyType.Name, typeof(DbSet<>).Name,
                        StringComparison.CurrentCultureIgnoreCase))
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
                                    repositoryOfEntityServiceType.MakeGenericType(entityType),
                                    typeof(EfCoreRepositoryBase<,>).MakeGenericType(dbContextType, entityType));
                            }

                            // Add service IRepository<TEntity,TPrimaryKey>
                            services.TryAddTransient(
                                repositoryOfEntityAndPrimaryKeyServiceType.MakeGenericType(entityType,
                                    idProperty.PropertyType),
                                typeof(EfCoreRepositoryBase<,,>).MakeGenericType(dbContextType, entityType,
                                    idProperty.PropertyType));
                        }
                    }
                }
            }
        }
    }
}