﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the context factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Reflection;
    using Kephas.Resources;
    using Kephas.Services.Composition;
    using Kephas.Services.Reflection;
    using Kephas.Text;

    /// <summary>
    /// A context factory.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class ContextFactory : IContextFactory
    {
        private const int AmbientServicesIndex = -1;
        private const int CompositionContextIndex = -2;

        private readonly ICompositionContext compositionContext;
        private readonly IAmbientServices ambientServices;
        private readonly ConcurrentDictionary<Type, IList<(ConstructorInfo ctor, ParameterInfo[] paramInfos)>> typeCache
            = new ConcurrentDictionary<Type, IList<(ConstructorInfo ctor, ParameterInfo[] paramInfos)>>();

        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Signature, Func<object[], IContext>>> signatureCache
            = new ConcurrentDictionary<Type, ConcurrentDictionary<Signature, Func<object[], IContext>>>();

        private readonly IList<(Type contractType, IAppServiceInfo appServiceInfo)> appServiceInfos;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextFactory"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        /// <param name="ambientServices">The ambient services.</param>
        public ContextFactory(ICompositionContext compositionContext, IAmbientServices ambientServices)
        {
            this.compositionContext = compositionContext;
            this.ambientServices = ambientServices;
            this.appServiceInfos = this.ambientServices.GetAppServiceInfos()?.ToList();
        }

        /// <summary>
        /// Creates a typed context.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="args">A variable-length parameters list containing arguments.</param>
        /// <returns>
        /// The new context.
        /// </returns>
        public TContext CreateContext<TContext>(params object[] args)
            where TContext : IContext
        {
            var contextType = typeof(TContext);
            if (!contextType.IsClass || contextType.IsAbstract)
            {
                throw new ArgumentException(Strings.ContextFactory_CreateContext_ContextTypeMustBeAnInstatiable.FormatWith(contextType));
            }

            var signature = new Signature(args.Select(a => a?.GetType()));
            var typeSignCache = this.signatureCache.GetOrAdd(contextType, _ => new ConcurrentDictionary<Signature, Func<object[], IContext>>());
            var creatorFunc = typeSignCache.GetOrAdd(signature, _ => this.GetCreatorFunc(contextType, signature));
            return (TContext)creatorFunc(args);
        }

        private Func<object[], IContext> GetCreatorFunc(Type contextType, Signature signature)
        {
            var ctorInfos = this.typeCache.GetOrAdd(
                contextType,
                _ =>
                {
                    var ctors = contextType.GetConstructors().Where(c => c.IsPublic).OrderByDescending(c => c.GetParameters().Length).ToList();
                    return ctors.Select(c => (c, c.GetParameters())).ToList();
                });
            foreach (var (ctor, paramInfos) in ctorInfos)
            {
                var (argIndexMap, argResolverMap) = this.GetSignatureMaps(signature, paramInfos);
                if (argIndexMap != null)
                {
                    return args =>
                    {
                        return (IContext)ctor.Invoke(argIndexMap.Select(i => i >= 0 ? args[i] : argResolverMap[-i]()).ToArray());
                    };
                }
            }

            throw new InvalidOperationException(Strings.ContextFactory_GetCreatorFunc_CannotFindMatchingConstructor.FormatWith(signature, contextType));
        }

        private (List<int> argIndexMap, List<Func<object>> argResolverMap) GetSignatureMaps(Signature signature, ParameterInfo[] paramInfos)
        {
            var argIndexMap = new List<int>();
            var argResolverMap = new List<Func<object>>
                {
                    null,
                    () => this.ambientServices,
                    () => this.compositionContext,
                };
            foreach (var paramInfo in paramInfos)
            {
                var paramType = paramInfo.ParameterType;
                var argIndex = this.GetArgIndex(signature, paramType);
                if (argIndex.HasValue)
                {
                    argIndexMap.Add(argIndex.Value);
                }
                else
                {
                    var appServiceInfo = this.appServiceInfos?.FirstOrDefault(i => i.contractType == paramType);
                    if (appServiceInfo != null && appServiceInfo.Value.appServiceInfo != null && !appServiceInfo.Value.appServiceInfo.AllowMultiple)
                    {
                        argIndexMap.Add(-argResolverMap.Count);
                        argResolverMap.Add(() => this.compositionContext.GetExport(paramType));
                    }
                    else if (paramInfo.HasDefaultValue)
                    {
                        argIndexMap.Add(-argResolverMap.Count);
                        argResolverMap.Add(() => paramInfo.DefaultValue);
                    }
                    else
                    {
                        // there are params which cannot be resolved, break iteration.
                        return (null, null);
                    }
                }
            }

            return (argIndexMap, argResolverMap);
        }

        private int? GetArgIndex(Signature signature, Type paramType)
        {
            if (paramType == typeof(IAmbientServices))
            {
                return AmbientServicesIndex;
            }
            else if (paramType == typeof(ICompositionContext))
            {
                return CompositionContextIndex;
            }
            else
            {
                for (var j = 0; j < signature.Count; j++)
                {
                    var argType = signature[j];
                    if (argType != null && paramType.IsAssignableFrom(argType))
                    {
                        return j;
                    }
                }
            }

            return null;
        }
    }
}