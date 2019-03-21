﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Builder for ambient services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.Hosting;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Builder for ambient services.
    /// </summary>
    public class AmbientServicesBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServicesBuilder"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        public AmbientServicesBuilder(IAmbientServices ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            this.AmbientServices = ambientServices;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientServicesBuilder"/> class.
        /// </summary>
        public AmbientServicesBuilder()
            : this(Kephas.AmbientServices.Instance)
        {
        }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public IAmbientServices AmbientServices { get; }

        /// <summary>
        /// Sets the log manager to the ambient services.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        /// <returns>
        /// The ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithLogManager(ILogManager logManager)
        {
            Requires.NotNull(logManager, nameof(logManager));

            this.AmbientServices.RegisterService(logManager);

            return this;
        }

        /// <summary>
        /// Sets the application runtime to the ambient services.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <returns>
        /// The ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithAppRuntime(IAppRuntime appRuntime)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));

            this.AmbientServices.RegisterService(appRuntime);

            return this;
        }

        /// <summary>
        /// Sets the composition container to the ambient services.
        /// </summary>
        /// <param name="compositionContainer">The composition container.</param>
        /// <returns>
        /// The ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithCompositionContainer(ICompositionContext compositionContainer)
        {
            Requires.NotNull(compositionContainer, nameof(compositionContainer));

            this.AmbientServices.RegisterService(compositionContainer);

            return this;
        }

        /// <summary>
        /// Sets the composition container to the ambient services.
        /// </summary>
        /// <typeparam name="TContainerBuilder">Type of the composition container builder.</typeparam>
        /// <param name="containerBuilderConfig">The container builder configuration.</param>
        /// <remarks>The container builder type must provide a constructor with one parameter of type <see cref="IContext" />.</remarks>
        /// <returns>
        /// The provided ambient services builder.
        /// </returns>
        public AmbientServicesBuilder WithCompositionContainer<TContainerBuilder>(Action<TContainerBuilder> containerBuilderConfig = null)
            where TContainerBuilder : ICompositionContainerBuilder
        {
            var builderType = typeof(TContainerBuilder).AsRuntimeTypeInfo();
            var context = new CompositionRegistrationContext(this.AmbientServices);

            var containerBuilder = (TContainerBuilder)builderType.CreateInstance(new[] { context });

            containerBuilderConfig?.Invoke(containerBuilder);

            return this.WithCompositionContainer(containerBuilder.CreateContainer());
        }
    }
}