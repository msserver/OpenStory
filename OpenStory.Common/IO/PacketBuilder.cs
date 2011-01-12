﻿using System;
using System.IO;
using System.Text;

namespace OpenStory.Common.IO
{
    public class PacketBuilder : IDisposable
    {
        private bool isDisposed;
        private MemoryStream stream;

        /// <summary>Initializes a new PacketBuilder instance with the default capacity.</summary>
        public PacketBuilder()
        {
            this.stream = new MemoryStream();
        }

        /// <summary>Initializes a new PacketBuilder instance.</summary>
        /// <param name="capacity">The initial capacity for the underlying stream.</param>
        public PacketBuilder(int capacity)
        {
            this.stream = new MemoryStream(capacity);
        }

        #region IDisposable Members

        /// <summary>Disposes of the underlying stream.</summary>
        /// <remarks>Calling any instance methods after this will cause them to throw an ObjectDisposedException.</remarks>
        public void Dispose()
        {
            if (this.isDisposed) return;
            this.isDisposed = true;
            this.stream.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>Writes a <see cref="System.Int64"/> to the end of the packet.</summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ObjectDisposedException">The exception is thrown if the PacketBuilder has been disposed.</exception>
        public void WriteLong(long value)
        {
            this.CheckDisposed();
            this.WriteDirect(BigEndianBitConverter.GetBytes(value));
        }

        /// <summary>Writes a <see cref="System.Int32"/> to the stream.</summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ObjectDisposedException">The exception is thrown if the PacketBuilder has been disposed.</exception>
        public void WriteInt(int value)
        {
            this.CheckDisposed();
            this.WriteDirect(BigEndianBitConverter.GetBytes(value));
        }

        /// <summary>Writes a <see cref="System.Int16"/> to the stream.</summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ObjectDisposedException">The exception is thrown if the PacketBuilder has been disposed.</exception>
        public void WriteShort(short value)
        {
            this.CheckDisposed();
            this.WriteDirect(BigEndianBitConverter.GetBytes(value));
        }

        /// <summary>Writes a <see cref="System.Byte"/> to the stream.</summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ObjectDisposedException">The exception is thrown if the PacketBuilder has been disposed.</exception>
        public void WriteByte(byte value)
        {
            this.CheckDisposed();
            this.stream.WriteByte(value);
        }

        /// <summary>Writes a <see cref="System.Boolean"/> to the stream.</summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ObjectDisposedException">The exception is thrown if the PacketBuilder has been disposed.</exception>
        public void WriteBool(bool value)
        {
            this.CheckDisposed();
            this.WriteDirect(BigEndianBitConverter.GetBytes(value));
        }

        /// <summary>Writes a string and its length to the stream.</summary>
        /// <remarks>The length of the stream is written first.</remarks>
        /// <param name="str">The string to write.</param>
        /// <exception cref="ArgumentNullException">The exception is thrown if <paramref name="str"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The exception is thrown if the PacketBuilder has been disposed.</exception>
        public void WriteLengthString(string str)
        {
            if (str == null) throw new ArgumentNullException("str");
            this.WriteShort((short) str.Length);
            this.WriteDirect(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>Writes a padded string to the stream.</summary>
        /// <param name="str">The string to write.</param>
        /// <param name="padLength">The length to pad the string to.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The exception is thrown 
        /// if <paramref name="padLength"/> is a non-positive number, 
        /// OR, 
        /// if <paramref name="str"/> is not shorter than padLength.
        /// </exception>
        /// <exception cref="ArgumentNullException">The exception is thrown if <paramref name="str"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">The exception is thrown if the PacketBuilder has been disposed.</exception>
        public void WritePaddedString(string str, int padLength)
        {
            this.CheckDisposed();
            if (str == null) throw new ArgumentNullException("str");
            if (padLength <= 0)
                throw new ArgumentOutOfRangeException("padLength", padLength,
                                                      "The pad length must be a positive number.");
            if (str.Length >= padLength)
                throw new ArgumentOutOfRangeException("str", "The string is not shorter than the pad length.");

            var stringBytes = new byte[padLength];
            Encoding.UTF8.GetBytes(str, 0, str.Length, stringBytes, 0);
            stringBytes[str.Length] = 0;
            this.WriteDirect(stringBytes);
        }

        private void WriteDirect(byte[] bytes)
        {
            byte[] buffer = this.stream.GetBuffer();
            Buffer.BlockCopy(bytes, 0, buffer, (int) this.stream.Position, bytes.Length);
            this.stream.Position += bytes.Length;
        }

        private void CheckDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException("PacketWriter");
            }
        }

        public byte[] ToByteArray()
        {
            byte[] buffer = this.stream.GetBuffer();
            int length = buffer.Length;
            var array = new byte[length];
            Buffer.BlockCopy(buffer, 0, array, 0, length);
            return array;
        }
    }
}