﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IScript.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IScript interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting
{
    using Kephas.Dynamic;

    /// <summary>
    /// Interface for script.
    /// </summary>
    public interface IScript : IExpando
    {
        /// <summary>
        /// Gets the script language.
        /// </summary>
        /// <value>
        /// The script language.
        /// </value>
        string Language { get; }

        /// <summary>
        /// Gets the source code as a stream.
        /// </summary>
        /// <value>
        /// The source code.
        /// </value>
        string SourceCode { get; }
    }
}