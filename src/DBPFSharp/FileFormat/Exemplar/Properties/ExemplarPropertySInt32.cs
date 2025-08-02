// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace DBPFSharp.FileFormat.Exemplar.Properties
{
    /// <summary>
    /// An exemplar property class for signed 32-bit integer values.
    /// </summary>
    /// <seealso cref="ExemplarProperty" />
    public sealed class ExemplarPropertySInt32 : ExemplarProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExemplarPropertySInt32"/> class.
        /// </summary>
        /// <param name="id">The property identifier.</param>
        /// <param name="value">The property value.</param>
        public ExemplarPropertySInt32(uint id, int value) : base(id, 0)
        {
            this.Values = Array.AsReadOnly([value]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExemplarPropertySInt32"/> class.
        /// </summary>
        /// <param name="id">The property identifier.</param>
        /// <param name="values">The property values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="values"/> must have at least one item.</exception>
        public ExemplarPropertySInt32(uint id, IReadOnlyList<int> values) : base(id)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            if (values.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(values), "Must have at least one item.");
            }

            List<int> valuesCopy = [..values];

            this.Values = valuesCopy.AsReadOnly();
            this.RepCount = valuesCopy.Count == 1 ? 0 : valuesCopy.Count;
        }

        internal ExemplarPropertySInt32(uint id,
                                        BinaryReader reader,
                                        int repCount) : base(id, repCount)
        {
            this.Values = Decode(reader, repCount);
        }

        /// <inheritdoc/>
        public override ExemplarPropertyDataType PropertyDataType => ExemplarPropertyDataType.SInt32;

        /// <summary>
        /// Gets the property values.
        /// </summary>
        /// <value>
        /// The property values.
        /// </value>
        public ReadOnlyCollection<int> Values { get; }

        private protected override void EncodeBinaryData(BinaryWriter writer)
        {
            foreach (int value in this.Values)
            {
                writer.Write(value);
            }
        }

        private static ReadOnlyCollection<int> Decode(BinaryReader reader, int repCount)
        {
            int[] values = new int[repCount];

            for (int i = 0; i < repCount; i++)
            {
                values[i] = reader.ReadInt32();
            }

            return Array.AsReadOnly(values);
        }
    }
}
