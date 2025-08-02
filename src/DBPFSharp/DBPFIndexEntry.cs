// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.IO;

namespace DBPFSharp
{
    /// <summary>
    /// An enumeration specifying the DatIndex states.
    /// </summary>
    public enum DatIndexState
    {
        /// <summary>
        /// The normal state of a DatIndex.
        /// </summary>
        Normal,
        /// <summary>
        /// The DatIndex is a new file.
        /// </summary>
        New,
        /// <summary>
        /// The DatIndex will be deleted when the file is saved.
        /// </summary>
        Deleted,
        /// <summary>
        /// The DatIndex is an existing file that has been modified.
        /// </summary>
        Modified
    }

    /// <summary>
    /// The class that holds the TGI and location data of an entry within the file
    /// </summary>
    public sealed class DBPFIndexEntry : IEquatable<DBPFIndexEntry>
    {
        internal const uint SizeOf = 20U;

        /// <summary>
        /// Initializes a new instance of the <see cref="DBPFIndexEntry"/> class.
        /// </summary>
        /// <param name="type">The type id of the entry.</param>
        /// <param name="group">The group id of the entry.</param>
        /// <param name="instance">The instance id of the entry.</param>
        /// <param name="location">The location of the entry.</param>
        /// <param name="fileSize">Size of the entry.</param>
        internal DBPFIndexEntry(uint type, uint group, uint instance, uint location, uint fileSize)
        {
            this.Type = type;
            this.Group = group;
            this.Instance = instance;
            this.Location = location;
            this.FileSize = fileSize;
            this.IndexState = DatIndexState.Normal;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DBPFIndexEntry" /> class.
        /// </summary>
        /// <param name="type">The type id of the entry.</param>
        /// <param name="group">The group id of the entry.</param>
        /// <param name="instance">The instance id of the entry.</param>
        /// <param name="fileItem">The file item.</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileItem"/> is null.</exception>
        internal DBPFIndexEntry(uint type, uint group, uint instance, DBPFEntry fileItem)
        {
            ArgumentNullException.ThrowIfNull(fileItem);

            this.Type = type;
            this.Group = group;
            this.Instance = instance;
            this.Location = 0;
            this.FileSize = 0;
            this.IndexState = DatIndexState.New;
            this.Entry = fileItem;
        }

        /// <summary>
        /// Gets the TGI of the index.
        /// </summary>
        public TGI TGI => new(this.Type, this.Group, this.Instance);

        /// <summary>
        /// Gets the type id of the index.
        /// </summary>
        public uint Type { get; }

        /// <summary>
        /// Gets the group id of the index.
        /// </summary>
        public uint Group { get; }

        /// <summary>
        /// Gets the instance id of the index.
        /// </summary>
        public uint Instance { get; }

        /// <summary>
        /// Gets the location of the item in the file.
        /// </summary>
        public uint Location { get; }

        /// <summary>
        /// Gets the size of the item in the file (possibly compressed).
        /// </summary>
        public uint FileSize { get; }

        /// <summary>
        /// Gets the state of the index.
        /// </summary>
        public DatIndexState IndexState { get; internal set; }

        /// <summary>
        /// Gets the entry containing the file data.
        /// </summary>
        /// <value>
        /// The entry containing the file data.
        /// </value>
        internal DBPFEntry? Entry { get; set; }

        /// <summary>
        /// Saves the DatIndex instance to the specified Stream.
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        internal void Save(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);

            stream.WriteUInt32(this.Type);
            stream.WriteUInt32(this.Group);
            stream.WriteUInt32(this.Instance);
            stream.WriteUInt32(this.Location);
            stream.WriteUInt32(this.FileSize);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj) => obj is DBPFIndexEntry other && Equals(other);

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(DBPFIndexEntry? other)
        {
            if (other is null)
            {
                return false;
            }

            return this.Type == other.Type
                && this.Group == other.Group
                && this.Instance == other.Instance;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode() => HashCode.Combine(this.Type, this.Group, this.Instance);

        /// <summary>
        /// Determines whether two DatIndex instances have the same value.
        /// </summary>
        /// <param name="index1">The first object to compare.</param>
        /// <param name="index2">The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if the DatIndex instances are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(DBPFIndexEntry? index1, DBPFIndexEntry? index2)
        {
            if (index1 is null || index2 is null)
            {
                return Equals(index1, index2);
            }

            return index1.Equals(index2);
        }

        /// <summary>
        /// Determines whether two DatIndex instances do not have the same value.
        /// </summary>
        /// <param name="index1">The first object to compare.</param>
        /// <param name="index2">The second object to compare.</param>
        /// <returns>
        /// <c>true</c> if the DatIndex instances are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(DBPFIndexEntry? index1, DBPFIndexEntry? index2)
        {
            return !(index1 == index2);
        }
    }
}
