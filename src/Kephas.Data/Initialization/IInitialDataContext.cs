﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInitialDataContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IInitialDataContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Initialization
{
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// Interface for initial data context.
    /// </summary>
    public interface IInitialDataContext : IContext
    {
        /// <summary>
        /// Gets the initial data kinds.
        /// </summary>
        /// <value>
        /// The initial data kinds.
        /// </value>
        IEnumerable<string> InitialDataKinds { get; }
    }
}