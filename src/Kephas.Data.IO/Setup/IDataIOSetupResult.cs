﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataIOSetupResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataIOSetupResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    using Kephas.Data.Setup;

    /// <summary>
    /// Contract for data exchange result.
    /// </summary>
    public interface IDataIOSetupResult : IDataIOResult, IDataSetupResult
    {
    }
}