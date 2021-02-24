using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EasyNet.Data;
using EasyNet.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace EasyNet.EntityFrameworkCore.Data
{
    public class EfCoreRepositoryBase<TDbContext, TEntity> : EfCoreRepositoryBase<TDbContext, TEntity, int>, IEfCoreRepository<TEntity>
        where TEntity : class, IEntity<int>
        where TDbContext : EasyNetDbContext
    {
        public EfCoreRepositoryBase(ICurrentDbConnectorProvider currentDbConnectorProvider)
            : base(currentDbConnectorProvider)
        {
        }
    }

    /// <summary>
    /// Implements IRepository for Entity Framework.
    /// </summary>
    /// <typeparam name="TDbContext">DbContext which contains <typeparamref name="TEntity"/>.</typeparam>
    /// <typeparam name="TEntity">Type of the Entity for this repository</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key of the entity</typeparam>
    public class EfCoreRepositoryBase<TDbContext, TEntity, TPrimaryKey> : IEfCoreRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TDbContext : EasyNetDbContext
    {
        protected readonly TDbContext DbContext;

        public EfCoreRepositoryBase(ICurrentDbConnectorProvider currentDbConnectorProvider)
        {
            DbContext = (TDbContext)currentDbConnectorProvider.GetOrCreate().GetDbContext();
        }
        protected virtual DbSet<TEntity> Table => DbContext.Set<TEntity>();

        public DbContext GetDbContext()
        {
            return DbContext;
        }

        public IQueryable<TEntity> GetAll()
        {
            return Table.AsQueryable();
        }

        #region Select/Get/Query

        public virtual List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        public virtual Task<List<TEntity>> GetAllListAsync(CancellationToken cancellationToken = default)
        {
            return GetAll().ToListAsync(cancellationToken);
        }

        public virtual List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public virtual Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return GetAll().Where(predicate).ToListAsync(cancellationToken);
        }

        public virtual TEntity Get(TPrimaryKey id)
        {
            var entity = SingleOrDefault(CreateEqualityExpressionForId(id));

            if (entity == null) throw new EasyNetNotFoundEntityException<TEntity, TPrimaryKey>(id);

            return entity;
        }

        public virtual async Task<TEntity> GetAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        {
            var entity = await SingleOrDefaultAsync(CreateEqualityExpressionForId(id), cancellationToken);

            if (entity == null) throw new EasyNetNotFoundEntityException<TEntity, TPrimaryKey>(id);

            return entity;
        }

        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Single(predicate);
        }

        public virtual Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return GetAll().SingleAsync(predicate, cancellationToken);
        }

        public TEntity First()
        {
            return GetAll().First();
        }

        public Task<TEntity> FirstAsync(CancellationToken cancellationToken = default)
        {
            return GetAll().FirstAsync(cancellationToken);
        }

        public virtual TEntity First(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().First(predicate);
        }

        public virtual Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return GetAll().FirstAsync(predicate, cancellationToken);
        }

        public virtual TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().SingleOrDefault(predicate);
        }

        public virtual Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return GetAll().SingleOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual TEntity FirstOrDefault()
        {
            return GetAll().FirstOrDefault();
        }

        public virtual Task<TEntity> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            return GetAll().FirstOrDefaultAsync(cancellationToken);
        }

        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return GetAll().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        #endregion

        #region Insert

        public virtual TEntity Insert(TEntity entity)
        {
            Table.Add(entity);
            return entity;
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await Table.AddAsync(entity, cancellationToken);
            return entity;
        }

        public virtual TPrimaryKey InsertAndGetId(TEntity entity)
        {
            Insert(entity);

            if (MayHaveTemporaryKey(entity))
            {
                DbContext.SaveChanges();
            }

            return entity.Id;
        }

        public virtual async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await InsertAsync(entity, cancellationToken);

            if (MayHaveTemporaryKey(entity))
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }

            return entity.Id;
        }

        public virtual TEntity InsertOrUpdate(TEntity entity)
        {
            if (MayHaveTemporaryKey(entity))
            {
                Insert(entity);
            }
            else
            {
                Update(entity);
            }

            return entity;
        }

        public async Task<TEntity> InsertOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (MayHaveTemporaryKey(entity))
            {
                await InsertAsync(entity, cancellationToken);
            }
            else
            {
                await UpdateAsync(entity, cancellationToken);
            }

            return entity;
        }

        public TPrimaryKey InsertOrUpdateAndGetId(TEntity entity)
        {
            entity = InsertOrUpdate(entity);

            if (MayHaveTemporaryKey(entity))
            {
                DbContext.SaveChanges();
            }

            return entity.Id;
        }

        public async Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity = await InsertOrUpdateAsync(entity, cancellationToken);

            if (MayHaveTemporaryKey(entity))
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }

            return entity.Id;
        }

        #endregion

        #region Update

        public virtual TEntity Update(TEntity entity)
        {
            AttachIfNot(entity);
            DbContext.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public virtual Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity = Update(entity);
            return Task.FromResult(entity);
        }

        public virtual TEntity Update(TPrimaryKey id, Action<TEntity> updateAction)
        {
            var entity = Get(id);
            updateAction(entity);

            if (DbContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.NoTracking)
            {
                AttachIfNot(entity);
                DbContext.Entry(entity).State = EntityState.Modified;
            }

            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task> updateAction, CancellationToken cancellationToken = default)
        {
            var entity = await GetAsync(id, cancellationToken);
            await updateAction(entity);

            if (DbContext.ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.NoTracking)
            {
                AttachIfNot(entity);
                DbContext.Entry(entity).State = EntityState.Modified;
            }

            return entity;
        }

        #endregion

        #region Delete

        public virtual void Delete(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);
        }

        public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Delete(entity);

            return Task.CompletedTask;
        }

        public virtual void Delete(TPrimaryKey id)
        {
            var entity = GetFromChangeTrackerOrNull(id);
            if (entity != null)
            {
                Delete(entity);
                return;
            }

            entity = SingleOrDefault(CreateEqualityExpressionForId(id));
            if (entity != null)
            {
                Delete(entity);
            }

            // Don't do anything if no entity can be found.
        }

        public virtual async Task DeleteAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        {
            var entity = GetFromChangeTrackerOrNull(id);
            if (entity != null)
            {
                await DeleteAsync(entity, cancellationToken);
            }

            entity = await SingleOrDefaultAsync(CreateEqualityExpressionForId(id), cancellationToken);
            if (entity != null)
            {
                await DeleteAsync(entity, cancellationToken);
            }

            // Don't do anything if no entity can be found.
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = GetAllList(predicate);

            foreach (var entity in entities)
            {
                Table.Remove(entity);
            }
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var entities = await GetAllListAsync(predicate, cancellationToken);

            foreach (var entity in entities)
            {
                Table.Remove(entity);
            }
        }

        #endregion

        #region Aggregates

        public virtual int Count()
        {
            return GetAll().Count();
        }

        public virtual Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return GetAll().CountAsync(cancellationToken);
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Count(predicate);
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return GetAll().CountAsync(predicate, cancellationToken);
        }

        public virtual long LongCount()
        {
            return GetAll().LongCount();
        }

        public virtual Task<long> LongCountAsync(CancellationToken cancellationToken = default)
        {
            return GetAll().LongCountAsync(cancellationToken);
        }

        public virtual long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().LongCount(predicate);
        }

        public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return GetAll().LongCountAsync(predicate, cancellationToken);
        }

        public virtual bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Any(predicate);
        }

        public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return GetAll().AnyAsync(predicate, cancellationToken);
        }

        #endregion

        protected virtual void AttachIfNot(TEntity entity)
        {
            var entry = DbContext.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null)
            {
                return;
            }

            Table.Attach(entity);
        }

        protected virtual TEntity GetFromChangeTrackerOrNull(TPrimaryKey id)
        {
            var entry = DbContext.ChangeTracker.Entries()
                .FirstOrDefault(
                    ent =>
                        ent.Entity is TEntity &&
                        EqualityComparer<TPrimaryKey>.Default.Equals(id, (ent.Entity as TEntity).Id)
                );

            return entry?.Entity as TEntity;
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

        protected virtual bool MayHaveTemporaryKey(TEntity entity)
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

            return false;
        }
    }
}
