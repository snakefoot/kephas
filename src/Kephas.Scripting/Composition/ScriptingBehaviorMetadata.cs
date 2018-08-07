﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScriptingBehaviorMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scripting behavior metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Composition
{
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services.Composition;

    /// <summary>
    /// A scripting behavior metadata.
    /// </summary>
    public class ScriptingBehaviorMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptingBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public ScriptingBehaviorMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.Language = (string)metadata.TryGetValue(nameof(this.Language));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptingBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="processingPriority">The processing priority (optional).</param>
        /// <param name="overridePriority">The override priority (optional).</param>
        /// <param name="optionalService">True to optional service (optional).</param>
        /// <param name="serviceName">Name of the service (optional).</param>
        public ScriptingBehaviorMetadata(string language, int processingPriority = 0, int overridePriority = 0, bool optionalService = false, string serviceName = null)
            : base(processingPriority, overridePriority, optionalService, serviceName)
        {
            Requires.NotNullOrEmpty(language, nameof(language));

            this.Language = language;
        }

        /// <summary>
        /// Gets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; }
    }
}