﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeParameterInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime parameter information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Dynamic;
    using Kephas.Reflection;

    /// <summary>
    /// Information about the runtime parameter.
    /// </summary>
    public class RuntimeParameterInfo : Expando, IRuntimeParameterInfo
    {
        /// <summary>
        /// The runtime type of <see cref="RuntimeParameterInfo"/>.
        /// </summary>
        private static readonly IRuntimeTypeInfo RuntimeTypeInfoOfRuntimeParameterInfo = new RuntimeTypeInfo(typeof(RuntimeParameterInfo));

        /// <summary>
        /// The declaring container reference.
        /// </summary>
        private readonly WeakReference<IElementInfo> declaringContainerRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeParameterInfo"/> class.
        /// </summary>
        /// <param name="parameterInfo">Information describing the parameter.</param>
        /// <param name="declaringContainer">The declaring element.</param>
        internal RuntimeParameterInfo(ParameterInfo parameterInfo, IElementInfo declaringContainer)
            : base(isThreadSafe: true)
        {
            this.ParameterInfo = parameterInfo;
            this.Name = parameterInfo.Name;
            this.FullName = parameterInfo.Member.DeclaringType?.FullName + "." + parameterInfo.Member.Name + "." + parameterInfo.Name;
            this.declaringContainerRef = new WeakReference<IElementInfo>(declaringContainer);
        }

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        /// <value>
        /// The name of the element.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the full name of the element.
        /// </summary>
        /// <value>
        /// The full name of the element.
        /// </value>
        public string FullName { get; }

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        public IEnumerable<object> Annotations => this.ParameterInfo.GetCustomAttributes();

        /// <summary>
        /// Gets the parent element declaring this element.
        /// </summary>
        /// <value>
        /// The declaring element.
        /// </value>
        public IElementInfo DeclaringContainer
        {
            get
            {
                if (this.declaringContainerRef.TryGetTarget(out var container))
                {
                    return container;
                }

                throw new ObjectDisposedException($"The container of {this.Name} is already disposed.");
            }
        }

        /// <summary>
        /// Gets information describing the parameter.
        /// </summary>
        /// <value>
        /// Information describing the parameter.
        /// </value>
        public ParameterInfo ParameterInfo { get; }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        public IRuntimeTypeInfo ParameterType => RuntimeTypeInfo.GetRuntimeType(this.ParameterInfo.ParameterType);

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        ITypeInfo IParameterInfo.ParameterType => RuntimeTypeInfo.GetRuntimeType(this.ParameterInfo.ParameterType);

        /// <summary>
        /// Gets the underlying member information.
        /// </summary>
        /// <returns>
        /// The underlying member information.
        /// </returns>
        public ICustomAttributeProvider GetUnderlyingElementInfo() => this.ParameterInfo;

        /// <summary>
        /// Gets the attribute of the provided type.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
        /// <returns>
        /// The attribute of the provided type.
        /// </returns>
        public IEnumerable<TAttribute> GetAttributes<TAttribute>()
            where TAttribute : Attribute
        {
            return this.ParameterInfo.GetCustomAttributes<TAttribute>(inherit: true);
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{this.Name}: {this.ParameterType.FullName}";
        }

        /// <summary>
        /// Gets the <see cref="ITypeInfo"/> of this expando object.
        /// </summary>
        /// <returns>
        /// The <see cref="ITypeInfo"/> of this expando object.
        /// </returns>
        protected override ITypeInfo GetThisTypeInfo()
        {
            return RuntimeTypeInfoOfRuntimeParameterInfo;
        }
    }
}