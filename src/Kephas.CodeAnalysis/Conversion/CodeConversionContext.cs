﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeConversionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the code conversion context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Conversion
{
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// A code conversion context.
    /// </summary>
    public class CodeConversionContext : Context, ICodeConversionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeConversionContext"/> class.
        /// </summary>
        /// <param name="codeConverter">The code converter.</param>
        public CodeConversionContext(ICodeConverter codeConverter)
        {
            Requires.NotNull(codeConverter, nameof(codeConverter));

            this.CodeConverter = codeConverter;
        }

        /// <summary>
        /// Gets the code converter.
        /// </summary>
        /// <value>
        /// The code converter.
        /// </value>
        public ICodeConverter CodeConverter { get; }
    }
}