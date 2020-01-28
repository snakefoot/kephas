﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LicensingState.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the licensing state class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Licensing
{
    using Kephas.Application;
    using Kephas.Operations;

    /// <summary>
    /// A licensing state.
    /// </summary>
    public class LicensingState : OperationResult<bool>, ILicensingState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LicensingState"/> class.
        /// </summary>
        /// <param name="appId">The app identity.</param>
        public LicensingState(AppIdentity appId)
            : this(appId, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicensingState"/> class.
        /// </summary>
        /// <param name="appId">The app identity.</param>
        /// <param name="isLicensed">True if the app is licensed, false if not.</param>
        public LicensingState(AppIdentity appId, bool isLicensed)
            : base(isLicensed)
        {
            this.AppId = appId;
        }

        /// <summary>
        /// Gets the app identity.
        /// </summary>
        /// <value>
        /// The app identity.
        /// </value>
        public AppIdentity AppId { get; }

        /// <summary>
        /// Gets a value indicating whether this app is licensed.
        /// </summary>
        /// <value>
        /// True if this app is licensed, false if not.
        /// </value>
        public bool IsLicensed => this.ReturnValue;
    }
}
