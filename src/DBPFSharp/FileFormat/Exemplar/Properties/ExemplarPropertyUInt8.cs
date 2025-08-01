// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace DBPFSharp.FileFormat.Exemplar.Properties
{
    /// <summary>
    /// An exemplar property class for unsigned 8-bit integer values.
    /// </summary>
    /// <seealso cref="ExemplarProperty" />
    public sealed class ExemplarPropertyUInt8 : ExemplarProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExemplarPropertyUInt8"/> class.
        /// </summary>
        /// <param name="id">The property identifier.</param>
        /// <param name="value">The property value.</param>
        public ExemplarPropertyUInt8(uint id, byte value) : base(id, 0)
        {
            Values = Array.AsReadOnly([value]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExemplarPropertyUInt8"/> class.
        /// </summary>
        /// <param name="id">The property identifier.</param>
        /// <param name="values">The property values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="values"/> must have at least one item.</exception>
        public ExemplarPropertyUInt8(uint id, IReadOnlyList<byte> values) : base(id)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            if (values.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(values), "Must have at least one item.");
            }

            List<byte> valuesCopy = [.. values];

            Values = valuesCopy.AsReadOnly();
            RepCount = valuesCopy.Count == 1 ? 0 : valuesCopy.Count;
        }

        internal ExemplarPropertyUInt8(uint id,
                                       BinaryReader reader,
                                       int repCount) : base(id, repCount)
        {
            Values = Array.AsReadOnly(reader.ReadBytes(repCount));
        }

        /// <inheritdoc/>
        public override ExemplarPropertyDataType PropertyDataType => ExemplarPropertyDataType.UInt8;

        /// <summary>
        /// Gets the property values.
        /// </summary>
        /// <value>
        /// The property values.
        /// </value>
        public ReadOnlyCollection<byte> Values { get; }

        private protected override void EncodeBinaryData(BinaryWriter writer)
        {
            byte[] data = [.. Values];

            writer.Write(data, 0, data.Length);
        }
    }
}
