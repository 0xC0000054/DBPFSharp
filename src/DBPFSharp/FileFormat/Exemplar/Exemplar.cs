// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using DBPFSharp.FileFormat.Exemplar.Properties;
using System;
using System.Collections.Generic;
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

            byte[] bytes = Array.Empty<byte>();

            using (MemoryStream stream = new())
            using (BinaryWriter writer = new(stream))
            {
                EncodeBinaryExemplar(writer);

                bytes = stream.ToArray();
            }

            return bytes;
        }

        private static (TGI parentCohort, bool isCohort, ExemplarPropertyCollection properties) Decode(byte[] bytes)
        {
            ReadOnlySpan<byte> bytesAsSpan = bytes;
            ReadOnlySpan<byte> signature = bytesAsSpan[..8];

            TGI parentCohort;
            bool isCohort;
            ExemplarPropertyCollection properties;

            if (signature.SequenceEqual(ExemplarBinarySignature)
                || signature.SequenceEqual(CohortBinarySignature))
            {
                isCohort = signature.SequenceEqual(CohortBinarySignature);

                using (MemoryStream stream = new(bytes, 8, bytes.Length - 8, false))
                using (BinaryReader reader = new(stream))
                {
                    (parentCohort, properties) = ParseBinaryExemplar(reader);
                }
            }
            else if (signature.SequenceEqual(ExemplarTextSignature)
                     || signature.SequenceEqual(CohortTextSignature))
            {
                isCohort = signature.SequenceEqual(CohortTextSignature);

                int firstNewLineIndex = bytesAsSpan.IndexOf((byte)'\n');

                if (firstNewLineIndex == -1)
                {
                    throw new InvalidOperationException($"The {signature.ToString()} header must end with a new line.");
                }

                using (MemoryStream stream = new(bytes, firstNewLineIndex + 1, bytes.Length - 1 - firstNewLineIndex, false))
                using (StreamReader reader = new(stream))
                {
                    (parentCohort, properties) = ParseTextExemplar(reader);
                }
            }
            else
            {
                throw new InvalidOperationException("The item is not a supported cohort or exemplar format.");
            }

            return (parentCohort, isCohort, properties);
        }

        private static (TGI parentCohort, ExemplarPropertyCollection properties) ParseBinaryExemplar(BinaryReader reader)
        {
            TGI parentCohort = BinaryExemplarUtil.ReadTGI(reader);
            int propertyCount = reader.ReadInt32();

            ExemplarPropertyCollection properties = [];

            if (propertyCount > 0)
            {
                properties.Capacity = propertyCount;

                for (int i = 0; i < propertyCount; i++)
                {
                    ExemplarProperty property = ExemplarPropertyFactory.Create(reader);

                    properties.TryAdd(property);
                }
            }

            return (parentCohort, properties);
        }

        private static (TGI parentCohort, ExemplarPropertyCollection properties) ParseTextExemplar(StreamReader reader)
        {
            TGI parentCohort = TextExemplarUtil.ParseParentCohort(reader.ReadLine());

            int propertyCount = TextExemplarUtil.ParsePropertyCount(reader.ReadLine());

            ExemplarPropertyCollection properties = [];

            if (propertyCount > 0)
            {
                properties.Capacity = propertyCount;

                for (int i = 0; i < propertyCount; i++)
                {
                    ExemplarProperty property = ExemplarPropertyFactory.Create(reader.ReadLine());

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
