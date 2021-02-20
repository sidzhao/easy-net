using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP.Internal;
using DotNetCore.CAP.Serialization;
using EasyNet.Domain.Uow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyNet.EventBus.Cap
{
    public class EasyNetSubscribeInvoker : ISubscribeInvoker
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISerializer _serializer;
        private readonly EasyNetOptions _options;

        public EasyNetSubscribeInvoker(ILoggerFactory loggerFactory, IServiceProvider serviceProvider, ISerializer serializer, IOptions<EasyNetOptions> options)
        {
            _loggerFactory = loggerFactory;
            _serviceProvider = serviceProvider;
            _serializer = serializer;
            _options = options.Value;
        }

        public async Task<ConsumerExecutedResult> InvokeAsync(ConsumerContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                // 全局定义禁止自动开启工作单元
                if (_options.SuppressAutoBeginUnitOfWork)
                {
                    return await InternalInvokeAsync(scope.ServiceProvider, context, cancellationToken);
                }

                // 使用UnitOfWorkAttribute来创建UnitOfWorkOptions或者禁止自动开启工作单元
                var attr = context.ConsumerDescriptor.ImplTypeInfo?.GetCustomAttribute(typeof(UnitOfWorkAttribute));
                var unitOfWorkOptions = new UnitOfWorkOptions();

                if (attr != null)
                {
                    var uowAttr = (UnitOfWorkAttribute)attr;

                    if (uowAttr.SuppressAutoBeginUnitOfWork)
                    {
                        return await InternalInvokeAsync(scope.ServiceProvider, context, cancellationToken);
                    }

                    unitOfWorkOptions = UnitOfWorkOptions.Create(uowAttr);
                }

                // 开启工作单元
                using (var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>().Begin(unitOfWorkOptions))
                {
                    var result = await InternalInvokeAsync(scope.ServiceProvider, context, cancellationToken);

                    await uow.CompleteAsync();

                    return result;
                }
            }
        }
         
        private async Task<ConsumerExecutedResult> InternalInvokeAsync(IServiceProvider provider, ConsumerContext context, CancellationToken cancellationToken)
        {
            var capSubscribeInvoker = new SubscribeInvoker(_loggerFactory, provider, _serializer);
            var result = await capSubscribeInvoker.InvokeAsync(context, cancellationToken);

            return result;
        }
    }
}
