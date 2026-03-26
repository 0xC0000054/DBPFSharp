// Copyright (c) 2023, 2025, 2026 Nicholas Hayes
// SPDX-License-Identifier: MIT

using DBPFSharp.FileFormat.Exemplar.Properties;
using System;
using System.IO;

namespace DBPFSharp.FileFormat.Exemplar
{
    /// <summary>
    /// A base class for the cohort and exemplar types.
    /// </summary>
    /// <seealso cref="FileFormat" />
    public abstract class ExemplarBase : FileFormat
    {
        private protected ExemplarBase()
        {
            this.ParentCohort = TGI.Empty;
            this.Properties = [];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExemplarBase"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        private protected ExemplarBase(byte[] data) : this()
        {
            ArgumentNullException.ThrowIfNull(data);

            Decode(data);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExemplarBase"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        private protected ExemplarBase(ReadOnlySpan<byte> data) : this()
        {
            Decode(data);
        }

        /// <inheritdoc/>
        public sealed override byte[] Encode()
        {
            // Because the exemplar text format expects the property names to
            // be included, we currently only support writing the binary format.

            byte[] bytes = [];

            using (MemoryStream stream = new())
            using (BinaryWriter writer = new(stream))
            {
                EncodeBinary(writer);

                bytes = stream.ToArray();
            }

            return bytes;
        }

        /// <summary>
        /// Gets or sets the parent cohort.
        /// </summary>
        /// <value>
        /// The parent cohort.
        /// </value>
        public TGI ParentCohort { get; set; }

        /// <summary>
        /// Gets the exemplar properties.
        /// </summary>
        /// <value>
        /// The exemplar properties.
        /// </value>
        public ExemplarPropertyCollection Properties { get; }

        private protected abstract ReadOnlySpan<byte> BinaryHeaderSignature { get; }

        private protected abstract ReadOnlySpan<byte> TextHeaderSignature { get; }

        private void Decode(ReadOnlySpan<byte> bytes)
        {
            ReadOnlySpan<byte> signature = bytes[..8];

            if (signature.SequenceEqual(this.BinaryHeaderSignature))
            {
                SpanBinaryReader reader = new(bytes[8..]);

                DecodeBinary(ref reader);
            }
            else if (signature.SequenceEqual(this.TextHeaderSignature))
            {
                byte firstNewLineCharacter = bytes[8];
                int dataStartOffset = 9;

                if (firstNewLineCharacter == (byte)'\r')
                {
                    // If we found '\r', consume any immediately following '\n'.
                    if (dataStartOffset < bytes.Length && bytes[dataStartOffset] == (byte)'\n')
                    {
                        dataStartOffset++;
                    }
                }
                else if (firstNewLineCharacter != (byte)'\n')
                {
                    throw new DBPFException($"The text format header must end with a new line.");
                }

                SpanAsciiTextReader reader = new(bytes[dataStartOffset..]);

                DecodeText(ref reader);
            }
            else
            {
                throw new DBPFException("The header has an unsupported signature.");
            }
        }

        private void DecodeBinary(ref SpanBinaryReader reader)
        {
            this.ParentCohort = BinaryExemplarUtil.ReadTGI(ref reader);
            int propertyCount = reader.ReadInt32();

            ExemplarPropertyCollection properties = this.Properties;

            if (propertyCount > 0)
            {
                properties.Capacity = propertyCount;

                for (int i = 0; i < propertyCount; i++)
                {
                    ExemplarProperty property = ExemplarPropertyFactory.CreateFromBinary(ref reader);

                    properties.TryAdd(property);
                }
            }
        }

        private void DecodeText(ref SpanAsciiTextReader reader)
        {
            this.ParentCohort = TextExemplarUtil.ParseParentCohort(reader.ReadLine());
            int propertyCount = TextExemplarUtil.ParsePropertyCount(reader.ReadLine());

            ExemplarPropertyCollection properties = this.Properties;

            if (propertyCount > 0)
            {
                properties.Capacity = propertyCount;

                for (int i = 0; i < propertyCount; i++)
                {
                    ExemplarProperty property = ExemplarPropertyFactory.CreateFromText(reader.ReadLine());

                    properties.TryAdd(property);
                }
            }
        }

        private void EncodeBinary(BinaryWriter writer)
        {
            writer.Write(this.BinaryHeaderSignature);
            BinaryExemplarUtil.WriteTGI(writer, this.ParentCohort);

            ExemplarPropertyCollection properties = this.Properties;

            writer.Write(properties.Count);

            foreach (var property in properties)
            {
                property.EncodeBinary(writer);
            }
        }
    }
}
