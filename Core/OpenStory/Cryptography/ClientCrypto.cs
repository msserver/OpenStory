﻿using System;

namespace OpenStory.Cryptography
{
    /// <summary>
    /// Represents a cryptographic transformer for client-side sessions.
    /// </summary>
    public sealed class ClientCrypto : EndpointCrypto
    {
        // Encryption uses the local IV, decryption uses the remote IV.
        // Server's local IV has flipped version, Client's local IV has regular version.

        /// <summary>
        /// Creates a new instance of <see cref="ClientCrypto"/>.
        /// </summary>
        /// <param name="factory">The <see cref="RollingIvFactory"/> instance to use.</param>
        /// <param name="clientIv">The IV for the client.</param>
        /// <param name="serverIv">The IV for the server.</param>
        /// <returns>a new <see cref="EndpointCrypto"/> instance.</returns>
        public static EndpointCrypto New(RollingIvFactory factory, byte[] clientIv, byte[] serverIv)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            if (clientIv == null)
            {
                throw new ArgumentNullException("clientIv");
            }

            if (serverIv == null)
            {
                throw new ArgumentNullException("serverIv");
            }

            var encryptor = factory.CreateEncryptIv(clientIv, VersionMaskType.None);
            var decryptor = factory.CreateDecryptIv(serverIv, VersionMaskType.Complement);

            return new ClientCrypto(encryptor, decryptor);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ClientCrypto"/>.
        /// </summary>
        /// <inheritdoc/>
        private ClientCrypto(RollingIv encryptor, RollingIv decryptor)
            : base(encryptor, decryptor)
        {
        }
    }
}
