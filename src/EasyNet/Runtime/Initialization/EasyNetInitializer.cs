using System;
using System.Reflection;
using EasyNet.Uow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EasyNet.Runtime.Initialization
{
    public class EasyNetInitializer : IEasyNetInitializer
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly EasyNetOptions Options;
        protected readonly EasyNetInitializerOptions InitializerOptions;

        public EasyNetInitializer(IServiceProvider serviceProvider, IOptions<EasyNetOptions> options, IOptions<EasyNetInitializerOptions> initializerOptions)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));
            Check.NotNull(options, nameof(options));

            ServiceProvider = serviceProvider;
            Options = options.Value;
            InitializerOptions = initializerOptions.Value;
        }

        public virtual void Init()
        {
            foreach (var jobType in InitializerOptions.JobTypes)
            {
                using (var scope = ServiceProvider.CreateScope())
                {
                    // 全局定义禁止自动开启工作单元
                    if (Options.SuppressAutoBeginUnitOfWork)
                    {
                        ExecuteJob(scope.ServiceProvider, jobType);
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
                            ExecuteJob(scope.ServiceProvider, jobType);
                            return;
                        }

                        unitOfWorkOptions = UnitOfWorkOptions.Create(uowAttr);
                    }

                    using (var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>().Begin(scope.ServiceProvider, unitOfWorkOptions))
                    {
                        ExecuteJob(scope.ServiceProvider, jobType);

                        uow.Complete();
                    }
                }
            }
        }

        protected virtual void ExecuteJob(IServiceProvider serviceProvider, Type jobType)
        {
            if (serviceProvider.GetService(jobType) is IEasyNetInitializationJob job)
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