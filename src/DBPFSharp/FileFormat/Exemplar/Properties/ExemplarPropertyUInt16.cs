// Copyright (c) 2023, 2025, 2026 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace DBPFSharp.FileFormat.Exemplar.Properties
{
    /// <summary>
    /// An exemplar property class for unsigned 16-bit integer values.
    /// </summary>
    /// <seealso cref="ExemplarProperty" />
    [DebuggerDisplay("{DebuggerDisplay}")]
    public sealed class ExemplarPropertyUInt16 : ExemplarProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExemplarPropertyUInt16"/> class.
        /// </summary>
        /// <param name="id">The property identifier.</param>
        /// <param name="value">The property value.</param>
        public ExemplarPropertyUInt16(uint id, ushort value) : base(id, 0)
        {
            this.Values = Array.AsReadOnly([value]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExemplarPropertyUInt16"/> class.
        /// </summary>
        /// <param name="id">The property identifier.</param>
        /// <param name="values">The property values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
        public ExemplarPropertyUInt16(uint id, IReadOnlyList<ushort> values) : base(id)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            List<ushort> valuesCopy = [.. values];

            this.Values = valuesCopy.AsReadOnly();
            this.RepCount = valuesCopy.Count == 1 ? 0 : valuesCopy.Count;
        }

        internal ExemplarPropertyUInt16(uint id,
                                        ReadOnlySpan<byte> text,
                                        int expectedRepCount) : base(id)
        {
            List<ushort> values = TextExemplarUtil.ParseUInt16Array(text, expectedRepCount);

            this.Values = values.AsReadOnly();
            this.RepCount = values.Count == 1 ? 0 : values.Count;
        }

        internal ExemplarPropertyUInt16(uint id,
                                        ref SpanBinaryReader reader,
                                        int repCount) : base(id, repCount)
        {
            this.Values = Decode(ref reader, repCount);
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"Id: 0x{this.Id:X8} Values: {this.PropertyDataType}[{this.Values.Count}]";

        private protected override void EncodeBinaryData(BinaryWriter writer)
        {
            foreach (ushort value in this.Values)
            {
                writer.Write(value);
            }
        }

        private static ReadOnlyCollection<ushort> Decode(ref SpanBinaryReader reader, int repCount)
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
