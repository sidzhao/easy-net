using System.Threading.Tasks;

namespace EasyNet.Application.Services
{
    public interface IEasyNetPassivableAppService : IEasyNetPassivableAppService<int>
    {
    }

    public interface IEasyNetPassivableAppService<in TPrimaryKey> : IEasyNetAppService
    {
        Task ArchiveAsync(TPrimaryKey id);

        Task ActivateAsync(TPrimaryKey id);
    }
}
