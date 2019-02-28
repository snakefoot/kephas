﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lock.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lock class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.MongoDB.JobStore.Model
{
    using System;

    using Kephas.Scheduling.Quartz.JobStore.Model;

    /// <summary>
    /// A lock entity.
    /// </summary>
    public class Lock : QuartzEntityBase, ILock
    {
        public const string TriggerAccess = "TRIGGER_ACCESS";
        public const string StateAccess = "STATE_ACCESS";

        public string InstanceId { get; set; }

        public DateTime AcquiredAt { get; set; }

        /// <summary>
        /// Gets or sets the type of the lock.
        /// </summary>
        /// <value>
        /// The type of the lock.
        /// </value>
        public LockType LockType { get; set; }
    }
}