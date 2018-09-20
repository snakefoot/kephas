﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataSpaceTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data space test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Reflection;

namespace Kephas.Data.Tests
{
    using System.Linq;
    using System.Security.Principal;
    using System.Text;

    using Kephas.Composition;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Store;
    using Kephas.Services;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DataSpaceTest
    {
        [Test]
        public void GetEnumerator()
        {
            var compositionContext = Substitute.For<ICompositionContext>();
            var dataContextFactory = Substitute.For<IDataContextFactory>();
            var dataStoreSelector = Substitute.For<IDataStoreSelector>();
            dataStoreSelector.GetDataStoreName(typeof(string)).Returns("default");
            var dataContext = Substitute.For<IDataContext>();
            dataContextFactory.CreateDataContext("default").Returns(dataContext);

            var dataSpace = new DataSpace(compositionContext, dataContextFactory, dataStoreSelector);
            var dc = dataSpace[typeof(string)];

            var dataContexts = dataSpace.ToList();
            Assert.AreEqual(1, dataContexts.Count);
            Assert.AreSame(dataContext, dataContexts[0]);
        }

        [Test]
        public void Indexer_type()
        {
            var compositionContext = Substitute.For<ICompositionContext>();
            var dataContextFactory = Substitute.For<IDataContextFactory>();
            var dataStoreSelector = Substitute.For<IDataStoreSelector>();
            dataStoreSelector.GetDataStoreName(typeof(string)).Returns("default");
            var dataContext = Substitute.For<IDataContext>();
            dataContextFactory.CreateDataContext("default").Returns(dataContext);

            var dataSpace = new DataSpace(compositionContext, dataContextFactory, dataStoreSelector);
            var dc = dataSpace[typeof(string)];
            Assert.AreSame(dataContext, dc);
        }

        [Test]
        public void Indexer_typeInfo()
        {
            var compositionContext = Substitute.For<ICompositionContext>();
            var dataContextFactory = Substitute.For<IDataContextFactory>();
            var dataStoreSelector = Substitute.For<IDataStoreSelector>();
            dataStoreSelector.GetDataStoreName(typeof(string)).Returns("default");
            var dataContext = Substitute.For<IDataContext>();
            dataContextFactory.CreateDataContext("default").Returns(dataContext);

            var dataSpace = new DataSpace(compositionContext, dataContextFactory, dataStoreSelector);
            var ti = (ITypeInfo)typeof(string).AsRuntimeTypeInfo();
            var dc = dataSpace[ti];
            Assert.AreSame(dataContext, dc);
        }

        [Test]
        public void Indexer_same_data_context_when_same_type()
        {
            var compositionContext = Substitute.For<ICompositionContext>();
            var dataContextFactory = Substitute.For<IDataContextFactory>();
            var dataStoreSelector = Substitute.For<IDataStoreSelector>();
            dataStoreSelector.GetDataStoreName(typeof(string)).Returns("default");
            var dataContext = Substitute.For<IDataContext>();
            dataContextFactory.CreateDataContext("default").Returns(dataContext);

            var dataSpace = new DataSpace(compositionContext, dataContextFactory, dataStoreSelector);
            var dc1 = dataSpace[typeof(string)];
            var dc2 = dataSpace[typeof(string)];
            Assert.AreSame(dc1, dc2);
            Assert.AreEqual(1, dataSpace.Count);
        }

        [Test]
        public void Dispose()
        {
            var compositionContext = Substitute.For<ICompositionContext>();
            var dataContextFactory = Substitute.For<IDataContextFactory>();
            var dataStoreSelector = Substitute.For<IDataStoreSelector>();
            dataStoreSelector.GetDataStoreName(typeof(string)).Returns("default");
            var dataContext = Substitute.For<IDataContext>();
            dataContextFactory.CreateDataContext("default").Returns(dataContext);

            var dataSpace = new DataSpace(compositionContext, dataContextFactory, dataStoreSelector);
            var dc = dataSpace[typeof(string)];

            dataSpace.Dispose();
            Assert.AreEqual(0, dataSpace.Count);
        }

        [Test]
        public void Initialize_Identity_set()
        {
            var compositionContext = Substitute.For<ICompositionContext>();
            var dataContextFactory = Substitute.For<IDataContextFactory>();
            var dataStoreSelector = Substitute.For<IDataStoreSelector>();

            var identity = Substitute.For<IIdentity>();
            var context = new Context(compositionContext) { Identity = identity };
            var dataSpace = new DataSpace(compositionContext, dataContextFactory, dataStoreSelector);
            dataSpace.Initialize(context);

            Assert.AreSame(identity, dataSpace.Identity);
        }

        [Test]
        public void Initialize_Identity_not_overwritten()
        {
            var compositionContext = Substitute.For<ICompositionContext>();
            var dataContextFactory = Substitute.For<IDataContextFactory>();
            var dataStoreSelector = Substitute.For<IDataStoreSelector>();

            var identity = Substitute.For<IIdentity>();
            var context = new Context { Identity = identity };
            var dataSpace = new DataSpace(compositionContext, dataContextFactory, dataStoreSelector) { Identity = Substitute.For<IIdentity>() };
            dataSpace.Initialize(context);

            Assert.AreNotSame(identity, dataSpace.Identity);
        }

        [Test]
        public void Initialize_initial_data()
        {
            var compositionContext = Substitute.For<ICompositionContext>();
            var dataContextFactory = Substitute.For<IDataContextFactory>();
            var dataStoreSelector = Substitute.For<IDataStoreSelector>();

            dataStoreSelector.GetDataStoreName(typeof(string)).Returns("string");
            dataStoreSelector.GetDataStoreName(typeof(StringBuilder)).Returns("builder");
            var strDataContext = Substitute.For<IDataContext>();
            var bldDataContext = Substitute.For<IDataContext>();
            dataContextFactory.CreateDataContext("string", Arg.Any<IContext>())
                .Returns(ci =>
                    {
                        var initialData = ci.Arg<IContext>().InitialData();
                        Assert.AreEqual(1, initialData.Count());
                        Assert.AreEqual("gigi", initialData.Single().ToString());
                        return strDataContext;
                    });
            dataContextFactory.CreateDataContext("builder", Arg.Any<IContext>())
                .Returns(ci =>
                    {
                        var initialData = ci.Arg<IContext>().InitialData();
                        Assert.AreEqual(1, initialData.Count());
                        Assert.AreEqual("belogea", initialData.Single().ToString());
                        return bldDataContext;
                    });

            var identity = Substitute.For<IIdentity>();
            var context = new Context { Identity = identity };
            context.WithInitialData(
                new[]
                    {
                        new EntityInfo("gigi") { ChangeState = ChangeState.Added },
                        new EntityInfo(new StringBuilder("belogea")) { ChangeState = ChangeState.Changed }
                    });
            var dataSpace = new DataSpace(compositionContext, dataContextFactory, dataStoreSelector) { Identity = Substitute.For<IIdentity>() };
            dataSpace.Initialize(context);

            Assert.AreNotSame(identity, dataSpace.Identity);
        }
    }
}