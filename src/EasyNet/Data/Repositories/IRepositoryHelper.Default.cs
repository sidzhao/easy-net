﻿using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using EasyNet.Data.Entities;
using EasyNet.Data.Entities.Auditing;
using EasyNet.Data.Entities.Helper;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.Linq;
using EasyNet.Runtime.Session;
using EasyNet.Timing;
using EasyNet.Uow;

namespace EasyNet.Data.Repositories
{
    public class RepositoryHelper : IRepositoryHelper
    {


        #region ExecuteFilter

        public Expression<Func<TEntity, bool>> ExecuteFilter<TEntity, TPrimaryKey>(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session, Expression<Func<TEntity, bool>> predicate) where TEntity : IEntity<TPrimaryKey>
        {
            var tenantFilter = CreateTenantFilter<TEntity, TPrimaryKey>(currentUnitOfWorkProvider, session);
            if (tenantFilter != null)
            {
                predicate = predicate == null ? tenantFilter : CombineExpressions(predicate, tenantFilter);
            }

            var softDeleteFilter = CreateSoftDeleteFilter<TEntity, TPrimaryKey>(currentUnitOfWorkProvider, session);
            if (softDeleteFilter != null)
            {
                predicate = predicate == null ? softDeleteFilter : CombineExpressions(predicate, softDeleteFilter);
            }

            return predicate;
        }

        protected virtual Expression<Func<TEntity, bool>> CreateTenantFilter<TEntity, TPrimaryKey>(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session) where TEntity : IEntity<TPrimaryKey>
        {
            var entityType = typeof(TEntity);
            var currentTenantId = GetCurrentTenantId(currentUnitOfWorkProvider, session);

            var mustHaveTenantRawGeneric = entityType.GetImplementedRawGeneric(typeof(IMustHaveTenant<>));

            if (mustHaveTenantRawGeneric != null && IsMustHaveTenantFilterEnabled(currentUnitOfWorkProvider))
            {
                var tenantIdType = mustHaveTenantRawGeneric.GenericTypeArguments[0];

                var idValue = string.IsNullOrEmpty(currentTenantId) ? null : Convert.ChangeType(currentTenantId, tenantIdType);
                if (idValue == null) return p => 1 == 2; // Always set false if the entity is IMustHaveTenant<> and current tenant id is null

                var lambdaParam = Expression.Parameter(entityType);

                var leftExpression = Expression.PropertyOrField(lambdaParam, "TenantId");

                Expression<Func<object>> closure = () => idValue;
                var rightExpression = Expression.Convert(closure.Body, leftExpression.Type);

                var lambdaBody = Expression.Equal(leftExpression, rightExpression);

                return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
            }
            else
            {
                var mayHaveTenantRawGeneric = entityType.GetImplementedRawGeneric(typeof(IMayHaveTenant<>));

                if (mayHaveTenantRawGeneric != null && IsMayHaveTenantFilterEnabled(currentUnitOfWorkProvider))
                {
                    var tenantIdType = mayHaveTenantRawGeneric.GenericTypeArguments[0];

                    var lambdaParam = Expression.Parameter(entityType);

                    var leftExpression = Expression.PropertyOrField(lambdaParam, "TenantId");

                    var tenantId = GetCurrentTenantId(currentUnitOfWorkProvider, session);
                    var idValue = string.IsNullOrEmpty(tenantId) ? null : Convert.ChangeType(GetCurrentTenantId(currentUnitOfWorkProvider, session), tenantIdType);

                    Expression<Func<object>> closure = () => idValue;
                    var rightExpression = Expression.Convert(closure.Body, leftExpression.Type);

                    var lambdaBody = Expression.Equal(leftExpression, rightExpression);

                    return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
                }
            }

            return null;
        }

