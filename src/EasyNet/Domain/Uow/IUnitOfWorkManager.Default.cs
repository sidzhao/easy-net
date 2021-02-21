using System;
using System.Transactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EasyNet.Domain.Uow
{
	/// <summary>
	/// Unit of work manager.
	/// </summary>
    internal class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;
        private readonly UnitOfWorkDefaultOptions _defaultOptions;

        public UnitOfWorkManager(
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
            IOptions<UnitOfWorkDefaultOptions> defaultOptions)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            _defaultOptions = defaultOptions.Value;
        }

        public IActiveUnitOfWork Current => _currentUnitOfWorkProvider.Current;

        public IUnitOfWorkCompleteHandle Begin(IServiceProvider serviceProvider)
        {
            return Begin(serviceProvider, new UnitOfWorkOptions());
        }

        public IUnitOfWorkCompleteHandle Begin(IServiceProvider serviceProvider, TransactionScopeOption scope)
        {
            return Begin(serviceProvider, new UnitOfWorkOptions { Scope = scope });
        }

        public IUnitOfWorkCompleteHandle Begin(IServiceProvider serviceProvider, UnitOfWorkOptions options)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            options.FillDefaultsForNonProvidedOptions(_defaultOptions);

            var outerUow = _currentUnitOfWorkProvider.Current;

            if (options.Scope == TransactionScopeOption.Required && outerUow != null)
            {
                return outerUow.Options?.Scope == TransactionScopeOption.Suppress
                    ? new InnerSuppressUnitOfWorkCompleteHandle(outerUow)
                    : new InnerUnitOfWorkCompleteHandle();
            }

            var uow = serviceProvider.GetRequiredService<IUnitOfWork>();

            uow.Completed += (sender, args) =>
            {
                _currentUnitOfWorkProvider.Current = null;
            };

            uow.Failed += (sender, args) =>
            {
                _currentUnitOfWorkProvider.Current = null;
            };

            uow.Disposed += (sender, args) =>
            {
                // No action
            };

            uow.Begin(options);

            _currentUnitOfWorkProvider.Current = uow;

            return uow;
        }
    }
}
