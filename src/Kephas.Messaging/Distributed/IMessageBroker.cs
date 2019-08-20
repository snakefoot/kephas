﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDistributedMessageBroker.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDistributedMessageBroker interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Singleton application service contract for distributed message broker.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IMessageBroker : IDisposable
    {
        /// <summary>
        /// Dispatches the brokered message asynchronously.
        /// </summary>
        /// <param name="brokeredMessage">The brokered message.</param>
        /// <param name="context">The dispatching context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result that yields an IMessage.
        /// </returns>
        Task<IMessage> DispatchAsync(
            IBrokeredMessage brokeredMessage,
            IContext context = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Notification method for a received reply.
        /// </summary>
        /// <param name="replyMessage">Message describing the reply.</param>
        /// <param name="context">The reply context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        Task ReplyReceivedAsync(
            IBrokeredMessage replyMessage,
            IContext context = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a brokered message builder.
        /// </summary>
        /// <typeparam name="TMessage">Type of the brokered message.</typeparam>
        /// <param name="context">Optional. The sending context.</param>
        /// <param name="brokeredMessage">Optional. The brokered message. If not set, a new one will be created.</param>
        /// <returns>
        /// The new brokered message builder.
        /// </returns>
        IBrokeredMessageBuilder CreateBrokeredMessageBuilder<TMessage>(IContext context = null, TMessage brokeredMessage = default)
            where TMessage : IBrokeredMessage;
    }
}