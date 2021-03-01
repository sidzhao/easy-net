using System;
using EasyNet.Data;
using EasyNet.Sqlite;
using EasyNet.Sqlite.Data;
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
        /// Add specified services to let the system use SqlLite.
        /// </summary>
        /// <param name="options">The <see cref="EasyNetOptions"/>.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <returns></returns>
        public static void UseSqlLite(this EasyNetOptions options, string connectionString)
        {
            Check.NotNull(options, nameof(options));
            Check.NotNullOrEmpty(connectionString, nameof(connectionString));

            options.UseSqlLite(opt => { opt.ConnectionString = connectionString; });
        }

        /// <summary>
        /// Add specified services to let the system use SqlLite.
        /// </summary>
        /// <param name="options">The <see cref="EasyNetOptions"/>.</param>
        /// <param name="setupAction">An <see cref="Action{EasyNetSqlLiteOptions}"/> to configure the provided <see cref="SqliteOptions"/>.</param>
        /// <returns></returns>
        public static void UseSqlLite(this EasyNetOptions options, Action<SqliteOptions> setupAction)
        {
            Check.NotNull(options, nameof(options));
            Check.NotNull(setupAction, nameof(setupAction));
            
            options.AddRegisterServicesAction(services =>
            {
                services.Configure(setupAction);

                services.TryAddSingleton<IDbConnectorCreator, SqliteConnectorCreator>();
            });
        }
    }
}
