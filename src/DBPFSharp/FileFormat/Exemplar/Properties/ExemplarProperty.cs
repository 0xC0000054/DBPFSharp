// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System.IO;

namespace DBPFSharp.FileFormat.Exemplar.Properties
{
    /// <summary>
    /// The base class for all exemplar properties.
    /// </summary>
    public abstract class ExemplarProperty
    {
        private protected ExemplarProperty(uint id)
        {
            this.Id = id;
        }

        private protected ExemplarProperty(uint id, int repCount)
        {
            this.Id = id;
            this.RepCount = repCount;
        }

        /// <summary>
        /// The property id.
        /// </summary>
        public uint Id { get; }

        /// <summary>
        /// The property data type.
        /// </summary>
        public abstract ExemplarPropertyDataType PropertyDataType { get; }

        private protected int RepCount { get; init; }

        internal void EncodeBinary(BinaryWriter stream)
        {
            ExemplarPropertyHeader.WriteBinary(stream, this.Id, this.PropertyDataType, this.RepCount);

            EncodeBinaryData(stream);
        }

        private protected abstract void EncodeBinaryData(BinaryWriter writer);
    }
}
