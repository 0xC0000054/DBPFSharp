// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace DBPFSharp.FileFormat.Exemplar.Properties
{
    /// <summary>
    /// An exemplar property class for strings.
    /// </summary>
    /// <seealso cref="ExemplarProperty" />
    public sealed class ExemplarPropertyString : ExemplarProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExemplarPropertyString"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        public ExemplarPropertyString(uint id, string value) : base(id)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
            this.RepCount = Encoding.ASCII.GetByteCount(value);
        }

        internal ExemplarPropertyString(uint id,
                                        BinaryReader reader,
                                        int repCount) : base(id, repCount)
        {
            this.Value = Decode(reader, repCount);
        }

        /// <inheritdoc/>
        public override ExemplarPropertyDataType PropertyDataType => ExemplarPropertyDataType.String;

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <value>
        /// The property value.
        /// </value>
        public string Value { get; }


        [SkipLocalsInit]
        private protected override void EncodeBinaryData(BinaryWriter writer)
        {
            const int MaxStackLimit = 256;

            // Because the strings are ASCII encoded, the repetition count is
            // equal to the string length in bytes.
            int bytesNeeded = this.RepCount;

            Span<byte> buffer = bytesNeeded <= MaxStackLimit ? stackalloc byte[bytesNeeded] : new byte[bytesNeeded];

            int bytesWritten = Encoding.ASCII.GetBytes(this.Value, buffer);

            if (bytesWritten != bytesNeeded)
            {
                throw new InvalidOperationException("Not all of the string data was written to the buffer.");
            }

            writer.Write(buffer);
        }

        [SkipLocalsInit]
        private static string Decode(BinaryReader reader, int repCount)
        {
            const int MaxStackLimit = 256;

            Span<byte> buffer = repCount <= MaxStackLimit ? stackalloc byte[repCount] : new byte[repCount];

            reader.BaseStream.ReadExactly(buffer);

            return Encoding.ASCII.GetString(buffer);
        }
    }
}
