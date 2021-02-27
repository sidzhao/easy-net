﻿using System;
using System.Linq.Expressions;
using EasyNet.Data.Entities;
using EasyNet.Runtime.Session;
using EasyNet.Uow;

namespace EasyNet.Data.Repositories
{
    public interface IRepositoryHelper
    {
        Expression<Func<TEntity, bool>> ExecuteFilter<TEntity, TPrimaryKey>(
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session,
            Expression<Func<TEntity, bool>> predicate) where TEntity : IEntity<TPrimaryKey>;

        Expression<Func<TEntity, bool>> CreateEqualityExpressionForId<TEntity, TPrimaryKey>(TPrimaryKey id) where TEntity : class, IEntity<TPrimaryKey>;

        bool MayHaveTemporaryKey<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>;

        void ApplyConceptsForAddedEntity<TEntity>(TEntity entity, IEasyNetSession session);

        void ApplyConceptsForModifiedEntity<TEntity>(TEntity entity, IEasyNetSession session);

        void ApplyConceptsForDeletedEntity<TEntity>(TEntity entity, IEasyNetSession session);

        bool CheckAndSetIsActive<TEntity>(TEntity entity, EasyNetOptions options);

        bool CheckAndSetMustHaveTenantIdProperty<TEntity>(TEntity entity, ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session, EasyNetOptions options);

        bool CheckAndSetMayHaveTenantIdProperty<TEntity>(TEntity entity, ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session, EasyNetOptions options);
    }
}
