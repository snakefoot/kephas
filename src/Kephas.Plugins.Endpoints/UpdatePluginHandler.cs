﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdatePluginHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the update plugin message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Application.Reflection;
using Kephas.Versioning;

namespace Kephas.Plugins.Endpoints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Operations;
    using Kephas.Plugins;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An update plugin message handler.
    /// </summary>
    public class UpdatePluginHandler : MessageHandlerBase<UpdatePluginMessage, ResponseMessage>
    {
        private readonly IPluginManager pluginManager;
        private readonly IAppContext appContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePluginHandler"/> class.
        /// </summary>
        /// <param name="pluginManager">Manager for plugins.</param>
        /// <param name="appContext">Context for the application.</param>
        public UpdatePluginHandler(IPluginManager pluginManager, IAppContext appContext)
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
        public override async Task<ResponseMessage> ProcessAsync(UpdatePluginMessage message, IMessagingContext context, CancellationToken token)
        {
            var toUpdate = await this.GetPackagesToUpdateAsync(message, token).PreserveThreadContext();

            if (toUpdate.Count == 0)
            {
                this.appContext.Logger.Info("No packages to update, all have the requested version.");

                return new ResponseMessage
                {
                    Message = "No packages to update, all have the requested version.",
                };
            }

            var successful = 0;
            var failed = 0;
            foreach (var pluginIdentity in toUpdate)
            {
                var result = await this.UpdatePluginAsync(pluginIdentity, context, token)
                    .PreserveThreadContext();
                if (result.OperationState == OperationState.Failed)
                {
                    failed++;
                }
                else
                {
                    successful++;
                }
            }

            return new ResponseMessage
            {
                Message = successful == 0
                    ? $"{failed} failed updates."
                    : failed == 0
                        ? $"{successful} successful updates."
                        : $"{successful} successful updates, {failed} failed updates.",
            };
        }

        private async Task<List<AppIdentity>> GetPackagesToUpdateAsync(UpdatePluginMessage message, CancellationToken token)
        {
            List<AppIdentity> toUpdate;
            if (message.Id.Equals(UpdatePluginMessage.All, StringComparison.InvariantCultureIgnoreCase))
            {
                var installedPlugins = this.pluginManager.GetInstalledPlugins().ToList();

                if (UpdatePluginMessage.LatestVersion.Equals(message.Version, StringComparison.InvariantCultureIgnoreCase))
                {
                    var availablePackages = new List<IAppInfo>();
                    foreach (var installedPlugin in installedPlugins)
                    {
                        var availablePackage = await this.pluginManager.GetLatestAvailablePluginVersionAsync(installedPlugin.Identity, message.IncludePrerelease, token).PreserveThreadContext();
                        if (availablePackage != null && availablePackage.Identity.Version != installedPlugin.Identity.Version)
                        {
                            availablePackages.Add(availablePackage);
                        }
                    }

                    toUpdate = availablePackages
                        .Select(p => new AppIdentity(p.Identity.Id, p.Identity.Version))
                        .ToList();
                }
                else
                {
                    toUpdate = installedPlugins
                        .Where(p => !p.Identity.Version!.Equals(message.Version))
                        .Select(p => new AppIdentity(p.Identity.Id, message.Version))
                        .ToList();
                }
            }
            else
            {
                var installedPlugin = this.pluginManager.GetInstalledPlugins()
                    .FirstOrDefault(p => p.Identity.Id.Equals(message.Id, StringComparison.OrdinalIgnoreCase));
                if (installedPlugin == null)
                {
                    toUpdate = new List<AppIdentity>();
                }
                else if (UpdatePluginMessage.LatestVersion.Equals(message.Version, StringComparison.InvariantCultureIgnoreCase))
                {
                    var availablePackage = await this.pluginManager.GetLatestAvailablePluginVersionAsync(new AppIdentity(message.Id), message.IncludePrerelease, token).PreserveThreadContext();
                    toUpdate = availablePackage != null && availablePackage.Identity.Version != installedPlugin.Identity.Version
                        ? new List<AppIdentity> { availablePackage.Identity }
                        : new List<AppIdentity>();
                }
                else
                {
                    toUpdate = SemanticVersion.Parse(message.Version) != installedPlugin.Identity.Version
                        ? new List<AppIdentity> { new AppIdentity(message.Id, message.Version) }
                        : new List<AppIdentity>();
                }
            }

            return toUpdate;
        }

        private async Task<IOperationResult> UpdatePluginAsync(AppIdentity pluginIdentity, IMessagingContext context, CancellationToken token)
        {
            try
            {
                this.appContext.Logger.Info(
                    "Updating plugin {plugin} to version {version}...",
                    pluginIdentity.Id,
                    pluginIdentity.Version);

                var result = await this.pluginManager
                    .UpdatePluginAsync(pluginIdentity, ctx => ctx.Merge(context), token)
                    .PreserveThreadContext();

                var plugin = result.Value;
                var pluginId = plugin?.GetTypeInfo().Name ?? pluginIdentity.Id;
                var pluginVersion = plugin?.GetTypeInfo().Identity.Version ?? pluginIdentity.Version;

                this.appContext.Logger.Info(
                    "Plugin {plugin} updated to version {version} in {pluginPath}. Elapsed: {elapsed:c}.",
                    pluginId,
                    pluginVersion,
                    plugin?.Location,
                    result.Elapsed);
                return result.MergeMessage(
                    $"Plugin {pluginId} updated to version {pluginVersion} in {plugin?.Location}. Elapsed: {result.Elapsed:c}.");
            }
            catch (Exception ex)
            {
                this.appContext.Logger.Error(
                    ex,
                    "Plugin {plugin} could not be updated to version {version}.",
                    pluginIdentity.Id,
                    pluginIdentity.Version);
                return new OperationResult<IPlugin>()
                    .MergeMessage(
                        $"Plugin {pluginIdentity.Id} could not be updated to version {pluginIdentity.Version}.")
                    .Complete(TimeSpan.Zero, OperationState.Failed);
            }
        }
    }
}