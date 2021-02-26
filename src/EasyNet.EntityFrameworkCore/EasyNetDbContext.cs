using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EasyNet.Data;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.Linq;
using EasyNet.Runtime.Session;
using EasyNet.Timing;
using EasyNet.Uow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;

namespace EasyNet.EntityFrameworkCore
{
    public class EasyNetDbContext : DbContext
    {
        //protected virtual bool IsSoftDeleteFilterEnabled => CurrentUnitOfWorkProvider.Current?.IsFilterEnabled(EasyNetDataFilters.SoftDelete) == true;

        //protected virtual bool IsMayHaveTenantFilterEnabled => CurrentUnitOfWorkProvider.Current?.IsFilterEnabled(EasyNetDataFilters.MayHaveTenant) == true;

        //protected virtual bool IsMustHaveTenantFilterEnabled => CurrentUnitOfWorkProvider.Current?.IsFilterEnabled(EasyNetDataFilters.MustHaveTenant) == true;

        //private static readonly MethodInfo ConfigureGlobalFiltersMethodInfo = typeof(EasyNetDbContext).GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.Instance | BindingFlags.NonPublic);

        //private readonly EasyNetOptions _easyNetOptions;

        public EasyNetDbContext(
            DbContextOptions options,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
            IEasyNetSession session,
            IOptions<EasyNetOptions> easyNetOptions
            ) : base(options)
        {
            Check.NotNull(currentUnitOfWorkProvider, nameof(currentUnitOfWorkProvider));
            Check.NotNull(session, nameof(session));

            //CurrentUnitOfWorkProvider = currentUnitOfWorkProvider;
            //EasyNetSession = session;
            //_easyNetOptions = easyNetOptions.Value;
        }

        //protected ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; }

        //protected IEasyNetSession EasyNetSession { get; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        //    {
        //        ConfigureGlobalFiltersMethodInfo
        //            .MakeGenericMethod(entityType.ClrType)
        //            .Invoke(this, new object[] { modelBuilder, entityType });
        //    }
        //}

        //protected void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
        //    where TEntity : class
        //{
        //    if (mutableEntityType.BaseType == null)
        //    {
        //        Expression<Func<TEntity, bool>> expression = null;
        //        var entityType = typeof(TEntity);

        //        if (typeof(ISoftDelete).IsAssignableFrom(entityType))
        //        {
        //            Expression<Func<TEntity, bool>> softDeleteFilter = e => !IsSoftDeleteFilterEnabled || !((ISoftDelete)e).IsDeleted;
        //            expression = softDeleteFilter;
        //        }

        //        var mustHaveTenantRawGeneric = entityType.GetImplementedRawGeneric(typeof(IMustHaveTenant<>));
        //        if (mustHaveTenantRawGeneric != null)
        //        {
        //            if (mustHaveTenantRawGeneric.GenericTypeArguments.Length != 1) throw new EasyNetException($"Invalid generic arguments {mustHaveTenantRawGeneric.AssemblyQualifiedName} in {entityType.AssemblyQualifiedName}.");

        //            var tenantIdType = mustHaveTenantRawGeneric.GenericTypeArguments[0];

        //            Expression<Func<TEntity, bool>> mustHaveTenantFilter;
        //            if (tenantIdType == typeof(string))
        //            {
        //                mustHaveTenantFilter = e => !IsMustHaveTenantFilterEnabled || ((IMustHaveTenant<string>)e).TenantId == GetCurrentTenantId();
        //            }
        //            else if (tenantIdType == typeof(short))
        //            {
        //                mustHaveTenantFilter = e => !IsMustHaveTenantFilterEnabled || ((IMustHaveTenant<short>)e).TenantId == (string.IsNullOrEmpty(GetCurrentTenantId()) ? 0 : short.Parse(GetCurrentTenantId()));
        //            }
        //            else if (tenantIdType == typeof(int))
        //            {
        //                mustHaveTenantFilter = e => !IsMustHaveTenantFilterEnabled || ((IMustHaveTenant<int>)e).TenantId == (string.IsNullOrEmpty(GetCurrentTenantId()) ? 0 : int.Parse(GetCurrentTenantId()));
        //            }
        //            else if (tenantIdType == typeof(long))
        //            {
        //                mustHaveTenantFilter = e => !IsMustHaveTenantFilterEnabled || ((IMustHaveTenant<long>)e).TenantId == (string.IsNullOrEmpty(GetCurrentTenantId()) ? 0 : long.Parse(GetCurrentTenantId()));
        //            }
        //            else if (tenantIdType == typeof(decimal))
        //            {
        //                mustHaveTenantFilter = e => !IsMustHaveTenantFilterEnabled || ((IMustHaveTenant<decimal>)e).TenantId == (string.IsNullOrEmpty(GetCurrentTenantId()) ? 0 : decimal.Parse(GetCurrentTenantId()));
        //            }
        //            else if (tenantIdType == typeof(Guid))
        //            {
        //                mustHaveTenantFilter = e => !IsMustHaveTenantFilterEnabled || ((IMustHaveTenant<Guid>)e).TenantId == (string.IsNullOrEmpty(GetCurrentTenantId()) ? Guid.Empty : Guid.Parse(GetCurrentTenantId()));
        //            }
        //            else
        //            {
        //                throw new InvalidOperationException($"Not support {tenantIdType.AssemblyQualifiedName} in {typeof(IMustHaveTenant<>)}.");
        //            }

