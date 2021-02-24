using System;
using System.Linq.Expressions;
using EasyNet.Data;
using EasyNet.Runtime.Session;
using EasyNet.Uow;

namespace EasyNet.Dapper.Repositories
{
    // ReSharper disable once IdentifierTypo
    public interface IQueryFilterExecuter
    {
        Expression<Func<TEntity, bool>> ExecuteFilter<TEntity, TPrimaryKey>(
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
            IEasyNetSession session,
            Expression<Func<TEntity, bool>> predicate)
            where TEntity : IEntity<TPrimaryKey>;
    }
}
