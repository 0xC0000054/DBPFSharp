// Copyright (c) 2023, 2025, 2026 Nicholas Hayes
// SPDX-License-Identifier: MIT

using DBPFSharp.FileFormat.Exemplar.Properties;
using System;
using System.IO;
using System.Linq;

namespace DBPFSharp.FileFormat.Exemplar
{
    /// <summary>
    /// Represents an exemplar or cohort.
    /// </summary>
    /// <seealso cref="FileFormat" />
    public sealed class Exemplar : FileFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Exemplar"/> class.
        /// </summary>
        public Exemplar()
        {
            this.ParentCohort = TGI.Empty;
            this.IsCohort = false;
            this.Properties = [];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Exemplar"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        public Exemplar(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data);

            (TGI parentCohort, bool isCohort, ExemplarPropertyCollection properties) = Decode(data);
            this.ParentCohort = parentCohort;
            this.IsCohort = isCohort;
            this.Properties = properties;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Exemplar"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public Exemplar(ReadOnlySpan<byte> data)
        {
            (TGI parentCohort, bool isCohort, ExemplarPropertyCollection properties) = Decode(data);
            this.ParentCohort = parentCohort;
            this.IsCohort = isCohort;
            this.Properties = properties;
        }

        /// <summary>
        /// Gets or sets the parent cohort.
        /// </summary>
        /// <value>
        /// The parent cohort.
        /// </value>
        public TGI ParentCohort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is cohort.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is cohort; otherwise, <c>false</c>.
        /// </value>
        public bool IsCohort { get; set; }

        /// <summary>
        /// Gets the exemplar properties.
        /// </summary>
        /// <value>
        /// The exemplar properties.
        /// </value>
        public ExemplarPropertyCollection Properties { get; }

        private static ReadOnlySpan<byte> CohortBinarySignature => "CQZB1###"u8;

        private static ReadOnlySpan<byte> CohortTextSignature => "CQZT1###"u8;

        private static ReadOnlySpan<byte> ExemplarBinarySignature => "EQZB1###"u8;

        private static ReadOnlySpan<byte> ExemplarTextSignature => "EQZT1###"u8;

        /// <inheritdoc/>
        public override byte[] Encode()
        {
            // Because the exemplar text format expects the property names to
            // be included, we currently only support writing the binary format.

            byte[] bytes = [];

            using (MemoryStream stream = new())
            using (BinaryWriter writer = new(stream))
            {
                EncodeBinaryExemplar(writer);

                bytes = stream.ToArray();
            }

            return bytes;
        }

        private static (TGI parentCohort, bool isCohort, ExemplarPropertyCollection properties) Decode(ReadOnlySpan<byte> bytes)
        {
            ReadOnlySpan<byte> signature = bytes[..8];

            TGI parentCohort;
            bool isCohort;
            ExemplarPropertyCollection properties;

            if (signature.SequenceEqual(ExemplarBinarySignature)
                || signature.SequenceEqual(CohortBinarySignature))
            {
                isCohort = signature.SequenceEqual(CohortBinarySignature);

                SpanBinaryReader reader = new(bytes[8..]);
 
                (parentCohort, properties) = ParseBinaryExemplar(ref reader);
                
            }
            else if (signature.SequenceEqual(ExemplarTextSignature)
                     || signature.SequenceEqual(CohortTextSignature))
            {
                isCohort = signature.SequenceEqual(CohortTextSignature);
                
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
                    throw new InvalidOperationException($"The text {(isCohort ? "cohort" : "exemplar")} header must end with a new line.");
                }

                SpanAsciiTextReader reader = new(bytes[dataStartOffset..]);
                
                (parentCohort, properties) = ParseTextExemplar(ref reader);
            }
            else
            {
                throw new InvalidOperationException("The item is not a supported cohort or exemplar format.");
            }

            return (parentCohort, isCohort, properties);
        }

        private static (TGI parentCohort, ExemplarPropertyCollection properties) ParseBinaryExemplar(ref SpanBinaryReader reader)
        {
            TGI parentCohort = BinaryExemplarUtil.ReadTGI(ref reader);
            int propertyCount = reader.ReadInt32();

            ExemplarPropertyCollection properties = [];

            if (propertyCount > 0)
            {
                properties.Capacity = propertyCount;

                for (int i = 0; i < propertyCount; i++)
                {
                    ExemplarProperty property = ExemplarPropertyFactory.CreateFromBinary(ref reader);

                    properties.TryAdd(property);
                }
            }

            return (parentCohort, properties);
        }

        private static (TGI parentCohort, ExemplarPropertyCollection properties) ParseTextExemplar(ref SpanAsciiTextReader reader)
        {
            TGI parentCohort = TextExemplarUtil.ParseParentCohort(reader.ReadLine());

            int propertyCount = TextExemplarUtil.ParsePropertyCount(reader.ReadLine());

            ExemplarPropertyCollection properties = [];

            if (propertyCount > 0)
            {
                properties.Capacity = propertyCount;

                for (int i = 0; i < propertyCount; i++)
                {
                    ExemplarProperty property = ExemplarPropertyFactory.CreateFromText(reader.ReadLine());

                    properties.TryAdd(property);
                }
            }

            return (parentCohort, properties);
        }

        private void EncodeBinaryExemplar(BinaryWriter writer)
        {
            ReadOnlySpan<byte> signature = this.IsCohort ? CohortBinarySignature : ExemplarBinarySignature;

            writer.Write(signature);
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
