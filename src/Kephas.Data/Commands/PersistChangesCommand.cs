﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistChangesCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the persist changes command base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Behaviors;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Resources;
    using Kephas.Data.Validation;
    using Kephas.Diagnostics;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for commands which persist dataContext changes.
    /// </summary>
    [DataContextType(typeof(DataContextBase))]
    public class PersistChangesCommand : DataCommandBase<IPersistChangesContext, IDataCommandResult>, IPersistChangesCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistChangesCommand"/> class.
        /// </summary>
        /// <param name="behaviorProvider">The behavior provider.</param>
        public PersistChangesCommand(IDataBehaviorProvider behaviorProvider)
        {
            Requires.NotNull(behaviorProvider, nameof(behaviorProvider));

            this.BehaviorProvider = behaviorProvider;
        }

        /// <summary>
        /// Gets the behavior provider.
        /// </summary>
        /// <value>
        /// The behavior provider.
        /// </value>
        public IDataBehaviorProvider BehaviorProvider { get; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<PersistChangesCommand> Logger { get; set; }

        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="IDataCommandResult"/>.
        /// </returns>
        public override async Task<IDataCommandResult> ExecuteAsync(
            IPersistChangesContext operationContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var changes = 0;
            Exception exception = null;
            var sb = new StringBuilder();

            var elapsed = await Profiler.WithStopwatchAsync(
                async () =>
                    {
                        try
                        {
                            var currentIteration = 0;
                            const int MaxIterations = 10;

                            var modifiedEntries = await this.DetectModifiedEntriesAsync(operationContext, cancellationToken).PreserveThreadContext();

                            while (modifiedEntries.Count > 0)
                            {
                                if (currentIteration > MaxIterations)
                                {
                                    var ex = new DataException(string.Format(Strings.PersistChangesCommand_MaximumNumberOfIterationsExceeded_Exception, MaxIterations));
                                    throw ex;
                                }

                                await this.ExecuteBeforeSaveBehaviorsAsync(operationContext, modifiedEntries, cancellationToken).PreserveThreadContext();

                                changes = await this.ValidateAndPersistModifiedEntriesAsync(modifiedEntries, changes, operationContext, sb, cancellationToken).PreserveThreadContext();

                                this.ResetChangeState(operationContext, modifiedEntries);

                                await this.ExecuteAfterSaveBehaviorsAsync(operationContext, modifiedEntries, cancellationToken).PreserveThreadContext();

                                // NOTE: after calling after save behaviors, it may happen that new changes occur, so try to save the new changes again.
                                modifiedEntries = await this.DetectModifiedEntriesAsync(operationContext, cancellationToken).PreserveThreadContext();
                                currentIteration++;
                            }
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }
                    }).PreserveThreadContext();

            // log exceptions and other important data.
            if (exception != null)
            {
                this.Logger.Error(
                    exception,
                        "PersistChangesCommand.ExecuteAsync|ID: {0}|Message: {1}|Elapsed: {2}|Change count: {3}|Data: {4}",
                        operationContext.DataContext.Id,
                        exception.Message,
                        elapsed,
                        changes,
                        sb);
                throw exception;
            }

            if (elapsed.TotalMilliseconds > 1000)
            {
                this.Logger.Warn(
                        "PersistChangesCommand.ExecuteAsync|ID: {0}|Message: {1}|Elapsed: {2}|Change count: {3}|Data: {4}",
                        operationContext.DataContext.Id,
                        "Elapsed time more than 1s",
                        elapsed,
                        changes,
                        sb);
            }
            else
            {
                this.Logger.Debug(
                    "PersistChangesCommand.ExecuteAsync|ID: {0}|Message: {1}|Elapsed: {2}|Change count: {3}|Data: {4}",
                    operationContext.DataContext.Id,
                    "OK",
                    elapsed,
                    changes,
                    sb);
            }

            return new DataCommandResult(string.Format(Strings.PersistChangesCommand_ResultMessage, changes));
        }

        /// <summary>
        /// Resets the change state of the entities after the persistance.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="modifiedEntries">The modified entries.</param>
        protected virtual void ResetChangeState(IPersistChangesContext operationContext, IList<IEntityInfo> modifiedEntries)
        {
            var localCache = this.TryGetLocalCache(operationContext.DataContext);
            foreach (var entry in modifiedEntries)
            {
                if (entry.ChangeState != ChangeState.Deleted)
                {
                    entry.ChangeState = ChangeState.NotChanged;
                }
                else
                {
                    localCache?.Remove(entry.Id);
                }
            }
        }

        /// <summary>
        /// Executes the after save behaviors asynchronously.
        /// </summary>
        /// <param name="operationContext">The operationContext for persisting the changes.</param>
        /// <param name="modifiedEntries">The modified entries.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual async Task ExecuteAfterSaveBehaviorsAsync(IPersistChangesContext operationContext, IList<IEntityInfo> modifiedEntries, CancellationToken cancellationToken)
        {
            foreach (var entityInfo in modifiedEntries)
            {
                var reversedBehaviors = this.BehaviorProvider.GetDataBehaviors<IOnPersistBehavior>(entityInfo.Entity).Reverse();
                foreach (var behavior in reversedBehaviors)
                {
                    await behavior.AfterPersistAsync(entityInfo.Entity, entityInfo, operationContext, cancellationToken).PreserveThreadContext();
                }
            }
        }

        /// <summary>
        /// Executes the before save behaviors asynchronously.
        /// </summary>
        /// <param name="operationContext">The operationContext for persisting the changes.</param>
        /// <param name="modifiedEntries">The modified entries.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual async Task ExecuteBeforeSaveBehaviorsAsync(IPersistChangesContext operationContext, IList<IEntityInfo> modifiedEntries, CancellationToken cancellationToken)
        {
            foreach (var entityInfo in modifiedEntries)
            {
                var behaviors = this.BehaviorProvider.GetDataBehaviors<IOnPersistBehavior>(entityInfo.Entity);
                foreach (var behavior in behaviors)
                {
                    await behavior.BeforePersistAsync(entityInfo.Entity, entityInfo, operationContext, cancellationToken).PreserveThreadContext();
                }
            }
        }

        /// <summary>
        /// Detects the modified entries and returns them.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A list of modified entries tuples.
        /// </returns>
        protected virtual Task<IList<IEntityInfo>> DetectModifiedEntriesAsync(
            IPersistChangesContext operationContext,
            CancellationToken cancellationToken)
        {
            var localCache = this.TryGetLocalCache(operationContext.DataContext);
            if (localCache == null)
            {
                return Task.FromResult<IList<IEntityInfo>>(new List<IEntityInfo>());
            }

            var changes = localCache.Values
                .Where(e => e.ChangeState != ChangeState.NotChanged)
                .ToList();

            return Task.FromResult<IList<IEntityInfo>>(changes);
        }

        /// <summary>
        /// Saves the modified entries.
        /// </summary>
        /// <param name="modifiedEntries">The modified entries.</param>
        /// <param name="changes">The changes.</param>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="sb">The string builder to append logging data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The aggregated number of changes.
        /// </returns>
        protected virtual async Task<int> ValidateAndPersistModifiedEntriesAsync(
          IList<IEntityInfo> modifiedEntries,
          int changes,
          IPersistChangesContext operationContext,
          StringBuilder sb,
          CancellationToken cancellationToken)
        {
            var modifiedRootEntities = modifiedEntries
                    .Select(e => e.GetGraphRoot() ?? e.Entity)
                    .Distinct()
                    .ToList();

            if (modifiedRootEntities.Count == 0 && modifiedEntries.Count > 0)
            {
                throw new DataException(Strings.PersistChangesCommand_NoRootEntitiesToSave_Exception);
            }

            await this.ValidateModifiedEntriesAsync(modifiedEntries, operationContext, cancellationToken).PreserveThreadContext();

            await this.PreProcessModifiedEntriesAsync(modifiedEntries, operationContext, cancellationToken).PreserveThreadContext();

            try
            {
                await this.PersistModifiedEntriesAsync(modifiedEntries, operationContext, cancellationToken).PreserveThreadContext();
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException ?? tie;
            }

            await this.PostProcessModifiedEntriesAsync(modifiedEntries, operationContext, cancellationToken).PreserveThreadContext();

            changes += modifiedEntries.Count;
            return changes;
        }

        /// <summary>
        /// Validates the modified entries asynchronously.
        /// </summary>
        /// <param name="modifiedEntries">The modified entries.</param>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual async Task ValidateModifiedEntriesAsync(IList<IEntityInfo> modifiedEntries, IPersistChangesContext operationContext, CancellationToken cancellationToken)
        {
            var graphOperationContext = new GraphOperationContext(operationContext.DataContext) { LoadLooseParts = false };
            foreach (var entityInfo in modifiedEntries)
            {
                var entityGraphParts = await entityInfo
                                            .GetFlattenedGraphAsync(graphOperationContext, cancellationToken)
                                            .PreserveThreadContext()
                                        ?? new[] { entityInfo.Entity };

                foreach (var entityPart in entityGraphParts)
                {
                    var validationErrors = await this.ValidateModifiedEntryAsync(entityPart, operationContext, cancellationToken).PreserveThreadContext();
                    if (validationErrors.HasErrors())
                    {
                        throw new DataValidationException(entityPart, validationErrors);
                    }
                }
            }
        }

        /// <summary>
        /// Validates the modified entry asynchronously.
        /// </summary>
        /// <param name="entityPart">The entity part.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A <see cref="Task{IDataValidationResult}"/>.
        /// </returns>
        protected virtual async Task<IDataValidationResult> ValidateModifiedEntryAsync(
            object entityPart,
            IDataOperationContext operationContext,
            CancellationToken cancellationToken)
        {
            var dataContext = operationContext.DataContext;
            var validators = this.BehaviorProvider.GetDataBehaviors<IOnValidateBehavior>(entityPart);
            var result = new DataValidationResult();
            foreach (var validator in validators)
            {
                // TODO optimize getting the entity information
                var validatorResult = await validator.ValidateAsync(entityPart, dataContext.GetEntityInfo(entityPart), operationContext, cancellationToken).PreserveThreadContext();
                result.Add(validatorResult.ToArray());
            }

            return result;
        }

        /// <summary>
        /// Pre processes the modified entries asynchronously.
        /// </summary>
        /// <param name="modifiedEntries">The modified entries.</param>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task PreProcessModifiedEntriesAsync(IList<IEntityInfo> modifiedEntries, IPersistChangesContext operationContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Post processes the modified entries asynchronously.
        /// </summary>
        /// <param name="modifiedEntries">The modified entries.</param>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task PostProcessModifiedEntriesAsync(IList<IEntityInfo> modifiedEntries, IPersistChangesContext operationContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        /// <summary>
        /// Persists the modified entries asynchronously.
        /// </summary>
        /// <param name="modifiedEntries">The modified entries.</param>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task PersistModifiedEntriesAsync(
            IList<IEntityInfo> modifiedEntries,
            IPersistChangesContext operationContext,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}