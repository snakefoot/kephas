﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediConventionsBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the medi conventions builder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Medi.Conventions
{
    using System;
    using System.Collections.Generic;

    using Kephas.Composition.Conventions;
    using Kephas.Diagnostics.Contracts;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A conventions builder for Microsoft.Extensions.DependencyInjection.
    /// </summary>
    public class MediConventionsBuilder : IConventionsBuilder, IMediServiceCollectionProvider, IMediServiceProviderBuilder
    {
        private readonly IServiceCollection serviceCollection;

        private IList<ServiceDescriptorBuilder> descriptorBuilders = new List<ServiceDescriptorBuilder>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MediConventionsBuilder"/> class.
        /// </summary>
        public MediConventionsBuilder()
            : this(new ServiceCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediConventionsBuilder"/> class.
        /// </summary>
        /// <param name="serviceCollection">Collection of services.</param>
        public MediConventionsBuilder(IServiceCollection serviceCollection)
        {
            Requires.NotNull(serviceCollection, nameof(serviceCollection));

            this.serviceCollection = serviceCollection;
        }

        public IPartConventionsBuilder ForTypesDerivedFrom(Type type)
        {
            throw new NotImplementedException();
        }

        public IPartConventionsBuilder ForTypesMatching(Predicate<Type> typePredicate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Define a rule that will apply to the specified type.
        /// </summary>
        /// <param name="type">The type from which matching types derive.</param>
        /// <returns>
        /// A <see cref="T:Kephas.Composition.Conventions.IPartConventionsBuilder" /> that must be used
        /// to specify the rule.
        /// </returns>
        public IPartConventionsBuilder ForType(Type type)
        {
            var descriptorBuilder = new ServiceDescriptorBuilder
                                        {
                                            ImplementationType = type,
                                        };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new MediPartConventionsBuilder(descriptorBuilder);
        }

        /// <summary>
        /// Defines a registration for the specified type and its singleton instance.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="instance">The instance.</param>
        /// <returns>
        /// A <see cref="T:Kephas.Composition.Conventions.IPartBuilder" /> to further configure the rule.
        /// </returns>
        public IPartBuilder ForInstance(Type type, object instance)
        {
            var descriptorBuilder = new ServiceDescriptorBuilder
                                     {
                                         ServiceType = type,
                                         Instance = instance,
                                     };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new MediPartBuilder(descriptorBuilder);
        }

        /// <summary>
        /// Defines a registration for the specified type and its instance factory.
        /// </summary>
        /// <param name="type">The registered service type.</param>
        /// <param name="factory">The service factory.</param>
        /// <returns>
        /// A <see cref="T:Kephas.Composition.Conventions.IPartBuilder" /> to further configure the rule.
        /// </returns>
        public IPartBuilder ForInstanceFactory(Type type, Func<ICompositionContext, object> factory)
        {
            var descriptorBuilder = new ServiceDescriptorBuilder
                                        {
                                            ServiceType = type,
                                            Factory = serviceProvider => factory(serviceProvider.GetService<ICompositionContext>()),
                                        };
            this.descriptorBuilders.Add(descriptorBuilder);
            return new MediPartBuilder(descriptorBuilder);
        }

        /// <summary>
        /// Gets service collection.
        /// </summary>
        /// <returns>
        /// The service collection.
        /// </returns>
        public IServiceCollection GetServiceCollection() => this.serviceCollection;

        /// <summary>
        /// Builds service provider.
        /// </summary>
        /// <param name="parts">The parts being built.</param>
        /// <returns>
        /// A ServiceProvider.
        /// </returns>
        public ServiceProvider BuildServiceProvider(IEnumerable<Type> parts)
        {
            foreach (var descriptorBuilder in this.descriptorBuilders)
            {
                this.serviceCollection.Add(descriptorBuilder.Build());
            }

            return this.serviceCollection.BuildServiceProvider();
        }
    }
}