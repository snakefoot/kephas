﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextQueryProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data context query provider base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq
{
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Kephas.Activation;
    using Kephas.Data.Linq.Expressions;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;

    /// <summary>
    /// A data context query provider base.
    /// </summary>
    public class DataContextQueryProvider : IDataContextQueryProvider
    {
        /// <summary>
        /// The generic method of IQueryable.CreateQuery{TElement}().
        /// </summary>
        private static readonly MethodInfo CreateQueryMethod =
            ReflectionHelper.GetGenericMethodOf(_ => ((IQueryProvider)null).CreateQuery<int>(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextQueryProvider"/> class.
        /// </summary>
        /// <param name="queryOperationContext">The query operation context.</param>
        /// <param name="nativeQueryProvider">The native query provider.</param>
        protected DataContextQueryProvider(IQueryOperationContext queryOperationContext, IQueryProvider nativeQueryProvider)
        {
            Requires.NotNull(queryOperationContext, nameof(queryOperationContext));
            Requires.NotNull(queryOperationContext.DataContext, nameof(queryOperationContext.DataContext));
            Requires.NotNull(nativeQueryProvider, nameof(nativeQueryProvider));

            this.QueryOperationContext = queryOperationContext;
            this.DataContext = queryOperationContext.DataContext;
            this.NativeQueryProvider = nativeQueryProvider;
        }

        /// <summary>
        /// Gets the bound data context.
        /// </summary>
        /// <value>
        /// The bound data context.
        /// </value>
        public IDataContext DataContext { get; }

        /// <summary>
        /// Gets an operation context for the query.
        /// </summary>
        /// <value>
        /// The query operation context.
        /// </value>
        public IQueryOperationContext QueryOperationContext { get; }

        /// <summary>
        /// Gets the native query provider.
        /// </summary>
        /// <value>
        /// The native query provider.
        /// </value>
        public IQueryProvider NativeQueryProvider { get; }

        /// <summary>Constructs an <see cref="T:System.Linq.IQueryable" /> object that can evaluate the query represented by a specified expression tree.</summary>
        /// <returns>An <see cref="T:System.Linq.IQueryable" /> that can evaluate the query represented by the specified expression tree.</returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        public virtual IQueryable CreateQuery(Expression expression)
        {
            Requires.NotNull(expression, nameof(expression));

            var elementType = expression.Type.TryGetEnumerableItemType();
            var createQuery = CreateQueryMethod.MakeGenericMethod(elementType);
            return (IQueryable)createQuery.Call(this, expression);
        }

        /// <summary>Constructs an <see cref="T:System.Linq.IQueryable`1" /> object that can evaluate the query represented by a specified expression tree.</summary>
        /// <returns>An <see cref="T:System.Linq.IQueryable`1" /> that can evaluate the query represented by the specified expression tree.</returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <typeparam name="TElement">The type of the elements of the <see cref="T:System.Linq.IQueryable`1" /> that is returned.</typeparam>
        public virtual IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var nativeQuery = this.NativeQueryProvider.CreateQuery<TElement>(expression);
            return this.CreateQuery(nativeQuery);
        }

        /// <summary>Executes the query represented by a specified expression tree.</summary>
        /// <returns>The value that results from executing the specified query.</returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        public virtual object Execute(Expression expression)
        {
            return this.NativeQueryProvider.Execute(this.GetExecutableExpression(expression));
        }

        /// <summary>Executes the strongly-typed query represented by a specified expression tree.</summary>
        /// <returns>The value that results from executing the specified query.</returns>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <typeparam name="TResult">The type of the value that results from executing the query.</typeparam>
        public virtual TResult Execute<TResult>(Expression expression)
        {
            return this.NativeQueryProvider.Execute<TResult>(this.GetExecutableExpression(expression));
        }

        /// <summary>
        /// Tries to get the data context's entity activator.
        /// </summary>
        /// <returns>
        /// An IActivator.
        /// </returns>
        protected internal virtual IActivator TryGetEntityActivator()
        {
            return (this.DataContext as DataContextBase)?.EntityActivator;
        }

        /// <summary>
        /// Prepares the provided expression for execution.
        /// </summary>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <returns>
        /// An expression prepared for execution.
        /// </returns>
        protected internal virtual Expression GetExecutableExpression(Expression expression)
        {
            var entityActivator = this.TryGetEntityActivator();
            if (entityActivator == null)
            {
                return expression;
            }

            var substituteTypeExpressionVisitor = new SubstituteTypeExpressionVisitor(entityActivator);
            expression = substituteTypeExpressionVisitor.Visit(expression);
            return expression;
        }

        /// <summary>
        /// Constructs an <see cref="T:System.Linq.IQueryable" /> object that can evaluate the query
        /// represented by a specified expression tree.
        /// </summary>
        /// <typeparam name="TElement">The query element type.</typeparam>
        /// <param name="nativeQuery">The native query.</param>
        /// <returns>
        /// An <see cref="T:System.Linq.IQueryable" /> that can evaluate the query represented by the
        /// specified expression tree.
        /// </returns>
        protected virtual IQueryable<TElement> CreateQuery<TElement>(IQueryable<TElement> nativeQuery)
        {
            return new DataContextQuery<TElement>(this, nativeQuery);
        }
    }
}