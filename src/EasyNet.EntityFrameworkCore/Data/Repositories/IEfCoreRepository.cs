using System.Linq;
using EasyNet.Data;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace EasyNet.EntityFrameworkCore.Data
{
    public interface IEfCoreRepository<TEntity> : IEfCoreRepository<TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
    }

    public interface IEfCoreRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// Gets <see cref="DbContext"/>
        /// </summary>
        /// <returns></returns>
        DbContext GetDbContext();

        /// <summary>
        /// Gets <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetAll();
    }
}
