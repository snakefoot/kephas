﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppSettingsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Configuration
{
    using Kephas.Configuration;
    using Kephas.Services;

    /// <summary>
    /// The default implementation of the <see cref="IAppSettingsProvider"/>
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultAppSettingsProvider : IAppSettingsProvider
    {
        private readonly IAppRuntime appRuntime;
        private readonly IConfiguration<SystemSettings> systemConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAppSettingsProvider"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="systemConfiguration">The system configuration.</param>
        public DefaultAppSettingsProvider(IAppRuntime appRuntime, IConfiguration<SystemSettings> systemConfiguration)
        {
            this.appRuntime = appRuntime;
            this.systemConfiguration = systemConfiguration;
        }

        /// <summary>
        /// Gets the application settings for the executing instance.
        /// </summary>
        /// <returns>The application settings.</returns>
        public AppSettings? GetAppSettings()
        {
            var systemSettings = this.systemConfiguration.Settings;
            var appId = this.appRuntime.GetAppId()!;
            return systemSettings.Instances?[appId];
        }
    }
}