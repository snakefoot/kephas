﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMethodInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for method information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Reflection
{
    using System.Collections.Generic;

    /// <summary>
    /// Contract for operation information.
    /// </summary>
    public interface IOperationInfo : IElementInfo
    {
        /// <summary>
        /// Gets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        ITypeInfo ReturnType { get; }

        /// <summary>
        /// Gets the method parameters.
        /// </summary>
        /// <value>
        /// The method parameters.
        /// </value>
        IEnumerable<IParameterInfo> Parameters { get; }

        /// <summary>
        /// Invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result.</returns>
        object? Invoke(object instance, IEnumerable<object?> args);
    }
}