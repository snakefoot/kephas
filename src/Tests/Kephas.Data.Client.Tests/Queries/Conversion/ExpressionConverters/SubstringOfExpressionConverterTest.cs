// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubstringOfExpressionConverterTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the substring of expression converter test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Tests.Queries.Conversion.ExpressionConverters
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Kephas.Data.Client.Queries.Conversion.ExpressionConverters;

    using NUnit.Framework;

    [TestFixture]
    public class SubstringOfExpressionConverterTest
    {
        [Test]
        public void Convert()
        {
            var converter = new SubstringOfExpressionConverter();

            var expr = (MethodCallExpression)converter.ConvertExpression(new List<Expression>
                                                                             {
                                                                                 Expression.Constant("123"),
                                                                                 Expression.Constant("12"),
                                                                             });
            var result = Expression.Lambda(expr).Compile().DynamicInvoke();
            Assert.IsTrue((bool)result);

            expr = (MethodCallExpression)converter.ConvertExpression(new List<Expression>
                                                                         {
                                                                             Expression.Constant("123"),
                                                                             Expression.Constant("23"),
                                                                         });
            result = Expression.Lambda(expr).Compile().DynamicInvoke();
            Assert.IsTrue((bool)result);
        }
    }
}