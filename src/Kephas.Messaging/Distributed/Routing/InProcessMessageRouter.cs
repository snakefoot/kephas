﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InProcessMessageBroker.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null distributed message broker class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Routing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Resources;
    using Kephas.Services;
    using Kephas.Text;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An in process message router invoking the message processor.
    /// </summary>
    /// <remarks>
    /// For the in-process message router, the <see cref="DispatchAsync"/> method represents the input queue.
    /// </remarks>
    [ProcessingPriority(Priority.Lowest)]
    [MessageRouter(IsFallback = true)]
    public class InProcessMessageRouter : MessageRouterBase
    {
        private readonly Lazy<IMessageBroker> lazyMessageBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="InProcessMessageRouter"/> class.
        /// </summary>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="messageBuilderFactory">The message builder factory.</param>
        /// <param name="lazyMessageBroker">The lazy message broker.</param>
        public InProcessMessageRouter(
            IMessageProcessor messageProcessor,
            IExportFactory<IBrokeredMessageBuilder> messageBuilderFactory,
            Lazy<IMessageBroker> lazyMessageBroker)
            : base(messageProcessor, messageBuilderFactory)
        {
            Requires.NotNull(lazyMessageBroker, nameof(lazyMessageBroker));

            this.lazyMessageBroker = lazyMessageBroker;
        }

        /// <summary>
        /// Sends the brokered message asynchronously over the physical medium.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The routing context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding an action to take further and an optional reply.
        /// </returns>
        public override Task<(RoutingInstruction action, IMessage reply)> DispatchAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            Requires.NotNull(brokeredMessage, nameof(brokeredMessage));
            Requires.NotNull(context, nameof(context));

            return this.RouteInputAsync(brokeredMessage, context, cancellationToken);
        }

        /// <summary>
        /// Processes the brokered message locally, asynchronously.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The routing context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the reply message.
        /// </returns>
        protected override Task<IMessage> ProcessAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            var completionSource = new TaskCompletionSource<IMessage>();

            // make processing really async for in-process handling
            Task.Factory.StartNew(
                async () =>
                {
                    try
                    {
                        var result = await base.ProcessAsync(brokeredMessage, context, cancellationToken).PreserveThreadContext();
                        completionSource.SetResult(result);
                    }
                    catch (Exception ex)
                    {
                        completionSource.SetException(ex);
                    }
                },
                cancellationToken);

            return completionSource.Task;
        }

        /// <summary>
        /// Routes the brokered message asynchronously, typically over the physical medium.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The send context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The asynchronous result yielding an action to take further and an optional reply.
        /// </returns>
        protected override Task<(RoutingInstruction action, IMessage reply)> RouteOutputAsync(IBrokeredMessage brokeredMessage, IContext context, CancellationToken cancellationToken)
        {
            // replies should be returned back to the message broker, without further routing
            // because the processing takes place in-process, so the initiator of the dispatch
            // is in-process.
            if (brokeredMessage.ReplyToMessageId != null)
            {
                return Task.FromResult<(RoutingInstruction action, IMessage reply)>((RoutingInstruction.Reply, brokeredMessage.Content));
            }

            // typically, this should not get here at all
            // because the only input expected is from DispatchAsync and, in this case,
            // the RouteOutputAsync is called only when a response is expected.
            this.lazyMessageBroker.Value
                .DispatchAsync(brokeredMessage, context, cancellationToken)
                .ContinueWith(
                    t => this.Logger.Error(t.Exception, Strings.MessageRouterBase_ErrorsOccurredWhileDispatching_Exception.FormatWith(brokeredMessage)),
                    TaskContinuationOptions.OnlyOnFaulted);

            return Task.FromResult<(RoutingInstruction action, IMessage reply)>((RoutingInstruction.None, null));
        }
    }
}