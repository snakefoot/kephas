﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspNetAppManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the owin application manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Application
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Composition;
    using Kephas.AspNetCore.Resources;
    using Kephas.Composition;
    using Kephas.Services;
    using Kephas.Services.Behavior;
    using Kephas.Services.Composition;

    /// <summary>
    /// An OWIN application manager.
    /// </summary>
    [OverridePriority(Priority.BelowNormal)]
    public class AspNetAppManager : DefaultAppManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetAppManager"/> class.
        /// </summary>
        /// <param name="appManifest">The application manifest.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="serviceBehaviorProvider">The service behavior provider.</param>
        /// <param name="appLifecycleBehaviorFactories">The application lifecycle behavior factories.</param>
        /// <param name="featureManagerFactories">The feature manager factories.</param>
        /// <param name="featureLifecycleBehaviorFactories">The feature lifecycle behavior factories.</param>
        public AspNetAppManager(
            IAppManifest appManifest,
            IAmbientServices ambientServices,
            IServiceBehaviorProvider serviceBehaviorProvider,
            ICollection<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>> appLifecycleBehaviorFactories,
            ICollection<IExportFactory<IFeatureManager, FeatureManagerMetadata>> featureManagerFactories,
            ICollection<IExportFactory<IFeatureLifecycleBehavior, AppServiceMetadata>> featureLifecycleBehaviorFactories)
            : base(
                appManifest,
                ambientServices,
                serviceBehaviorProvider,
                appLifecycleBehaviorFactories,
                featureManagerFactories,
                featureLifecycleBehaviorFactories)
        {
        }

        /// <summary>
        /// Initializes the application asynchronously.
        /// </summary>
        /// <param name="appContext">       Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override Task InitializeAppAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            if (!(appContext is IAspNetAppContext))
            {
                throw new InvalidOperationException(string.Format(Strings.AspNetFeatureManager_InvalidAppContext_Exception, appContext?.GetType().FullName, typeof(IAspNetAppContext).FullName));
            }

            return base.InitializeAppAsync(appContext, cancellationToken);
        }

        /// <summary>
        /// Finaliyes the application asynchronously.
        /// </summary>
        /// <param name="appContext">       Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public override Task FinalizeAppAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            if (!(appContext is IAspNetAppContext))
            {
                throw new InvalidOperationException(string.Format(Strings.AspNetFeatureManager_InvalidAppContext_Exception, appContext?.GetType().FullName, typeof(IAspNetAppContext).FullName));
            }

            return base.FinalizeAppAsync(appContext, cancellationToken);
        }
    }
}