        protected virtual Expression<Func<TEntity, bool>> CreateSoftDeleteFilter<TEntity, TPrimaryKey>(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session) where TEntity : IEntity<TPrimaryKey>
        {
            var entityType = typeof(TEntity);
            var erp = EntityReflectionPropertiesPool.GetOrAdd(entityType);

            if (IsSoftDeleteFilterEnabled(currentUnitOfWorkProvider) && erp.IsSoftDelete)
            {
                var lambdaParam = Expression.Parameter(entityType);

                var leftExpression = Expression.PropertyOrField(lambdaParam, "IsDeleted");

                Expression<Func<object>> closure = () => false;
                var rightExpression = Expression.Convert(closure.Body, leftExpression.Type);

                var lambdaBody = Expression.Equal(leftExpression, rightExpression);

                return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
            }

            return null;
        }

        protected virtual Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            return ExpressionCombiner.Combine(expression1, expression2);
        }

        protected virtual bool IsSoftDeleteFilterEnabled(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            return currentUnitOfWorkProvider.Current?.IsFilterEnabled(EasyNetDataFilters.SoftDelete) == true;
        }

        protected virtual bool IsMustHaveTenantFilterEnabled(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            return currentUnitOfWorkProvider.Current?.IsFilterEnabled(EasyNetDataFilters.MustHaveTenant) == true;
        }

        protected virtual bool IsMayHaveTenantFilterEnabled(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            return currentUnitOfWorkProvider.Current?.IsFilterEnabled(EasyNetDataFilters.MayHaveTenant) == true;
        }

        #endregion

        public virtual bool MayHaveTemporaryKey<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            if (typeof(TPrimaryKey) == typeof(byte))
            {
                return true;
            }

            if (typeof(TPrimaryKey) == typeof(int))
            {
                return Convert.ToInt32(entity.Id) <= 0;
            }

            if (typeof(TPrimaryKey) == typeof(long))
            {
                return Convert.ToInt64(entity.Id) <= 0;
            }

            if (typeof(TPrimaryKey) == typeof(Guid))
            {
                return Guid.Parse(entity.Id.ToString()) == Guid.Empty;
            }

