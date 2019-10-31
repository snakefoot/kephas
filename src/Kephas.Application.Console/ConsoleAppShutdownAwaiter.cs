﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleAppShutdownAwaiter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the console application shutdown awaiter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Interaction;
    using Kephas.Services;

    /// <summary>
    /// A console application shutdown awaiter.
    /// </summary>
    [OverridePriority(Priority.BelowNormal)]
    public class ConsoleAppShutdownAwaiter : DefaultAppShutdownAwaiter, IInitializable
    {
        private readonly ICommandShell shell;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleAppShutdownAwaiter"/> class.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="shell">The shell.</param>
        /// <param name="eventHub">The event hub.</param>
        public ConsoleAppShutdownAwaiter(ICommandShell shell, IEventHub eventHub)
            : base(eventHub)
        {
            this.shell = shell;
        }

        /// <summary>
        /// Gets a context for the application.
        /// </summary>
        /// <value>
        /// The application context.
        /// </value>
        public IContext AppContext { get; private set; }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="context">Optional. An optional context for initialization.</param>
        public virtual void Initialize(IContext context = null)
        {
            this.AppContext = context;
        }

        /// <summary>
        /// Executes the attended asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected override Task RunAttendedAsync(CancellationToken cancellationToken)
        {
            return this.shell.StartAsync(this.AppContext, cancellationToken);
        }
    }
}
