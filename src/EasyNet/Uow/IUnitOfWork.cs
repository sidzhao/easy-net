﻿namespace EasyNet.Uow
{
    /// <summary>
    /// Defines a unit of work.
    /// Use <see cref="IUnitOfWorkManager.Begin(System.IServiceProvider)"/> to start a new unit of work.
    /// </summary>
    public interface IUnitOfWork : IActiveUnitOfWork, IUnitOfWorkCompleteHandle
    {
        /// <summary>
        /// Unique id of this UOW.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Reference to the outer UOW if exists.
        /// </summary>
        IUnitOfWork Outer { get; set; }

        /// <summary>
        /// Begins the unit of work with given options.
        /// </summary>
        /// <param name="options">The <see cref="UnitOfWorkOptions"/> .</param>
        void Begin(UnitOfWorkOptions options);
    }
}
