﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonSerializerConfigurator.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IJsonSerializerConfigurator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text
{
    using Kephas.Services;

    /// <summary>
    /// Interface for JSON serializer configurator.
    /// </summary>
    [SharedAppServiceContract]
    public interface IJsonSerializerConfigurator
    {
        /// <summary>
        /// Configures the JSON serialization.
        /// </summary>
        /// <param name="overwrite">True to overwrite the configuration, false to preserve it (optional).</param>
        /// <returns>
        /// True if the configuration was changed, false otherwise.
        /// </returns>
        bool ConfigureJsonSerialization(bool overwrite = false);
    }
}