using EasyNet.CommonTests.Core.Entities;
using EasyNet.Runtime.Session;
using EasyNet.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyNet.EntityFrameworkCore.Tests.DbContext
{
    public class EfCoreContext : EasyNetDbContext
    {
        public EfCoreContext(DbContextOptions options, ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session, IOptions<EasyNetOptions> easyNetOptions) : base(options, currentUnitOfWorkProvider, session, easyNetOptions)
        {
        }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<TestCreationAudited> TestCreationAudited { get; set; }

        public virtual DbSet<TestModificationAudited> TestModificationAudited { get; set; }

        public virtual DbSet<TestDeletionAudited> TestDeletionAudited { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder =>
            {
                builder.AddDebug();
            }));
        }
    }
}
