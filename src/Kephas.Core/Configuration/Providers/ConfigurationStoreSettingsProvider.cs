﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationStoreSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the configuration store settings provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration.Providers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// A configuration store settings provider.
    /// </summary>
    [ProcessingPriority(Priority.Low)]
    public class ConfigurationStoreSettingsProvider : ISettingsProvider
    {
        private readonly IConfigurationStore configurationStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationStoreSettingsProvider"/> class.
        /// </summary>
        /// <param name="configurationStore">The configuration store.</param>
        public ConfigurationStoreSettingsProvider(IConfigurationStore configurationStore)
        {
            Requires.NotNull(configurationStore, nameof(configurationStore));

            this.configurationStore = configurationStore;
        }

        /// <summary>
        /// Gets the settings with the provided type.
        /// </summary>
        /// <param name="settingsType">Type of the settings.</param>
        /// <returns>
        /// The settings.
        /// </returns>
        public object GetSettings(Type settingsType)
        {
            Requires.NotNull(settingsType, nameof(settingsType));

            return this.configurationStore.TryGetSettings(settingsType);
        }

        /// <summary>
        /// Updates the settings asynchronously.
        /// </summary>
        /// <param name="settings">The settings to be updated.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task UpdateSettingsAsync(object settings, CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            this.configurationStore.UpdateSettings(settings);
        }
    }
}
