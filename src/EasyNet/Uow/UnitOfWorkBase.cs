using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using EasyNet.Data;
using EasyNet.EventBus;
using EasyNet.Extensions.DependencyInjection;
using EasyNet.Runtime.Session;
using Microsoft.Extensions.Options;

namespace EasyNet.Uow
{
    /// <summary>
    /// Base for all Unit Of Work classes.
    /// </summary>
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        /// <summary>
        /// Is <see cref="Begin"/> method called before?
        /// </summary>
        private bool _isBeginCalledBefore;

        /// <summary>
        /// Is <see cref="Complete"/> method called before?
        /// </summary>
        private bool _isCompleteCalledBefore;

        /// <summary>
        /// Is this unit of work successfully completed.
        /// </summary>
        private bool _succeed;

        /// <summary>
        /// A reference to the exception if this unit of work failed.
        /// </summary>
        private Exception _exception;

        protected UnitOfWorkBase(IEasyNetSession session, IEasyNetEventMessageBuffer eventMessageBuffer, IOptions<UnitOfWorkDefaultOptions> defaultOptions)
        {
            DefaultOptions = defaultOptions.Value;
            EasyNetSession = session;
            EventMessageBuffer = eventMessageBuffer;

            Id = Guid.NewGuid().ToString("N");
            _filters = DefaultOptions.Filters.ToList();

        }

        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc/>
        public IUnitOfWork Outer { get; set; }

        /// <inheritdoc/>
        public event EventHandler Completed;

        /// <inheritdoc/>
        public event EventHandler<UnitOfWorkFailedEventArgs> Failed;

        /// <inheritdoc/>
        public event EventHandler Disposed;

        public IDbConnector DbConnector { get; private set; }

