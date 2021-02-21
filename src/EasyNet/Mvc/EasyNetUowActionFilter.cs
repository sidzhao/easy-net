using System;
using System.Reflection;
using System.Threading.Tasks;
using EasyNet.Domain.Uow;
using EasyNet.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace EasyNet.Mvc
{
    /// <summary>
    /// A filter implementation which delegates to the controller for starting a unit of work.
    /// </summary>
    public class EasyNetUowActionFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly EasyNetOptions _options;

        public EasyNetUowActionFilter(IServiceProvider serviceProvider, IUnitOfWorkManager unitOfWorkManager, IOptions<EasyNetOptions> options)
        {
            _serviceProvider = serviceProvider;
            _unitOfWorkManager = unitOfWorkManager;
            _options = options.Value;
        }

        /// <inheritdoc />
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Check.NotNull(context, nameof(context));
            Check.NotNull(next, nameof(next));

            if (!context.ActionDescriptor.IsControllerAction())
            {
                await next();
                return;
            }

            // 全局定义禁止自动开启工作单元
            if (_options.SuppressAutoBeginUnitOfWork)
            {
                await next();
                return;
            }

            // 使用UnitOfWorkAttribute来创建UnitOfWorkOptions或者禁止自动开启工作单元
            var actionMethodInfo = context.ActionDescriptor.GetMethodInfo();
            var attr = actionMethodInfo?.GetCustomAttribute(typeof(UnitOfWorkAttribute));
            var unitOfWorkOptions = new UnitOfWorkOptions();

            if (attr != null)
            {
                var uowAttr = (UnitOfWorkAttribute)attr;
                if (uowAttr.SuppressAutoBeginUnitOfWork)
                {
                    await next();
                    return;
                }

                unitOfWorkOptions = UnitOfWorkOptions.Create(uowAttr);
            }

            // 开启工作单元
            using (var uow = _unitOfWorkManager.Begin(_serviceProvider, unitOfWorkOptions))
            {
                var result = await next();
                if (result.Exception == null || result.ExceptionHandled)
                {
                    await uow.CompleteAsync();
                }
            }
        }
    }
}
