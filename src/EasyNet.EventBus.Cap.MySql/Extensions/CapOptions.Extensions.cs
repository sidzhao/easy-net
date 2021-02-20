using System;
using DotNetCore.CAP;
using DotNetCore.CAP.Persistence;
using EasyNet.EventBus.Cap.MySql;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class CapOptionsExtensions
    {
        public static CapOptions UseEasyNetMySql(this CapOptions options, string connectionString)
        {
            return options.UseEasyNetMySql(opt => { opt.ConnectionString = connectionString; });
        }

        public static CapOptions UseEasyNetMySql(this CapOptions options, Action<MySqlOptions> setupAction)
        {
            options.UseMySql(setupAction);

            options.RegisterExtension(new EasyNetMySqlCapOptionsExtension());

            return options;
        }
    }

    internal class EasyNetMySqlCapOptionsExtension : ICapOptionsExtension
    {
        public void AddServices(IServiceCollection services)
        {
           services.Replace(new ServiceDescriptor(typeof(IDataStorage), typeof(EasyNetCapMySqlDataStorage), ServiceLifetime.Singleton));
           services.Replace(new ServiceDescriptor(typeof(ICapTransaction), typeof(EasyNetMySqlCapTransaction), ServiceLifetime.Transient));
        }
    }
}
