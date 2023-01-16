// Copyright (c) 2023 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.IO;

namespace DBPFSharp
{
    /// <summary>
    /// Encapsulates a DBPF compression directory entry
    /// </summary>
    public sealed class CompressionDirectoryEntry
    {
        internal const int SizeOf = 16;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompressionDirectoryEntry"/> class.
        /// </summary>
        /// <param name="type">The type id of the entry.</param>
        /// <param name="group">The group id of the entry.</param>
        /// <param name="instance">The instance id of the entry.</param>
        /// <param name="unCompressedSize">The uncompressed size of the entry.</param>
        internal CompressionDirectoryEntry(uint type, uint group, uint instance, uint unCompressedSize)
        {
            this.Type = type;
            this.Group = group;
            this.Instance = instance;
            this.UncompressedSize = unCompressedSize;
        }

        /// <summary>
        /// Gets the type id of the compression directory.
        /// </summary>
        public uint Type { get; }

        /// <summary>
        /// Gets the group id of the compression directory.
        /// </summary>
        public uint Group { get; }

        /// <summary>
        /// Gets the instance id of the compression directory.
        /// </summary>
        public uint Instance { get; }

        /// <summary>
        /// Gets the uncompressed size of the file.
        /// </summary>
        public uint UncompressedSize { get; }

        /// <summary>
        /// Saves the <see cref="CompressionDirectoryEntry"/> to the specified stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> where the entry will be saved..</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        internal void Save(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            stream.WriteUInt32(this.Type);
            stream.WriteUInt32(this.Group);
            stream.WriteUInt32(this.Instance);
            stream.WriteUInt32(this.UncompressedSize);
        }
    }

}
