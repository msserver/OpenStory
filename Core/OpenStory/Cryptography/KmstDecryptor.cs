using System;
using OpenStory.Common.Tools;

namespace OpenStory.Cryptography
{
    /// <summary>
    /// Represents a decryption transformer based on the custom KMST algorithm.
    /// </summary>
    public sealed class KmstDecryptor : CryptoTransformBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="KmstDecryptor"/>.
        /// </summary>
        /// <inheritdoc />
        public KmstDecryptor(byte[] table, byte[] initialIv)
            : base(table, initialIv)
        {
        }

        /// <inheritdoc />
        public override void TransformArraySegment(byte[] data, byte[] iv, int segmentStart, int segmentEnd)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (iv == null)
            {
                throw new ArgumentNullException("iv");
            }

            if (iv.Length != 4)
            {
                throw new ArgumentException(Exceptions.IvMustBe4Bytes, "iv");
            }

            // Thanks to Diamondo25 for this.
            byte[] stepIv = iv.FastClone();
            for (int i = segmentStart; i < segmentEnd; i++)
            {
                byte initial = data[i];

                byte x = (byte)(initial ^ this.Table[stepIv[0]]);
                byte b = (byte)((x >> 1) & 0x55);
                byte a = (byte)((x & 0xD5) << 1);
                byte r = (byte)(a | b);

                data[i] = (byte)((r >> 4) | (r << 4));

                // NOTE: passing the new value is CORRECT.
                this.ShuffleIvStep(stepIv, data[i]);
            }
        }
    }
}
