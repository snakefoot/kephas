﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRedisConnectionFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default Redis connection factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Redis
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.ExceptionHandling;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Redis.Configuration;
    using Kephas.Redis.Interaction;
    using Kephas.Redis.Logging;
    using Kephas.Services;
    using Kephas.Services.Transitions;
    using Kephas.Threading.Tasks;
    using StackExchange.Redis;

    /// <summary>
    /// The default Redis connection factory.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultRedisConnectionFactory : Loggable, IRedisConnectionFactory, IAsyncInitializable, IAsyncFinalizable
    {
        private static int connectionCounter;

        private readonly InitializationMonitor<IRedisConnectionFactory> initMonitor;
        private readonly FinalizationMonitor<IRedisConnectionFactory> finMonitor;
        private readonly ILogManager logManager;
        private readonly IAppRuntime appRuntime;
        private readonly IConfiguration<RedisClientSettings> redisConfiguration;
        private readonly IEventHub eventHub;
        private IContext appContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRedisConnectionFactory"/> class.
        /// </summary>
        /// <param name="logManager">Manager for log.</param>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="redisConfiguration">The redis configuration.</param>
        /// <param name="eventHub">The event hub.</param>
        public DefaultRedisConnectionFactory(
            ILogManager logManager,
            IAppRuntime appRuntime,
            IConfiguration<RedisClientSettings> redisConfiguration,
            IEventHub eventHub)
            : base(logManager)
        {
            this.logManager = logManager;
            this.appRuntime = appRuntime;
            this.redisConfiguration = redisConfiguration;
            this.eventHub = eventHub;
            this.initMonitor = new InitializationMonitor<IRedisConnectionFactory>(this.GetType());
            this.finMonitor = new FinalizationMonitor<IRedisConnectionFactory>(this.GetType());
        }

        /// <summary>
        /// Gets a value indicating whether the Redis client is initialized.
        /// </summary>
        /// <value>
        /// True if the Redis client is initialized, false if not.
        /// </value>
        public bool IsInitialized => this.initMonitor.IsCompletedSuccessfully;

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <returns>
        /// The new connection.
        /// </returns>
        public ConnectionMultiplexer CreateConnection()
        {
            this.initMonitor.AssertIsCompletedSuccessfully();

            return this.CreateConnectionCore(this.appContext);
        }

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">Optional. An optional context for initialization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        public async Task InitializeAsync(IContext context = null, CancellationToken cancellationToken = default)
        {
            this.initMonitor.AssertIsNotStarted();

            this.initMonitor.Start();

            try
            {
                var settings = this.redisConfiguration.Settings;
                this.appContext = context;
                using (var connection = this.CreateConnectionCore(context))
                {
                    this.Logger.Info($"Connected successfully to Redis.");
                }

                this.initMonitor.Complete();

                await this.eventHub.PublishAsync(new ConnectionFactoryStartedSignal(), context, cancellationToken).PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(ex, "Error while connecting to Redis.");
                this.initMonitor.Fault(ex);

                await this.eventHub.PublishAsync(new ConnectionFactoryStartedSignal(ex.Message, SeverityLevel.Error), context, cancellationToken).PreserveThreadContext();
            }
        }

        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <param name="context">Optional. An optional context for finalization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public async Task FinalizeAsync(IContext context = null, CancellationToken cancellationToken = default)
        {
            this.finMonitor.AssertIsNotStarted();

            this.finMonitor.Start();

            try
            {
                await this.eventHub.PublishAsync(new ConnectionFactoryStoppingSignal(), context, cancellationToken).PreserveThreadContext();

                this.finMonitor.Complete();
            }
            catch (Exception ex)
            {
                this.Logger.Fatal(ex, "Error while closing the Redis connection.");
                this.finMonitor.Fault(ex);

                await this.eventHub.PublishAsync(new ConnectionFactoryStoppingSignal(ex.Message, SeverityLevel.Error), context, cancellationToken).PreserveThreadContext();
            }
            finally
            {
                this.initMonitor.Reset();
            }
        }

        /// <summary>
        /// Creates a Redis logger.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <returns>
        /// The new Redis logger.
        /// </returns>
        protected virtual TextWriter CreateRedisLogger(IContext context)
        {
            return new RedisLogger(this.logManager);
        }

        /// <summary>
        /// Creates the connection. This method may be overridden.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <returns>
        /// The new connection.
        /// </returns>
        protected virtual ConnectionMultiplexer CreateConnectionCore(IContext context)
        {
            return ConnectionMultiplexer.Connect(this.GetConfigurationOptions(context), this.CreateRedisLogger(context));
        }

        /// <summary>
        /// Gets configuration options.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <returns>
        /// The configuration options.
        /// </returns>
        protected virtual ConfigurationOptions GetConfigurationOptions(IContext context)
        {
            var settings = this.redisConfiguration.Settings;
            var configuration = ConfigurationOptions.Parse(settings.ConnectionString);
            var connectionId = Interlocked.Increment(ref connectionCounter);
            configuration.ClientName = $"{this.appRuntime.GetAppInstanceId()}-{connectionId}";
            return configuration;
        }
    }
}
