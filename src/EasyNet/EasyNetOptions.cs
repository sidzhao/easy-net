using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;
using IServiceCollection = Microsoft.Extensions.DependencyInjection.IServiceCollection;

namespace EasyNet
{
    /// <summary>
    /// Provides programmatic configuration for the EasyNet framework.
    /// </summary>
    public class EasyNetOptions
    {
        public EasyNetOptions()
        {
            RegisterServicesActions = new List<Action<IServiceCollection>>();
        }

        /// <summary>
        /// All related assemblies using EasyNet.
        /// The system looks for the classes that need to be used in these assemblies.
        /// </summary>
        public Assembly[] Assemblies { get; set; }

        /// <summary>
        /// Used to control whether to automatically start a new unit of work before <see cref="IAsyncActionFilter.OnActionExecutionAsync" />.
        /// Default is false.
        /// </summary>
        public bool SuppressAutoBeginUnitOfWork { get; set; }

        /// <summary>
        /// Can be used to suppress automatically setting TenantId on SaveChanges.
        /// Default: false.
        /// </summary>
        public bool SuppressAutoSetTenantId { get; set; }

        /// <summary>
        /// Can be used to suppress automatically setting IsActive on SaveChanges.
        /// Default: false.
        /// </summary>
        public bool SuppressAutoSetIsActive { get; set; }

        internal IList<Action<IServiceCollection>> RegisterServicesActions { get; }

        public void AddRegisterServicesAction(Action<IServiceCollection> action)
        {
            Check.NotNull(action, nameof(action));

            RegisterServicesActions.Add(action);
        }
    }
}
