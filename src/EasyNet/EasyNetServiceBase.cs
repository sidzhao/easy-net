using System;
using AutoMapper;
using EasyNet.Runtime.Session;
using EasyNet.Uow;
using Microsoft.Extensions.DependencyInjection;

namespace EasyNet
{
    /// <summary>
    /// This class can be used as a base class for services.
    /// It has some useful objects property-injected and has some basic methods most of services may need to.
    /// </summary>
    public abstract class EasyNetServiceBase
    {
        protected EasyNetServiceBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Reference to <see cref="IUnitOfWorkManager"/>.
        /// </summary>
        protected IUnitOfWorkManager UnitOfWorkManager => _uowManager ?? (_uowManager = ServiceProvider.GetRequiredService<IUnitOfWorkManager>());
        private IUnitOfWorkManager _uowManager;

        /// <summary>
        /// Gets current unit of work.
        /// </summary>
        protected IActiveUnitOfWork CurrentUnitOfWork => UnitOfWorkManager?.Current;

        /// <summary>
        /// Gets current session information.
        /// </summary>
        protected IEasyNetSession CurrentSession => _currentSession ?? (_currentSession = ServiceProvider.GetRequiredService<IEasyNetSession>());
        private IEasyNetSession _currentSession;

        /// <summary>
        /// Reference to the object to object mapper.
        /// </summary>
        protected IMapper ObjectMapper => _objectMapper ?? (_objectMapper = ServiceProvider.GetRequiredService<IMapper>());
        private IMapper _objectMapper;
    }
}
