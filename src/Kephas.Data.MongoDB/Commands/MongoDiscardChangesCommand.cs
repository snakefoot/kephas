﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDiscardChangesCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the mongo discard changes command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Commands
{
    using Kephas.Data.Commands;

    /// <summary>
    /// Discard changes command for <see cref="MongoDataContext"/>.
    /// </summary>
    [DataContextType(typeof(MongoDataContext))]
    public class MongoDiscardChangesCommand : DiscardChangesCommandBase
    {
    }
}