        //            expression = expression == null ? mustHaveTenantFilter : CombineExpressions(expression, mustHaveTenantFilter);
        //        }
        //        else
        //        {
        //            var mayHaveTenantRawGeneric = entityType.GetImplementedRawGeneric(typeof(IMayHaveTenant<>));
        //            if (mayHaveTenantRawGeneric != null)
        //            {
        //                if (mayHaveTenantRawGeneric.GenericTypeArguments.Length != 1) throw new EasyNetException($"Invalid generic arguments {mayHaveTenantRawGeneric.AssemblyQualifiedName} in {entityType.AssemblyQualifiedName}.");

        //                var tenantIdType = mayHaveTenantRawGeneric.GenericTypeArguments[0];

        //                Expression<Func<TEntity, bool>> mayHaveTenantFilter;
        //                if (tenantIdType == typeof(short))
        //                {
        //                    mayHaveTenantFilter = e => !IsMayHaveTenantFilterEnabled || ((IMayHaveTenant<short>)e).TenantId == (string.IsNullOrEmpty(GetCurrentTenantId()) ? (short?)null : short.Parse(GetCurrentTenantId()));
        //                }
        //                else if (tenantIdType == typeof(int))
        //                {
        //                    mayHaveTenantFilter = e => !IsMayHaveTenantFilterEnabled || ((IMayHaveTenant<int>)e).TenantId == (string.IsNullOrEmpty(GetCurrentTenantId()) ? (int?)null : int.Parse(GetCurrentTenantId()));
        //                }
        //                else if (tenantIdType == typeof(long))
        //                {
        //                    mayHaveTenantFilter = e => !IsMayHaveTenantFilterEnabled || ((IMayHaveTenant<long>)e).TenantId == (string.IsNullOrEmpty(GetCurrentTenantId()) ? (long?)null : long.Parse(GetCurrentTenantId()));
        //                }
        //                else if (tenantIdType == typeof(decimal))
        //                {
        //                    mayHaveTenantFilter = e => !IsMayHaveTenantFilterEnabled || ((IMayHaveTenant<decimal>)e).TenantId == (string.IsNullOrEmpty(GetCurrentTenantId()) ? (decimal?)null : decimal.Parse(GetCurrentTenantId()));
        //                }
        //                else if (tenantIdType == typeof(Guid))
        //                {
        //                    mayHaveTenantFilter = e => !IsMayHaveTenantFilterEnabled || ((IMayHaveTenant<Guid>)e).TenantId == (string.IsNullOrEmpty(GetCurrentTenantId()) ? (Guid?)null : Guid.Parse(GetCurrentTenantId()));
        //                }
        //                else
        //                {
        //                    throw new InvalidOperationException($"Not support {tenantIdType.AssemblyQualifiedName} in {typeof(IMayHaveTenant<>)}.");
        //                }

        //                expression = expression == null ? mayHaveTenantFilter : CombineExpressions(expression, mayHaveTenantFilter);
        //            }
        //        }

        //        if (expression != null)
        //        {
        //            modelBuilder.Entity<TEntity>().HasQueryFilter(expression);
        //        }
        //    }
        //}

        //public override int SaveChanges()
        //{
        //    ApplyEasyNetConcepts();

