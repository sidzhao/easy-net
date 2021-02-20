using System;
using System.Reflection;
using EasyNet.DependencyInjection;
using EasyNet.Domain.Uow;
using EasyNet.Ioc;
using Microsoft.Extensions.Options;

namespace EasyNet.Runtime.Initialization
{
    public class EasyNetInitializer : IEasyNetInitializer
    {
        protected readonly IIocResolver IocResolver;
        protected readonly EasyNetOptions Options;
        protected readonly EasyNetInitializerOptions InitializerOptions;

        public EasyNetInitializer(IIocResolver serviceProvider, IOptions<EasyNetOptions> options, IOptions<EasyNetInitializerOptions> initializerOptions)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            Check.NotNull(options, nameof(options));

            IocResolver = serviceProvider;
            Options = options.Value;
            InitializerOptions = initializerOptions.Value;
        }

        public virtual void Init()
        {
            foreach (var jobType in InitializerOptions.JobTypes)
            {
                using (var scope = IocResolver.CreateScope())
                {
                    // 全局定义禁止自动开启工作单元
                    if (Options.SuppressAutoBeginUnitOfWork)
                    {
                        ExecuteJob(scope, jobType);
                        return;
                    }

                    // 使用UnitOfWorkAttribute来创建UnitOfWorkOptions或者禁止自动开启工作单元
                    var attr = jobType.GetCustomAttribute(typeof(UnitOfWorkAttribute));
                    var unitOfWorkOptions = new UnitOfWorkOptions();

                    if (attr != null)
                    {
                        var uowAttr = (UnitOfWorkAttribute)attr;
                        if (uowAttr.SuppressAutoBeginUnitOfWork)
                        {
                            ExecuteJob(scope, jobType);
                            return;
                        }

                        unitOfWorkOptions = UnitOfWorkOptions.Create(uowAttr);
                    }

                    using (var uow = scope.GetService<IUnitOfWorkManager>().Begin(unitOfWorkOptions))
                    {
                        ExecuteJob(scope, jobType);

                        uow.Complete();
                    }
                }
            }
        }

        protected virtual void ExecuteJob(IScopeIocResolver scopeIocResolver, Type jobType)
        {
            if (scopeIocResolver.GetService(jobType) is IEasyNetInitializationJob job)
            {
                job.Start();
            }
            else
            {
                throw new EasyNetException($"Type {jobType.AssemblyQualifiedName} does not inherit {typeof(IEasyNetInitializationJob).AssemblyQualifiedName}.");
            }
        }
    }
}