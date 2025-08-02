// Copyright (c) 2023, 2025 Nicholas Hayes
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
        private readonly byte[]? compressedData;
        private byte[]? uncompressedData;
        private readonly bool shouldBeCompressed;

        private DBPFEntry(byte[]? compressedData, byte[]? uncompressedData, bool shouldBeCompressed = false)
        {
            if (compressedData != null)
            {
                if (uncompressedData != null)
                {
                    throw new ArgumentException($"One of {nameof(compressedData)} or {nameof(uncompressedData)} must be null.");
                }

                this.IsCompressed = true;
                this.shouldBeCompressed = false;
            }
            else
            {
                if (uncompressedData is null)
                {
                    throw new ArgumentException($"Both {nameof(compressedData)} and {nameof(uncompressedData)} cannot be null.");
                }

                this.IsCompressed = false;
                this.shouldBeCompressed = shouldBeCompressed;
            }

            this.compressedData = compressedData;
            this.uncompressedData = uncompressedData;
        }

        /// <summary>
        /// Creates a <see cref="DBPFEntry" /> from compressed data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>A DBPFEntry.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        internal static DBPFEntry FromCompressedData(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data);

            return new DBPFEntry(compressedData: data, uncompressedData: null);
        }

        /// <summary>
        /// Creates a <see cref="DBPFEntry" /> from uncompressed data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="shouldBeCompressed">
        /// <see langword="true"/> if the data should be compressed when saving; otherwise, <see langword="false"/>.
        /// </param>
        /// <returns>A DBPFEntry.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        internal static DBPFEntry FromUncompressedData(byte[] data, bool shouldBeCompressed = false)
        {
            ArgumentNullException.ThrowIfNull(data);

            return new DBPFEntry(compressedData: null, uncompressedData: data, shouldBeCompressed);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is is compressed.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if this instance is compressed; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsCompressed { get; }

        internal uint RawDataLength => (uint)(this.uncompressedData?.Length ?? 0);

        /// <summary>
        /// Gets the uncompressed data.
        /// </summary>
        /// <returns>
        ///   The uncompressed data.
        /// </returns>
        public byte[] GetUncompressedData()
        {
            if (this.uncompressedData is null)
            {
                if (this.compressedData is null)
                {
                    throw new InvalidOperationException("Both the compressed and uncompressed data are null.");
                }

                this.uncompressedData = QfsCompression.Decompress(this.compressedData);
            }

            byte[] bytes = GC.AllocateUninitializedArray<byte>(this.uncompressedData!.Length);
            this.uncompressedData.CopyTo(bytes, 0);

            return bytes;
        }

        internal (uint bytesWritten, bool isCompressed) Save(Stream stream)
        {
            uint bytesWritten;
            bool isCompressed;

            if (this.IsCompressed)
            {
                if (this.compressedData is null)
                {
                    throw new InvalidOperationException($"{nameof(this.IsCompressed)} is true when the compressed data is null.");
                }

                stream.Write(this.compressedData, 0, this.compressedData.Length);
                bytesWritten = (uint)this.compressedData.Length;
                isCompressed = true;
            }
            else
            {
                if (this.uncompressedData is null)
                {
                    throw new InvalidOperationException($"{nameof(this.IsCompressed)} is false when the uncompressed data is null.");
                }

                if (this.shouldBeCompressed)
                {
                    byte[]? data = QfsCompression.Compress(this.uncompressedData, prefixLength: true);

                    if (data != null)
                    {
                        stream.Write(data, 0, data.Length);
                        bytesWritten = (uint)data.Length;
                        isCompressed = true;
                    }
                    else
                    {
                        // The data could not be compressed.
                        stream.Write(this.uncompressedData!, 0, this.uncompressedData!.Length);
                        bytesWritten = (uint)this.uncompressedData!.Length;
                        isCompressed = false;
                    }
                }
                else
                {
                    stream.Write(this.uncompressedData!, 0, this.uncompressedData!.Length);
                    bytesWritten = (uint)this.uncompressedData.Length;
                    isCompressed = false;
                }
            }

            return (bytesWritten, isCompressed);
        }
    }
}
