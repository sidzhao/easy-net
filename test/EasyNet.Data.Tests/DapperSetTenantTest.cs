﻿using EasyNet.CommonTests.Common;
using EasyNet.CommonTests.Common.Entities;
using EasyNet.Data.Tests.Base;
using EasyNet.DependencyInjection;
using EasyNet.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EasyNet.Data.Tests
{
    public class DapperSetTenantTest : SetTenantTest
    {
        public DapperSetTenantTest()
        {
            var services = new ServiceCollection();

            services
                .AddEasyNet(x =>
                {
                    x.Assemblies = new[] { this.GetType().Assembly };
                    x.UseSqlite("Filename=:memory:");
                    x.UseDapper().AsDefault(typeof(User).Assembly);
                })
                .AddSession<TestSession>()
                .AddCurrentDbConnectorProvider<TestCurrentDbConnectorProvider>();

            ServiceProvider = services.BuildServiceProvider();
        }

        protected override bool UseEfCoreSaveChanges()
        {
            return false;
        }
    }
}
