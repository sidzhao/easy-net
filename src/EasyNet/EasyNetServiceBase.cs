using AutoMapper;
using EasyNet.DependencyInjection;
using EasyNet.Domain.Uow;
using EasyNet.Ioc;
using EasyNet.Runtime.Session;

namespace EasyNet
{
    /// <summary>
    /// This class can be used as a base class for services.
    /// It has some useful objects property-injected and has some basic methods most of services may need to.
    /// </summary>
    public abstract class EasyNetServiceBase
    {
        protected EasyNetServiceBase(IIocResolver iocResolver)
        {
            IocResolver = iocResolver;
        }

        protected IIocResolver IocResolver { get; }

        /// <summary>
        /// Reference to <see cref="IUnitOfWorkManager"/>.
        /// </summary>
        protected IUnitOfWorkManager UnitOfWorkManager => _uowManager ?? (_uowManager = IocResolver.GetService<IUnitOfWorkManager>());
        private IUnitOfWorkManager _uowManager;

        /// <summary>
        /// Gets current unit of work.
        /// </summary>
        protected IActiveUnitOfWork CurrentUnitOfWork => UnitOfWorkManager?.Current;

        /// <summary>
        /// Gets current session information.
        /// </summary>
        protected IEasyNetSession EasyNetSession => _asyNetSession ?? (_asyNetSession = IocResolver.GetService<IEasyNetSession>());
        private IEasyNetSession _asyNetSession;

        /// <summary>
        /// Reference to the object to object mapper.
        /// </summary>
        protected IMapper ObjectMapper => _objectMapper ?? (_objectMapper = IocResolver.GetService<IMapper>());
        private IMapper _objectMapper;
    }
}
