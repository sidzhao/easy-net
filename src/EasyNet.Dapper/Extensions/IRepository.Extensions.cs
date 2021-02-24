using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EasyNet.Dapper.Data;
using EasyNet.Data;

// ReSharper disable once CheckNamespace
namespace EasyNet.Extensions.DependencyInjection
{
    public static class DapperRepositoryExtensions
    {
        /// <summary>
        /// Used to get all entities with sql.
        /// </summary>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="buffered">Whether to buffer results in memory.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns></returns>
        public static IEnumerable<TEntity> GetAllList<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> repository,
            string sql,
            object param = null,
            bool buffered = true,
            int? commandTimeout = null,
            CommandType? commandType = null)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            if (repository is DapperRepositoryBase<TEntity, TPrimaryKey> dapperRepository)
            {
                return dapperRepository.GetAllList(sql, param, buffered, commandTimeout, commandType);
            }

            throw new EasyNetException($"The interface {typeof(IRepository<,>).AssemblyQualifiedName} is not implemented with class {typeof(DapperRepositoryBase<,>)}.");
        }

        /// <summary>
        /// Used to get all entities with sql.
        /// </summary>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="buffered">Whether to buffer results in memory.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns></returns>
        public static Task<IEnumerable<TEntity>> GetAllListAsync<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> repository,
            string sql,
            object param = null,
            bool buffered = true,
            int? commandTimeout = null,
            CommandType? commandType = null)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            if (repository is DapperRepositoryBase<TEntity, TPrimaryKey> dapperRepository)
            {
                return dapperRepository.GetAllListAsync(sql, param, buffered, commandTimeout, commandType);
            }

            throw new EasyNetException($"The interface {typeof(IRepository<,>).AssemblyQualifiedName} is not implemented with class {typeof(DapperRepositoryBase<,>)}.");
        }

        /// <summary>
        /// Gets exactly one entity with sql.
        /// </summary>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static TEntity GetSingle<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> repository,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            if (repository is DapperRepositoryBase<TEntity, TPrimaryKey> dapperRepository)
            {
                return dapperRepository.GetSingle(sql, param, commandTimeout, commandType);
            }

            throw new EasyNetException($"The interface {typeof(IRepository<,>).AssemblyQualifiedName} is not implemented with class {typeof(DapperRepositoryBase<,>)}.");
        }

        /// <summary>
        /// Gets exactly one entity with sql.
        /// </summary>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<TEntity> GetSingleAsync<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> repository,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            if (repository is DapperRepositoryBase<TEntity, TPrimaryKey> dapperRepository)
            {
                return dapperRepository.GetSingleAsync(sql, param, commandTimeout, commandType);
            }

            throw new EasyNetException($"The interface {typeof(IRepository<,>).AssemblyQualifiedName} is not implemented with class {typeof(DapperRepositoryBase<,>)}.");
        }

        /// <summary>
        /// Gets exactly one entity or null with sql.
        /// </summary>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static TEntity GetSingleOrDefault<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> repository,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            if (repository is DapperRepositoryBase<TEntity, TPrimaryKey> dapperRepository)
            {
                return dapperRepository.GetSingleOrDefault(sql, param, commandTimeout, commandType);
            }

            throw new EasyNetException($"The interface {typeof(IRepository<,>).AssemblyQualifiedName} is not implemented with class {typeof(DapperRepositoryBase<,>)}.");
        }

        /// <summary>
        /// Gets exactly one entity or null with sql.
        /// </summary>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<TEntity> GetSingleOrDefaultAsync<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> repository,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            if (repository is DapperRepositoryBase<TEntity, TPrimaryKey> dapperRepository)
            {
                return dapperRepository.GetSingleOrDefaultAsync(sql, param, commandTimeout, commandType);
            }

            throw new EasyNetException($"The interface {typeof(IRepository<,>).AssemblyQualifiedName} is not implemented with class {typeof(DapperRepositoryBase<,>)}.");
        }

        /// <summary>
        /// Gets a first entity with sql.
        /// </summary>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static TEntity GetFirst<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> repository,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            if (repository is DapperRepositoryBase<TEntity, TPrimaryKey> dapperRepository)
            {
                return dapperRepository.GetFirst(sql, param, commandTimeout, commandType);
            }

            throw new EasyNetException($"The interface {typeof(IRepository<,>).AssemblyQualifiedName} is not implemented with class {typeof(DapperRepositoryBase<,>)}.");
        }

        /// <summary>
        /// Gets a first entity with sql.
        /// </summary>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<TEntity> GetFirstAsync<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> repository,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            if (repository is DapperRepositoryBase<TEntity, TPrimaryKey> dapperRepository)
            {
                return dapperRepository.GetFirstAsync(sql, param, commandTimeout, commandType);
            }

            throw new EasyNetException($"The interface {typeof(IRepository<,>).AssemblyQualifiedName} is not implemented with class {typeof(DapperRepositoryBase<,>)}.");
        }

        /// <summary>
        /// Gets a first entity or null with sql.
        /// </summary>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static TEntity GetFirstOrDefault<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> repository,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            if (repository is DapperRepositoryBase<TEntity, TPrimaryKey> dapperRepository)
            {
                return dapperRepository.GetFirstOrDefault(sql, param, commandTimeout, commandType);
            }

            throw new EasyNetException($"The interface {typeof(IRepository<,>).AssemblyQualifiedName} is not implemented with class {typeof(DapperRepositoryBase<,>)}.");
        }

        /// <summary>
        /// Gets a first entity or null with sql.
        /// </summary>
        /// <param name="repository">The <see cref="IRepository{TEntity}"/>.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static Task<TEntity> GetFirstOrDefaultAsync<TEntity, TPrimaryKey>(
            this IRepository<TEntity, TPrimaryKey> repository,
            string sql,
            object param = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            if (repository is DapperRepositoryBase<TEntity, TPrimaryKey> dapperRepository)
            {
                return dapperRepository.GetFirstOrDefaultAsync(sql, param, commandTimeout, commandType);
            }

            throw new EasyNetException($"The interface {typeof(IRepository<,>).AssemblyQualifiedName} is not implemented with class {typeof(DapperRepositoryBase<,>)}.");
        }
    }
}