        /// <inheritdoc/>
        public UnitOfWorkOptions Options { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<DataFilterConfiguration> Filters => _filters.ToImmutableList();

        private readonly List<DataFilterConfiguration> _filters;

        private string _tenantId;

        /// <summary>
        /// Gets default UOW options.
        /// </summary>
        protected UnitOfWorkDefaultOptions DefaultOptions { get; }

        /// <summary>
        /// Gets a value indicates that this unit of work is disposed or not.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Reference to current <see cref="IEasyNetSession"/>.
        /// </summary>
        public IEasyNetSession EasyNetSession { get; set; }

        /// <summary>
        /// Reference to current <see cref="IEasyNetEventMessageBuffer"/>.
        /// </summary>
        public IEasyNetEventMessageBuffer EventMessageBuffer { get; set; }

        /// <inheritdoc/>
        public void Begin(UnitOfWorkOptions options)
        {
            Check.NotNull(options, nameof(options));

            PreventMultipleBegin();
            Options = options; //TODO: Do not set options like that, instead make a copy?

            SetFilters(options.FilterOverrides);

            _tenantId = EasyNetSession.CurrentUsingTenantId;

            BeginUow();
        }

        /// <inheritdoc/>
        public abstract void SaveChanges();

        /// <inheritdoc/>
        public abstract Task SaveChangesAsync();

        /// <inheritdoc/>
        public void Complete()
        {
            PreventMultipleComplete();
            try
            {
                CompleteUow();
                FlushEventMessages();
                _succeed = true;
                OnCompleted();
            }
            catch (Exception ex)
            {
                _exception = ex;
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task CompleteAsync()
        {
            PreventMultipleComplete();
            try
            {
                await CompleteUowAsync();
                await FlushEventMessagesAsync();
                _succeed = true;
                OnCompleted();
            }
            catch (Exception ex)
            {
                _exception = ex;
                throw;
            }
        }

        /// <inheritdoc/>
        public bool IsFilterEnabled(string filterName)
        {
            return GetFilter(filterName).IsEnabled;
        }

        /// <inheritdoc/>
        public IDisposable DisableFilter(params string[] filterNames)
        {
            //TODO: Check if filters exists?

            var disabledFilters = new List<string>();

            foreach (var filterName in filterNames)
            {
                var filterIndex = GetFilterIndex(filterName);
                if (_filters[filterIndex].IsEnabled)
                {
                    disabledFilters.Add(filterName);
                    _filters[filterIndex] = new DataFilterConfiguration(_filters[filterIndex], false);
                }
            }

            return new DisposeAction(() => EnableFilter(disabledFilters.ToArray()));
        }

        /// <inheritdoc/>
        public IDisposable EnableFilter(params string[] filterNames)
        {
            //TODO: Check if filters exists?

            var enabledFilters = new List<string>();

            foreach (var filterName in filterNames)
            {
                var filterIndex = GetFilterIndex(filterName);
                if (!_filters[filterIndex].IsEnabled)
                {
                    enabledFilters.Add(filterName);
                    _filters[filterIndex] = new DataFilterConfiguration(_filters[filterIndex], true);
                }
            }

            return new DisposeAction(() => DisableFilter(enabledFilters.ToArray()));
        }

        public virtual IDisposable SetTenantId(string tenantId)
        {
            var oldTenantId = _tenantId;
            _tenantId = tenantId;

            return new DisposeAction(() =>
            {
                _tenantId = oldTenantId;
            });
        }
        public virtual string GetTenantId()
        {
            return _tenantId;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (!_isBeginCalledBefore || IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            if (!_succeed)
            {
                OnFailed(_exception);
            }

            DisposeUow();
            DbConnector?.Dispose();
            OnDisposed();
        }

        /// <summary>
        /// Can be implemented by derived classes to start UOW.
        /// </summary>
        protected virtual void BeginUow()
        {

        }

        /// <summary>
        /// Should be implemented by derived classes to complete UOW.
        /// </summary>
        protected abstract void CompleteUow();

        /// <summary>
        /// Should be implemented by derived classes to complete UOW.
        /// </summary>
        protected abstract Task CompleteUowAsync();

        /// <summary>
        /// Should be implemented by derived classes to dispose UOW.
        /// </summary>
        protected abstract void DisposeUow();

        /// <summary>
        /// Flush all event messages
        /// </summary>
        protected void FlushEventMessages()
        {
            EventMessageBuffer.Flush();
        }

        /// <summary>
        /// Flush all event messages
        /// </summary>
        protected Task FlushEventMessagesAsync()
        {
            return EventMessageBuffer.FlushAsync();
        }

        /// <summary>
        /// Called to trigger <see cref="Completed"/> event.
        /// </summary>
        protected virtual void OnCompleted()
        {
            Completed.InvokeSafely(this);
        }

        /// <summary>
        /// Called to trigger <see cref="Failed"/> event.
        /// </summary>
        /// <param name="exception">Exception that cause failure</param>
        protected virtual void OnFailed(Exception exception)
        {
            Failed.InvokeSafely(this, new UnitOfWorkFailedEventArgs(exception));
        }

        /// <summary>
        /// Called to trigger <see cref="Disposed"/> event.
        /// </summary>
        protected virtual void OnDisposed()
        {
            Disposed.InvokeSafely(this);
        }

        internal void SetDbConnector(IDbConnector dbConnector)
        {
            if (DbConnector != null)
            {
                throw new EasyNetException("Already has a database connector in unit of work.");
            }

            DbConnector = dbConnector;
        }

        private void PreventMultipleBegin()
        {
            if (_isBeginCalledBefore)
            {
                throw new EasyNetException("This unit of work has started before. Can not call Start method more than once.");
            }

            _isBeginCalledBefore = true;
        }

        private void PreventMultipleComplete()
        {
            if (_isCompleteCalledBefore)
            {
                throw new EasyNetException("Complete is called before!");
            }

            _isCompleteCalledBefore = true;
        }

        private void SetFilters(List<DataFilterConfiguration> filterOverrides)
        {
            for (var i = 0; i < _filters.Count; i++)
            {
                var filterOverride = filterOverrides.FirstOrDefault(f => f.FilterName == _filters[i].FilterName);
                if (filterOverride != null)
                {
                    _filters[i] = filterOverride;
                }
            }
        }

        private DataFilterConfiguration GetFilter(string filterName)
        {
            var filter = _filters.FirstOrDefault(f => f.FilterName == filterName);
            if (filter == null)
            {
                throw new EasyNetException("Unknown filter name: " + filterName + ". Be sure this filter is registered before.");
            }

            return filter;
        }

        private int GetFilterIndex(string filterName)
        {
            var filterIndex = _filters.FindIndex(f => f.FilterName == filterName);
            if (filterIndex < 0)
            {
                throw new EasyNetException("Unknown filter name: " + filterName + ". Be sure this filter is registered before.");
            }

            return filterIndex;
        }

        public override string ToString()
        {
            return $"[UnitOfWork {Id}]";
        }
    }
}
