// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Diagnostics.CodeAnalysis;

namespace DBPFSharp
{
    /// <summary>
    /// The type, group, instance triple that is used to identify a DBPF entry.
    /// </summary>
    public readonly struct TGI : IEquatable<TGI>
    {
        /// <summary>
        /// An empty TGI that has all values set to zero.
        /// </summary>
        public static readonly TGI Empty = new(0, 0, 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="TGI"/> structure.
        /// </summary>
        /// <param name="type">The type id.</param>
        /// <param name="group">The group id.</param>
        /// <param name="instance">The instance id.</param>
        public TGI(uint type, uint group, uint instance)
        {
            this.Type = type;
            this.Group = group;
            this.Instance = instance;
        }

        /// <summary>
        /// Gets the type id.
        /// </summary>
        public uint Type { get; }

        /// <summary>
        /// Gets the group id.
        /// </summary>
        public uint Group { get; }

        /// <summary>
        /// Gets the instance id.
        /// </summary>
        public uint Instance { get; }

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is TGI other && Equals(other);
        }

        /// <inheritdoc/>
        public bool Equals(TGI other)
        {
            return this.Type == other.Type
                   && this.Group == other.Group
                   && this.Instance == other.Instance;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Type, this.Group, this.Instance);
        }

        /// <inheritdoc/>
        public override string? ToString()
        {
            return $"Type=0x{this.Type:X8}, Group=0x{this.Group:X8}, Instance=0x{this.Instance:X8}";
        }

        /// <inheritdoc/>
        public static bool operator ==(TGI left, TGI right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(TGI left, TGI right)
        {
            return !(left == right);
        }
    }
}
