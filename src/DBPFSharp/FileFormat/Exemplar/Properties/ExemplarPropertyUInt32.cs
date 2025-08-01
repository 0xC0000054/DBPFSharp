// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace DBPFSharp.FileFormat.Exemplar.Properties
{
    /// <summary>
    /// An exemplar property class for unsigned 32-bit integer values.
    /// </summary>
    /// <seealso cref="ExemplarProperty" />
    public sealed class ExemplarPropertyUInt32 : ExemplarProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExemplarPropertyUInt32"/> class.
        /// </summary>
        /// <param name="id">The property identifier.</param>
        /// <param name="value">The property value.</param>
        public ExemplarPropertyUInt32(uint id, uint value) : base(id, 0)
        {
            Values = Array.AsReadOnly([value]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExemplarPropertyUInt32"/> class.
        /// </summary>
        /// <param name="id">The property identifier.</param>
        /// <param name="values">The property values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="values"/> must have at least one item.</exception>
        public ExemplarPropertyUInt32(uint id, IReadOnlyList<uint> values) : base(id)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            if (values.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(values), "Must have at least one item.");
            }

            List<uint> valuesCopy = [.. values];

            Values = valuesCopy.AsReadOnly();
            RepCount = valuesCopy.Count == 1 ? 0 : valuesCopy.Count;
        }

        internal ExemplarPropertyUInt32(uint id,
                                        BinaryReader reader,
                                        int repCount) : base(id, repCount)
        {
            Values = Decode(reader, repCount);
        }

        /// <inheritdoc/>
        public override ExemplarPropertyDataType PropertyDataType => ExemplarPropertyDataType.UInt32;

        /// <summary>
        /// Gets the property values.
        /// </summary>
        /// <value>
        /// The property values.
        /// </value>
        public ReadOnlyCollection<uint> Values { get; }

        private protected override void EncodeBinaryData(BinaryWriter writer)
        {
            foreach (uint value in Values)
            {
                writer.Write(value);
            }
        }

        private static ReadOnlyCollection<uint> Decode(BinaryReader reader, int repCount)
        {
            uint[] values = new uint[repCount];

            for (int i = 0; i < repCount; i++)
            {
                values[i] = reader.ReadUInt32();
            }

            return Array.AsReadOnly(values);
        }
    }
}
