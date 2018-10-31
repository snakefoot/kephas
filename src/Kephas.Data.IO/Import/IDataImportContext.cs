﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataImportContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataImportContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Import
{
    using System;

    using Kephas.Data;
    using Kephas.Data.Commands;
    using Kephas.Data.Conversion;
    using Kephas.Data.IO;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Operations;

    /// <summary>
    /// Contract for data import contexts.
    /// </summary>
    public interface IDataImportContext : IDataIOContext
    {
        /// <summary>
        /// Gets the data space.
        /// </summary>
        /// <value>
        /// The data space.
        /// </value>
        IDataSpace DataSpace { get; }

        /// <summary>
        /// Gets or sets the data conversion context configuration.
        /// </summary>
        /// <value>
        /// The data conversion context configuration.
        /// </value>
        Action<object, IDataConversionContext> DataConversionContextConfig { get; set; }

        /// <summary>
        /// Gets or sets the persist changes context configuration.
        /// </summary>
        /// <value>
        /// The persist changes context configuration.
        /// </value>
        Action<IPersistChangesContext> PersistChangesContextConfig { get; set; }
    }

    /// <summary>
    /// Extensions for <see cref="IDataImportContext"/>.
    /// </summary>
    public static class DataImportContextExtensions
    {
        /// <summary>
        /// The result key.
        /// </summary>
        private const string ResultKey = "SYSTEM_Result";

        /// <summary>
        /// Ensures that a result is set in the options.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="resultFactory">The result factory.</param>
        /// <returns>The result, once it is set into the options.</returns>
        public static IOperationResult EnsureResult(this IDataImportContext self, Func<IOperationResult> resultFactory = null)
        {
            Requires.NotNull(self, nameof(self));

            if (!(self[ResultKey] is IOperationResult result))
            {
                resultFactory = resultFactory ?? (() => new OperationResult());
                self[ResultKey] = result = resultFactory();
            }

            return result;
        }

        /// <summary>
        /// Gets the result from the options.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>The result, once it is set into the options.</returns>
        public static IOperationResult GetResult(this IDataImportContext self)
        {
            Requires.NotNull(self, nameof(self));

            return self[ResultKey] as IOperationResult;
        }

        /// <summary>
        /// Sets the data conversion context configuration.
        /// </summary>
        /// <param name="dataImportContext">The data import context to act on.</param>
        /// <param name="dataConversionContextConfig">The data conversion context configuration.</param>
        /// <returns>
        /// The data import context.
        /// </returns>
        public static IDataImportContext WithDataConversionContextConfig(
            this IDataImportContext dataImportContext,
            Action<object, IDataConversionContext> dataConversionContextConfig)
        {
            Requires.NotNull(dataImportContext, nameof(dataImportContext));

            dataImportContext.DataConversionContextConfig = dataConversionContextConfig;

            return dataImportContext;
        }

        /// <summary>
        /// Sets the persist changes context configuration.
        /// </summary>
        /// <param name="dataImportContext">The data import context to act on.</param>
        /// <param name="persistChangesContextConfig">The persist changes context configuration.</param>
        /// <returns>
        /// The data import context.
        /// </returns>
        public static IDataImportContext WithPersistChangesContextConfig(
            this IDataImportContext dataImportContext,
            Action<IPersistChangesContext> persistChangesContextConfig)
        {
            Requires.NotNull(dataImportContext, nameof(dataImportContext));

            dataImportContext.PersistChangesContextConfig = persistChangesContextConfig;

            return dataImportContext;
        }
    }
}