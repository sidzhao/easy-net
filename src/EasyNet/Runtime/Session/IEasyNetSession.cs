namespace EasyNet.Runtime.Session
{
    /// <summary>
    /// Defines some session information that can be useful for applications.
    /// </summary>
    public interface IEasyNetSession
    {
        /// <summary>
        /// Gets current UserId or null.
        /// It can be empty if no user logged in.
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// Gets current TenantId or null.
        /// It can be empty if no user logged in.
        /// </summary>
        string TenantId { get; }

        /// <summary>
        /// Gets current UserName or empty.
        /// It can be empty if no user logged in.
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// Gets current UserRole or empty.
        /// It can be empty if no user logged in.
        /// </summary>
        string Role { get; }


        /// <summary>
        /// UserId of the impersonator.
        /// This is filled if a user is performing actions behalf of the <see cref="UserId"/>.
        /// </summary>
        string ImpersonatorUserId { get; }

        /// <summary>
        /// TenantId of the impersonator.
        /// This is filled if a user with <see cref="ImpersonatorUserId"/> performing actions behalf of the <see cref="UserId"/>.
        /// </summary>
        string ImpersonatorTenantId { get; }

        /// <summary>
        /// Current using user id, default is <see cref="IEasyNetSession.UserId"/>.
        /// But if <see cref="IEasyNetSession.ImpersonatorUserId"/> is not null, then use <see cref="IEasyNetSession.ImpersonatorUserId"/>.
        /// </summary>
        string CurrentUsingUserId { get; }

        /// <summary>
        /// Current using tenant id, default is <see cref="IEasyNetSession.TenantId"/>.
        /// But if <see cref="IEasyNetSession.ImpersonatorTenantId"/> is not null, then use <see cref="IEasyNetSession.ImpersonatorTenantId"/>.
        /// </summary>
        string CurrentUsingTenantId { get; }
    }
}
