﻿using System;

namespace OpenStory.Cryptography
{
    /// <summary>
    /// Provides encryption and decryption static methods for the MapleStory custom data transformation.
    /// </summary>
    public static class CustomCrypto
    {
        /// <summary>
        /// Encrypts an array in-place.
        /// </summary>
        /// <remarks>
        /// The array given in <paramref name="data"/> will be modified.
        /// </remarks>
        /// <param name="data">The array to encrypt.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="data" /> is <see langword="null"/>.</exception>
        public static void Encrypt(byte[] data)
        {
            Guard.NotNull(() => data, data);

            int length = data.Length;
            var truncatedLength = unchecked((byte)length);
            for (int j = 0; j < 6; j++)
            {
                if ((j & 1) != 0)
                {
                    OddEncryptTransform(data, length, truncatedLength);
                }
                else
                {
                    EvenEncryptTransform(data, length, truncatedLength);
                }
            }
        }

        /// <summary>
        /// Performs the encryption transform for the even case.
        /// </summary>
        /// <param name="data">The data to transform.</param>
        /// <param name="length">The length of the data to transform.</param>
        /// <param name="lengthByte">An initial length byte for the transformation.</param>
        private static void EvenEncryptTransform(byte[] data, int length, byte lengthByte)
        {
            byte remember = 0;
            for (int i = 0; i < length; i++)
            {
                byte current = RollLeft(data[i], 3);
                current += lengthByte;

                current ^= remember;
                remember = current;

                current = RollRight(current, lengthByte);
                current = (byte)(~current & 0xFF);
                current += 0x48;
                data[i] = current;

                lengthByte--;
            }
        }

        /// <summary>
        /// Performs the encryption transform for the odd case.
        /// </summary>
        /// <param name="data">The data to transform.</param>
        /// <param name="length">The length of the data to transform.</param>
        /// <param name="lengthByte">An initial length byte for the transformation.</param>
        private static void OddEncryptTransform(byte[] data, int length, byte lengthByte)
        {
            byte remember = 0;
            for (int i = length - 1; i >= 0; i--)
            {
                byte current = RollLeft(data[i], 4);
                current += lengthByte;

                current ^= remember;
                remember = current;

                current ^= 0x13;
                current = RollRight(current, 3);
                data[i] = current;

                lengthByte--;
            }
        }

        /// <summary>
        /// Decrypts an array in-place.
        /// </summary>
        /// <remarks>
        /// The array given in <paramref name="data"/> will be modified.
        /// </remarks>
        /// <param name="data">The array to decrypt.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="data" /> is <see langword="null"/>.</exception>
        public static void Decrypt(byte[] data)
        {
            Guard.NotNull(() => data, data);

            int length = data.Length;
            var truncatedLength = unchecked((byte)length);
            for (int j = 1; j <= 6; j++)
            {
                if ((j & 1) != 0)
                {
                    OddDecryptTransform(data, length, truncatedLength);
                }
                else
                {
                    EvenDecryptTransform(data, length, truncatedLength);
                }
            }
        }

        /// <summary>
        /// Performs the decryption transform for the even case.
        /// </summary>
        /// <param name="data">The data to transform.</param>
        /// <param name="length">The length of the data to transform.</param>
        /// <param name="lengthByte">An initial length byte for the transformation.</param>
        private static void EvenDecryptTransform(byte[] data, int length, byte lengthByte)
        {
            byte remember = 0;
            for (int i = 0; i < length; i++)
            {
                byte current = data[i];
                current -= 0x48;
                current = unchecked((byte)(~current));
                current = RollLeft(current, lengthByte);

                byte tmp = current;
                current ^= remember;
                remember = tmp;

                current -= lengthByte;
                data[i] = RollRight(current, 3);

                lengthByte--;
            }
        }

        /// <summary>
        /// Performs the decryption transform for the odd case.
        /// </summary>
        /// <param name="data">The data to transform.</param>
        /// <param name="length">The length of the data to transform.</param>
        /// <param name="lengthByte">An initial length byte for the transformation.</param>
        private static void OddDecryptTransform(byte[] data, int length, byte lengthByte)
        {
            byte remember = 0;
            for (int i = length - 1; i >= 0; i--)
            {
                byte current = RollLeft(data[i], 3);
                current ^= 0x13;

                byte tmp = current;
                current ^= remember;
                remember = tmp;

                current -= lengthByte;
                data[i] = RollRight(current, 4);

                lengthByte--;
            }
        }

        /// <summary>
        /// Performs a bit-wise left roll on a byte.
        /// </summary>
        /// <param name="b">The byte to roll left.</param>
        /// <param name="count">The number of bit positions to roll.</param>
        /// <returns>the resulting byte.</returns>
        private static byte RollLeft(byte b, int count)
        {
            int tmp = b << (count & 7);
            return unchecked((byte)(tmp | (tmp >> 8)));
        }

        /// <summary>
        /// Performs a bit-wise right roll on a byte.
        /// </summary>
        /// <param name="b">The byte to roll right.</param>
        /// <param name="count">The number of bit positions to roll.</param>
        /// <returns>the resulting byte.</returns>
        private static byte RollRight(byte b, int count)
        {
            int tmp = b << (8 - (count & 7));
            return unchecked((byte)(tmp | (tmp >> 8)));
        }
    }
}
