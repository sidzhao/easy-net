using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using EasyNet.Data;

// ReSharper disable once CheckNamespace
namespace EasyNet.Dapper.Data
{
    public class DapperRepositoryBase<TEntity> : DapperRepositoryBase<TEntity, int>, IDapperRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
        public DapperRepositoryBase(ICurrentDbConnectorProvider currentDbConnectorProvider)
            : base(currentDbConnectorProvider)
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
        protected IDbConnector DbConnector { get; }

        protected IDbConnection Connection => DbConnector.Connection;

        protected IDbTransaction Transaction => DbConnector.Transaction;

        public DapperRepositoryBase(ICurrentDbConnectorProvider currentDbConnectorProvider)
        {
            DbConnector = currentDbConnectorProvider.GetOrCreate();
        }

        #region Select/Get/Query

        public virtual List<TEntity> GetAllList()
        {
            throw new NotImplementedException();
        }

        public virtual Task<List<TEntity>> GetAllListAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public virtual Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public virtual Task<TEntity> GetAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public TEntity Single(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QuerySingle<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public Task<TEntity> SingleAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QuerySingleAsync<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public TEntity First()
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FirstAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual TEntity First(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public TEntity First(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QueryFirst<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public Task<TEntity> FirstAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QueryFirstAsync<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public virtual TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        public TEntity SingleOrDefault(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QuerySingleOrDefault<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public Task<TEntity> SingleOrDefaultAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QuerySingleOrDefaultAsync<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public virtual TEntity FirstOrDefault()
        {
            throw new NotImplementedException();
        }

        public virtual Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        public TEntity FirstOrDefault(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Connection.QueryFirstOrDefault<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public Task<TEntity> FirstOrDefaultAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public virtual Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual long LongCount()
        {
            throw new NotImplementedException();
        }

        public virtual Task<long> LongCountAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public virtual bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
