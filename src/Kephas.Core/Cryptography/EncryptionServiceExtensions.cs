﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptionServiceExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the encryption service extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Cryptography
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An encryption service extensions.
    /// </summary>
    public static class EncryptionServiceExtensions
    {
        /// <summary>
        /// Encrypts the input string and returns a promise of the encrypted string.
        /// </summary>
        /// <param name="encryptionService">The encryptionService to act on.</param>
        /// <param name="input">The input string.</param>
        /// <param name="context">The encryption context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of an encrypted string.
        /// </returns>
        public static async Task<string> EncryptAsync(
            this IEncryptionService encryptionService,
            string input,
            IEncryptionContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(encryptionService, nameof(encryptionService));

            using (var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            using (var outputStream = new MemoryStream())
            {
                await encryptionService.EncryptAsync(inputStream, outputStream, context, cancellationToken).PreserveThreadContext();
                var outputBytes = outputStream.ToArray();
                return Convert.ToBase64String(outputBytes);
            }
        }

        /// <summary>
        /// Decrypts the input string and returns a promise of the decrypted string.
        /// </summary>
        /// <param name="encryptionService">The encryptionService to act on.</param>
        /// <param name="input">The input string.</param>
        /// <param name="context">The encryption context (optional).</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a decrypted string.
        /// </returns>
        public static async Task<string> DecryptAsync(
            this IEncryptionService encryptionService,
            string input,
            IEncryptionContext context = null,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(encryptionService, nameof(encryptionService));

            using (var inputStream = new MemoryStream(Convert.FromBase64String(input)))
            using (var outputStream = new MemoryStream())
            {
                await encryptionService.DecryptAsync(inputStream, outputStream, context, cancellationToken).PreserveThreadContext();
                var outputBytes = outputStream.ToArray();
                return Encoding.UTF8.GetString(outputBytes);
            }
        }
    }
}