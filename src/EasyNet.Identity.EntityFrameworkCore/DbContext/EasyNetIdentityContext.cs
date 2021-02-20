﻿using System;
using EasyNet.Domain.Uow;
using EasyNet.Identity.EntityFrameworkCore.Domain.Entities;
using EasyNet.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EasyNet.Identity.EntityFrameworkCore.DbContext
{
	public class EasyNetIdentityDbContext : EasyNetIdentityDbContext<EasyNetUser<int>, EasyNetRole<int>, int>
	{
        public EasyNetIdentityDbContext(DbContextOptions options, ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session, IOptions<EasyNetOptions> easyNetOptions) : base(options, currentUnitOfWorkProvider, session, easyNetOptions)
        {
        }
    }

	public class EasyNetIdentityDbContext<TUser> : EasyNetIdentityDbContext<TUser, EasyNetRole<int>, int>
		where TUser : EasyNetUser<int>
	{
        public EasyNetIdentityDbContext(DbContextOptions options, ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session, IOptions<EasyNetOptions> easyNetOptions) : base(options, currentUnitOfWorkProvider, session, easyNetOptions)
        {
        }
    }

	public class EasyNetIdentityDbContext<TUser, TRole, TPrimaryKey> : EasyNetIdentityDbContext<TUser, TRole, EasyNetUserRole<TPrimaryKey>, EasyNetUserClaim<TPrimaryKey>, EasyNetRoleClaim<TPrimaryKey>, EasyNetUserLogin<TPrimaryKey>, EasyNetUserToken<TPrimaryKey>, TPrimaryKey>
		where TUser : EasyNetUser<TPrimaryKey>
		where TRole : EasyNetRole<TPrimaryKey>
		where TPrimaryKey : IEquatable<TPrimaryKey>
	{
        public EasyNetIdentityDbContext(DbContextOptions options, ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session, IOptions<EasyNetOptions> easyNetOptions) : base(options, currentUnitOfWorkProvider, session, easyNetOptions)
        {
        }
    }

	public class EasyNetIdentityDbContext<TUser, TRole, TUserRole, TUserClaim, TRoleClaim, TUserLogin, TUserToken, TPrimaryKey> : EasyNetIdentityUserContext<TUser, TUserClaim, TUserLogin, TUserToken, TPrimaryKey>
		where TUser : EasyNetUser<TPrimaryKey>
		where TRole : EasyNetRole<TPrimaryKey>
		where TUserRole : EasyNetUserRole<TPrimaryKey>
		where TUserClaim : EasyNetUserClaim<TPrimaryKey>
		where TRoleClaim : EasyNetRoleClaim<TPrimaryKey>
		where TUserLogin : EasyNetUserLogin<TPrimaryKey>
		where TUserToken : EasyNetUserToken<TPrimaryKey>
		where TPrimaryKey : IEquatable<TPrimaryKey>
	{

		public EasyNetIdentityDbContext(DbContextOptions options, ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session, IOptions<EasyNetOptions> easyNetOptions) : base(options, currentUnitOfWorkProvider, session, easyNetOptions)
        {
        }

		public virtual DbSet<TUserRole> UserRoles { get; set; }

        public virtual DbSet<TRole> Roles { get; set; }

        public virtual DbSet<TRoleClaim> RoleClaims { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<TUser>(b =>
			{
				b.HasMany<TUserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
			});

			builder.Entity<TRole>(b =>
			{
				b.HasKey(r => r.Id);
#if Net50
				b.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();
#else
                b.HasIndex(r => r.NormalizedName).HasName("RoleNameIndex").IsUnique();
#endif
				b.ToTable("EasyNetRoles");
				b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

				b.Property(u => u.Name).HasMaxLength(256);
				b.Property(u => u.NormalizedName).HasMaxLength(256);

				b.HasMany<TUserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
				b.HasMany<TRoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
			});

			builder.Entity<TRoleClaim>(b =>
			{
				b.HasKey(rc => rc.Id);
				b.ToTable("EasyNetRoleClaims");
			});

			builder.Entity<TUserRole>(b =>
			{
				b.HasKey(r => new { r.UserId, r.RoleId });
				b.ToTable("EasyNetUserRoles");
			});
		}
    }
}
