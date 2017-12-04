﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSerializer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the XML serializer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Net.Mime;
    using Kephas.Services;

    /// <summary>
    /// An XML serializer based on the ServiceStack infrastructure.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class XmlSerializer : ISerializer<XmlMediaType>
    {
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
        public Task SerializeAsync(
            object obj,
            TextWriter textWriter,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default)
        {
            global::ServiceStack.Text.XmlSerializer.SerializeToWriter(obj, textWriter);
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
        public Task<object> DeserializeAsync(
            TextReader textReader,
            ISerializationContext context = null,
            CancellationToken cancellationToken = default)
        {
            // TODO better implement
            var obj = global::ServiceStack.Text.XmlSerializer.DeserializeFromString(textReader.ReadToEnd(), context?.RootObjectType ?? typeof(object));
            return Task.FromResult(obj);
        }
    }
}