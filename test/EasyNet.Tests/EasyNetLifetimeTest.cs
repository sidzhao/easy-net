using System.Collections.Generic;
using System.Threading.Tasks;
using EasyNet.Data;
using EasyNet.Uow;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace EasyNet.Tests
{
    public class EasyNetLifetimeTest
    {
        [Fact]
        public async Task TestCurrentUnitOfWorkProvider()
        {
            ICurrentUnitOfWorkProvider rootCurrentUnitOfWorkProvider = null;

            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .ConfigureServices(services =>
                        {
                            services.AddEasyNet();
                        })
                        .Configure(app =>
                        {
                            app.Run(async context =>
                            {
                                var currentUnitOfWorkProvider = context.RequestServices.GetRequiredService<ICurrentUnitOfWorkProvider>();

                                // ReSharper disable once AccessToModifiedClosure
                                Assert.Equal(rootCurrentUnitOfWorkProvider, currentUnitOfWorkProvider);
                                Assert.Null(currentUnitOfWorkProvider.Current);

                                await Task.Run(async () =>
                                {
                                    var asyncUow = context.RequestServices.GetRequiredService<IUnitOfWork>();
                                    IUnitOfWork asyncUow1 = null;
                                    var asyncUnitOfWorkProvider = context.RequestServices.GetRequiredService<ICurrentUnitOfWorkProvider>();
                                    asyncUnitOfWorkProvider.Current = asyncUow;

                                    // ReSharper disable once AccessToModifiedClosure
                                    Assert.Equal(rootCurrentUnitOfWorkProvider, asyncUnitOfWorkProvider);
                                    Assert.Equal(asyncUnitOfWorkProvider.Current, asyncUow);

                                    await Task.Run(() =>
                                    {
                                        var asyncUnitOfWorkProvider1 = context.RequestServices.GetRequiredService<ICurrentUnitOfWorkProvider>();

                                        // ReSharper disable once AccessToModifiedClosure
                                        Assert.Equal(rootCurrentUnitOfWorkProvider, asyncUnitOfWorkProvider1);
                                        Assert.Equal(asyncUnitOfWorkProvider1.Current, asyncUow);

                                        asyncUow1 = context.RequestServices.GetRequiredService<IUnitOfWork>();
                                        asyncUnitOfWorkProvider1.Current = asyncUow1;
                                        Assert.Equal(asyncUnitOfWorkProvider1.Current, asyncUow1);
                                    });

                                    Assert.Equal(asyncUnitOfWorkProvider.Current, asyncUow1);
                                });
                            });
                        });
                })
                .StartAsync();

            var rootUow = host.Services.GetRequiredService<IUnitOfWork>();
            rootCurrentUnitOfWorkProvider = host.Services.GetRequiredService<ICurrentUnitOfWorkProvider>();
            rootCurrentUnitOfWorkProvider.Current = rootUow;

            Assert.Equal(rootUow, rootCurrentUnitOfWorkProvider.Current);

            async Task RequestAsync(IHost paramHost)
            {
                await paramHost.GetTestClient().GetAsync("/");
                Assert.Equal(rootUow, rootCurrentUnitOfWorkProvider.Current);
            };

            await Task.WhenAll(new List<Task>
            {
                RequestAsync(host),
                RequestAsync(host),
                RequestAsync(host)
            });

            Assert.Equal(rootUow, rootCurrentUnitOfWorkProvider.Current);
        }

        [Fact]
        public async Task TestDbConnectorProviderWithoutUow()
        {
            var dbConnectorMockList = new List<Mock<DbConnector>>();

            using var host = await new HostBuilder()
              .ConfigureWebHost(webBuilder =>
              {
                  webBuilder
                      .UseTestServer()
                      .ConfigureServices(services =>
                      {
                          services.AddEasyNet();

                          services.AddScoped(provider =>
                          {
                              var dbConnectorCreatorMock = new Mock<IDbConnectorCreator>();
                              var dbConnectorMock = new Mock<DbConnector>();

                              dbConnectorCreatorMock
                                  .Setup(p => p.Create(false, null))
                                  .Returns(dbConnectorMock.Object);

                              dbConnectorMockList.Add(dbConnectorMock);

                              return dbConnectorCreatorMock.Object;
                          });
                      })
                      .Configure(app =>
                      {
                          app.Run(async context =>
                          {
                              var currentDbConnectorProvider =
                                  context.RequestServices.GetService<ICurrentDbConnectorProvider>();

                              var dbConnector = currentDbConnectorProvider.GetOrCreate();
                              Assert.Equal(dbConnector, currentDbConnectorProvider.Current);

                              var dbConnector1 = currentDbConnectorProvider.GetOrCreate();
                              Assert.Equal(dbConnector, dbConnector1);
                              Assert.Equal(dbConnector1, currentDbConnectorProvider.Current);

                              await Task.CompletedTask;
                          });
                      });
              })
              .StartAsync();

            async Task RequestAsync(IHost paramHost)
            {
                await paramHost.GetTestClient().GetAsync("/");
            };

            await Task.WhenAll(new List<Task>
            {
                RequestAsync(host),
                RequestAsync(host),
                RequestAsync(host)
            });

            foreach (var dbConnectorMock in dbConnectorMockList)
            {
                dbConnectorMock.Verify(p => p.Dispose(), Times.Once);
            }
        }
    }
}
