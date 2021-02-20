using System.Threading.Tasks;

namespace EasyNet.EventBus
{
    public interface IEasyNetEventMessageBuffer
    {
        void Flush();

        Task FlushAsync();
    }
}
