using EasyNet.Identity.EntityFrameworkCore.DbContext;
using EasyNet.Identity.EntityFrameworkCore.Tests.Domain.Entities;
using EasyNet.Runtime.Session;
using EasyNet.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EasyNet.Identity.EntityFrameworkCore.Tests.DbContext
{
    public class IdentityContext : EasyNetIdentityDbContext<User, Role, int>
    {
        public IdentityContext(DbContextOptions options, ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session, IOptions<EasyNetOptions> easyNetOptions) : base(options, currentUnitOfWorkProvider, session, easyNetOptions)
        {
        }
    }
}
