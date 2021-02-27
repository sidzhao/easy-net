using EasyNet.CommonTests.Common.Entities;
using EasyNet.EntityFrameworkCore;
using EasyNet.Runtime.Session;
using EasyNet.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyNet.CommonTests.Common
{
    public class EfCoreContext : EasyNetDbContext
    {
        public EfCoreContext(DbContextOptions options) : base(options)
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
