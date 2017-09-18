﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the configuration base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Configuration.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// Provides the configuration for the settings type indicated as the generic paramter type.
    /// </summary>
    /// <remarks>
    /// Being an <see cref="Expando"/>, various values may be added to runtime to this configuration.
    /// </remarks>
    /// <typeparam name="TSettings">Type of the settings.</typeparam>
    public class Configuration<TSettings> : Expando, IConfiguration<TSettings>
        where TSettings : class
    {
        /// <summary>
        /// The provider factories.
        /// </summary>
        private readonly ICollection<IExportFactory<IConfigurationProvider, ConfigurationProviderMetadata>> providerFactories;

        /// <summary>
        /// The settings.
        /// </summary>
        private TSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration{TSettings}"/> class.
        /// </summary>
        /// <param name="providerFactories">The provider factories.</param>
        protected Configuration(ICollection<IExportFactory<IConfigurationProvider, ConfigurationProviderMetadata>> providerFactories)
        {
            Requires.NotNull(providerFactories, nameof(providerFactories));

            this.providerFactories = providerFactories;
        }

        /// <summary>
        /// Gets the settings associated to this configuration.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        public TSettings Settings => this.settings ?? (this.settings = this.ComputeSettings());

        /// <summary>
        /// Calculates the settings.
        /// </summary>
        /// <returns>
        /// The calculated settings.
        /// </returns>
        private TSettings ComputeSettings()
        {
            var orderedFactories = this.providerFactories
                .OrderBy(f => f.Metadata.OverridePriority)
                .ThenBy(f => f.Metadata.ProcessingPriority)
                .ToList();

            var factory = orderedFactories.FirstOrDefault(f => f.Metadata.SettingsType == typeof(TSettings));
            if (factory == null)
            {
                factory = orderedFactories.FirstOrDefault(f => f.Metadata.SettingsType.GetTypeInfo().IsAssignableFrom(typeof(TSettings).GetTypeInfo()));
                if (factory == null)
                {
                    factory = orderedFactories.FirstOrDefault(f => f.Metadata.SettingsType == null);
                }
            }

            if (factory == null)
            {
                throw new NotSupportedException();
            }

            return (TSettings)factory.CreateExportedValue().GetSettings(typeof(TSettings));
        }
    }
}