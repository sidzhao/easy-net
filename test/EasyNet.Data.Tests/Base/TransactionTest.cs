using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using System.Transactions;
using EasyNet.CommonTests.Common;
using EasyNet.CommonTests.Common.Entities;
using EasyNet.Data.Repositories;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.Uow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace EasyNet.Data.Tests.Base
{
    public abstract class TransactionTest
    {
        protected IServiceProvider ServiceProvider;

        protected abstract bool IsEfCore { get; }

        protected abstract string ConnectionString { get; }

        protected abstract string PrefixName { get; }

        protected abstract void Use(EasyNetOptions options);

        [Fact]
        public async Task TestTransaction()
        {
            var prefixName = PrefixName + "TestTransactionSuccess";

            using var host = await new HostBuilder()
               .ConfigureWebHost(webBuilder =>
               {
                   webBuilder
                       .UseTestServer()
                       .ConfigureServices(services =>
                       {
                           services.AddEasyNet(Use);
                       })
                       .Configure(app =>
                       {
                           app.Map(new PathString("/init"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;

                                   var dbContext = new EfCoreContext(new DbContextOptionsBuilder().UseSqlServer(ConnectionString).Options);
                                   await dbContext.Database.EnsureCreatedAsync();

                                   var currentDbConnectorProvider = serviceProvider.GetService<ICurrentDbConnectorProvider>();
                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   await userRepo.DeleteAsync(p => p.Name.Contains(prefixName));
                                   await roleRepo.DeleteAsync(p => p.Name.Contains(prefixName));

                                   if (IsEfCore)
                                   {
                                       await currentDbConnectorProvider.Current.GetDbContext().SaveChangesAsync();
                                   }
                               });
                           });

                           app.Map(new PathString("/insert"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;

                                   using var uow = serviceProvider.GetService<IUnitOfWorkManager>()
                                       .Begin(serviceProvider);

                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   var roleId = await roleRepo.InsertAndGetIdAsync(new Role
                                   {
                                       Name = $"{prefixName}1"
                                   });

                                   await userRepo.InsertAsync(new User
                                   {
                                       TenantId = 1,
                                       Name = $"{prefixName}1",
                                       RoleId = roleId
                                   });

                                   await uow.CompleteAsync();
                               });
                           });

                           app.Map(new PathString("/verify"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;

                                   using var uow = serviceProvider.GetService<IUnitOfWorkManager>()
                                       .Begin(serviceProvider);

                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   // Assert
                                   using (((IUnitOfWork)uow).DisableFilter(EasyNetDataFilters.MustHaveTenant, EasyNetDataFilters.MayHaveTenant))
                                   {
                                       Assert.NotNull(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}1"));
                                       Assert.NotNull(await roleRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}1"));
                                   }
                               });
                           });
                       });
               })
               .StartAsync();

            await host.GetTestClient().GetAsync("/init");
            await host.GetTestClient().GetAsync("/insert");
            await host.GetTestClient().GetAsync("/verify");
        }

        [Fact]
        public async Task TestTransactionRollback()
        {
            var prefixName = PrefixName + "TestTransactionRollback";

            using var host = await new HostBuilder()
               .ConfigureWebHost(webBuilder =>
               {
                   webBuilder
                       .UseTestServer()
                       .ConfigureServices(services =>
                       {
                           services.AddEasyNet(Use);
                       })
                       .Configure(app =>
                       {
                           app.Map(new PathString("/init"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;

                                   var dbContext = new EfCoreContext(new DbContextOptionsBuilder().UseSqlServer(ConnectionString).Options);
                                   await dbContext.Database.EnsureCreatedAsync();

                                   var currentDbConnectorProvider = serviceProvider.GetService<ICurrentDbConnectorProvider>();

                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   await userRepo.DeleteAsync(p => p.Name.Contains(prefixName));
                                   await roleRepo.DeleteAsync(p => p.Name.Contains(prefixName));

                                   if (IsEfCore)
                                   {
                                       await currentDbConnectorProvider.Current.GetDbContext().SaveChangesAsync();
                                   }
                               });
                           });

                           app.Map(new PathString("/insert"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;

                                   using var uow = serviceProvider.GetService<IUnitOfWorkManager>()
                                       .Begin(serviceProvider);

                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   var roleId = await roleRepo.InsertAndGetIdAsync(new Role
                                   {
                                       Name = $"{prefixName}1"
                                   });

                                   await userRepo.InsertAsync(new User
                                   {
                                       TenantId = 1,
                                       Name = $"{prefixName}1",
                                       RoleId = roleId
                                   });

                                   throw new Exception();
                               });
                           });

                           app.Map(new PathString("/verify"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;

                                   using var uow = serviceProvider.GetService<IUnitOfWorkManager>()
                                       .Begin(serviceProvider);

                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   // Assert
                                   using (((IUnitOfWork)uow).DisableFilter(EasyNetDataFilters.MustHaveTenant, EasyNetDataFilters.MayHaveTenant))
                                   {
                                       Assert.Null(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}1"));
                                       Assert.Null(await roleRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}1"));
                                   }
                               });
                           });
                       });
               })
               .StartAsync();

            await host.GetTestClient().GetAsync("/init");
            await Assert.ThrowsAsync<Exception>(async () => await host.GetTestClient().GetAsync("/insert"));
            await host.GetTestClient().GetAsync("/verify");
        }

        [Fact]
        public async Task TestNestingTransactionWithRequiresNewTransactionScope()
        {
            var prefixName = PrefixName + "TestNestingTransactionWithRequiresNewTransactionScope";

            using var host = await new HostBuilder()
               .ConfigureWebHost(webBuilder =>
               {
                   webBuilder
                       .UseTestServer()
                       .ConfigureServices(services =>
                       {
                           services.AddEasyNet(Use);
                       })
                       .Configure(app =>
                       {
                           app.Map(new PathString("/init"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;

                                   var dbContext = new EfCoreContext(new DbContextOptionsBuilder().UseSqlServer(ConnectionString).Options);
                                   await dbContext.Database.EnsureCreatedAsync();

                                   var currentDbConnectorProvider = serviceProvider.GetService<ICurrentDbConnectorProvider>();
                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   await userRepo.DeleteAsync(p => p.Name.Contains(prefixName));
                                   await roleRepo.DeleteAsync(p => p.Name.Contains(prefixName));

                                   if (IsEfCore)
                                   {
                                       await currentDbConnectorProvider.Current.GetDbContext().SaveChangesAsync();
                                   }
                               });
                           });

                           app.Map(new PathString("/insert1"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;
                                   var uowManager = serviceProvider.GetService<IUnitOfWorkManager>();
                                   var currentDbConnectorProvider = serviceProvider.GetService<ICurrentDbConnectorProvider>();

                                   using var uow = uowManager.Begin(serviceProvider);

                                   var dbConnector = currentDbConnectorProvider.GetOrCreate();
                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   Assert.Equal(dbConnector, currentDbConnectorProvider.Current);

                                   using (var uow1 = uowManager.Begin(serviceProvider, TransactionScopeOption.RequiresNew))
                                   {
                                       var role1Id = await roleRepo.InsertAndGetIdAsync(new Role
                                       {
                                           Name = $"{prefixName}1"
                                       });

                                       await userRepo.InsertAsync(new User
                                       {
                                           TenantId = 1,
                                           Name = $"{prefixName}1",
                                           RoleId = role1Id
                                       });

                                       Assert.NotEqual(((IUnitOfWork)uow).DbConnector, ((IUnitOfWork)uow1).DbConnector);

                                       await uow1.CompleteAsync();
                                   }

                                   await userRepo.InsertAsync(new User
                                   {
                                       TenantId = 1,
                                       Name = $"{prefixName}2",
                                       RoleId = (await roleRepo.SingleAsync(p => p.Name == $"{prefixName}1")).Id
                                   });

                                   await uow.CompleteAsync();
                               });
                           });

                           app.Map(new PathString("/verify1"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;

                                   using var uow = serviceProvider.GetService<IUnitOfWorkManager>()
                                       .Begin(serviceProvider);

                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   // Assert
                                   using (((IUnitOfWork)uow).DisableFilter(EasyNetDataFilters.MustHaveTenant, EasyNetDataFilters.MayHaveTenant))
                                   {
                                       Assert.NotNull(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}1"));
                                       Assert.NotNull(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}2"));
                                       Assert.NotNull(await roleRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}1"));
                                   }
                               });
                           });

                           app.Map(new PathString("/insert2"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;
                                   var uowManager = serviceProvider.GetService<IUnitOfWorkManager>();
                                   var currentDbConnectorProvider = serviceProvider.GetService<ICurrentDbConnectorProvider>();

                                   using var uow = uowManager.Begin(serviceProvider);

                                   currentDbConnectorProvider.GetOrCreate();
                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   using (var uow1 = uowManager.Begin(serviceProvider, TransactionScopeOption.RequiresNew))
                                   {
                                       var role1Id = await roleRepo.InsertAndGetIdAsync(new Role
                                       {
                                           Name = $"{prefixName}3"
                                       });

                                       await userRepo.InsertAsync(new User
                                       {
                                           TenantId = 1,
                                           Name = $"{prefixName}3",
                                           RoleId = role1Id
                                       });

                                       // Don't call complete function
                                   }

                                   await userRepo.InsertAsync(new User
                                   {
                                       TenantId = 1,
                                       Name = $"{prefixName}4",
                                       RoleId = (await roleRepo.SingleAsync(p => p.Name == $"{prefixName}1")).Id
                                   });

                                   await uow.CompleteAsync();
                               });
                           });

                           app.Map(new PathString("/verify2"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;

                                   using var uow = serviceProvider.GetService<IUnitOfWorkManager>()
                                       .Begin(serviceProvider);

                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   // Assert
                                   using (((IUnitOfWork)uow).DisableFilter(EasyNetDataFilters.MustHaveTenant, EasyNetDataFilters.MayHaveTenant))
                                   {
                                       Assert.Null(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}3"));
                                       Assert.NotNull(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}4"));
                                       Assert.Null(await roleRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}3"));
                                   }
                               });
                           });
                       });
               })
               .StartAsync();

            await host.GetTestClient().GetAsync("/init");
            await host.GetTestClient().GetAsync("/insert1");
            await host.GetTestClient().GetAsync("/verify1");
            await host.GetTestClient().GetAsync("/insert2");
            await host.GetTestClient().GetAsync("/verify2");
        }

        [Fact]
        public async Task TestNestingTransactionWithRequiredScope()
        {
            var prefixName = PrefixName + "TestNestingTransactionWithRequiredScope";

            using var host = await new HostBuilder()
               .ConfigureWebHost(webBuilder =>
               {
                   webBuilder
                       .UseTestServer()
                       .ConfigureServices(services =>
                       {
                           services.AddEasyNet(Use);
                       })
                       .Configure(app =>
                       {
                           app.Map(new PathString("/init"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;

                                   var dbContext = new EfCoreContext(new DbContextOptionsBuilder().UseSqlServer(ConnectionString).Options);
                                   await dbContext.Database.EnsureCreatedAsync();

                                   var currentDbConnectorProvider = serviceProvider.GetService<ICurrentDbConnectorProvider>();
                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   await userRepo.DeleteAsync(p => p.Name.Contains(prefixName));
                                   await roleRepo.DeleteAsync(p => p.Name.Contains(prefixName));

                                   if (IsEfCore)
                                   {
                                       await currentDbConnectorProvider.Current.GetDbContext().SaveChangesAsync();
                                   }
                               });
                           });

                           app.Map(new PathString("/insert1"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;
                                   var uowManager = serviceProvider.GetService<IUnitOfWorkManager>();
                                   var currentDbConnectorProvider = serviceProvider.GetService<ICurrentDbConnectorProvider>();

                                   using var uow = uowManager.Begin(serviceProvider);

                                   var dbConnector = currentDbConnectorProvider.GetOrCreate();
                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   Assert.Equal(dbConnector, currentDbConnectorProvider.Current);

                                   using (var uow1 = uowManager.Begin(serviceProvider, TransactionScopeOption.Required))
                                   {
                                       var role1Id = await roleRepo.InsertAndGetIdAsync(new Role
                                       {
                                           Name = $"{prefixName}1"
                                       });

                                       await userRepo.InsertAsync(new User
                                       {
                                           TenantId = 1,
                                           Name = $"{prefixName}1",
                                           RoleId = role1Id
                                       });

                                       Assert.True(uow1 is InnerUnitOfWorkCompleteHandle);

                                       await uow1.CompleteAsync();

                                       Assert.NotNull(await roleRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}1"));
                                       Assert.Null(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}1"));
                                   }

                                   await userRepo.InsertAsync(new User
                                   {
                                       TenantId = 1,
                                       Name = $"{prefixName}2",
                                       RoleId = (await roleRepo.SingleAsync(p => p.Name == $"{prefixName}1")).Id
                                   });

                                   await uow.CompleteAsync();
                               });
                           });

                           app.Map(new PathString("/verify1"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;

                                   using var uow = serviceProvider.GetService<IUnitOfWorkManager>()
                                       .Begin(serviceProvider);

                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   // Assert
                                   using (((IUnitOfWork)uow).DisableFilter(EasyNetDataFilters.MustHaveTenant, EasyNetDataFilters.MayHaveTenant))
                                   {
                                       Assert.NotNull(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}1"));
                                       Assert.NotNull(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}2"));
                                       Assert.NotNull(await roleRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}1"));
                                   }
                               });
                           });

                           app.Map(new PathString("/insert2"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;
                                   var uowManager = serviceProvider.GetService<IUnitOfWorkManager>();
                                   var currentDbConnectorProvider = serviceProvider.GetService<ICurrentDbConnectorProvider>();

                                   using var uow = uowManager.Begin(serviceProvider);

                                   currentDbConnectorProvider.GetOrCreate();
                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   using (var uow1 = uowManager.Begin(serviceProvider, TransactionScopeOption.Required))
                                   {
                                       var role1Id = await roleRepo.InsertAndGetIdAsync(new Role
                                       {
                                           Name = $"{prefixName}3"
                                       });

                                       await userRepo.InsertAsync(new User
                                       {
                                           TenantId = 1,
                                           Name = $"{prefixName}3",
                                           RoleId = role1Id
                                       });

                                       await uow1.CompleteAsync();
                                   }

                                   await userRepo.InsertAsync(new User
                                   {
                                       TenantId = 1,
                                       Name = $"{prefixName}4",
                                       RoleId = (await roleRepo.SingleAsync(p => p.Name == $"{prefixName}1")).Id
                                   });

                                   // Don't call complete function
                               });
                           });

                           app.Map(new PathString("/verify2"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;

                                   using var uow = serviceProvider.GetService<IUnitOfWorkManager>()
                                       .Begin(serviceProvider);

                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   // Assert
                                   using (((IUnitOfWork)uow).DisableFilter(EasyNetDataFilters.MustHaveTenant, EasyNetDataFilters.MayHaveTenant))
                                   {
                                       Assert.Equal(1,  await roleRepo.CountAsync(p => p.Name.Contains($"{prefixName}")));
                                       Assert.Equal(2, await userRepo.CountAsync(p => p.Name.Contains($"{prefixName}")));
                                       Assert.Null(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}3"));
                                       Assert.Null(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}4"));
                                       Assert.Null(await roleRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}3"));
                                   }
                               });
                           });

                           app.Map(new PathString("/insert3"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;
                                   var uowManager = serviceProvider.GetService<IUnitOfWorkManager>();
                                   var currentDbConnectorProvider = serviceProvider.GetService<ICurrentDbConnectorProvider>();

                                   using var uow = uowManager.Begin(serviceProvider);

                                   currentDbConnectorProvider.GetOrCreate();
                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   var role1Id = await roleRepo.InsertAndGetIdAsync(new Role
                                   {
                                       Name = $"{prefixName}5"
                                   });

                                   await userRepo.InsertAsync(new User
                                   {
                                       TenantId = 1,
                                       Name = $"{prefixName}5",
                                       RoleId = role1Id
                                   });

                                   using (var uow1 = uowManager.Begin(serviceProvider, TransactionScopeOption.Required))
                                   {
                                       await userRepo.InsertAsync(new User
                                       {
                                           TenantId = 1,
                                           Name = $"{prefixName}6",
                                           RoleId = (await roleRepo.SingleAsync(p => p.Name == $"{prefixName}1")).Id
                                       });

                                       throw new Exception();
                                   }
                               });
                           });

                           app.Map(new PathString("/verify3"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;

                                   using var uow = serviceProvider.GetService<IUnitOfWorkManager>()
                                       .Begin(serviceProvider);

                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   // Assert
                                   using (((IUnitOfWork)uow).DisableFilter(EasyNetDataFilters.MustHaveTenant, EasyNetDataFilters.MayHaveTenant))
                                   {
                                       Assert.Equal(1, await roleRepo.CountAsync(p => p.Name.Contains($"{prefixName}")));
                                       Assert.Equal(2, await userRepo.CountAsync(p => p.Name.Contains($"{prefixName}")));
                                       Assert.Null(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}5"));
                                       Assert.Null(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}6"));
                                       Assert.Null(await roleRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}5"));
                                   }
                               });
                           });
                       });
               })
               .StartAsync();

            await host.GetTestClient().GetAsync("/init");
            await host.GetTestClient().GetAsync("/insert1");
            await host.GetTestClient().GetAsync("/verify1");
            await host.GetTestClient().GetAsync("/insert2");
            await host.GetTestClient().GetAsync("/verify2");
            await Assert.ThrowsAsync<Exception>(async () => await host.GetTestClient().GetAsync("/insert3"));
            await host.GetTestClient().GetAsync("/verify3");
        }

        [Fact]
        public async Task TestNestingTransactionWithSuppressTransactionScope()
        {
            var prefixName = PrefixName + "TestNestingTransactionWithSuppressTransactionScope";

            using var host = await new HostBuilder()
               .ConfigureWebHost(webBuilder =>
               {
                   webBuilder
                       .UseTestServer()
                       .ConfigureServices(services =>
                       {
                           services.AddEasyNet(Use);
                       })
                       .Configure(app =>
                       {
                           app.Map(new PathString("/init"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;

                                   var dbContext = new EfCoreContext(new DbContextOptionsBuilder().UseSqlServer(ConnectionString).Options);
                                   await dbContext.Database.EnsureCreatedAsync();

                                   var currentDbConnectorProvider = serviceProvider.GetService<ICurrentDbConnectorProvider>();
                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   await userRepo.DeleteAsync(p => p.Name.Contains(prefixName));
                                   await roleRepo.DeleteAsync(p => p.Name.Contains(prefixName));

                                   if (IsEfCore)
                                   {
                                       await currentDbConnectorProvider.Current.GetDbContext().SaveChangesAsync();
                                   }
                               });
                           });

                           app.Map(new PathString("/insert1"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;
                                   var uowManager = serviceProvider.GetService<IUnitOfWorkManager>();
                                   var currentDbConnectorProvider = serviceProvider.GetService<ICurrentDbConnectorProvider>();

                                   using var uow = uowManager.Begin(serviceProvider, TransactionScopeOption.Suppress);

                                   var dbConnector = currentDbConnectorProvider.GetOrCreate();
                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   Assert.Equal(dbConnector, currentDbConnectorProvider.Current);

                                   using (var uow1 = uowManager.Begin(serviceProvider, TransactionScopeOption.Required))
                                   {
                                       var role1Id = await roleRepo.InsertAndGetIdAsync(new Role
                                       {
                                           Name = $"{prefixName}1"
                                       });

                                       await userRepo.InsertAsync(new User
                                       {
                                           TenantId = 1,
                                           Name = $"{prefixName}1",
                                           RoleId = role1Id
                                       });

                                       Assert.True(uow1 is InnerSuppressUnitOfWorkCompleteHandle);
                                       Assert.Equal(uow, uow1.GetPrivateField<IUnitOfWork>("_parentUnitOfWork"));

                                       await uow1.CompleteAsync();

                                       using (((IUnitOfWork) uow).DisableFilter(EasyNetDataFilters.MustHaveTenant,
                                           EasyNetDataFilters.MayHaveTenant))
                                       {
                                           Assert.NotNull(
                                               await roleRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}1"));
                                           Assert.NotNull(
                                               await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}1"));
                                       }
                                   }

                                   await userRepo.InsertAsync(new User
                                   {
                                       TenantId = 1,
                                       Name = $"{prefixName}2",
                                       RoleId = (await roleRepo.SingleAsync(p => p.Name == $"{prefixName}1")).Id
                                   });

                                   await uow.CompleteAsync();
                               });
                           });

                           app.Map(new PathString("/verify1"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;

                                   using var uow = serviceProvider.GetService<IUnitOfWorkManager>()
                                       .Begin(serviceProvider);

                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   // Assert
                                   using (((IUnitOfWork)uow).DisableFilter(EasyNetDataFilters.MustHaveTenant, EasyNetDataFilters.MayHaveTenant))
                                   {
                                       Assert.NotNull(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}1"));
                                       Assert.NotNull(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}2"));
                                       Assert.NotNull(await roleRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}1"));
                                   }
                               });
                           });

                           app.Map(new PathString("/insert2"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;
                                   var uowManager = serviceProvider.GetService<IUnitOfWorkManager>();
                                   var currentDbConnectorProvider = serviceProvider.GetService<ICurrentDbConnectorProvider>();

                                   using var uow = uowManager.Begin(serviceProvider, TransactionScopeOption.Suppress);

                                   currentDbConnectorProvider.GetOrCreate();
                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   using (var uow1 = uowManager.Begin(serviceProvider, TransactionScopeOption.Required))
                                   {
                                       var role1Id = await roleRepo.InsertAndGetIdAsync(new Role
                                       {
                                           Name = $"{prefixName}3"
                                       });

                                       await userRepo.InsertAsync(new User
                                       {
                                           TenantId = 1,
                                           Name = $"{prefixName}3",
                                           RoleId = role1Id
                                       });

                                       await uow1.CompleteAsync();
                                   }

                                   await userRepo.InsertAsync(new User
                                   {
                                       TenantId = 1,
                                       Name = $"{prefixName}4",
                                       RoleId = (await roleRepo.SingleAsync(p => p.Name == $"{prefixName}1")).Id
                                   });

                                   // Don't call complete function
                               });
                           });

                           app.Map(new PathString("/verify2"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;

                                   using var uow = serviceProvider.GetService<IUnitOfWorkManager>()
                                       .Begin(serviceProvider);

                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   // Assert
                                   using (((IUnitOfWork)uow).DisableFilter(EasyNetDataFilters.MustHaveTenant, EasyNetDataFilters.MayHaveTenant))
                                   {
                                       Assert.Equal(1, await roleRepo.CountAsync(p => p.Name.Contains($"{prefixName}")));
                                       Assert.Equal(2, await userRepo.CountAsync(p => p.Name.Contains($"{prefixName}")));
                                       Assert.Null(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}3"));
                                       Assert.Null(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}4"));
                                       Assert.Null(await roleRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}3"));
                                   }
                               });
                           });

                           app.Map(new PathString("/insert3"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;
                                   var uowManager = serviceProvider.GetService<IUnitOfWorkManager>();
                                   var currentDbConnectorProvider = serviceProvider.GetService<ICurrentDbConnectorProvider>();

                                   using var uow = uowManager.Begin(serviceProvider, TransactionScopeOption.Suppress);

                                   currentDbConnectorProvider.GetOrCreate();
                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   var role1Id = await roleRepo.InsertAndGetIdAsync(new Role
                                   {
                                       Name = $"{prefixName}5"
                                   });

                                   await userRepo.InsertAsync(new User
                                   {
                                       TenantId = 1,
                                       Name = $"{prefixName}5",
                                       RoleId = role1Id
                                   });

                                   using (var uow1 = uowManager.Begin(serviceProvider, TransactionScopeOption.Required))
                                   {
                                       await userRepo.InsertAsync(new User
                                       {
                                           TenantId = 1,
                                           Name = $"{prefixName}6",
                                           RoleId = (await roleRepo.SingleAsync(p => p.Name == $"{prefixName}1")).Id
                                       });

                                       throw new Exception();
                                   }
                               });
                           });

                           app.Map(new PathString("/verify3"), builder =>
                           {
                               builder.Run(async context =>
                               {
                                   var serviceProvider = context.RequestServices;

                                   using var uow = serviceProvider.GetService<IUnitOfWorkManager>()
                                       .Begin(serviceProvider);

                                   var userRepo = serviceProvider.GetService<IRepository<User, long>>();
                                   var roleRepo = serviceProvider.GetService<IRepository<Role>>();

                                   // Assert
                                   using (((IUnitOfWork)uow).DisableFilter(EasyNetDataFilters.MustHaveTenant, EasyNetDataFilters.MayHaveTenant))
                                   {
                                       Assert.Equal(1, await roleRepo.CountAsync(p => p.Name.Contains($"{prefixName}")));
                                       Assert.Equal(2, await userRepo.CountAsync(p => p.Name.Contains($"{prefixName}")));
                                       Assert.Null(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}5"));
                                       Assert.Null(await userRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}6"));
                                       Assert.Null(await roleRepo.SingleOrDefaultAsync(p => p.Name == $"{prefixName}5"));
                                   }
                               });
                           });
                       });
               })
               .StartAsync();

            await host.GetTestClient().GetAsync("/init");
            await host.GetTestClient().GetAsync("/insert1");
            await host.GetTestClient().GetAsync("/verify1");
            await host.GetTestClient().GetAsync("/insert2");
            await host.GetTestClient().GetAsync("/verify2");
            await Assert.ThrowsAsync<Exception>(async () => await host.GetTestClient().GetAsync("/insert3"));
            await host.GetTestClient().GetAsync("/verify3");
        }
    }
}
