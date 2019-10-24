﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializationService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for serialization services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Contract for singleton serialization services.
    /// </summary>
    [SingletonAppServiceContract]
    public interface ISerializationService
    {
        /// <summary>
        /// Serializes the object with the provided options.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="textWriter">The text writer where the serialized object should be written.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        Task SerializeAsync(
            object obj,
            TextWriter textWriter,
            Action<ISerializationContext> optionsConfig = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Serializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the serialized object.
        /// </returns>
        Task<string> SerializeAsync(
            object obj,
            Action<ISerializationContext> optionsConfig = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deserializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="textReader">The text reader where from the serialized object should be read.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        Task<object> DeserializeAsync(
            TextReader textReader,
            Action<ISerializationContext> optionsConfig = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deserializes the object with the options provided in the serialization context.
        /// </summary>
        /// <param name="serializedObj">The serialized object.</param>
        /// <param name="optionsConfig">Optional. Function for serialization options configuration.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the deserialized object.
        /// </returns>
        Task<object> DeserializeAsync(
            string serializedObj,
            Action<ISerializationContext> optionsConfig = null,
            CancellationToken cancellationToken = default);
    }
}