﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using EasyDapperExtensions;
using EasyNet.Dapper.Repositories;
using EasyNet.Data;
using EasyNet.Runtime.Session;
using EasyNet.Uow;

// ReSharper disable once CheckNamespace
namespace EasyNet.Dapper.Data
{
    public class DapperRepositoryBase<TEntity> : DapperRepositoryBase<TEntity, int>, IDapperRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
        public DapperRepositoryBase(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session, ICurrentDbConnectorProvider currentDbConnectorProvider, IQueryFilterExecuter queryFilterExecuter) : base(currentUnitOfWorkProvider, session, currentDbConnectorProvider, queryFilterExecuter)
        {
        }
    }

    /// <summary>
    /// Implements IRepository for Entity Framework.
    /// </summary>
    /// <typeparam name="TEntity">Type of the Entity for this repository</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key of the entity</typeparam>
    public class DapperRepositoryBase<TEntity, TPrimaryKey> : IDapperRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected readonly ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider;
        protected readonly IEasyNetSession EasyNetSession;
        protected readonly IDbConnector DbConnector;

        // ReSharper disable once IdentifierTypo
        protected readonly IQueryFilterExecuter QueryFilterExecuter;

        // ReSharper disable once IdentifierTypo
        public DapperRepositoryBase(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider, IEasyNetSession session, ICurrentDbConnectorProvider currentDbConnectorProvider, IQueryFilterExecuter queryFilterExecuter)
        {
            EasyNetSession = session;
            CurrentUnitOfWorkProvider = currentUnitOfWorkProvider;
            DbConnector = currentDbConnectorProvider.GetOrCreate();
            QueryFilterExecuter = queryFilterExecuter;
        }

        protected IDbConnection Connection => DbConnector.Connection;

        protected IDbTransaction Transaction => DbConnector.Transaction;


        #region Select/Get/Query

        public virtual List<TEntity> GetAllList()
        {
            return GetAllList(null);
        }

        public virtual Task<List<TEntity>> GetAllListAsync(CancellationToken cancellationToken = default)
        {
            return GetAllListAsync(null, cancellationToken);
        }

        public virtual List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            var enumerable = Connection.GetAll(ExecuteFilter(predicate), Transaction);
            return enumerable.ToList();
        }

        public virtual async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var enumerable = await Connection.GetAllAsync(ExecuteFilter(predicate), Transaction);
            return enumerable.ToList();
        }

        public IEnumerable<TEntity> GetAllList(string sql, object param = null, bool buffered = true, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return Connection.Query<TEntity>(sql, param, Transaction, buffered, commandTimeout, commandType);
        }

        public Task<IEnumerable<TEntity>> GetAllListAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QueryAsync<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public virtual TEntity Get(TPrimaryKey id)
        {
            return Single(CreateEqualityExpressionForId(id));
        }

        public virtual Task<TEntity> GetAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        {
            return SingleAsync(CreateEqualityExpressionForId(id), cancellationToken);
        }

        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return Connection.GetSingle(ExecuteFilter(predicate), Transaction);
        }

        public virtual Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Connection.GetSingleAsync(ExecuteFilter(predicate), Transaction);
        }

        public TEntity GetSingle(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QuerySingle<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public Task<TEntity> GetSingleAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QuerySingleAsync<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public TEntity First()
        {
            return First(null);
        }

        public Task<TEntity> FirstAsync(CancellationToken cancellationToken = default)
        {
            return FirstAsync(null, cancellationToken);
        }

        public virtual TEntity First(Expression<Func<TEntity, bool>> predicate)
        {
            return Connection.GetFirst(ExecuteFilter(predicate), Transaction);
        }

        public virtual Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Connection.GetFirstAsync(ExecuteFilter(predicate), Transaction);
        }

        public TEntity GetFirst(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QueryFirst<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public Task<TEntity> GetFirstAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QueryFirstAsync<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public virtual TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Connection.GetSingleOrDefault(ExecuteFilter(predicate), Transaction);
        }

        public virtual Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Connection.GetSingleOrDefaultAsync(ExecuteFilter(predicate), Transaction);
        }

        public TEntity GetSingleOrDefault(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QuerySingleOrDefault<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public Task<TEntity> GetSingleOrDefaultAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QuerySingleOrDefaultAsync<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public virtual TEntity FirstOrDefault()
        {
            return FirstOrDefault(null);
        }

        public virtual Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            return FirstOrDefaultAsync(null, cancellationToken);
        }

        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Connection.GetFirstOrDefault(ExecuteFilter(predicate), Transaction);
        }

        public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Connection.GetFirstOrDefaultAsync(ExecuteFilter(predicate), Transaction);
        }

        public TEntity GetFirstOrDefault(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QueryFirstOrDefault<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public Task<TEntity> GetFirstOrDefaultAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QueryFirstOrDefaultAsync<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        #endregion

        #region Insert

        public virtual TEntity Insert(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual TPrimaryKey InsertAndGetId(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual TEntity InsertOrUpdate(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> InsertOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public TPrimaryKey InsertOrUpdateAndGetId(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Update

        public virtual TEntity Update(TEntity entity)
        {
            Connection.Update(entity);

            return entity;
        }

        public virtual Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual TEntity Update(TPrimaryKey id, Action<TEntity> updateAction)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task> updateAction, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Delete

        public virtual void Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(TPrimaryKey id)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Aggregates

        public virtual int Count()
        {
            return Count(null);
        }

        public virtual Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return CountAsync(null, cancellationToken);
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Connection.Count<TEntity, int>(ExecuteFilter(predicate), Transaction);
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Connection.CountAsync<TEntity, int>(ExecuteFilter(predicate), Transaction);
        }

        public virtual long LongCount()
        {
            return LongCount(null);
        }

        public virtual Task<long> LongCountAsync(CancellationToken cancellationToken = default)
        {
            return LongCountAsync(null, cancellationToken);
        }

        public virtual long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return Connection.Count<TEntity, long>(ExecuteFilter(predicate), Transaction);
        }

        public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Connection.CountAsync<TEntity, long>(ExecuteFilter(predicate), Transaction);
        }

        public virtual bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return Connection.Any(predicate);
        }

        public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Connection.AnyAsync(predicate);
        }

        #endregion

        protected virtual Expression<Func<TEntity, bool>> ExecuteFilter(Expression<Func<TEntity, bool>> predicate)
        {
            return QueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(CurrentUnitOfWorkProvider, EasyNetSession, predicate);
        }

        protected virtual Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var leftExpression = Expression.PropertyOrField(lambdaParam, "Id");

            var idValue = Convert.ChangeType(id, typeof(TPrimaryKey));

            Expression<Func<object>> closure = () => idValue;
            var rightExpression = Expression.Convert(closure.Body, leftExpression.Type);

            var lambdaBody = Expression.Equal(leftExpression, rightExpression);

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }
    }
}
