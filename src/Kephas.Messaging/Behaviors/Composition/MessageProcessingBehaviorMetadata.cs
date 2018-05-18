﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageProcessingBehaviorMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Composition metadata for <see cref="IMessageProcessingBehavior" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Behaviors.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Messaging.Behaviors;
    using Kephas.Services.Composition;

    /// <summary>
    /// Composition metadata for <see cref="IMessageProcessingBehavior"/>.
    /// </summary>
    public class MessageProcessingBehaviorMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessingBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public MessageProcessingBehaviorMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.MessageType = (Type)metadata.TryGetValue(nameof(this.MessageType));
            this.MessageName = (string)metadata.TryGetValue(nameof(this.MessageName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessingBehaviorMetadata" /> class.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageName">The name of the message (optional).</param>
        /// <param name="processingPriority">The processing priority.</param>
        /// <param name="overridePriority">The override priority.</param>
        public MessageProcessingBehaviorMetadata(Type messageType, string messageName = null, int processingPriority = 0, int overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            this.MessageType = messageType;
            this.MessageName = messageName;
        }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        public Type MessageType { get; }

        /// <summary>
        /// Gets the name of the message.
        /// </summary>
        /// <value>
        /// The name of the message.
        /// </value>
        public string MessageName { get; }
    }
}