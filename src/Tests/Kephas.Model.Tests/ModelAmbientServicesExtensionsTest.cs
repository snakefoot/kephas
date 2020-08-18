﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelAmbientServicesExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Model.Elements;
    using Kephas.Services;
    using Kephas.Testing.Model;
    using NUnit.Framework;

    [TestFixture]
    public class ModelAmbientServicesExtensionsTest : ModelTestBase
    {
        public override IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            return new List<Assembly>(base.GetDefaultConventionAssemblies())
            {
                typeof(IModelSpace).Assembly,
            };
        }

        [Test]
        public async Task WithModel_EnsureModelSpaceRegistered()
        {
            var ambientServices = new AmbientServices()
                .WithModel();
            var container = this.CreateContainer(ambientServices);
            var provider = container.GetExport<IModelSpaceProvider>();
            await provider.InitializeAsync(new Context(container));
            var modelSpace = container.GetExport<IModelSpace>();

            Assert.IsInstanceOf<DefaultModelSpace>(modelSpace);
        }
    }
}