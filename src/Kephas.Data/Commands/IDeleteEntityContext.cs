﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDeleteEntityContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDeleteEntityContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    /// <summary>
    /// Interface for delete entity context.
    /// </summary>
    public interface IDeleteEntityContext : IDataOperationContext
    {
        /// <summary>
        /// Gets the entity to delete.
        /// </summary>
        /// <value>
        /// The entity to delete.
        /// </value>
        object Entity { get; }
    }
}