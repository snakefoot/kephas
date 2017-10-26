﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppManagerTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default application appManager test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Composition;
    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Composition.ExportFactoryImporters;
    using Kephas.Services;
    using Kephas.Services.Behavior;
    using Kephas.Services.Behavior.Composition;
    using Kephas.Services.Composition;

    using NSubstitute;
    using NSubstitute.Core;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultAppManagerTest
    {
        [Test]
        public void Constructor_order_dependencies_with_processing_priority()
        {
            var featureManager1 = this.CreateFeatureManager();
            var featureManager2 = this.CreateFeatureManager();
            var featureManager3 = this.CreateFeatureManager();

            var appManager = new DefaultAppManager(
                Substitute.For<IAppManifest>(),
                this.GetAmbientServices(),
                this.GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        this.CreateFeatureManagerFactory(featureManager1, "1", "1.0", dependencies: new[] { "3" }),
                        this.CreateFeatureManagerFactory(featureManager2, "2", "1.0", dependencies: new[] { "1" }),
                        // make the manager 3 with a lower priority than 1 and 2 which actually depend on it
                        this.CreateFeatureManagerFactory(featureManager3, "3", "1.0", processingPriority: Priority.Low),
                    },
                null);

            var orderedManagers = appManager.FeatureManagerFactories.Select(f => f.CreateExportedValue()).ToList();

            Assert.AreSame(featureManager1, orderedManagers[1]);
            Assert.AreSame(featureManager2, orderedManagers[2]);
            Assert.AreSame(featureManager3, orderedManagers[0]);
        }

        [Test]
        public async Task InitializeAppAsync_right_app_behavior_order()
        {
            var order = new List<int>();

            var featureManager1 = Substitute.For<IFeatureManager>();
            featureManager1.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(1));

            var featureManager2 = Substitute.For<IFeatureManager>();
            featureManager2.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(2));

            var behavior1 = Substitute.For<IAppLifecycleBehavior>();
            behavior1.BeforeAppInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(11));
            behavior1.AfterAppInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(21));

            var behavior2 = Substitute.For<IAppLifecycleBehavior>();
            behavior2.BeforeAppInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(12));
            behavior2.AfterAppInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(22));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppManifest>(),
                this.GetAmbientServices(),
                this.GetServiceBehaviorProvider(),
                new[]
                    {
                        new ExportFactory<IAppLifecycleBehavior, AppServiceMetadata>(() => behavior1, new FeatureManagerMetadata(processingPriority: 2)),
                        new ExportFactory<IAppLifecycleBehavior, AppServiceMetadata>(() => behavior2, new FeatureManagerMetadata(processingPriority: 1)),
                    },
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager1, new FeatureManagerMetadata(processingPriority: 2)),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager2, new FeatureManagerMetadata(processingPriority: 1)),
                    },
                null);

            await appManager.InitializeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(6, order.Count);
            Assert.AreEqual(12, order[0]);
            Assert.AreEqual(11, order[1]);
            Assert.AreEqual(2, order[2]);
            Assert.AreEqual(1, order[3]);
            Assert.AreEqual(22, order[4]);
            Assert.AreEqual(21, order[5]);
        }

        [Test]
        public async Task InitializeAppAsync_right_featureManager_order_processingPriority()
        {
            var order = new List<int>();

            var featureManager1 = Substitute.For<IFeatureManager>();
            featureManager1.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(1));

            var featureManager2 = Substitute.For<IFeatureManager>();
            featureManager2.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(2));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppManifest>(),
                this.GetAmbientServices(),
                this.GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager1, new FeatureManagerMetadata(processingPriority: 2)),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager2, new FeatureManagerMetadata(processingPriority: 1)),
                    },
                null);

            await appManager.InitializeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(2, order.Count);
            Assert.AreEqual(2, order[0]);
            Assert.AreEqual(1, order[1]);
        }

        [Test]
        public async Task InitializeAppAsync_right_featureManager_order_processingPriority_marginal_values()
        {
            var order = new List<int>();

            var featureManager1 = Substitute.For<IFeatureManager>();
            featureManager1.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(1));

            var featureManager2 = Substitute.For<IFeatureManager>();
            featureManager2.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(2));

            var featureManager3 = Substitute.For<IFeatureManager>();
            featureManager3.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(3));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppManifest>(),
                this.GetAmbientServices(),
                this.GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager1, new FeatureManagerMetadata(processingPriority: 0)),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager2, new FeatureManagerMetadata(processingPriority: int.MaxValue)),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager3, new FeatureManagerMetadata(processingPriority: int.MinValue)),
                    },
                null);

            await appManager.InitializeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(3, order.Count);
            Assert.AreEqual(3, order[0]);
            Assert.AreEqual(1, order[1]);
            Assert.AreEqual(2, order[2]);
        }

        [Test]
        public async Task InitializeAppAsync_right_featureManager_order_dependencies()
        {
            var order = new List<int>();

            var featureManager1 = Substitute.For<IFeatureManager>();
            featureManager1.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(1));

            var featureManager2 = Substitute.For<IFeatureManager>();
            featureManager2.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(2));

            var featureManager3 = Substitute.For<IFeatureManager>();
            featureManager3.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(3));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppManifest>(),
                this.GetAmbientServices(),
                this.GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager1, new FeatureManagerMetadata(new FeatureInfo("1", "1.0", dependencies: new[] { "3" }))),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager2, new FeatureManagerMetadata(new FeatureInfo("2", "1.0", dependencies: new[] { "3" }))),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager3, new FeatureManagerMetadata(new FeatureInfo("3", "1.0"))),
                    },
                null);

            await appManager.InitializeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(3, order.Count);
            Assert.AreEqual(3, order[0]);
            Assert.AreEqual(1, order[1]);
            Assert.AreEqual(2, order[2]);
        }

        [Test]
        public async Task InitializeAppAsync_right_behavior_order()
        {
            var order = new List<string>();

            var featureManager1 = Substitute.For<IFeatureManager>();
            featureManager1.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0));

            var featureManager2 = Substitute.For<IFeatureManager>();
            featureManager2.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0));

            var behavior1 = Substitute.For<IFeatureLifecycleBehavior>();
            behavior1.BeforeInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<FeatureManagerMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add("Before1"));
            behavior1.AfterInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<FeatureManagerMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add("After1"));

            var behavior2 = Substitute.For<IFeatureLifecycleBehavior>();
            behavior2.BeforeInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<FeatureManagerMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add("Before2"));
            behavior2.AfterInitializeAsync(Arg.Any<IAppContext>(), Arg.Any<FeatureManagerMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add("After2"));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppManifest>(),
                this.GetAmbientServices(),
                this.GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager1, new FeatureManagerMetadata()),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager2, new FeatureManagerMetadata()),
                    },
                new[]
                    {
                        new ExportFactory<IFeatureLifecycleBehavior, AppServiceMetadata>(() => behavior1, new AppServiceMetadata(processingPriority: 2)),
                        new ExportFactory<IFeatureLifecycleBehavior, AppServiceMetadata>(() => behavior2, new AppServiceMetadata(processingPriority: 1)),
                    });

            await appManager.InitializeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(8, order.Count);
            Assert.AreEqual("Before2", order[0]);
            Assert.AreEqual("Before1", order[1]);
            Assert.AreEqual("After1", order[2]);
            Assert.AreEqual("After2", order[3]);
            Assert.AreEqual("Before2", order[4]);
            Assert.AreEqual("Before1", order[5]);
            Assert.AreEqual("After1", order[6]);
            Assert.AreEqual("After2", order[7]);
        }

        [Test]
        public async Task FinalizeAppAsync_right_app_behavior_order()
        {
            var order = new List<int>();

            var featureManager1 = Substitute.For<IFeatureManager>();
            featureManager1.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(1));

            var featureManager2 = Substitute.For<IFeatureManager>();
            featureManager2.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(2));

            var behavior1 = Substitute.For<IAppLifecycleBehavior>();
            behavior1.BeforeAppFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(11));
            behavior1.AfterAppFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(21));

            var behavior2 = Substitute.For<IAppLifecycleBehavior>();
            behavior2.BeforeAppFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(12));
            behavior2.AfterAppFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(22));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppManifest>(),
                this.GetAmbientServices(),
                this.GetServiceBehaviorProvider(),
                new[]
                    {
                        new ExportFactory<IAppLifecycleBehavior, AppServiceMetadata>(() => behavior1, new FeatureManagerMetadata(processingPriority: 2)),
                        new ExportFactory<IAppLifecycleBehavior, AppServiceMetadata>(() => behavior2, new FeatureManagerMetadata(processingPriority: 1)),
                    },
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager1, new FeatureManagerMetadata(processingPriority: 2)),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager2, new FeatureManagerMetadata(processingPriority: 1)),
                    },
                null);

            await appManager.FinalizeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(6, order.Count);
            Assert.AreEqual(11, order[0]);
            Assert.AreEqual(12, order[1]);
            Assert.AreEqual(1, order[2]);
            Assert.AreEqual(2, order[3]);
            Assert.AreEqual(21, order[4]);
            Assert.AreEqual(22, order[5]);
        }

        [Test]
        public async Task FinalizeAppAsync_right_featureManager_order_processingPriority()
        {
            var order = new List<int>();

            var finalizer1 = Substitute.For<IFeatureManager>();
            finalizer1.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(1));

            var finalizer2 = Substitute.For<IFeatureManager>();
            finalizer2.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(2));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppManifest>(),
                this.GetAmbientServices(),
                this.GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer1, new FeatureManagerMetadata(processingPriority: 2)),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer2, new FeatureManagerMetadata(processingPriority: 1)),
                    },
                null);

            await appManager.FinalizeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(2, order.Count);
            Assert.AreEqual(1, order[0]);
            Assert.AreEqual(2, order[1]);
        }


        [Test]
        public async Task FinalizeAppAsync_right_featureManager_order_processingPriority_marginal_values()
        {
            var order = new List<int>();

            var finalizer1 = Substitute.For<IFeatureManager>();
            finalizer1.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(1));

            var finalizer2 = Substitute.For<IFeatureManager>();
            finalizer2.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(2));

            var finalizer3 = Substitute.For<IFeatureManager>();
            finalizer3.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add(3));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppManifest>(),
                this.GetAmbientServices(),
                this.GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer1, new FeatureManagerMetadata(processingPriority: 0)),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer2, new FeatureManagerMetadata(processingPriority: int.MaxValue)),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer3, new FeatureManagerMetadata(processingPriority: int.MinValue)),
                    },
                null);

            await appManager.FinalizeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(3, order.Count);
            Assert.AreEqual(2, order[0]);
            Assert.AreEqual(1, order[1]);
            Assert.AreEqual(3, order[2]);
        }

        [Test]
        public async Task FinalizeAppAsync_right_featureManager_order_dependencies()
        {
            var order = new List<int>();

            var finalizer1 = this.CreateFeatureManager(finalization: _ => order.Add(1));
            var finalizer2 = this.CreateFeatureManager(finalization: _ => order.Add(2));
            var finalizer3 = this.CreateFeatureManager(finalization: _ => order.Add(3));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppManifest>(),
                this.GetAmbientServices(),
                this.GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer1, new FeatureManagerMetadata(new FeatureInfo("1", "1.0", dependencies: new[] { "3" }))),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer2, new FeatureManagerMetadata(new FeatureInfo("2", "1.0", dependencies: new[] { "1" }))),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => finalizer3, new FeatureManagerMetadata(new FeatureInfo("3", "1.0"))),
                    },
                null);

            await appManager.FinalizeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(3, order.Count);
            Assert.AreEqual(2, order[0]);
            Assert.AreEqual(1, order[1]);
            Assert.AreEqual(3, order[2]);
        }

        [Test]
        public async Task FinalizeAppAsync_right_behavior_order()
        {
            var order = new List<string>();

            var featureManager1 = this.CreateFeatureManager();
            var featureManager2 = this.CreateFeatureManager();

            var behavior1 = Substitute.For<IFeatureLifecycleBehavior>();
            behavior1.BeforeFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<FeatureManagerMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add("Before1"));
            behavior1.AfterFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<FeatureManagerMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add("After1"));

            var behavior2 = Substitute.For<IFeatureLifecycleBehavior>();
            behavior2.BeforeFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<FeatureManagerMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add("Before2"));
            behavior2.AfterFinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<FeatureManagerMetadata>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => order.Add("After2"));

            var appManager = new DefaultAppManager(
                Substitute.For<IAppManifest>(),
                this.GetAmbientServices(),
                this.GetServiceBehaviorProvider(),
                new List<IExportFactory<IAppLifecycleBehavior, AppServiceMetadata>>(),
                new[]
                    {
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager1, new FeatureManagerMetadata()),
                        new ExportFactory<IFeatureManager, FeatureManagerMetadata>(() => featureManager2, new FeatureManagerMetadata()),
                    },
                new[]
                    {
                        new ExportFactory<IFeatureLifecycleBehavior, AppServiceMetadata>(() => behavior1, new AppServiceMetadata(processingPriority: 2)),
                        new ExportFactory<IFeatureLifecycleBehavior, AppServiceMetadata>(() => behavior2, new AppServiceMetadata(processingPriority: 1)),
                    });

            await appManager.FinalizeAppAsync(Substitute.For<IAppContext>(), CancellationToken.None);

            Assert.AreEqual(8, order.Count);
            Assert.AreEqual("Before1", order[0]);
            Assert.AreEqual("Before2", order[1]);
            Assert.AreEqual("After1", order[2]);
            Assert.AreEqual("After2", order[3]);
            Assert.AreEqual("Before1", order[4]);
            Assert.AreEqual("Before2", order[5]);
            Assert.AreEqual("After1", order[6]);
            Assert.AreEqual("After2", order[7]);
        }

        private ExportFactory<IFeatureManager, FeatureManagerMetadata> CreateFeatureManagerFactory(
            IFeatureManager featureManager,
            string name,
            string version = null,
            bool isRequired = false,
            string[] dependencies = null,
            Priority processingPriority = Priority.Normal)
        {
            return new ExportFactory<IFeatureManager, FeatureManagerMetadata>(
                () => featureManager,
                new FeatureManagerMetadata(new FeatureInfo(name, version, isRequired, dependencies), processingPriority: (int)processingPriority));
        }

        private IFeatureManager CreateFeatureManager(Action<CallInfo> initialization = null, Action<CallInfo> finalization = null)
        {
            var featureManager = Substitute.For<IFeatureManager>();
            featureManager.InitializeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => initialization?.Invoke(_));
            featureManager.FinalizeAsync(Arg.Any<IAppContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(0))
                .AndDoes(_ => finalization?.Invoke(_));

            return featureManager;
        }

        private IServiceBehaviorProvider GetServiceBehaviorProvider()
        {
            var provider = new DefaultServiceBehaviorProvider(Substitute.For<IAmbientServices>(), new List<IExportFactory<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>>());
            return provider;
        }

        private IAmbientServices GetAmbientServices()
        {
            var exporter = Substitute.For<ICollectionExportFactoryImporter>();
            exporter.ExportFactories.Returns(new IExportFactory<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>[0]);

            var compositionContext = Substitute.For<ICompositionContext>();
            compositionContext.GetExport(typeof(ICollectionExportFactoryImporter<IEnabledServiceBehaviorRule, ServiceBehaviorRuleMetadata>), null)
                .Returns(exporter);

            var ambientServices = Substitute.For<IAmbientServices>();
            ambientServices.CompositionContainer.Returns(compositionContext);
            return ambientServices;
        }
    }
}