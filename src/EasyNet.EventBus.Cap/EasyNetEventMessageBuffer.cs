using System.Collections.Concurrent;
using System.Threading.Tasks;
using DotNetCore.CAP.Persistence;
using DotNetCore.CAP.Transport;

namespace EasyNet.EventBus.Cap
{
    public class EasyNetEventMessageBuffer : IEasyNetEventMessageBuffer
    {
        private readonly IDispatcher _dispatcher;
        private readonly ConcurrentQueue<MediumMessage> _bufferList;

        public EasyNetEventMessageBuffer(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _bufferList = new ConcurrentQueue<MediumMessage>();
        }

        public void Flush()
        {
            while (!_bufferList.IsEmpty)
            {
                _bufferList.TryDequeue(out var message);

                _dispatcher.EnqueueToPublish(message);
            }
        }

        public Task FlushAsync()
        {
            return Task.Run(Flush);
        }

        public void Add(MediumMessage message)
        {
            _bufferList.Enqueue(message);
        }

        public void AddRange(ConcurrentQueue<MediumMessage> messageList)
        {
            while (!messageList.IsEmpty)
            {
                messageList.TryDequeue(out var message);

                _bufferList.Enqueue(message);
            }
        }
    }
}
