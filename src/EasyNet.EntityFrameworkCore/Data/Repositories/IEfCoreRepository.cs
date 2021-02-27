using System.Linq;
using EasyNet.Data.Entities;
using EasyNet.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EasyNet.EntityFrameworkCore.Data.Repositories
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
