﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSourceHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data source handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Endpoints
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Data.Editing;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Messaging;
    using Kephas.Model.Services;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A data source handler.
    /// </summary>
    public class DataSourceHandler : MessageHandlerBase<DataSourceMessage, DataSourceResponseMessage>
    {
        /// <summary>
        /// The data space factory.
        /// </summary>
        private readonly IExportFactory<IDataSpace> dataSpaceFactory;

        /// <summary>
        /// The data source service.
        /// </summary>
        private readonly IDataSourceService dataSourceService;

        /// <summary>
        /// The type resolver.
        /// </summary>
        private readonly ITypeResolver typeResolver;

        /// <summary>
        /// The projected type resolver.
        /// </summary>
        private readonly IProjectedTypeResolver projectedTypeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceHandler"/> class.
        /// </summary>
        /// <param name="dataSpaceFactory">The data space factory.</param>
        /// <param name="dataSourceService">The data source service.</param>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="projectedTypeResolver">The projected type resolver.</param>
        public DataSourceHandler(IExportFactory<IDataSpace> dataSpaceFactory, IDataSourceService dataSourceService, ITypeResolver typeResolver, IProjectedTypeResolver projectedTypeResolver)
        {
            Requires.NotNull(dataSpaceFactory, nameof(dataSpaceFactory));
            Requires.NotNull(dataSourceService, nameof(dataSourceService));
            Requires.NotNull(typeResolver, nameof(typeResolver));
            Requires.NotNull(projectedTypeResolver, nameof(projectedTypeResolver));

            this.dataSpaceFactory = dataSpaceFactory;
            this.dataSourceService = dataSourceService;
            this.typeResolver = typeResolver;
            this.projectedTypeResolver = projectedTypeResolver;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public override async Task<DataSourceResponseMessage> ProcessAsync(DataSourceMessage message, IMessageProcessingContext context, CancellationToken token)
        {
            var entityType = this.ResolveEntityType(message.EntityType);
            var property = this.ResolveProperty(entityType, message.Property);

            using (var dataSpace = this.dataSpaceFactory.CreateExportedValue())
            {
                dataSpace.Initialize(context);
                var projectedType = this.ResolveProjectedEntityType(entityType);
                var dataContext = dataSpace[projectedType];
                var listSource = await this.dataSourceService.GetDataSourceAsync(
                                     property,
                                     new DataSourceContext(dataContext, entityType, projectedType)
                                         {
                                             Options = message.Options
                                         },
                                     token).PreserveThreadContext();

                return new DataSourceResponseMessage
                {
                    DataSource = listSource?.ToList()
                };
            }
        }

        /// <summary>
        /// Resolve the entity type.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="entityTypeName">Name of the entity type.</param>
        /// <returns>
        /// An ITypeInfo.
        /// </returns>
        protected virtual ITypeInfo ResolveEntityType(string entityTypeName)
        {
            var entityType = this.typeResolver.ResolveType(entityTypeName, throwOnNotFound: false);
            if (entityType == null)
            {
                // TODO localization
                throw new InvalidOperationException($"Could not resolve type '{entityTypeName}'.");
            }

            return entityType.AsRuntimeTypeInfo();
        }

        /// <summary>
        /// Resolve the projected entity type.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>
        /// An ITypeInfo.
        /// </returns>
        protected virtual ITypeInfo ResolveProjectedEntityType(ITypeInfo entityType)
        {
            if (entityType is IRuntimeTypeInfo runtimeEntityType)
            {
                return this.projectedTypeResolver.ResolveProjectedType(runtimeEntityType.Type).AsRuntimeTypeInfo();
            }

            return entityType;
        }

        /// <summary>
        /// Resolve property information.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// An IPropertyInfo.
        /// </returns>
        protected virtual IPropertyInfo ResolveProperty(ITypeInfo entityType, string propertyName)
        {
            propertyName = propertyName.ToPascalCase();
            var propertyInfo = entityType.Properties.FirstOrDefault(p => p.Name == propertyName);
            if (propertyInfo == null)
            {
                // TODO localization
                throw new InvalidOperationException($"Could not resolve property '{propertyName}' in type '{entityType}'.");
            }

            return propertyInfo;
        }
    }
}