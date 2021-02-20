using System.Threading.Tasks;

namespace EasyNet.EventBus
{
    public interface IEasyNetEventSubscribe<in TEventData>
    {
        Task ReceiveAsync(TEventData data);
    }
}
