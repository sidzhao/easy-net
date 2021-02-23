using System.Linq;
using System.Reflection;
using EasyNet.Dapper.Data;
using EasyNet.Data;
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
        /// Add specified services to let the system use Dapper.
        /// </summary>
        /// <param name="options">The <see cref="EasyNetOptions"/>.</param>
        /// <returns></returns>
        public static void UseDapper(this EasyNetOptions options)
        {
            Check.NotNull(options, nameof(options));

            options.AddRegisterServicesAction(services =>
            {
                if (options.Assemblies == null || options.Assemblies.Length == 0)
                    throw new EasyNetException("Please add related assemblies first.");

                RegisterRepositories(services, options.Assemblies);
            });
        }

        private static void RegisterRepositories(IServiceCollection services, params Assembly[] assemblies)
        {
            var entityInterface = typeof(IEntity);

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetExportedTypes().Where(p => entityInterface.IsAssignableFrom(p));
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
                                typeof(IDapperRepository<>).MakeGenericType(entityType),
                                typeof(DapperRepositoryBase<>).MakeGenericType(entityType));
                        }

                        // Add service IDapperRepository<TEntity,TPrimaryKey>
                        services.TryAddTransient(
                            typeof(IDapperRepository<,>).MakeGenericType(entityType, idProperty.PropertyType),
                            typeof(DapperRepositoryBase<,>).MakeGenericType(entityType, idProperty.PropertyType));
                    }
                }
            }
        }
    }
}
