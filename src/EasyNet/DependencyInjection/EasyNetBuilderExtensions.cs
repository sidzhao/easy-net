using System;
using EasyNet.Mvc;
using EasyNet.Runtime.Initialization;
using EasyNet.Runtime.Session;
using EasyNet.Uow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EasyNet.DependencyInjection
{
    /// <summary>
    /// Extensions for configuring EasyNet using an <see cref="IMvcBuilder"/>.
    /// </summary>
    public static class EasyNetBuilderExtensions
    {
        /// <summary>
        /// Configure <see cref="UnitOfWorkDefaultOptions"/>
        /// </summary>
        /// <param name="builder">The <see cref="EasyNetBuilder"/>.</param>
        /// <param name="setupAction">An <see cref="Action{UnitOfWorkDefaultOptions}"/> to configure the provided <see cref="UnitOfWorkDefaultOptions"/>.</param>
        /// <returns>An <see cref="EasyNetBuilder"/> that can be used to further configure the EasyNet services.</returns>
        public static EasyNetBuilder ConfigureUnitOfWorkDefaultOptions(this EasyNetBuilder builder, Action<UnitOfWorkDefaultOptions> setupAction)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(setupAction, nameof(setupAction));

            builder.Services.Configure(setupAction);

            return builder;
        }

        /// <summary>
        /// Add a job to be executed when EasyNet initialization.
        /// </summary>
        /// <param name="builder">The <see cref="EasyNetBuilder"/>.</param>
        /// <param name="jobTypes">The job which need to be executed when EasyNet initialization.</param>
        /// <returns>An <see cref="EasyNetBuilder"/> that can be used to further configure the EasyNet services.</returns>
        public static EasyNetBuilder AddInitializationJob(this EasyNetBuilder builder, params Type[] jobTypes)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNullOrEmpty(jobTypes, nameof(jobTypes));

            foreach (var type in jobTypes)
            {
                if (!typeof(IEasyNetInitializationJob).IsAssignableFrom(type))
                    throw new EasyNetException($"Type {type.AssemblyQualifiedName} does not inherit {typeof(IEasyNetInitializationJob).AssemblyQualifiedName}.");

                builder.Services.TryAddTransient(type);

                builder.Services.Configure<EasyNetInitializerOptions>(options =>
                {
                    options.JobTypes.Add(type);
                });
            }

            return builder;
        }

        /// <summary>
        /// Add a new <see cref="IEasyNetSession"/> implementation.
        /// </summary>
        /// <typeparam name="TSession"></typeparam>
        /// <param name="builder">The <see cref="EasyNetBuilder"/>.</param>
        /// <returns>An <see cref="EasyNetBuilder"/> that can be used to further configure the EasyNet services.</returns>
        public static EasyNetBuilder AddSession<TSession>(this EasyNetBuilder builder)
            where TSession : IEasyNetSession
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.Replace(new ServiceDescriptor(typeof(IEasyNetSession), typeof(TSession), ServiceLifetime.Scoped));

            return builder;
        }

        /// <summary>
        /// Add a new <see cref="IEasyNetExceptionHandler"/> implementation.
        /// </summary>
        /// <typeparam name="TExceptionHandler"></typeparam>
        /// <param name="builder">The <see cref="EasyNetBuilder"/>.</param>
        /// <returns>An <see cref="EasyNetBuilder"/> that can be used to further configure the EasyNet services.</returns>
        public static EasyNetBuilder AddExceptionHandler<TExceptionHandler>(this EasyNetBuilder builder)
            where TExceptionHandler : IEasyNetExceptionHandler
        {
            Check.NotNull(builder, nameof(builder));

            builder.Services.Replace(new ServiceDescriptor(typeof(IEasyNetExceptionHandler), typeof(TExceptionHandler), ServiceLifetime.Transient));

            return builder;
        }
    }
}
