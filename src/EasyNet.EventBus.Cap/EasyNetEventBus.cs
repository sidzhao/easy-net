using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using DotNetCore.CAP.Persistence;
using EasyNet.Data;
using EasyNet.DependencyInjection;
using EasyNet.Extensions;

namespace EasyNet.EventBus.Cap
{
    public class EasyNetEventBus : IEasyNetEventBus
    {
        private readonly ICapPublisher _capPublisher;
        private readonly IIocResolver _iocResolver;
        private readonly IActiveDbTransactionProvider _activeDbTransactionProvider;
        private readonly IEasyNetEventMessageBuffer _eventMessageBuffer;

        public EasyNetEventBus(
            IIocResolver iocResolver,
            ICapPublisher capPublisher,
            IEasyNetEventMessageBuffer eventMessageBuffer,
            IActiveDbTransactionProvider activeDbTransactionProvider)
        {
            _iocResolver = iocResolver;
            _activeDbTransactionProvider = activeDbTransactionProvider;
            _capPublisher = capPublisher;
            _eventMessageBuffer = eventMessageBuffer;
        }

        public void Publish<TEventData>(string name, TEventData data) where TEventData : EventData
        {
            _capPublisher.Transaction.Value = GetCapTransaction();

            _capPublisher.Publish(name, data);

            AddMessageToBugger(_capPublisher.Transaction.Value);
        }

        public async Task PublishAsync<TEventData>(string name, TEventData data, CancellationToken cancellationToken = default) where TEventData : EventData
        {
            _capPublisher.Transaction.Value = GetCapTransaction();

            await _capPublisher.PublishAsync(name, data, cancellationToken: cancellationToken);

            AddMessageToBugger(_capPublisher.Transaction.Value);
        }

        private ICapTransaction GetCapTransaction()
        {
            var capTransaction = _iocResolver.GetService<ICapTransaction>();
            capTransaction.AutoCommit = false;
            capTransaction.DbTransaction = _activeDbTransactionProvider.Transaction;

            return capTransaction;
        }

        private void AddMessageToBugger(ICapTransaction capTransaction)
        {
            if (capTransaction == null) return;

            var bufferList = capTransaction.GetPrivateField<ConcurrentQueue<MediumMessage>>("_bufferList");
            if (_eventMessageBuffer is EasyNetEventMessageBuffer buffer)
            {
                buffer.AddRange(bufferList);
            }
        }
    }
}