            return false;
        }

        public Expression<Func<TEntity, bool>> CreateEqualityExpressionForId<TEntity, TPrimaryKey>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var leftExpression = Expression.PropertyOrField(lambdaParam, "Id");

            var idValue = Convert.ChangeType(id, typeof(TPrimaryKey));

            Expression<Func<object>> closure = () => idValue;
            var rightExpression = Expression.Convert(closure.Body, leftExpression.Type);

            var lambdaBody = Expression.Equal(leftExpression, rightExpression);

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }

        public void ApplyConceptsForAddedEntity<TEntity>(TEntity entity, IEasyNetSession session)
        {
            var entityType = entity.GetType();
            var erp = EntityReflectionPropertiesPool.GetOrAdd(entityType);

            if (entity is IHasCreationTime hasCreationTimeEntity)
            {
                if (hasCreationTimeEntity.CreationTime == default)
                {
                    hasCreationTimeEntity.CreationTime = Clock.Now;
                }
            }

            if (erp.IsCreationAudited)
            {
                erp.CreationUserIdProperty.SetValueAndChangeType(entity, session.CurrentUsingUserId, erp.CreationUserIdType);
            }
        }

        public void ApplyConceptsForModifiedEntity<TEntity>(TEntity entity, IEasyNetSession session)
        {
            if (entity is IHasModificationTime hasModificationTime)
            {
                hasModificationTime.LastModificationTime = Clock.Now;
            }

            var entityType = entity.GetType();
            var erp = EntityReflectionPropertiesPool.GetOrAdd(entityType);

            if (erp.IsModifiedAudited)
            {
                erp.ModifiedUserIdProperty.SetValueAndChangeType(entity, session.CurrentUsingUserId, erp.ModifiedUserIdType);
            }
        }

        public void ApplyConceptsForDeletedEntity<TEntity>(TEntity entity, IEasyNetSession session)
        {
            var entityType = entity.GetType();
            var erp = EntityReflectionPropertiesPool.GetOrAdd(entityType);

            if (entity is ISoftDelete iSoftDelete)
            {
                iSoftDelete.IsDeleted = true;

                if (entity is IHasDeletionTime hasDeletionTime)
                {
                    hasDeletionTime.DeletionTime = Clock.Now;
                }

                if (erp.IsDeletionAudited)
                {
                    erp.DeleterUserIdProperty.SetValueAndChangeType(entity, session.CurrentUsingUserId, erp.DeleterUserIdType);
                }
            }
        }

        public virtual bool CheckAndSetIsActive<TEntity>(TEntity entity, EasyNetOptions options)
        {
            if (options.SuppressAutoSetIsActive)
            {
                return false;
            }

            if (entity is IPassivable passivable)
            {
                passivable.IsActive = true;

                return true;
            }

            return false;
        }

        public virtual bool CheckAndSetMustHaveTenantIdProperty<TEntity>(TEntity entity, ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session, EasyNetOptions options)
        {
            if (options.SuppressAutoSetTenantId)
            {
                return false;
            }

            var entityType = entity.GetType();
            var erp = EntityReflectionPropertiesPool.GetOrAdd(entityType);

            // Only set IMustHaveTenant entities
            if (!erp.IsMustHaveTenant)
            {
                return false;
            }

            // Don't set if it's already set
            var tenantIdProperty = erp.TenantIdProperty;

            bool alreadySetTenantId;
            var tenantIdType = erp.TenantIdType;
            if (tenantIdType == typeof(string))
            {
                alreadySetTenantId = !string.IsNullOrEmpty(tenantIdProperty.GetValue(entity).ToString());
            }
            else if (tenantIdType == typeof(short))
            {
                alreadySetTenantId = Convert.ToInt16(tenantIdProperty.GetValue(entity)) != 0;
            }
            else if (tenantIdType == typeof(int))
            {
                alreadySetTenantId = Convert.ToInt32(tenantIdProperty.GetValue(entity)) != 0;
            }
            else if (tenantIdType == typeof(long))
            {
                alreadySetTenantId = Convert.ToInt64(tenantIdProperty.GetValue(entity)) != 0;
            }
            else if (tenantIdType == typeof(decimal))
            {
                alreadySetTenantId = Convert.ToDecimal(tenantIdProperty.GetValue(entity)) != 0;
            }
            else if (tenantIdType == typeof(Guid))
            {
                alreadySetTenantId = Guid.Parse(tenantIdProperty.GetValue(entity).ToString()) != Guid.Empty;
            }
            else
            {
                throw new InvalidOperationException($"Not support {tenantIdType.AssemblyQualifiedName} in {typeof(IMustHaveTenant<>)}.");
            }

            if (!alreadySetTenantId)
            {
                if (string.IsNullOrEmpty(GetCurrentTenantId(currentUnitOfWorkProvider, session)))
                {
                    throw new EasyNetException("Can not set TenantId to empty for IMustHaveTenant entities!");
                }

                erp.TenantIdProperty.SetValueAndChangeType(entity, GetCurrentTenantId(currentUnitOfWorkProvider, session), erp.TenantIdType);
            }

            return true;
        }

        public virtual bool CheckAndSetMayHaveTenantIdProperty<TEntity>(TEntity entity, ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session, EasyNetOptions options)
        {
            if (options.SuppressAutoSetTenantId)
            {
                return false;
            }

            var entityType = entity.GetType();
            var erp = EntityReflectionPropertiesPool.GetOrAdd(entityType);

            // Only set IMayHaveTenant entities
            if (!erp.IsMayHaveTenant)
            {
                return false;
            }

            // Don't set if it's already set
            if (erp.TenantIdProperty.GetValue(entity) == null)
            {
                erp.TenantIdProperty.SetValueAndChangeType(entity, GetCurrentTenantId(currentUnitOfWorkProvider, session), erp.TenantIdType);
            }

            return true;
        }

        protected virtual string GetCurrentTenantId(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session)
        {
            var tenantId = currentUnitOfWorkProvider.Current != null ?
                currentUnitOfWorkProvider.Current.GetTenantId() :
                session.CurrentUsingTenantId;

            return string.IsNullOrEmpty(tenantId) ? null : tenantId;
        }
    }
}
