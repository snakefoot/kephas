﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DescExpressionConverter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the description expression converter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries.Conversion.ExpressionConverters
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Kephas.Data.Client.Resources;

    /// <summary>
    /// A descending expression converter.
    /// </summary>
    [Operator(Operator)]
    public class DescExpressionConverter : IExpressionConverter
    {
        /// <summary>
        /// The operator for descending sort.
        /// </summary>
        public const string Operator = "$desc";

        /// <summary>
        /// Converts the provided expression to a LINQ expression.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The converted expression.
        /// </returns>
        public Expression ConvertExpression(IList<Expression> args)
        {
            if (args.Count != 1)
            {
                throw new DataException(string.Format(Strings.ExpressionConverter_BadArgumentsCount_Exception, args.Count, 1));
            }

            return args[0];
        }
    }
}