﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelTestBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the model test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Model.Runtime;
    using Kephas.Testing.Composition.Mef;

    using NSubstitute;

    /// <summary>
    /// A model test base.
    /// </summary>
    public abstract class ModelTestBase : CompositionTestBase
    {
        public IRuntimeModelRegistry GetModelRegistry(params Type[] elements)
        {
            var registry = Substitute.For<IRuntimeModelRegistry>();
            registry.GetRuntimeElementsAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IEnumerable<object>>(elements));
            return registry;
        }

        public ICompositionContext CreateContainerForModel(params Type[] elements)
        {
            var container = this.CreateContainer(
                new [] { typeof(IModelSpace).GetTypeInfo().Assembly }, 
                config: b => b.WithFactoryExportProvider(() => this.GetModelRegistry(elements), isShared: true));

            return container;
        }
    }
}