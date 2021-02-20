using System.Threading;
using System.Threading.Tasks;

namespace EasyNet.EventBus
{
    public interface IEasyNetEventBus
    {
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="name">路由名称或者交换机名称</param>
        /// <param name="data">消息主体，消息对象需要可系列化，允许为空</param>
        void Publish<TEventData>(string name, TEventData data) where TEventData : EventData;

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="name">路由名称或者交换机名称</param>
        /// <param name="data">消息主体，消息对象需要可系列化，允许为空</param>
        /// <param name="cancellationToken"></param>
        Task PublishAsync<TEventData>(string name, TEventData data, CancellationToken cancellationToken = default) where TEventData : EventData;
    }
}
