﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicParameterInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dynamic parameter information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Dynamic
{
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;

    /// <summary>
    /// Dynamic parameter information.
    /// </summary>
    public class DynamicParameterInfo : DynamicElementInfo, IParameterInfo
    {
        /// <summary>
        /// Gets or sets the parameter value type.
        /// </summary>
        /// <value>
        /// The parameter value type.
        /// </value>
        public ITypeInfo ValueType { get; protected internal set; }

        /// <summary>
        /// Gets or sets the position in the parameter's list.
        /// </summary>
        /// <value>
        /// The position in the parameter's list.
        /// </value>
        public int Position { get; protected internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether this parameter is optional.
        /// </summary>
        /// <value>
        /// <c>true</c> if the parameter is optional, <c>false</c> otherwise.
        /// </value>
        public bool IsOptional { get; protected internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter is for input.
        /// </summary>
        /// <value>
        /// True if this parameter is for input, false if not.
        /// </value>
        public bool IsIn { get; protected internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter is for output.
        /// </summary>
        /// <value>
        /// True if this parameter is for output, false if not.
        /// </value>
        public bool IsOut { get; protected internal set; }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public void SetValue(object? obj, object? value)
        {
            Requires.NotNull(obj, nameof(obj));

            if (obj is IExpando expando)
            {
                expando[this.Name] = value;
            }

            obj?.SetPropertyValue(this.Name, value);
        }

        /// <summary>
        /// Gets the value from the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The value.
        /// </returns>
        public object? GetValue(object? obj)
        {
            Requires.NotNull(obj, nameof(obj));

            if (obj is IExpando expando)
            {
                return expando[this.Name];
            }

            return obj?.GetPropertyValue(this.Name);
        }
    }
}