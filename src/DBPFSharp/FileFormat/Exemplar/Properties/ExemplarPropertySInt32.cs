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
    /// An exemplar property class for signed 32-bit integer values.
    /// </summary>
    /// <seealso cref="ExemplarProperty" />
    [DebuggerDisplay("{DebuggerDisplay}")]
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
        public ExemplarPropertySInt32(uint id, IReadOnlyList<int> values) : base(id)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            List<int> valuesCopy = [..values];

            this.Values = valuesCopy.AsReadOnly();
            this.RepCount = valuesCopy.Count == 1 ? 0 : valuesCopy.Count;
        }

        internal ExemplarPropertySInt32(uint id,
                                        ReadOnlySpan<byte> text,
                                        int expectedRepCount) : base(id)
        {
            List<int> values = TextExemplarUtil.ParseSInt32Array(text, expectedRepCount);

            this.Values = values.AsReadOnly();
            this.RepCount = values.Count == 1 ? 0 : values.Count;
        }

        internal ExemplarPropertySInt32(uint id,
                                        ref SpanBinaryReader reader,
                                        int repCount) : base(id, repCount)
        {
            this.Values = Decode(ref reader, repCount);
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"Id: 0x{this.Id:X8} Values: {this.PropertyDataType}[{this.Values.Count}]";

        private protected override void EncodeBinaryData(BinaryWriter writer)
        {
            foreach (int value in this.Values)
            {
                writer.Write(value);
            }
        }

        private static ReadOnlyCollection<int> Decode(ref SpanBinaryReader reader, int repCount)
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
