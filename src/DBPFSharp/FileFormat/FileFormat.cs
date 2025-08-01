// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;

namespace DBPFSharp.FileFormat
{
    /// <summary>
    /// The base class of all DBPF entry parsers.
    /// </summary>
    public abstract class FileFormat
    {
        private protected FileFormat()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileFormat"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        private protected FileFormat(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));

            Decode(data);
        }

        private protected abstract void Decode(byte[] data);

        /// <summary>
        /// Encodes the data to a byte array.
        /// </summary>
        /// <returns>The encoded data.</returns>
        public abstract byte[] Encode();
    }
}
