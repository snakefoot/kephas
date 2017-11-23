﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializerTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the JSON serializer test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Net.Mime;
    using Kephas.Reflection;

    using NSubstitute;

    using NUnit.Framework;

    using JsonSerializer = Kephas.Serialization.ServiceStack.Text.JsonSerializer;

    /// <summary>
    /// Tests for <see cref="JsonSerializer"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class JsonSerializerTest
    {
        [Test]
        public async Task SerializeAsync()
        {
            var settingsProvider = new DefaultJsonSerializerConfigurator(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new TestEntity
            {
                Name = "John Doe"
            };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.ServiceStack.Text.Tests.JsonSerializerTest+TestEntity, Kephas.Serialization.ServiceStack.Text.Tests"",""name"":""John Doe""}", serializedObj);
        }

        [Test]
        public async Task SerializeAsync_Expando()
        {
            var settingsProvider = new DefaultJsonSerializerConfigurator(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var obj = new ExpandoEntity
                          {
                              Description = "John Doe"
                          };
            var serializedObj = await serializer.SerializeAsync(obj);

            Assert.AreEqual(@"{""$type"":""Kephas.Serialization.ServiceStack.Text.Tests.JsonSerializerTest+ExpandoEntity, Kephas.Serialization.ServiceStack.Text.Tests"",""description"":""John Doe""}", serializedObj);
        }

        [Test]
        public async Task DeserializeAsync_with_serialized_types()
        {
            var settingsProvider = new DefaultJsonSerializerConfigurator(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.ServiceStack.Text.Tests.JsonSerializerTest+TestEntity, Kephas.Serialization.ServiceStack.Text.Tests"",""name"":""John Doe""}";
            var obj = await serializer.DeserializeAsync(serializedObj);

            Assert.IsInstanceOf<TestEntity>(obj);

            var testEntity = (TestEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Name);
        }

        [Test]
        public async Task DeserializeAsync_with_serialized_types_expando()
        {
            var settingsProvider = new DefaultJsonSerializerConfigurator(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.ServiceStack.Text.Tests.JsonSerializerTest+ExpandoEntity, Kephas.Serialization.ServiceStack.Text.Tests"",""description"":""John Doe""}";
            var obj = await serializer.DeserializeAsync(serializedObj);

            Assert.IsInstanceOf<ExpandoEntity>(obj);

            var testEntity = (ExpandoEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Description);
        }

        [Test]
        public async Task DeserializeAsync_with_in_string_provided_type()
        {
            var settingsProvider = new DefaultJsonSerializerConfigurator(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""$type"":""Kephas.Serialization.ServiceStack.Text.Tests.JsonSerializerTest+TestEntity, Kephas.Serialization.ServiceStack.Text.Tests"",""name"":""John Doe""}";
            var context = Substitute.For<ISerializationContext>();
            context.MediaType.Returns(typeof(JsonMediaType));
            context.RootObjectType.Returns(typeof(TestEntity));
            context.RootObjectFactory.Returns((Func<object>)null);
            var obj = await serializer.DeserializeAsync(serializedObj, context);

            Assert.IsInstanceOf<TestEntity>(obj);

            var testEntity = (TestEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Name);
        }

        [Test]
        public async Task DeserializeAsync_with_runtime_provided_type()
        {
            var settingsProvider = new DefaultJsonSerializerConfigurator(new DefaultTypeResolver(new DefaultAssemblyLoader()));
            var serializer = new JsonSerializer(settingsProvider);
            var serializedObj = @"{""name"":""John Doe""}";
            var context = Substitute.For<ISerializationContext>();
            context.MediaType.Returns(typeof(JsonMediaType));
            context.RootObjectType.Returns(typeof(TestEntity));
            context.RootObjectFactory.Returns((Func<object>)null);
            var obj = await serializer.DeserializeAsync(serializedObj, context);

            Assert.IsInstanceOf<TestEntity>(obj);

            var testEntity = (TestEntity)obj;

            Assert.AreEqual("John Doe", testEntity.Name);
        }

        [Test]
        public async Task JsonSerializer_Composition()
        {
            var asBuilder = new AmbientServicesBuilder();
            await asBuilder.WithMefCompositionContainerAsync(
                b =>
                b.WithAssemblies(new[] { typeof(ISerializationService).GetTypeInfo().Assembly, typeof(JsonSerializer).GetTypeInfo().Assembly }));
            var ambientServices = asBuilder.AmbientServices;
            var serializationService = ambientServices.CompositionContainer.GetExport<ISerializationService>();
            var jsonSerializer = serializationService.GetSerializer(SerializationContext.Create<JsonMediaType>(serializationService));

            Assert.IsInstanceOf<JsonSerializer>(jsonSerializer);
        }

        public class TestEntity
        {
            public string Name { get; set; }
        }

        public class ExpandoEntity : Expando
        {
            public string Description { get; set; }
        }
    }
}