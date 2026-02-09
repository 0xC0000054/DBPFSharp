// Copyright (c) 2023, 2025, 2026 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Buffers.Binary;
using System.IO;

namespace DBPFSharp
{
    internal ref struct SpanBinaryReader
    {
        private readonly ReadOnlySpan<byte> buffer;
        private int position;

        public SpanBinaryReader(ReadOnlySpan<byte> buffer)
        {
            this.buffer = buffer;
            this.position = 0;
        }

        public int Position
        {
            readonly get { return this.position; }
            set
            { 
                ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(value));
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, this.buffer.Length, nameof(value));

                this.position = value;
            }
        }

        public byte ReadByte()
        {
            CheckForSufficientBufferSpace(sizeof(byte));

            byte value = this.buffer[this.position];
            this.position++;

            return value;
        }

        public ReadOnlySpan<byte> ReadBytes(int count)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(count);

            ReadOnlySpan<byte> bytes = [];

            if (count > 0)
            {
                CheckForSufficientBufferSpace(count);

                bytes = this.buffer.Slice(this.position, count);
                this.position += count;
            }

            return bytes;
        }

        public sbyte ReadSByte() => unchecked((sbyte)ReadByte());

        public short ReadInt16() => unchecked((short)ReadUInt16());

        public ushort ReadUInt16()
        {
            CheckForSufficientBufferSpace(sizeof(ushort));

            ushort value = BinaryPrimitives.ReadUInt16LittleEndian(this.buffer[this.position..]);
            this.position += sizeof(ushort);

            return value;
        }

        public int ReadInt32() => unchecked((int)ReadUInt32());

        public uint ReadUInt32()
        {
            CheckForSufficientBufferSpace(sizeof(uint));

            uint value = BinaryPrimitives.ReadUInt32LittleEndian(this.buffer[this.position..]);
            this.position += sizeof(uint);
            
            return value;
        }

        public long ReadInt64() => unchecked((long)ReadUInt64());

        public ulong ReadUInt64()
        {
            CheckForSufficientBufferSpace(sizeof(ulong));

            ulong value = BinaryPrimitives.ReadUInt64LittleEndian(this.buffer[this.position..]);
            this.position += sizeof(ulong);

            return value;
        }

        public float ReadSingle() => BitConverter.UInt32BitsToSingle(ReadUInt32());

        public double ReadDouble() => BitConverter.UInt64BitsToDouble(ReadUInt32());

        /// <summary>
        /// Checks for sufficient buffer space.
        /// </summary>
        /// <param name="minBytes">The minimum bytes.</param>
        /// <exception cref="EndOfStreamException">Attempted to read past the end of the buffer.</exception>
        private readonly void CheckForSufficientBufferSpace(int minBytes)
        {
            ulong newOffset = (ulong)this.position + (ulong)minBytes;

            if (newOffset > (ulong)this.buffer.Length)
            {
                throw new EndOfStreamException($"Attempted to read past the end of the {nameof(SpanBinaryReader)} buffer.");
            }
        }
    }
}
