﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptionServiceExtensionsTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the encryption service extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Cryptography
{
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Cryptography;

    using NSubstitute;
    using NSubstitute.Core;

    using NUnit.Framework;

    [TestFixture]
    public class EncryptionServiceExtensionsTest
    {
        [Test]
        [TestCase("password", "ZHJvd3NzYXA=")]
        [TestCase("123", "MzIx")]
        public async Task EncryptAsync(string input, string output)
        {
            var encryptionService = Substitute.For<IEncryptionService>();
            encryptionService.EncryptAsync(null, null, null, default(CancellationToken))
                .ReturnsForAnyArgs(Task.FromResult(0))
                .AndDoes(this.ReverseBytes);

            var encrypted = await EncryptionServiceExtensions.EncryptAsync(encryptionService, input);
            Assert.AreEqual(output, encrypted);
        }

        [Test]
        [TestCase("ZHJvd3NzYXA=", "password")]
        [TestCase("MzIx", "123")]
        public async Task DecryptAsync(string input, string output)
        {
            var encryptionService = Substitute.For<IEncryptionService>();
            encryptionService.DecryptAsync(null, null, null, default(CancellationToken))
                .ReturnsForAnyArgs(Task.FromResult(0))
                .AndDoes(this.ReverseBytes);

            var encrypted = await EncryptionServiceExtensions.DecryptAsync(encryptionService, input);
            Assert.AreEqual(output, encrypted);
        }

        [Test]
        [TestCase("password")]
        [TestCase("123")]
        public async Task EncryptAsync_DecryptAsync_are_inverse(string input)
        {
            var encryptionService = Substitute.For<IEncryptionService>();
            encryptionService.EncryptAsync(null, null, null, default(CancellationToken))
                .ReturnsForAnyArgs(Task.FromResult(0))
                .AndDoes(this.ReverseBytes);
            encryptionService.DecryptAsync(null, null, null, default(CancellationToken))
                .ReturnsForAnyArgs(Task.FromResult(0))
                .AndDoes(this.ReverseBytes);

            var encrypted = await EncryptionServiceExtensions.EncryptAsync(encryptionService, input);
            var decrypted = await EncryptionServiceExtensions.DecryptAsync(encryptionService, encrypted);
            Assert.AreEqual(input, decrypted);
        }

        public void ReverseBytes(CallInfo ci)
        {
            var inputStream = ci.ArgAt<Stream>(0);
            var outputStream = ci.ArgAt<Stream>(1);
            var inputArray = ((MemoryStream)inputStream).ToArray();
            var outputArray = inputArray.Reverse().ToArray();
            outputStream.Write(outputArray, 0, outputArray.Length);
        }
    }
}