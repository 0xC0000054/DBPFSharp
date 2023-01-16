// Copyright (c) 2023 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;

namespace DBPFSharp
{
    internal static class StreamExtensions
    {
        /// <summary>
        /// Reads a 4-byte unsigned integer from the stream in little endian byte order and advances the position of the stream by four bytes.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="EndOfStreamException">The end of the stream is reached.</exception>
        /// <returns>A 4-byte unsigned integer read from the current stream.</returns>
        [SkipLocalsInit]
        public static uint ReadUInt32(this Stream stream)
        {
            Span<byte> bytes = stackalloc byte[4];

            stream.ReadExactly(bytes);

            return BinaryPrimitives.ReadUInt32LittleEndian(bytes);
        }

#if !NET7_0_OR_GREATER
        public static void ReadExactly(this Stream stream, byte[] bytes, int offset, int count) => stream.ReadExactly(new Span<byte>(bytes, offset, count));

        public static void ReadExactly(this Stream stream, Span<byte> bytes)
        {
            Span<byte> span = bytes;

            while (span.Length > 0)
            {
                int bytesRead = stream.Read(span);

                if (bytesRead == 0)
                {
                    throw new EndOfStreamException();
                }

                span = span.Slice(bytesRead);
            }
        }
#endif

        /// <summary>
        /// Writes an unsigned 16-bit integer to the stream in little endian byte order.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="value">The value.</param>
        [SkipLocalsInit]
        public static void WriteUInt16(this Stream stream, ushort value)
        {
            Span<byte> bytes = stackalloc byte[2];

            BinaryPrimitives.WriteUInt16LittleEndian(bytes, value);

            stream.Write(bytes);
        }

        /// <summary>
        /// Writes an unsigned 32-bit integer to the stream in little endian byte order.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="value">The value.</param>
        [SkipLocalsInit]
        public static void WriteUInt32(this Stream stream, uint value)
        {
            Span<byte> bytes = stackalloc byte[4];

            BinaryPrimitives.WriteUInt32LittleEndian(bytes, value);

            stream.Write(bytes);
        }

        /// <summary>
        /// Writes an signed 32-bit integer to the stream in little endian byte order.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="value">The value.</param>
        [SkipLocalsInit]
        public static void WriteInt32(this Stream stream, int value)
        {
            Span<byte> bytes = stackalloc byte[4];

            BinaryPrimitives.WriteInt32LittleEndian(bytes, value);

            stream.Write(bytes);
        }
    }
}
