using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using EasyDapperExtensions;
using EasyNet.Data;
using EasyNet.Data.Entities;
using EasyNet.Data.Repositories;
using EasyNet.Runtime.Session;
using EasyNet.Uow;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyNet.Dapper.Data.Repositories
{
    public class DapperRepositoryBase<TEntity> : DapperRepositoryBase<TEntity, int>, IDapperRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
        public DapperRepositoryBase(
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
            IEasyNetSession session,
            ICurrentDbConnectorProvider currentDbConnectorProvider,
            IRepositoryHelper repositoryHelper,
            IOptions<EasyNetOptions> options,
            ILogger<DapperRepositoryBase<TEntity>> logger = null) : base(currentUnitOfWorkProvider, session, currentDbConnectorProvider, repositoryHelper, options, logger)
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
        protected readonly ICurrentDbConnectorProvider CurrentDbConnectorProvider;
        protected readonly IEasyNetSession CurrentSession;
        protected readonly IRepositoryHelper RepositoryHelper;
        protected readonly EasyNetOptions Options;
        protected readonly ILogger<DapperRepositoryBase<TEntity, TPrimaryKey>> Logger;

        // ReSharper disable once IdentifierTypo
        public DapperRepositoryBase(
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
            IEasyNetSession session,
            ICurrentDbConnectorProvider currentDbConnectorProvider,
            IRepositoryHelper repositoryHelper,
            IOptions<EasyNetOptions> options,
            ILogger<DapperRepositoryBase<TEntity, TPrimaryKey>> logger = null)
        {
            CurrentSession = session;
            CurrentUnitOfWorkProvider = currentUnitOfWorkProvider;
            CurrentDbConnectorProvider = currentDbConnectorProvider;
            RepositoryHelper = repositoryHelper;
            Options = options.Value;
            Logger = logger;
        }

        // Get DbConnector realtime, since maybe current uow was changed.
        protected IDbConnector DbConnector => CurrentDbConnectorProvider.GetOrCreate();

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
            var enumerable = Connection.GetAll(ExecuteFilter(predicate), Transaction, logger: Logger);
            return enumerable.ToList();
        }

        public virtual async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var enumerable = await Connection.GetAllAsync(ExecuteFilter(predicate), Transaction, logger: Logger);
            return enumerable.ToList();
        }

        public PagedEntities<TEntity> GetPagedList(int skip, int take)
        {
            return GetPagedList(skip, take, null);
        }

        public Task<PagedEntities<TEntity>> GetPagedListAsync(int skip, int take, CancellationToken cancellationToken = default)
        {
            return GetPagedListAsync(skip, take, null, cancellationToken);
        }

        public PagedEntities<TEntity> GetPagedList(int skip, int take, Expression<Func<TEntity, bool>> predicate)
        {
            var result = Connection.GetPaged(skip, take, ExecuteFilter(predicate), Transaction, logger: Logger);

            return new PagedEntities<TEntity>(result.TotalCount, result.Result.ToList());
        }

        public async Task<PagedEntities<TEntity>> GetPagedListAsync(int skip, int take, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var result = await Connection.GetPagedAsync(skip, take, ExecuteFilter(predicate), Transaction, logger: Logger);

            return new PagedEntities<TEntity>(result.TotalCount, result.Result.ToList());
        }

        public IEnumerable<TEntity> GetAllList(string sql, object param = null, bool buffered = true, int? commandTimeout = null,
            CommandType? commandType = null)
        {
            Logger?.LogInformationDbCommand(sql, param);

            return Connection.Query<TEntity>(sql, param, Transaction, buffered, commandTimeout, commandType);
        }

        public Task<IEnumerable<TEntity>> GetAllListAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            Logger?.LogInformationDbCommand(sql, param);

            return Connection.QueryAsync<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public virtual TEntity Get(TPrimaryKey id)
        {
            return Connection.Get<TEntity>(id, Transaction, logger: Logger);
        }

        public virtual Task<TEntity> GetAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        {
            return Connection.GetAsync<TEntity>(id, Transaction, logger: Logger);
        }

        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return Connection.GetSingle(ExecuteFilter(predicate), Transaction, logger: Logger);
        }

        public virtual Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Connection.GetSingleAsync(ExecuteFilter(predicate), Transaction, logger: Logger);
        }

        public TEntity GetSingle(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            Logger?.LogInformationDbCommand(sql, param);

            return Connection.QuerySingle<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public Task<TEntity> GetSingleAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            Logger?.LogInformationDbCommand(sql, param);

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
            return Connection.GetFirst(ExecuteFilter(predicate), Transaction, logger: Logger);
        }

        public virtual Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Connection.GetFirstAsync(ExecuteFilter(predicate), Transaction, logger: Logger);
        }

        public TEntity GetFirst(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            Logger?.LogInformationDbCommand(sql, param);

            return Connection.QueryFirst<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public Task<TEntity> GetFirstAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            Logger?.LogInformationDbCommand(sql, param);

            return Connection.QueryFirstAsync<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public virtual TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Connection.GetSingleOrDefault(ExecuteFilter(predicate), Transaction, logger: Logger);
        }

        public virtual Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Connection.GetSingleOrDefaultAsync(ExecuteFilter(predicate), Transaction, logger: Logger);
        }

        public TEntity GetSingleOrDefault(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            Logger?.LogInformationDbCommand(sql, param);

            return Connection.QuerySingleOrDefault<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public Task<TEntity> GetSingleOrDefaultAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            Logger?.LogInformationDbCommand(sql, param);

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
            return Connection.GetFirstOrDefault(ExecuteFilter(predicate), Transaction, logger: Logger);
        }

        public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Connection.GetFirstOrDefaultAsync(ExecuteFilter(predicate), Transaction, logger: Logger);
        }

        public TEntity GetFirstOrDefault(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            Logger?.LogInformationDbCommand(sql, param);

            return Connection.QueryFirstOrDefault<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        public Task<TEntity> GetFirstOrDefaultAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            Logger?.LogInformationDbCommand(sql, param);

            return Connection.QueryFirstOrDefaultAsync<TEntity>(sql, param, Transaction, commandTimeout, commandType);
        }

        #endregion

        #region Insert

        public virtual TEntity Insert(TEntity entity)
        {
            ApplyConceptsForAddedEntity(entity);

            entity.Id = Connection.InsertAndGetId<TEntity, TPrimaryKey>(entity, Transaction, logger: Logger);

            return entity;
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            ApplyConceptsForAddedEntity(entity);

            entity.Id = await Connection.InsertAndGetIdAsync<TEntity, TPrimaryKey>(entity, Transaction, logger: Logger);

            return entity;
        }

        public virtual TPrimaryKey InsertAndGetId(TEntity entity)
        {
            return Insert(entity).Id;
        }

        public virtual async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await InsertAsync(entity, cancellationToken);
            return entity.Id;
        }

        public virtual TEntity InsertOrUpdate(TEntity entity)
        {
            return MayHaveTemporaryKey(entity) ? Insert(entity) : Update(entity);
        }

        public Task<TEntity> InsertOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return MayHaveTemporaryKey(entity) ? InsertAsync(entity, cancellationToken) : UpdateAsync(entity, cancellationToken);
        }

        public TPrimaryKey InsertOrUpdateAndGetId(TEntity entity)
        {
            entity = MayHaveTemporaryKey(entity) ? Insert(entity) : Update(entity);

            return entity.Id;
        }

        public async Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity = MayHaveTemporaryKey(entity) ? await InsertAsync(entity, cancellationToken) : await UpdateAsync(entity, cancellationToken);

            return entity.Id;
        }

        #endregion

        #region Update

        public virtual TEntity Update(TEntity entity)
        {
            ApplyConceptsForModifiedEntity(entity);

            Connection.Update(entity, Transaction, logger: Logger);

            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            ApplyConceptsForModifiedEntity(entity);

            await Connection.UpdateAsync(entity, Transaction, logger: Logger);

            return entity;
        }

        public virtual TEntity Update(TPrimaryKey id, Action<TEntity> updateAction)
        {
            var entity = Get(id);

            updateAction(entity);

            return Update(entity);
        }

        public virtual async Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task> updateAction, CancellationToken cancellationToken = default)
        {
            var entity = await GetAsync(id, cancellationToken);

            await updateAction(entity);

            await UpdateAsync(entity, cancellationToken);

            return entity;
        }

        #endregion

        #region Delete

        public virtual void Delete(TEntity entity)
        {
            if (entity is ISoftDelete)
            {
                ApplyConceptsForDeletedEntity(entity);

                Connection.Update(entity, Transaction, logger: Logger);
            }
            else
            {
                Connection.Delete<TEntity>(entity.Id, Transaction, logger: Logger);
            }
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is ISoftDelete)
            {
                ApplyConceptsForDeletedEntity(entity);

                await Connection.UpdateAsync(entity, Transaction, logger: Logger);
            }
            else
            {
                await Connection.DeleteAsync<TEntity>(entity.Id, Transaction, logger: Logger);
            }
        }

        public virtual void Delete(TPrimaryKey id)
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                var entity = Get(id);

                Delete(entity);
            }
            else
            {
                Connection.Delete(CreateEqualityExpressionForId(id));
            }
        }

        public virtual async Task DeleteAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                var entity = await GetAsync(id, cancellationToken);

                await DeleteAsync(entity, cancellationToken);
            }
            else
            {
                await Connection.DeleteAsync(CreateEqualityExpressionForId(id));
            }
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                var entities = GetAllList(predicate);

                foreach (var entity in entities)
                {
                    Delete(entity);
                }
            }
            else
            {
                Connection.Delete(predicate, logger: Logger);
            }
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                var entities = await GetAllListAsync(predicate, cancellationToken);

                foreach (var entity in entities)
                {
                    await DeleteAsync(entity, cancellationToken);
                }
            }
            else
            {
                await Connection.DeleteAsync(predicate, logger: Logger);
            }
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
            return Connection.Count<TEntity, int>(ExecuteFilter(predicate), Transaction, logger: Logger);
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Connection.CountAsync<TEntity, int>(ExecuteFilter(predicate), Transaction, logger: Logger);
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
            return Connection.Count<TEntity, long>(ExecuteFilter(predicate), Transaction, logger: Logger);
        }

        public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Connection.CountAsync<TEntity, long>(ExecuteFilter(predicate), Transaction, logger: Logger);
        }

        public virtual bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return Connection.Any(predicate, logger: Logger);
        }

        public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Connection.AnyAsync(predicate, logger: Logger);
        }

        #endregion

        protected virtual Expression<Func<TEntity, bool>> ExecuteFilter(Expression<Func<TEntity, bool>> predicate)
        {
            return RepositoryHelper.ExecuteFilter<TEntity, TPrimaryKey>(CurrentUnitOfWorkProvider, CurrentSession, predicate);
        }

        protected virtual Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            return RepositoryHelper.CreateEqualityExpressionForId<TEntity, TPrimaryKey>(id);
        }

        protected virtual bool MayHaveTemporaryKey(TEntity entity)
        {
            return RepositoryHelper.MayHaveTemporaryKey<TEntity, TPrimaryKey>(entity);
        }

        protected virtual void ApplyConceptsForAddedEntity(TEntity entity)
        {
            RepositoryHelper.ApplyConceptsForAddedEntity(entity, CurrentSession);
            RepositoryHelper.CheckAndSetIsActive(entity, Options);
            if (!RepositoryHelper.CheckAndSetMustHaveTenantIdProperty(entity, CurrentUnitOfWorkProvider, CurrentSession, Options))
            {
                RepositoryHelper.CheckAndSetMayHaveTenantIdProperty(entity, CurrentUnitOfWorkProvider, CurrentSession, Options);
            }
        }

        protected virtual void ApplyConceptsForModifiedEntity(TEntity entity)
        {
            RepositoryHelper.ApplyConceptsForModifiedEntity(entity, CurrentSession);
        }

        protected virtual void ApplyConceptsForDeletedEntity(TEntity entity)
        {
            RepositoryHelper.ApplyConceptsForDeletedEntity(entity, CurrentSession);
        }
    }
}
