using System;
using System.Linq;
using System.Reflection;
using EasyNet.Dapper.Data;
using EasyNet.Data;
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
        public static EasyNetRepositoryBuilder AsDefault(this EasyNetRepositoryBuilder builder, params Assembly[] assemblies)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Options.AddRegisterServicesAction(services =>
            {
                RegisterRepositories(
                    services,
                    typeof(IRepository<>),
                    typeof(IRepository<,>),
                    assemblies);
            });

            return builder;
        }

        /// <summary>
        /// Use <see cref="IDapperRepository{TEntity}"/> to access repository implementation class.
        /// </summary>
        public static EasyNetRepositoryBuilder AsIDapperRepository(this EasyNetRepositoryBuilder builder, params Assembly[] assemblies)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Options.AddRegisterServicesAction(services =>
            {
                RegisterRepositories(
                    services,
                    typeof(IDapperRepository<>),
                    typeof(IDapperRepository<,>),
                    assemblies);
            });

            return builder;
        }

        private static void RegisterRepositories(
            IServiceCollection services,
            Type repositoryOfEntityServiceType,
            Type repositoryOfEntityAndPrimaryKeyServiceType,
            params Assembly[] assemblies)
        {
            var entityInterface = typeof(IEntity<>);

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetExportedTypes().Where(p => p.HasImplementedRawGeneric(entityInterface));
                foreach (var entityType in types)
                {
                    // Try to get id property
                    var idProperty = entityType.GetProperty("Id");
                    if (idProperty != null)
                    {
                        // Add short service IDapperRepository<TEntity> if the id property type is int.
                        if (idProperty.PropertyType == typeof(int))
                        {
                            services.TryAddTransient(
                                repositoryOfEntityServiceType.MakeGenericType(entityType),
                                typeof(DapperRepositoryBase<>).MakeGenericType(entityType));
                        }

                        // Add service IDapperRepository<TEntity,TPrimaryKey>
                        services.TryAddTransient(
                            repositoryOfEntityAndPrimaryKeyServiceType.MakeGenericType(entityType, idProperty.PropertyType),
                            typeof(DapperRepositoryBase<,>).MakeGenericType(entityType, idProperty.PropertyType));
                    }
                }
            }
        }
    }
}