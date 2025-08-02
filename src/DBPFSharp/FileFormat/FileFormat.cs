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
        /// Encodes the data to a byte array.
        /// </summary>
        /// <returns>The encoded data.</returns>
        public abstract byte[] Encode();
    }
}
