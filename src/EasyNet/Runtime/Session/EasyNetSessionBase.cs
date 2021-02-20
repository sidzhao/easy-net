namespace EasyNet.Runtime.Session
{
    public abstract class EasyNetSessionBase : IEasyNetSession
    {
        public abstract string UserId { get; }

        public abstract string TenantId { get; }

        public abstract string UserName { get; }

        public abstract string Role { get; }

        public abstract string ImpersonatorUserId { get; }

        public abstract string ImpersonatorTenantId { get; }

        public virtual string CurrentUsingUserId => string.IsNullOrEmpty(ImpersonatorUserId) ? UserId : ImpersonatorUserId;

        public virtual string CurrentUsingTenantId => string.IsNullOrEmpty(ImpersonatorTenantId) ? TenantId : ImpersonatorTenantId;
    }
}
