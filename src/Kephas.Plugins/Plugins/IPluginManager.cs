﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPluginManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IPluginManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Reflection;
    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// Interface for plugin manager.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IPluginManager
    {
        /// <summary>
        /// Downloads the plugin asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the download operation result.
        /// </returns>
        Task<IOperationResult> DownloadPluginAsync(AppIdentity pluginIdentity, Action<IPluginContext> options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Installs the plugin asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the install operation result.
        /// </returns>
        Task<IOperationResult<IPlugin>> InstallPluginAsync(AppIdentity pluginIdentity, Action<IPluginContext> options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uninstalls the plugin asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the uninstall operation result.
        /// </returns>
        Task<IOperationResult<IPlugin>> UninstallPluginAsync(AppIdentity pluginIdentity, Action<IPluginContext> options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Initializes the plugin asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the initialize operation result.
        /// </returns>
        Task<IOperationResult<IPlugin>> InitializePluginAsync(AppIdentity pluginIdentity, Action<IPluginContext> options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uninitializes the plugin asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the uninitialize operation result.
        /// </returns>
        Task<IOperationResult<IPlugin>> UninitializePluginAsync(AppIdentity pluginIdentity, Action<IPluginContext> options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the plugin asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the update operation result.
        /// </returns>
        Task<IOperationResult<IPlugin>> UpdatePluginAsync(AppIdentity pluginIdentity, Action<IPluginContext> options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Enables the plugin asynchronously if the plugin was previously disabled.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the enable operation result.
        /// </returns>
        Task<IOperationResult<IPlugin>> EnablePluginAsync(AppIdentity pluginIdentity, Action<IPluginContext> options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Disables the plugin asynchronously if the plugin was previously enabled.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the enable operation result.
        /// </returns>
        Task<IOperationResult<IPlugin>> DisablePluginAsync(AppIdentity pluginIdentity, Action<IPluginContext> options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the plugin state.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <returns>
        /// The plugin state.
        /// </returns>
        PluginState GetPluginState(AppIdentity pluginIdentity);

        /// <summary>
        /// Gets the installed plugins.
        /// </summary>
        /// <returns>
        /// An enumeration of installed plugins.
        /// </returns>
        IEnumerable<IPlugin> GetInstalledPlugins();

        /// <summary>
        /// Gets the plugins available in the package sources asynchronously.
        /// </summary>
        /// <param name="filter">Specifies the filter.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the available plugins.
        /// </returns>
        Task<IOperationResult<IEnumerable<IAppInfo>>> GetAvailablePluginsAsync(Action<ISearchContext> filter = null, CancellationToken cancellationToken = default);
    }
}
