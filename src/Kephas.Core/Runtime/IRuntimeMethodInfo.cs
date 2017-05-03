﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeMethodInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for dynamically invoking a method.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// Contract for a dynamic <see cref="MethodInfo"/>.
    /// </summary>
    public interface IRuntimeMethodInfo : IMethodInfo, IRuntimeElementInfo
    {
        /// <summary>
        /// Gets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        new IRuntimeTypeInfo ReturnType { get; }

        /// <summary>
        /// Gets the method information.
        /// </summary>
        /// <value>
        /// The method information.
        /// </value>
        MethodInfo MethodInfo { get; }

        /// <summary>
        /// Tries to invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="result">The invocation result.</param>
        /// <returns>A boolean value indicating whether the invocation was successful or not.</returns>
        bool TryInvoke(object instance, IEnumerable<object> args, out object result);
    }
}
