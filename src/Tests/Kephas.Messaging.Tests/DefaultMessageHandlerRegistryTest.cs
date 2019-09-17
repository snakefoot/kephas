﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultMessageHandlerRegistryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default message handler registry test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Messaging.Composition;
    using Kephas.Messaging.HandlerSelectors;
    using Kephas.Messaging.Messages;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultMessageHandlerRegistryTest
    {
        [Test]
        public void RegisterHandler()
        {
            var registry = new DefaultMessageHandlerRegistry(
                new DefaultMessageMatchService(),
                this.GetHandlerSelectorFactories().ToList(),
                this.GetHandlerFactories().ToList());

            var handler = Substitute.For<IMessageHandler>();
            registry.RegisterHandler(handler, new MessageHandlerMetadata(typeof(string)));

            var handlers = registry.ResolveMessageHandlers(new MessageAdapter { Message = "hello" });
            Assert.AreSame(handler, handlers.Single());
        }

        [Test]
        public void RegisterHandler_resets_cache()
        {
            var registry = new DefaultMessageHandlerRegistry(
                new DefaultMessageMatchService(),
                this.GetHandlerSelectorFactories().ToList(),
                this.GetHandlerFactories().ToList());

            var handler = Substitute.For<IMessageHandler>();
            registry.RegisterHandler(handler, new MessageHandlerMetadata(typeof(string)));

            var handlers = registry.ResolveMessageHandlers(new MessageAdapter { Message = "hello" });

            var handler2 = Substitute.For<IMessageHandler>();
            registry.RegisterHandler(handler2, new MessageHandlerMetadata(typeof(string), overridePriority: (int)Priority.High));

            handlers = registry.ResolveMessageHandlers(new MessageAdapter { Message = "hello" });
            Assert.AreSame(handler2, handlers.Single());
        }

        private IEnumerable<IExportFactory<IMessageHandlerSelector, AppServiceMetadata>> GetHandlerSelectorFactories()
        {
            yield return new ExportFactory<IMessageHandlerSelector, AppServiceMetadata>(
                () => new DefaultMessageHandlerSelector(new DefaultMessageMatchService()),
                new AppServiceMetadata());
        }

        private IEnumerable<IExportFactory<IMessageHandler, MessageHandlerMetadata>> GetHandlerFactories()
        {
            yield break;
        }
    }
}