        //    return base.SaveChanges();
        //}

        //public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        //{
        //    ApplyEasyNetConcepts();

        //    return base.SaveChangesAsync(cancellationToken);
        //}

        //protected virtual void ApplyEasyNetConcepts()
        //{
        //    foreach (var entry in ChangeTracker.Entries().ToList())
        //    {
        //        if (entry.State != EntityState.Modified && entry.CheckOwnedEntityChange())
        //        {
        //            Entry(entry.Entity).State = EntityState.Modified;
        //        }

        //        switch (entry.State)
        //        {
        //            case EntityState.Added:
        //                ApplyAbpConceptsForAddedEntity(entry);
        //                CheckAndSetMustHaveTenantIdProperty(entry);
        //                CheckAndSetMayHaveTenantIdProperty(entry);
        //                CheckAndSetIsActive(entry);
        //                break;
        //            case EntityState.Modified:
        //                ApplyAbpConceptsForModifiedEntity(entry);
        //                break;
        //            case EntityState.Deleted:
        //                ApplyAbpConceptsForDeletedEntity(entry);
        //                break;
        //        }
        //    }
        //}

        //protected virtual void ApplyAbpConceptsForAddedEntity(EntityEntry entry)
        //{
        //    var entityType = entry.Entity.GetType();

        //    if (entry.Entity is IHasCreationTime hasCreationTimeEntity)
        //    {
        //        if (hasCreationTimeEntity.CreationTime == default)
        //        {
        //            hasCreationTimeEntity.CreationTime = Clock.Now;
        //        }
        //    }

        //    var creationGeneric = entityType.GetImplementedRawGeneric(typeof(ICreationAudited<>));
        //    if (creationGeneric != null)
        //    {
        //        var userIdProperty = entityType.GetProperty("CreatorUserId");
        //        if (userIdProperty == null) throw new EasyNetException($"Cannot found property CreatorUserId in entity {entityType.AssemblyQualifiedName}.");
        //        userIdProperty.SetValueAndAutoFit(entry.Entity, EasyNetSession.CurrentUsingUserId, creationGeneric.GenericTypeArguments[0]);
        //    }
        //}

        //protected virtual void ApplyAbpConceptsForModifiedEntity(EntityEntry entry)
        //{
        //    var entityType = entry.Entity.GetType();

        //    if (entry.Entity is IHasModificationTime hasModificationTime)
        //    {
        //        hasModificationTime.LastModificationTime = Clock.Now;
        //    }

        //    var modificationGeneric = entityType.GetImplementedRawGeneric(typeof(IModificationAudited<>));
        //    if (modificationGeneric != null)
        //    {
        //        var userIdProperty = entityType.GetProperty("LastModifierUserId");
        //        if (userIdProperty == null) throw new EasyNetException($"Cannot found property LastModifierUserId in entity {entityType.AssemblyQualifiedName}.");
        //        userIdProperty.SetValueAndAutoFit(entry.Entity, EasyNetSession.CurrentUsingUserId, modificationGeneric.GenericTypeArguments[0]);
        //    }
        //}

        //protected virtual void ApplyAbpConceptsForDeletedEntity(EntityEntry entry)
        //{
        //    if (entry.Entity is ISoftDelete iSoftDelete)
        //    {
        //        iSoftDelete.IsDeleted = true;
        //        entry.State = EntityState.Modified;

        //        var entityType = entry.Entity.GetType();
        //        if (entry.Entity is IHasDeletionTime hasDeletionTime)
        //        {
        //            hasDeletionTime.DeletionTime = Clock.Now;
        //        }

        //        var deletionGeneric = entityType.GetImplementedRawGeneric(typeof(IDeletionAudited<>));
        //        if (deletionGeneric != null)
        //        {
        //            var userIdProperty = entityType.GetProperty("DeleterUserId");
        //            if (userIdProperty == null) throw new EasyNetException($"Cannot found property DeleterUserId in entity {entityType.AssemblyQualifiedName}.");
        //            userIdProperty.SetValueAndAutoFit(entry.Entity, EasyNetSession.CurrentUsingUserId, deletionGeneric.GenericTypeArguments[0]);
        //        }
        //    }
        //}

        //protected virtual void CheckAndSetIsActive(EntityEntry entry)
        //{
        //    if (_easyNetOptions.SuppressAutoSetIsActive)
        //    {
        //        return;
        //    }

