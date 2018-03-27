﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFinalizable.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IFinalizable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    /// <summary>
    /// Provides the <see cref="Finalize"/> method for service finalization.
    /// </summary>
    public interface IFinalizable
    {
        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <param name="context">An optional context for finalization.</param>
        void Finalize(IContext context = null);
    }
}