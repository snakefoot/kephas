﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPermissionType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IPermissionType interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Security.Authorization
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for permission type.
    /// </summary>
    public interface IPermissionType : IClassifier
    {
        /// <summary>
        /// Gets the granted permissions.
        /// </summary>
        /// <remarks>
        /// When this permission is granted, the permissions granted by this are also granted.
        /// Using this mechanism one can define a hierarchy of permissions.
        /// </remarks>
        /// <value>
        /// The granted permissions.
        /// </value>
        IEnumerable<IPermissionType> GrantedPermissions { get; }
    }
}