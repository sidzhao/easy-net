using AutoMapper;
using EasyNet.DependencyInjection;
using EasyNet.Domain.Uow;
using EasyNet.Runtime.Session;

namespace EasyNet
{
    public abstract class EasyNetServiceBase
    {
        protected EasyNetServiceBase(IIocResolver iocResolver)
        {
            IocResolver = iocResolver;
        }

        protected IIocResolver IocResolver { get; }

        protected IUnitOfWorkManager UnitOfWorkManager => _uowManager ?? (_uowManager = IocResolver.GetService<IUnitOfWorkManager>());
        private IUnitOfWorkManager _uowManager;

        protected IActiveUnitOfWork CurrentUnitOfWork => UnitOfWorkManager?.Current;

        protected IEasyNetSession EasyNetSession => _asyNetSession ?? (_asyNetSession = IocResolver.GetService<IEasyNetSession>());
        private IEasyNetSession _asyNetSession;

        protected IMapper ObjectMapper => _objectMapper ?? (_objectMapper = IocResolver.GetService<IMapper>());
        private IMapper _objectMapper;
    }
}
