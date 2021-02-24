using System;
using System.Linq.Expressions;
using EasyNet.Data;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.Linq;
using EasyNet.Runtime.Session;
using EasyNet.Uow;

namespace EasyNet.Dapper.Repositories
{
    // ReSharper disable once IdentifierTypo
    public class QueryFilterExecuter : IQueryFilterExecuter
    {
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

        protected Expression<Func<TEntity, bool>> CreateTenantFilter<TEntity, TPrimaryKey>(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session) where TEntity : IEntity<TPrimaryKey>
        {
            var entityType = typeof(TEntity);


            var mustHaveTenantRawGeneric = entityType.GetImplementedRawGeneric(typeof(IMustHaveTenant<>));

            if (mustHaveTenantRawGeneric != null && IsMustHaveTenantFilterEnabled(currentUnitOfWorkProvider))
            {
                var tenantIdType = mustHaveTenantRawGeneric.GenericTypeArguments[0];

                var lambdaParam = Expression.Parameter(entityType);

                var leftExpression = Expression.PropertyOrField(lambdaParam, "TenantId");

                var idValue = Convert.ChangeType(GetTenantId(currentUnitOfWorkProvider, session), tenantIdType);

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

                    var tenantId = GetTenantId(currentUnitOfWorkProvider, session);
                    var idValue = string.IsNullOrEmpty(tenantId) ? null : Convert.ChangeType(GetTenantId(currentUnitOfWorkProvider, session), tenantIdType);

                    Expression<Func<object>> closure = () => idValue;
                    var rightExpression = Expression.Convert(closure.Body, leftExpression.Type);

                    var lambdaBody = Expression.Equal(leftExpression, rightExpression);

                    return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
                }
            }

            return null;
        }

        protected Expression<Func<TEntity, bool>> CreateSoftDeleteFilter<TEntity, TPrimaryKey>(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session) where TEntity : IEntity<TPrimaryKey>
        {
            var entityType = typeof(TEntity);

            if (IsSoftDeleteFilterEnabled(currentUnitOfWorkProvider) && typeof(ISoftDelete).IsAssignableFrom(entityType))
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

        protected virtual string GetTenantId(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session)
        {
            if (currentUnitOfWorkProvider.Current != null)
            {
                return currentUnitOfWorkProvider.Current.GetTenantId();
            }

            return session.CurrentUsingTenantId;
        }
    }
}
