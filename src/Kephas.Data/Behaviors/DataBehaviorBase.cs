﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataBehaviorBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   An entity behavior base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Validation;

    /// <summary>
    /// An entity behavior base.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public abstract class DataBehaviorBase<TEntity> : IDataBehavior<TEntity>, IOnPersistBehavior, IOnInitializeBehavior, IOnValidateBehavior
    {
        /// <summary>
        /// Callback invoked upon entity initialization.
        /// </summary>
        /// <param name="entity">The entitiy to be initialized.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        public virtual void Initialize(TEntity entity, IEntityInfo entityInfo, IDataOperationContext operationContext)
        {
        }

        /// <summary>
        /// Callback invoked before an entity is being persisted.
        /// </summary>
        /// <param name="entity">The entity to be persisted.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        public virtual void BeforePersist(TEntity entity, IEntityInfo entityInfo, IDataOperationContext operationContext)
        {
        }

        /// <summary>
        /// Callback invoked after an entity has been persisted.
        /// </summary>
        /// <param name="entity">The entity to be persisted.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        public virtual void AfterPersist(TEntity entity, IEntityInfo entityInfo, IDataOperationContext operationContext)
        {
        }

        /// <summary>
        /// Callback invoked after upon entity validation.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// An <see cref="IDataValidationResult"/>.
        /// </returns>
        public virtual IDataValidationResult Validate(TEntity entity, IEntityInfo entityInfo, IDataOperationContext operationContext)
        {
            return DataValidationResult.Success;
        }

        /// <summary>
        /// Callback invoked before an entity is being persisted.
        /// </summary>
        /// <param name="entity">The entity to be persisted.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task IOnPersistBehavior.BeforePersistAsync(object entity, IEntityInfo entityInfo, IDataOperationContext operationContext, CancellationToken cancellationToken)
        {
            return this.BeforePersistAsync((TEntity)entity, entityInfo, operationContext, cancellationToken);
        }

        /// <summary>
        /// Callback invoked before an entity is being persisted.
        /// </summary>
        /// <param name="entity">The entity to be persisted.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public virtual Task BeforePersistAsync(TEntity entity, IEntityInfo entityInfo, IDataOperationContext operationContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.BeforePersist(entity, entityInfo, operationContext);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Callback invoked after an entity was persisted.
        /// </summary>
        /// <param name="entity">The entity to be persisted.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task IOnPersistBehavior.AfterPersistAsync(object entity, IEntityInfo entityInfo, IDataOperationContext operationContext, CancellationToken cancellationToken)
        {
            return this.AfterPersistAsync((TEntity)entity, entityInfo, operationContext, cancellationToken);
        }

        /// <summary>
        /// Callback invoked after an entity was persisted.
        /// </summary>
        /// <param name="entity">The entity to be persisted.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public virtual Task AfterPersistAsync(TEntity entity, IEntityInfo entityInfo, IDataOperationContext operationContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.AfterPersist(entity, entityInfo, operationContext);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Initializes the entity asynchronously.
        /// </summary>
        /// <param name="entity">The entitiy to be initialized.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task IOnInitializeBehavior.InitializeAsync(object entity, IEntityInfo entityInfo, IDataOperationContext operationContext, CancellationToken cancellationToken)
        {
            return this.InitializeAsync((TEntity)entity, entityInfo, operationContext, cancellationToken);
        }

        /// <summary>
        /// Initializes the entity asynchronously.
        /// </summary>
        /// <param name="entity">The entitiy to be initialized.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public virtual Task InitializeAsync(TEntity entity, IEntityInfo entityInfo, IDataOperationContext operationContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.Initialize(entity, entityInfo, operationContext);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Validates the provided instance asynchronously.
        /// </summary>
        /// <param name="entity">The entity to be validated.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="operationContext">Context for the validation operation.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="IDataValidationResult"/>.
        /// </returns>
        Task<IDataValidationResult> IOnValidateBehavior.ValidateAsync(object entity, IEntityInfo entityInfo, IDataOperationContext operationContext, CancellationToken cancellationToken)
        {
            return this.ValidateAsync((TEntity)entity, entityInfo, operationContext, cancellationToken);
        }

        /// <summary>
        /// Validates the provided instance asynchronously.
        /// </summary>
        /// <param name="entity">The entity to be validated.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="operationContext">Context for the validation operation.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="IDataValidationResult"/>.
        /// </returns>
        public virtual Task<IDataValidationResult> ValidateAsync(TEntity entity, IEntityInfo entityInfo, IDataOperationContext operationContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(this.Validate(entity, entityInfo, operationContext));
        }
    }
}
