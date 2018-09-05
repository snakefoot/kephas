﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InProcessMessageBrokerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the in process message broker test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Tests.Distributed
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Events;
    using Kephas.Messaging.Messages;
    using Kephas.Security;
    using Kephas.Security.Authentication;
    using Kephas.Serialization;
    using Kephas.Serialization.Json;
    using Kephas.Services;
    using Kephas.Testing.Composition.Mef;
    using Kephas.Threading.Tasks;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class InProcessMessageBrokerTest : CompositionTestBase
    {
        public override ICompositionContext CreateContainer(IEnumerable<Assembly> assemblies = null, IEnumerable<Type> parts = null, Action<MefCompositionContainerBuilder> config = null)
        {
            var assemblyList = new List<Assembly>(assemblies ?? new Assembly[0]);
            assemblyList.Add(typeof(IMessageProcessor).GetTypeInfo().Assembly); /* Kephas.Messaging */
            return base.CreateContainer(assemblyList, parts, config);
        }

        [Test]
        public void InProcessMessageBroker_Composition_success()
        {
            var container = this.CreateContainer();
            var messageBroker = container.GetExport<IMessageBroker>();
            Assert.IsInstanceOf<InProcessMessageBroker>(messageBroker);
        }

        [Test]
        public async Task DispatchAsync_timeout()
        {
            var container = this.CreateContainer(parts: new[] { typeof(TimeoutMessageHandler) });
            var messageBroker = (InProcessMessageBroker)container.GetExport<IMessageBroker>();

            var brokeredMessage = new BrokeredMessage
            {
                Content = new TimeoutMessage(),
                Timeout = TimeSpan.FromMilliseconds(30)
            };
            Assert.That(() => messageBroker.DispatchAsync(brokeredMessage), Throws.InstanceOf<TimeoutException>());
        }

        [Test]
        public async Task DispatchAsync_timeout_logging()
        {
            var sb = new StringBuilder();
            var logger = this.GetLogger<IMessageBroker>(sb);

            var container = this.CreateContainer(parts: new[] { typeof(TimeoutMessageHandler) });
            var messageBroker = (InProcessMessageBroker)container.GetExport<IMessageBroker>();
            messageBroker.Logger = logger;

            var brokeredMessage = new BrokeredMessage
                                      {
                                          Content = new TimeoutMessage(),
                                          Timeout = TimeSpan.FromMilliseconds(30)
                                      };

            try
            {
                await messageBroker.DispatchAsync(brokeredMessage).PreserveThreadContext();
                throw new InvalidOperationException("Should have timed out.");
            }
            catch (TimeoutException tex)
            {
            }

            var log = sb.ToString();
            Assert.IsTrue(log.Contains("Warning"));
            Assert.IsTrue(log.Contains("Timeout"));
        }

        [Test]
        public async Task DispatchAsync_Ping_success_with_timeout()
        {
            var container = this.CreateContainer();
            var messageBroker = container.GetExport<IMessageBroker>();

            var pingBack = await messageBroker.DispatchAsync(new BrokeredMessage
            {
                Content = new PingMessage(),
                Timeout = TimeSpan.FromSeconds(100)
            });

            Assert.IsInstanceOf<PingBackMessage>(pingBack);
        }

        [Test]
        public async Task ProcessAsync_Ping_over_serialization_success()
        {
            var container = this.CreateContainer(assemblies: new[] { typeof(IJsonSerializerSettingsProvider).Assembly }, parts: new[] { typeof(RemoteMessageBroker) });
            var messageBroker = container.GetExport<IMessageBroker>();

            var pingBack = await messageBroker.ProcessAsync(new PingMessage());

            Assert.IsInstanceOf<PingBackMessage>(pingBack);
        }

        [Test]
        public async Task ProcessAsync_Ping_success()
        {
            var container = this.CreateContainer();
            var messageBroker = container.GetExport<IMessageBroker>();

            var pingBack = await messageBroker.ProcessAsync(new PingMessage());

            Assert.IsInstanceOf<PingBackMessage>(pingBack);
        }

        [Test]
        public async Task ProcessAsync_Ping_exception()
        {
            var container = this.CreateContainer(parts: new[] { typeof(ExceptionEventHandler) });
            var messageBroker = container.GetExport<IMessageBroker>();

            Assert.That(() => messageBroker.ProcessAsync(new PingMessage()), Throws.InstanceOf<MessagingException>());
        }

        [Test]
        public async Task ProcessAsync_null_response_event_no_handlers()
        {
            var container = this.CreateContainer();
            var messageBroker = container.GetExport<IMessageBroker>();

            var nullResponse = await messageBroker.ProcessAsync(new TestEvent());

            Assert.IsNull(nullResponse);
        }

        [Test]
        public async Task PublishAsync_event()
        {
            var container = this.CreateContainer(parts: new[] { typeof(TestEventHandler) });
            var messageBroker = container.GetExport<IMessageBroker>();

            var message = new TestEvent { TaskCompletionSource = new TaskCompletionSource<(string, IBrokeredMessage)>() };
            await messageBroker.PublishAsync(message);

            var response = await message.TaskCompletionSource.Task;
            Assert.AreEqual("ok", response.response);
        }

        [Test]
        public async Task ProcessOneWayAsync_event()
        {
            var container = this.CreateContainer(parts: new[] { typeof(TestEventHandler) });
            var messageBroker = container.GetExport<IMessageBroker>();

            var message = new TestEvent { TaskCompletionSource = new TaskCompletionSource<(string, IBrokeredMessage)>() };
            await messageBroker.ProcessOneWayAsync(message);

            var response = await message.TaskCompletionSource.Task;
            Assert.AreEqual("ok", response.response);
            Assert.AreSame(message, response.brokeredMessage.Content);
        }

        private ILogger<T> GetLogger<T>(StringBuilder sb)
        {
            var logger = Substitute.For<ILogger<T>>();
            logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
            logger.WhenForAnyArgs(l => l.Log(LogLevel.Debug, null, null, new object[0])).Do(
                ci => { sb.Append($"{ci.Arg<LogLevel>()} {ci.Arg<string>()} {ci.Arg<Exception>()?.GetType().Name}"); });
            return logger;
        }

        public class TestEvent : Expando, IEvent
        {
            public TaskCompletionSource<(string response, IBrokeredMessage brokeredMessage)> TaskCompletionSource { get; set; }

            public IMessage Response { get; set; }
        }

        public class TestEventHandler : MessageHandlerBase<TestEvent, IMessage>
        {
            /// <summary>
            /// Processes the provided message asynchronously and returns a response promise.
            /// </summary>
            /// <param name="message">The message to be handled.</param>
            /// <param name="context">The processing context.</param>
            /// <param name="token">The cancellation token.</param>
            /// <returns>
            /// The response promise.
            /// </returns>
            public override async Task<IMessage> ProcessAsync(TestEvent message, IMessageProcessingContext context, CancellationToken token)
            {
                message.TaskCompletionSource?.SetResult(("ok", context.GetBrokeredMessage()));

                return message.Response;
            }
        }

        [OverridePriority(Priority.High)]
        public class ExceptionEventHandler : MessageHandlerBase<PingMessage, PingBackMessage>
        {
            /// <summary>
            /// Processes the provided message asynchronously and returns a response promise.
            /// </summary>
            /// <param name="message">The message to be handled.</param>
            /// <param name="context">The processing context.</param>
            /// <param name="token">The cancellation token.</param>
            /// <returns>
            /// The response promise.
            /// </returns>
            public override async Task<PingBackMessage> ProcessAsync(PingMessage message, IMessageProcessingContext context, CancellationToken token)
            {
                throw new ArgumentException();
            }
        }

        public class TimeoutMessage : IMessage { }

        public class TimeoutMessageHandler : MessageHandlerBase<TimeoutMessage, IMessage>
        {
            /// <summary>
            /// Processes the provided message asynchronously and returns a response promise.
            /// </summary>
            /// <param name="message">The message to be handled.</param>
            /// <param name="context">The processing context.</param>
            /// <param name="token">The cancellation token.</param>
            /// <returns>
            /// The response promise.
            /// </returns>
            public override async Task<IMessage> ProcessAsync(TimeoutMessage message, IMessageProcessingContext context, CancellationToken token)
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));

                return Substitute.For<IMessage>();
            }
        }

        [OverridePriority(Priority.Normal)]
        public class RemoteMessageBroker : InProcessMessageBroker
        {
            private readonly ISerializationService serializationService;

            public RemoteMessageBroker(IAppManifest appManifest, IAuthenticationService authenticationService, IMessageProcessor messageProcessor, ISerializationService serializationService)
                : base(appManifest, authenticationService, messageProcessor)
            {
                this.serializationService = serializationService;
            }

            protected override async Task SendAsync(
                IBrokeredMessage brokeredMessage,
                IContext context,
                CancellationToken cancellationToken)
            {
                var serializedMessage = await this.serializationService.JsonSerializeAsync(
                                            brokeredMessage,
                                            cancellationToken: cancellationToken);
                var deserializedMessage = await this.serializationService.JsonDeserializeAsync(
                                              serializedMessage,
                                              cancellationToken: cancellationToken);
                await base.SendAsync((IBrokeredMessage)deserializedMessage, context, cancellationToken);
            }
        }
    }
}