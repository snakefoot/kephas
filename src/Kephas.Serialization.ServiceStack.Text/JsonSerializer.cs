﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the JSON serializer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Net.Mime;
    using Kephas.Services;

    /// <summary>
    /// A JSON serializer based on the ServiceStack infrastructure.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class JsonSerializer : ISerializer<JsonMediaType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializer"/> class.
        /// </summary>
        /// <param name="jsonSerializerConfigurator">The JSON serializer configurator.</param>
        public JsonSerializer(IJsonSerializerConfigurator jsonSerializerConfigurator)
        {
            Requires.NotNull(jsonSerializerConfigurator, nameof(jsonSerializerConfigurator));

            jsonSerializerConfigurator.ConfigureJsonSerialization();
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<JsonSerializer> Logger { get; set; }

        /// <summary>
        /// Serializes the provided object asynchronously.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="textWriter">The <see cref="TextWriter"/> used to write the object content.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the serialized object as a string.
        /// </returns>
        public Task SerializeAsync(object obj, TextWriter textWriter, ISerializationContext context = null, CancellationToken cancellationToken = default)
        {
            global::ServiceStack.Text.JsonSerializer.SerializeToWriter(obj, textWriter);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Deserialize an object asynchronously.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task promising the deserialized object.
        /// </returns>
        public Task<object> DeserializeAsync(TextReader textReader, ISerializationContext context = null, CancellationToken cancellationToken = default)
        {
            var obj = global::ServiceStack.Text.JsonSerializer.DeserializeFromReader(textReader, context?.RootObjectType ?? typeof(object));
            return Task.FromResult(obj);
        }
    }
}