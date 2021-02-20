using System.Linq;
using EasyNet.Runtime.Security;

namespace EasyNet.Runtime.Session
{
    public class ClaimsEasyNetSession : EasyNetSessionBase
    {
        private readonly IPrincipalAccessor _principalAccessor;
        private bool _loaded;
        private string _userId;
        private string _tenantId;
        private string _userName;
        private string _role;
        private string _impersonatorUserId;
        private string _impersonatorTenantId;

        public ClaimsEasyNetSession(IPrincipalAccessor principalAccessor)
        {
            _principalAccessor = principalAccessor;
        }

        public override string UserId
        {
            get
            {
                EnsureLoadFromPrincipal();
                return _userId;
            }
        }

        public override string TenantId
        {
            get
            {
                EnsureLoadFromPrincipal();

                return _tenantId;
            }
        }

		public override string UserName
        {
            get
            {
                EnsureLoadFromPrincipal();

                return _userName;
            }
        }

        public override string Role
        {
            get
            {
                EnsureLoadFromPrincipal();

                return _role;
            }
        }

        public override string ImpersonatorUserId
        {
            get
            {
                EnsureLoadFromPrincipal();

                return _impersonatorUserId;
            }
        }

        public override string ImpersonatorTenantId
        {
            get
            {
                EnsureLoadFromPrincipal();

                return _impersonatorTenantId;
            }
        }

        private void EnsureLoadFromPrincipal()
        {
            if (!_loaded)
            {
                if (_principalAccessor.Principal != null)
                {
                    _userId = _principalAccessor.Principal.Claims.FirstOrDefault(c => c.Type == EasyNetClaimTypes.UserId)?.Value;
                    _tenantId = _principalAccessor.Principal.Claims.FirstOrDefault(c => c.Type == EasyNetClaimTypes.TenantId)?.Value;
                    _userName = _principalAccessor.Principal.Claims.FirstOrDefault(c => c.Type == EasyNetClaimTypes.UserName)?.Value;
                    _role = _principalAccessor.Principal.Claims.FirstOrDefault(c => c.Type == EasyNetClaimTypes.Role)?.Value;
                    _impersonatorUserId = _principalAccessor.Principal.Claims.FirstOrDefault(c => c.Type == EasyNetClaimTypes.ImpersonatorUserId)?.Value;
                    _impersonatorTenantId = _principalAccessor.Principal.Claims.FirstOrDefault(c => c.Type == EasyNetClaimTypes.ImpersonatorTenantId)?.Value;
                }

                _loaded = true;
            }
        }
    }
}
