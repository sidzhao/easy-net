using EasyNet.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EasyNet.Dapper.Tests
{
    public class DapperRepositoryTest
    {
        private readonly IServiceProvider _serviceProvider;

        public DapperRepositoryTest()
        {
            var services = new ServiceCollection();

            services
                .AddEasyNet(x =>
                {
                    x.Assemblies = new[] {this.GetType().Assembly};
                    x.UseSqlLite("Filename=:memory:");
                    x.UseDapper();
                });

            _serviceProvider = services.BuildServiceProvider();

            //InitData();
        }
    }
}
