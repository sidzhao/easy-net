using System;
using System.Reflection;
using System.Threading.Tasks;
using EasyNet.Uow;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace EasyNet.Mvc
{
    public class EasyNetPageFilter : IAsyncPageFilter
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly EasyNetOptions _options;

        public EasyNetPageFilter(IServiceProvider serviceProvider, IUnitOfWorkManager unitOfWorkManager, IOptions<EasyNetOptions> options)
        {
            _serviceProvider = serviceProvider;
            _unitOfWorkManager = unitOfWorkManager;
            _options = options.Value;
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (context.HandlerMethod == null)
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
            var attr = context.HandlerMethod.MethodInfo.GetCustomAttribute(typeof(UnitOfWorkAttribute));
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
