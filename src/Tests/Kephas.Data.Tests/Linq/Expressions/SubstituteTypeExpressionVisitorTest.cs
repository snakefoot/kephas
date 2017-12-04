﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubstituteTypeExpressionVisitorTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the substitute type expression visitor test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Kephas.Activation;
    using Kephas.Data.Linq.Expressions;
    using Kephas.Reflection;
    using Kephas.Services;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class SubstituteTypeExpressionVisitorTest
    {
        [Test]
        public void Visit_Where()
        {
            var activator = Substitute.For<IActivator>();
            activator.GetImplementationType(typeof(ITest).AsRuntimeTypeInfo(), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(typeof(Test).AsRuntimeTypeInfo());

            var baseQuery = (IQueryable<ITest>)new List<Test>(new[]
                                                                  {
                                                                      new Test { Name = "gigi" },
                                                                      new Test { Name = "belogea" }
                                                                  }).AsQueryable();
            var query = baseQuery.Where(t => t.Name == "gigi");
            var visitor = new SubstituteTypeExpressionVisitor(activator);
            var newExpression = visitor.Visit(query.Expression);

            var methodCallExpression = (MethodCallExpression)newExpression;
            var genericArg = methodCallExpression.Method.GetGenericArguments()[0];
            Assert.AreEqual(typeof(Test), genericArg);

            var result = baseQuery.Provider.Execute<IEnumerable<Test>>(newExpression);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void Visit_Where_captured_local_variable()
        {
            var activator = Substitute.For<IActivator>();
            activator.GetImplementationType(typeof(ITest).AsRuntimeTypeInfo(), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(typeof(Test).AsRuntimeTypeInfo());
            activator.GetImplementationType(typeof(string).AsRuntimeTypeInfo(), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(typeof(string).AsRuntimeTypeInfo());

            var baseQuery = (IQueryable<ITest>)new List<Test>(new[]
                                                                  {
                                                                      new Test { Name = "gigi" },
                                                                      new Test { Name = "belogea" }
                                                                  }).AsQueryable();
            var gigi = "gigi";
            var query = baseQuery.Where(t => t.Name == gigi);
            var visitor = new SubstituteTypeExpressionVisitor(activator);
            var newExpression = visitor.Visit(query.Expression);

            var methodCallExpression = (MethodCallExpression)newExpression;
            var genericArg = methodCallExpression.Method.GetGenericArguments()[0];
            Assert.AreEqual(typeof(Test), genericArg);

            var result = baseQuery.Provider.Execute<IEnumerable<Test>>(newExpression);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void Visit_Where_captured_local_variable_property_access()
        {
            var activator = Substitute.For<IActivator>();
            activator.GetImplementationType(Arg.Any<ITypeInfo>(), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(ci =>
                    {
                        var typeInfo = ci.Arg<ITypeInfo>();
                        return typeInfo == typeof(ITest).AsRuntimeTypeInfo()
                                   ? typeof(Test).AsRuntimeTypeInfo()
                                   : typeInfo;
                    });

            var baseQuery = (IQueryable<ITest>)new List<Test>(new[]
                                                                  {
                                                                      new Test { Name = "gigi" },
                                                                      new Test { Name = "belogea" }
                                                                  }).AsQueryable();
            var gigi = "gigi";
            var query = baseQuery.Where(t => t.Name.Length == gigi.Length);
            var visitor = new SubstituteTypeExpressionVisitor(activator);
            var newExpression = visitor.Visit(query.Expression);

            var methodCallExpression = (MethodCallExpression)newExpression;
            var genericArg = methodCallExpression.Method.GetGenericArguments()[0];
            Assert.AreEqual(typeof(Test), genericArg);

            var result = baseQuery.Provider.Execute<IEnumerable<Test>>(newExpression);
            Assert.AreEqual(1, result.Count());
        }


        [Test]
        public void Visit_Where_static_property_access()
        {
            var activator = Substitute.For<IActivator>();
            activator.GetImplementationType(Arg.Any<ITypeInfo>(), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(ci =>
                    {
                        var typeInfo = ci.Arg<ITypeInfo>();
                        return typeInfo == typeof(ITest).AsRuntimeTypeInfo()
                                   ? typeof(Test).AsRuntimeTypeInfo()
                                   : typeInfo;
                    });

            var baseQuery = (IQueryable<ITest>)new List<Test>(new[]
                                                                  {
                                                                      new Test { Name = "gigi" },
                                                                      new Test { Name = "Test" }
                                                                  }).AsQueryable();
            var gigi = "gigi";
            var query = baseQuery.Where(t => t.Name == Test.ClassName);
            var visitor = new SubstituteTypeExpressionVisitor(activator);
            var newExpression = visitor.Visit(query.Expression);

            var methodCallExpression = (MethodCallExpression)newExpression;
            var genericArg = methodCallExpression.Method.GetGenericArguments()[0];
            Assert.AreEqual(typeof(Test), genericArg);

            var result = baseQuery.Provider.Execute<IEnumerable<Test>>(newExpression);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void Visit_Where_captured_local_variable_field_access()
        {
            var activator = Substitute.For<IActivator>();
            activator.GetImplementationType(Arg.Any<ITypeInfo>(), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(ci =>
                    {
                        var typeInfo = ci.Arg<ITypeInfo>();
                        return typeInfo == typeof(ITest).AsRuntimeTypeInfo()
                                   ? typeof(Test).AsRuntimeTypeInfo()
                                   : typeInfo;
                    });

            var baseQuery = (IQueryable<ITest>)new List<Test>(new[]
                                                                  {
                                                                      new Test { Name = "gigi" },
                                                                      new Test { Name = "belogea" }
                                                                  }).AsQueryable();
            var gigi = new Test();
            var query = baseQuery.Where(t => t.Name == gigi.Gigi.Item1);
            var visitor = new SubstituteTypeExpressionVisitor(activator);
            var newExpression = visitor.Visit(query.Expression);

            var methodCallExpression = (MethodCallExpression)newExpression;
            var genericArg = methodCallExpression.Method.GetGenericArguments()[0];
            Assert.AreEqual(typeof(Test), genericArg);

            var result = baseQuery.Provider.Execute<IEnumerable<Test>>(newExpression);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void Visit_Where_static_field_access()
        {
            var activator = Substitute.For<IActivator>();
            activator.GetImplementationType(Arg.Any<ITypeInfo>(), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(ci =>
                    {
                        var typeInfo = ci.Arg<ITypeInfo>();
                        return typeInfo == typeof(ITest).AsRuntimeTypeInfo()
                                   ? typeof(Test).AsRuntimeTypeInfo()
                                   : typeInfo;
                    });

            var baseQuery = (IQueryable<ITest>)new List<Test>(new[]
                                                                  {
                                                                      new Test { Name = "gigi" },
                                                                      new Test { Name = "belogea" }
                                                                  }).AsQueryable();
            var query = baseQuery.Where(t => t.Name == Test.Belogea.Item1);
            var visitor = new SubstituteTypeExpressionVisitor(activator);
            var newExpression = visitor.Visit(query.Expression);

            var methodCallExpression = (MethodCallExpression)newExpression;
            var genericArg = methodCallExpression.Method.GetGenericArguments()[0];
            Assert.AreEqual(typeof(Test), genericArg);

            var result = baseQuery.Provider.Execute<IEnumerable<Test>>(newExpression);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void Visit_OfType()
        {
            var activator = Substitute.For<IActivator>();
            activator.GetImplementationType(typeof(ITest).AsRuntimeTypeInfo(), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(typeof(Test).AsRuntimeTypeInfo());

            var baseQuery = (IQueryable<ITest>)new List<Test>(new[]
                                                                  {
                                                                      new Test { Name = "gigi" },
                                                                      new DerivedTest { Name = "belogea" }
                                                                  }).AsQueryable();
            var query = baseQuery.OfType<DerivedTest>();
            var visitor = new SubstituteTypeExpressionVisitor(activator);
            var newExpression = visitor.Visit(query.Expression);

            var methodCallExpression = (MethodCallExpression)newExpression;
            Assert.AreEqual("OfType", methodCallExpression.Method.Name);

            var result = baseQuery.Provider.Execute<IEnumerable<Test>>(newExpression);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public void Visit_OfType_strip_away()
        {
            var activator = Substitute.For<IActivator>();
            activator.GetImplementationType(typeof(ITest).AsRuntimeTypeInfo(), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(typeof(Test).AsRuntimeTypeInfo());

            var baseQuery = (IQueryable<ITest>)new List<Test>(new[]
                                                                  {
                                                                      new Test { Name = "gigi" },
                                                                      new DerivedTest { Name = "belogea" }
                                                                  }).AsQueryable();
            var query = baseQuery.OfType<Test>();
            var visitor = new SubstituteTypeExpressionVisitor(activator);
            var newExpression = visitor.Visit(query.Expression);

            var constantExpression = (ConstantExpression)newExpression;
            Assert.AreEqual(baseQuery, constantExpression.Value);

            var result = baseQuery.Provider.Execute<IEnumerable<Test>>(newExpression);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void Visit_Cast()
        {
            var activator = Substitute.For<IActivator>();
            activator.GetImplementationType(typeof(ITest).AsRuntimeTypeInfo(), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(typeof(Test).AsRuntimeTypeInfo());

            var baseQuery = (IQueryable<ITest>)new List<Test>(new[]
                                                                  {
                                                                      new Test { Name = "gigi" },
                                                                      new DerivedTest { Name = "belogea" }
                                                                  }).AsQueryable();
            var query = baseQuery.Cast<DerivedTest>();
            var visitor = new SubstituteTypeExpressionVisitor(activator);
            var newExpression = visitor.Visit(query.Expression);

            var methodCallExpression = (MethodCallExpression)newExpression;
            Assert.AreEqual("Cast", methodCallExpression.Method.Name);

            Assert.Throws<InvalidCastException>(() => baseQuery.Provider.Execute<IEnumerable<Test>>(newExpression).ToList());
        }

        [Test]
        public void Visit_Cast_strip_away()
        {
            var activator = Substitute.For<IActivator>();
            activator.GetImplementationType(typeof(ITest).AsRuntimeTypeInfo(), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(typeof(Test).AsRuntimeTypeInfo());

            var baseQuery = (IQueryable<ITest>)new List<Test>(new[]
                                                                  {
                                                                      new Test { Name = "gigi" },
                                                                      new DerivedTest { Name = "belogea" }
                                                                  }).AsQueryable();
            var query = baseQuery.Cast<Test>();
            var visitor = new SubstituteTypeExpressionVisitor(activator);
            var newExpression = visitor.Visit(query.Expression);

            var constantExpression = (ConstantExpression)newExpression;
            Assert.AreEqual(baseQuery, constantExpression.Value);

            var result = baseQuery.Provider.Execute<IEnumerable<Test>>(newExpression);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void VisitConstant_nullable_value_null()
        {
            var activator = Substitute.For<IActivator>();
            activator.GetImplementationType(typeof(long?).AsRuntimeTypeInfo(), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(typeof(long?).AsRuntimeTypeInfo());


            var expression = Expression.Constant((long?)null, typeof(long?));
            var visitor = new SubstituteTypeExpressionVisitor(activator);
            var newExpression = (ConstantExpression)visitor.Visit(expression);

            Assert.AreSame(expression, newExpression);
        }

        [Test]
        public void VisitConstant_nullable_value_non_null()
        {
            var activator = Substitute.For<IActivator>();
            activator.GetImplementationType(typeof(long?).AsRuntimeTypeInfo(), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(typeof(long?).AsRuntimeTypeInfo());


            var expression = Expression.Constant((long?)2, typeof(long?));
            var visitor = new SubstituteTypeExpressionVisitor(activator);
            var newExpression = (ConstantExpression)visitor.Visit(expression);

            Assert.AreSame(expression, newExpression);
        }

        public interface ITest
        {
            string Name { get; set; }
        }

        public class Test : ITest
        {
            public readonly Tuple<string, string> Gigi = Tuple.Create("gigi", "");

            public static readonly Tuple<string, string> Belogea = Tuple.Create("belogea", "");

            public string Name { get; set; }

            public static string ClassName => "Test";
        }

        public class DerivedTest : Test
        {
            public string Name { get; set; }
        }
    }
}