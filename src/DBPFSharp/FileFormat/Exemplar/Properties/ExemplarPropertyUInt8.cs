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
    /// An exemplar property class for unsigned 8-bit integer values.
    /// </summary>
    /// <seealso cref="ExemplarProperty" />
    [DebuggerDisplay("{DebuggerDisplay}")]
    public sealed class ExemplarPropertyUInt8 : ExemplarProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExemplarPropertyUInt8"/> class.
        /// </summary>
        /// <param name="id">The property identifier.</param>
        /// <param name="value">The property value.</param>
        public ExemplarPropertyUInt8(uint id, byte value) : base(id, 0)
        {
            this.Values = Array.AsReadOnly([value]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExemplarPropertyUInt8"/> class.
        /// </summary>
        /// <param name="id">The property identifier.</param>
        /// <param name="values">The property values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
        public ExemplarPropertyUInt8(uint id, IReadOnlyList<byte> values) : base(id)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            List<byte> valuesCopy = [.. values];

            this.Values = valuesCopy.AsReadOnly();
            this.RepCount = valuesCopy.Count == 1 ? 0 : valuesCopy.Count;
        }

        internal ExemplarPropertyUInt8(uint id,
                                       ReadOnlySpan<byte> text,
                                       int expectedRepCount) : base(id)
        {
            List<byte> values = TextExemplarUtil.ParseUInt8Array(text, expectedRepCount);

            this.Values = values.AsReadOnly();
            this.RepCount = values.Count == 1 ? 0 : values.Count;
        }

        internal ExemplarPropertyUInt8(uint id,
                                       ref SpanBinaryReader reader,
                                       int repCount) : base(id, repCount)
        {
            this.Values = Array.AsReadOnly(reader.ReadBytes(repCount).ToArray());
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"Id: 0x{this.Id:X8} Values: {this.PropertyDataType}[{this.Values.Count}]";

        private protected override void EncodeBinaryData(BinaryWriter writer)
        {
            byte[] data = [.. this.Values];

            writer.Write(data, 0, data.Length);
        }
    }
}
