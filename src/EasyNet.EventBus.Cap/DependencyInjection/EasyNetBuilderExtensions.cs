using System;
using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using EasyNet.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EasyNet.EventBus.Cap.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up EasyNet.Identity services in an <see cref="IEasyNetBuilder" />.
    /// </summary>
    public static class EasyNetBuilderExtensions
    {
        public static IEasyNetBuilder AddEventBus(this IEasyNetBuilder builder, Action<CapOptions> setupAction)
        {
            // 为了将CAP集成在EasyNet中，重新注册一些新的服务
            builder.Services.TryAddSingleton<IConsumerServiceSelector, EasyNetCapConsumerServiceSelector>();
            builder.Services.TryAddSingleton<ISubscribeInvoker, EasyNetSubscribeInvoker>();

            builder.Services.AddCap(setupAction);

            builder.Services.TryAddScoped<IEasyNetEventBus, EasyNetEventBus>();
            builder.Services.TryAddScoped<IEasyNetEventMessageBuffer, EasyNetEventMessageBuffer>();

            return builder;
        }
    }
}
