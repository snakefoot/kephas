﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAttributeAnnotation.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IAttributeAnnotation interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System;

    /// <summary>
    /// Contract for annotations based on attributes.
    /// </summary>
    public interface IAttributeAnnotation : IAnnotation
    {
        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <value>
        /// The attribute.
        /// </value>
        Attribute Attribute { get; }
    }
}