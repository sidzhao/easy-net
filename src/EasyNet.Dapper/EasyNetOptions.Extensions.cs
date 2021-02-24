using EasyNet.Dapper.Repositories;
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
        public static EasyNetRepositoryBuilder UseDapper(this EasyNetOptions options)
        {
            Check.NotNull(options, nameof(options));

            options.AddRegisterServicesAction(services =>
            {
                services.TryAddSingleton<IQueryFilterExecuter, QueryFilterExecuter>();
            });

            return new EasyNetRepositoryBuilder(options);
        }
    }
}
