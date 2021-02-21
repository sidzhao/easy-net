using System.Threading.Tasks;

namespace EasyNet.Application
{
    public interface IEasyNetPassivableAppService : IEasyNetPassivableAppService<int>
    {
    }

    public interface IEasyNetPassivableAppService<in TPrimaryKey> : IEasyNetAppService
    {
        /// <summary>
        /// Archive a object by id.
        /// </summary>
        /// <param name="id">The primary key of the object.</param>
        Task ArchiveAsync(TPrimaryKey id);

        /// <summary>
        /// Activate a object by id.
        /// </summary>
        /// <param name="id">The primary key of the object.</param>
        Task ActivateAsync(TPrimaryKey id);
    }
}
