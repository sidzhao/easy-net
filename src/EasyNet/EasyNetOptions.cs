using System.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EasyNet
{
    /// <summary>
    /// Provides programmatic configuration for the EasyNet framework.
    /// </summary>
    public class EasyNetOptions
    {
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

        /// <summary>
        /// 使用EasyNet的所有相关程序集
        /// </summary>
        public Assembly[] Assemblies { get; set; }
    }
}
