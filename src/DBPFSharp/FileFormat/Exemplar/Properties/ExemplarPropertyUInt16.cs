// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace DBPFSharp.FileFormat.Exemplar.Properties
{
    /// <summary>
    /// An exemplar property class for unsigned 16-bit integer values.
    /// </summary>
    /// <seealso cref="ExemplarProperty" />
    public sealed class ExemplarPropertyUInt16 : ExemplarProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExemplarPropertyUInt16"/> class.
        /// </summary>
        /// <param name="id">The property identifier.</param>
        /// <param name="value">The property value.</param>
        public ExemplarPropertyUInt16(uint id, ushort value) : base(id, 0)
        {
            Values = Array.AsReadOnly([value]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExemplarPropertyUInt16"/> class.
        /// </summary>
        /// <param name="id">The property identifier.</param>
        /// <param name="values">The property values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="values"/> must have at least one item.</exception>
        public ExemplarPropertyUInt16(uint id, IReadOnlyList<ushort> values) : base(id)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            if (values.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(values), "Must have at least one item.");
            }

            List<ushort> valuesCopy = [.. values];

            Values = valuesCopy.AsReadOnly();
            RepCount = valuesCopy.Count == 1 ? 0 : valuesCopy.Count;
        }

        internal ExemplarPropertyUInt16(uint id,
                                        BinaryReader reader,
                                        int repCount) : base(id, repCount)
        {
            Values = Decode(reader, repCount);
        }

        /// <inheritdoc/>
        public override ExemplarPropertyDataType PropertyDataType => ExemplarPropertyDataType.UInt16;

        /// <summary>
        /// Gets the property values.
        /// </summary>
        /// <value>
        /// The property values.
        /// </value>
        public ReadOnlyCollection<ushort> Values { get; }

        private protected override void EncodeBinaryData(BinaryWriter writer)
        {
            foreach (ushort value in Values)
            {
                writer.Write(value);
            }
        }

        private static ReadOnlyCollection<ushort> Decode(BinaryReader reader, int repCount)
        {
            ushort[] values = new ushort[repCount];

            for (int i = 0; i < repCount; i++)
            {
                values[i] = reader.ReadUInt16();
            }

            return Array.AsReadOnly(values);
        }
    }
}
