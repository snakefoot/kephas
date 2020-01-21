﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnablePluginMessageHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the enable plugin message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Endpoints
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Plugins;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An enable plugin message handler.
    /// </summary>
    public class EnablePluginMessageHandler : MessageHandlerBase<EnablePluginMessage, ResponseMessage>
    {
        private readonly IPluginManager pluginManager;
        private readonly IAppContext appContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnablePluginMessageHandler"/> class.
        /// </summary>
        /// <param name="pluginManager">Manager for plugins.</param>
        /// <param name="appContext">Context for the application.</param>
        public EnablePluginMessageHandler(IPluginManager pluginManager, IAppContext appContext)
        {
            this.pluginManager = pluginManager;
            this.appContext = appContext;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public override async Task<ResponseMessage> ProcessAsync(EnablePluginMessage message, IMessagingContext context, CancellationToken token)
        {
            this.appContext.Logger.Info("Enabling plugin {plugin}...", message.Id);

            var result = await this.pluginManager.EnablePluginAsync(new PluginIdentity(message.Id)).PreserveThreadContext();

            this.appContext.Logger.Info("Plugin {plugin} enabled.", message.Id);

            return new ResponseMessage
            {
                Message = $"Plugin {message.Id} enabled.",
            };
        }
    }
}