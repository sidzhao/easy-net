using System;
using Microsoft.Extensions.DependencyInjection;

namespace EasyNet
{
    /// <summary>
    /// Allows fine grained configuration of EasyNet services.
    /// </summary>
    public  class EasyNetBuilder
    {
        public EasyNetBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }
    }
}
