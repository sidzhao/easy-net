using System;
using System.Collections.Generic;
using System.Transactions;
using EasyNet.Extensions.DependencyInjection;

namespace EasyNet.Uow
{
    /// <summary>
    /// Unit of work options.
    /// </summary>
    public class UnitOfWorkOptions
    {
        public UnitOfWorkOptions()
        {
            FilterOverrides = new List<DataFilterConfiguration>();
        }

        /// <summary>
        /// Scope option.
        /// </summary>
        public TransactionScopeOption? Scope { get; set; }

        /// <summary>
        /// Is this UOW transactional?
        /// Uses default value if not supplied.
        /// </summary>
        public bool? IsTransactional { get; set; }

        /// <summary>
        /// Timeout of UOW As milliseconds.
        /// Uses default value if not supplied.
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// If this UOW is transactional, this option indicated the isolation level of the transaction.
        /// Uses default value if not supplied.
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// Can be used to enable/disable some filters.
        /// </summary>
        public List<DataFilterConfiguration> FilterOverrides { get; }

        public System.Data.IsolationLevel GetSystemDataIsolationLevel()
        {
            return (IsolationLevel ?? System.Transactions.IsolationLevel.ReadUncommitted)
                .ToSystemDataIsolationLevel();
        }

        internal void FillDefaultsForNonProvidedOptions(UnitOfWorkDefaultOptions defaultOptions)
        {
            //TODO: Do not change options object..?

            if (!IsTransactional.HasValue)
            {
                IsTransactional = defaultOptions.IsTransactional;
            }

            if (!Scope.HasValue)
            {
                Scope = defaultOptions.Scope;
            }

            if (!Timeout.HasValue && defaultOptions.Timeout.HasValue)
            {
                Timeout = defaultOptions.Timeout.Value;
            }

            if (!IsolationLevel.HasValue && defaultOptions.IsolationLevel.HasValue)
            {
                IsolationLevel = defaultOptions.IsolationLevel.Value;
            }
        }

        internal static UnitOfWorkOptions Create(UnitOfWorkAttribute attribute)
        {
            Check.NotNull(attribute, nameof(attribute));

            var options = new UnitOfWorkOptions
            {
                IsTransactional = attribute.IsTransactional,
                Scope = attribute.Scope,
                Timeout = attribute.Timeout,
                IsolationLevel = attribute.IsolationLevel
            };

            return options;
        }
    }
}
