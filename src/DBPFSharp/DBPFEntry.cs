// Copyright (c) 2023 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.IO;

namespace DBPFSharp
{
    /// <summary>
    /// Encapsulates a file entry within a <see cref="DBPFFile"/>.
    /// </summary>
    public sealed class DBPFEntry
    {
        private readonly byte[] rawData;

        /// <summary>
        /// Initializes a new instance of the Entry class with the specified data
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="isCompressed">
        /// <see langword="true"/> if the data is isCompressed; otherwise, <see langword="false"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">The data is null.</exception>
        internal DBPFEntry(byte[] data, bool isCompressed)
        {
            ArgumentNullException.ThrowIfNull(data);

            rawData = data;
            IsCompressed = isCompressed;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is isCompressed.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if this instance is isCompressed; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsCompressed { get; internal set; }

        internal uint RawDataLength => (uint)this.rawData.Length;

        /// <summary>
        /// Gets the uncompressed data.
        /// </summary>
        /// <returns>
        ///   The uncompressed data.
        /// </returns>
        public byte[] GetUncompressedData()
        {
            byte[] bytes;

            if (IsCompressed)
            {
                bytes = QfsCompression.Decompress(this.rawData);
            }
            else
            {
                bytes = GC.AllocateUninitializedArray<byte>(this.rawData.Length);
                this.rawData.CopyTo(bytes, 0);
            }

            return bytes;
        }

        internal uint Save(Stream stream)
        {
            uint bytesWritten;

            if (IsCompressed)
            {
                byte[]? compressedData = QfsCompression.Compress(this.rawData, prefixLength: true);

                if (compressedData != null)
                {
                    stream.Write(compressedData, 0, compressedData.Length);

                    bytesWritten = (uint)compressedData.Length;
                }
                else
                {
                    // The data could not be isCompressed.
                    IsCompressed = false;

                    stream.Write(rawData, 0, rawData.Length);

                    bytesWritten = (uint)rawData.Length;
                }
            }
            else
            {
                stream.Write(rawData, 0, rawData.Length);

                bytesWritten = (uint)rawData.Length;
            }

            return bytesWritten;
        }
    }
}
