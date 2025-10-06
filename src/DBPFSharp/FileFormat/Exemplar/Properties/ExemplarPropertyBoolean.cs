// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace DBPFSharp.FileFormat.Exemplar.Properties
{
    /// <summary>
    /// An exemplar property class for Boolean values.
    /// </summary>
    /// <seealso cref="ExemplarProperty" />
    public sealed class ExemplarPropertyBoolean : ExemplarProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExemplarPropertyBoolean"/> class.
        /// </summary>
        /// <param name="id">The property identifier.</param>
        /// <param name="value">The property value.</param>
        public ExemplarPropertyBoolean(uint id, bool value) : base(id, 0)
        {
            this.Values = Array.AsReadOnly([value]);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExemplarPropertyBoolean"/> class.
        /// </summary>
        /// <param name="id">The property identifier.</param>
        /// <param name="values">The property values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
        public ExemplarPropertyBoolean(uint id, IReadOnlyList<bool> values) : base(id)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            List<bool> valuesCopy = [.. values];

            this.Values = valuesCopy.AsReadOnly();
            this.RepCount = valuesCopy.Count == 1 ? 0 : valuesCopy.Count;
        }

        internal ExemplarPropertyBoolean(uint id,
                                         BinaryReader reader,
                                         int repCount) : base(id, repCount)
        {
            this.Values = Decode(reader, repCount);
        }

        /// <inheritdoc/>
        public override ExemplarPropertyDataType PropertyDataType => ExemplarPropertyDataType.Boolean;

        /// <summary>
        /// Gets the property values.
        /// </summary>
        /// <value>
        /// The property values.
        /// </value>
        public ReadOnlyCollection<bool> Values { get; }

        [SkipLocalsInit]
        private protected override void EncodeBinaryData(BinaryWriter writer)
        {
            const int MaxStackLimit = 256;

            ReadOnlyCollection<bool> values = this.Values;

            int count = values.Count;

            if (count > 0)
            {
                Span<byte> buffer = count <= MaxStackLimit ? stackalloc byte[count] : new byte[count];

                for (int i = 0; i < count; i++)
                {
                    buffer[i] = (byte)(values[i] ? 1 : 0);
                }

                writer.Write(buffer);
            }
        }

        private static ReadOnlyCollection<bool> Decode(BinaryReader reader, int repCount)
        {
            bool[] values = new bool[repCount];

            for (int i = 0; i < repCount; i++)
            {
                values[i] = reader.ReadByte() != 0;
            }

            return Array.AsReadOnly(values);
        }
    }
}
