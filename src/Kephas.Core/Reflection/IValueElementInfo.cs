﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValueElementInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IValueElementInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Reflection
{
    /// <summary>
    /// Contract for reflection elements holding a value.
    /// </summary>
    public interface IValueElementInfo : IElementInfo
    {
        /// <summary>
        /// Gets the type of the element's value.
        /// </summary>
        /// <value>
        /// The type of the element's value.
        /// </value>
        ITypeInfo ValueType { get; }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        void SetValue(object? obj, object? value);

        /// <summary>
        /// Gets the value from the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The value.
        /// </returns>
        object? GetValue(object? obj);
    }
}