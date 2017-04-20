﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IEntityInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Capabilities
{
    using Kephas.Dynamic;

    /// <summary>
    /// Provides extended entity information like the <see cref="ChangeState"/>.
    /// </summary>
    public interface IEntityInfo : IExpando, IChangeStateTrackable, IIdentifiable
    {
        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        object Entity { get; }
    }
}