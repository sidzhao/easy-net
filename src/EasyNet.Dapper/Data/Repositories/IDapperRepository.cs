using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EasyNet.Data.Entities;
using EasyNet.Data.Repositories;

namespace EasyNet.Dapper.Data.Repositories
{
    public interface IDapperRepository<TEntity> : IDapperRepository<TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
    }

    public interface IDapperRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// Used to get all entities with sql.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="buffered">Whether to buffer results in memory.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns></returns>
        IEnumerable<TEntity> GetAllList(string sql, object param = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);

        /// <summary>
        /// Used to get all entities with sql.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetAllListAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);

        /// <summary>
        /// Gets exactly one entity with sql.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        TEntity GetSingle(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);

        /// <summary>
        /// Gets exactly one entity with sql.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        Task<TEntity> GetSingleAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);

        /// <summary>
        /// Gets a first entity with sql.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        TEntity GetFirst(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);

        /// <summary>
        /// Gets a first entity with sql.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        Task<TEntity> GetFirstAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);

        /// <summary>
        /// Gets only one entity or null with sql.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>The <see cref="TEntity"/> or null.</returns>
        TEntity GetSingleOrDefault(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);

        /// <summary>
        /// Gets only one entity or null with sql.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>The <see cref="TEntity"/> or null.</returns>
        Task<TEntity> GetSingleOrDefaultAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);

        /// <summary>
        /// Gets a first entity or null with sql.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>The <see cref="TEntity"/> or null.</returns>
        TEntity GetFirstOrDefault(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);

        /// <summary>
        /// Gets a first entity or null with sql.
        /// </summary>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>The <see cref="TEntity"/> or null.</returns>
        Task<TEntity> GetFirstOrDefaultAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null);
    }
}