        //    if (entry.Entity is IPassivable passivable)
        //    {
        //        passivable.IsActive = true;
        //    }
        //}

        //protected virtual void CheckAndSetMustHaveTenantIdProperty(EntityEntry entry)
        //{
        //    if (_easyNetOptions.SuppressAutoSetTenantId)
        //    {
        //        return;
        //    }

        //    var entityType = entry.Entity.GetType();

        //    // Only set IMustHaveTenant entities
        //    var tenantGeneric = entityType.GetImplementedRawGeneric(typeof(IMustHaveTenant<>));
        //    if (tenantGeneric == null)
        //    {
        //        return;
        //    }

        //    if (string.IsNullOrEmpty(GetCurrentTenantId()))
        //    {
        //        throw new EasyNetException("Can not set TenantId to empty for IMustHaveTenant entities!");
        //    }

        //    // Don't set if it's already set
        //    var tenantIdProperty = entityType.GetProperty("TenantId");
        //    if (tenantIdProperty == null) throw new EasyNetException($"Cannot found property TenantId in entity {entityType.AssemblyQualifiedName}.");

        //    bool alreadySetTenantId;
        //    var tenantIdType = tenantGeneric.GenericTypeArguments[0];
        //    if (tenantIdType == typeof(string))
        //    {
        //        alreadySetTenantId = !string.IsNullOrEmpty(tenantIdProperty.GetValue(entry.Entity).ToString());
        //    }
        //    else if (tenantIdType == typeof(short))
        //    {
        //        alreadySetTenantId = Convert.ToInt16(tenantIdProperty.GetValue(entry.Entity)) != 0;
        //    }
        //    else if (tenantIdType == typeof(int))
        //    {
        //        alreadySetTenantId = Convert.ToInt32(tenantIdProperty.GetValue(entry.Entity)) != 0;
        //    }
        //    else if (tenantIdType == typeof(long))
        //    {
        //        alreadySetTenantId = Convert.ToInt64(tenantIdProperty.GetValue(entry.Entity)) != 0;
        //    }
        //    else if (tenantIdType == typeof(decimal))
        //    {
        //        alreadySetTenantId = Convert.ToDecimal(tenantIdProperty.GetValue(entry.Entity)) != 0;
        //    }
        //    else if (tenantIdType == typeof(Guid))
        //    {
        //        alreadySetTenantId = Guid.Parse(tenantIdProperty.GetValue(entry.Entity).ToString()) != Guid.Empty;
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException($"Not support {tenantIdType.AssemblyQualifiedName} in {typeof(IMustHaveTenant<>)}.");
        //    }

        //    if (!alreadySetTenantId)
        //    {
        //        tenantIdProperty.SetValueAndAutoFit(entry.Entity, GetCurrentTenantId(), tenantGeneric.GenericTypeArguments[0]);
        //    }
        //}

        //protected virtual void CheckAndSetMayHaveTenantIdProperty(EntityEntry entry)
        //{
        //    if (_easyNetOptions.SuppressAutoSetTenantId)
        //    {
        //        return;
        //    }

        //    var entityType = entry.Entity.GetType();

        //    // Only set IMayHaveTenant entities
        //    var tenantGeneric = entityType.GetImplementedRawGeneric(typeof(IMayHaveTenant<>));
        //    if (tenantGeneric == null)
        //    {
        //        return;
        //    }

        //    // Don't set if it's already set
        //    var tenantIdProperty = entityType.GetProperty("TenantId");
        //    if (tenantIdProperty == null) throw new EasyNetException($"Cannot found property TenantId in entity {entityType.AssemblyQualifiedName}.");

        //    if (tenantIdProperty.GetValue(entry.Entity) == null)
        //    {
        //        tenantIdProperty.SetValueAndAutoFit(entry.Entity, GetCurrentTenantId(),
        //            tenantGeneric.GenericTypeArguments[0]);
        //    }
        //}

        //protected virtual string GetCurrentTenantId()
        //{
        //    if (CurrentUnitOfWorkProvider?.Current != null)
        //    {
        //        return CurrentUnitOfWorkProvider.Current.GetTenantId();
        //    }

        //    return EasyNetSession.CurrentUsingTenantId;
        //}

        //protected virtual Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        //{
        //    return ExpressionCombiner.Combine(expression1, expression2);
        //}
    }
}
