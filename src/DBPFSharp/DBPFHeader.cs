// Copyright (c) 2023 Nicholas Hayes
// SPDX-License-Identifier: MIT

using DBPFSharp.Properties;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace DBPFSharp
{
    /// <summary>
    /// Encapsulates the header of a DBPF file.
    /// </summary>
    public sealed class DBPFHeader
    {
        /// <summary>
        /// The major version of the Sim City 4 DBPF format.
        /// </summary>
        private const uint SC4FormatMajorVersion = 1;

        /// <summary>
        /// The minor version of the Sim City 4 DBPF format.
        /// </summary>
        private const uint SC4FormatMinorVersion = 0;

        /// <summary>
        /// The index major version of the Sim City 4 DBPF format.
        /// </summary>
        private const uint SC4IndexMajorVersion = 7;

        /// <summary>
        /// Initializes a new instance of the <see cref="DBPFHeader"/> class.
        /// </summary>
        internal DBPFHeader()
        {
            this.MajorVersion = SC4FormatMajorVersion;
            this.MinorVersion = SC4FormatMinorVersion;
            this.UserMajorVersion = 0;
            this.UserMinorVersion = 0;
            this.Flags = 0;
            this.DateCreated = DateTimeOffset.Now;
            this.DateModified = DateTimeOffset.Now;
            this.IndexMajorVersion = SC4IndexMajorVersion;
            this.Entries = 0;
            this.IndexLocation = 96;
            this.IndexSize = 0;
            this.HoleCount = 0;
            this.HoleIndexSize = 0;
            this.HoleIndexLocation = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DBPFHeader" /> class.
        /// </summary>
        /// <param name="input">The Stream to read from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="input"/> is null.</exception>
        /// <exception cref="DBPFException">
        /// The header signature is invalid.
        /// -or-
        /// The DBPF version is not supported.
        /// -or-
        /// The IndexSize property has an invalid size.
        /// </exception>
        [SkipLocalsInit]
        internal DBPFHeader(Stream input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            input.Position = 0L;

            Span<byte> signatureBytes = stackalloc byte[4];

            input.ReadExactly(signatureBytes);

            if (!DBPFSignature.SequenceEqual(signatureBytes))
            {
                throw new DBPFException(Resources.DBPFHeaderInvalidIdentifer);
            }

            this.MajorVersion = input.ReadUInt32();
            this.MinorVersion = input.ReadUInt32();
            if (this.MajorVersion != SC4FormatMajorVersion || this.MinorVersion != SC4FormatMinorVersion)
            {
                throw new DBPFException(string.Format(CultureInfo.CurrentCulture, Resources.UnsupportedDBPFVersion, this.MajorVersion, this.MinorVersion));
            }

            this.UserMajorVersion = input.ReadUInt32();
            this.UserMinorVersion = input.ReadUInt32();
            this.Flags = input.ReadUInt32();
            this.DateCreated = DateTimeOffset.FromUnixTimeSeconds(input.ReadUInt32()).ToLocalTime();
            this.DateModified = DateTimeOffset.FromUnixTimeSeconds(input.ReadUInt32()).ToLocalTime();
            this.IndexMajorVersion = input.ReadUInt32();
            if (this.IndexMajorVersion != SC4IndexMajorVersion)
            {
                throw new DBPFException(string.Format(CultureInfo.CurrentCulture, Resources.UnsupportedIndexVersion, this.IndexMajorVersion));
            }

            this.Entries = input.ReadUInt32();
            this.IndexLocation = input.ReadUInt32();
            this.IndexSize = input.ReadUInt32();

            uint expectedIndexSize = this.Entries * DBPFIndexEntry.SizeOf;
            if (this.IndexSize != expectedIndexSize)
            {
                throw new DBPFException(Resources.InvalidIndexTableSize);
            }

            this.HoleCount = input.ReadUInt32();
            this.HoleIndexLocation = input.ReadUInt32();
            this.HoleIndexSize = input.ReadUInt32();
        }

        /// <summary>
        /// Gets the header major version.
        /// </summary>
        public uint MajorVersion { get; }

        /// <summary>
        /// Gets the header minor version.
        /// </summary>
        public uint MinorVersion { get; }

        /// <summary>
        /// Gets the user major version.
        /// </summary>
        public uint UserMajorVersion { get; }

        /// <summary>
        /// Gets the user minor version.
        /// </summary>
        public uint UserMinorVersion { get; }

        /// <summary>
        /// Gets the header flags.
        /// </summary>
        public uint Flags { get; }

        /// <summary>
        /// The Date the file was Created
        /// </summary>
        public DateTimeOffset DateCreated { get; internal set; }

        /// <summary>
        /// The Date the file was Modified
        /// </summary>
        public DateTimeOffset DateModified { get; internal set; }

        /// <summary>
        /// Gets the major version of the index table.
        /// </summary>
        public uint IndexMajorVersion { get; }

        /// <summary>
        /// Gets the number of entries in the file.
        /// </summary>
        public uint Entries { get; internal set; }

        /// <summary>
        /// Gets the location of the index table.
        /// </summary>
        public uint IndexLocation { get; internal set; }

        /// <summary>
        /// Gets the size of the index table.
        /// </summary>
        public uint IndexSize { get; internal set; }

        /// <summary>
        /// Gets the hole count.
        /// </summary>
        public uint HoleCount { get; }

        /// <summary>
        /// Gets the location of the hole index table.
        /// </summary>
        public uint HoleIndexLocation { get; }

        /// <summary>
        /// Gets the size of the hole index table.
        /// </summary>
        public uint HoleIndexSize { get; }

        private static ReadOnlySpan<byte> DBPFSignature => new byte[] { (byte)'D', (byte)'B', (byte)'P', (byte)'F' };

        /// <summary>
        /// Saves the <see cref="DBPFHeader"/>.
        /// </summary>
        /// <param name="stream">The Stream to save to</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        internal void Save(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);

            stream.Write(DBPFSignature);
            stream.WriteUInt32(this.MajorVersion);
            stream.WriteUInt32(this.MinorVersion);
            stream.WriteUInt32(this.UserMajorVersion);
            stream.WriteUInt32(this.UserMinorVersion);
            stream.WriteUInt32(this.Flags);
            stream.WriteUInt32(checked((uint)this.DateCreated.ToUnixTimeSeconds()));
            stream.WriteUInt32(checked((uint)this.DateModified.ToUnixTimeSeconds()));
            stream.WriteUInt32(this.IndexMajorVersion);
            stream.WriteUInt32(this.Entries);
            stream.WriteUInt32(this.IndexLocation);
            stream.WriteUInt32(this.IndexSize);
            stream.WriteUInt32(this.HoleCount);
            stream.WriteUInt32(this.HoleIndexLocation);
            stream.WriteUInt32(this.HoleIndexSize);

            // Write the reserved header bytes.
            Span<byte> reservedBytes = stackalloc byte[36];
            reservedBytes.Clear();

            stream.Write(reservedBytes);
        }


    }

}
