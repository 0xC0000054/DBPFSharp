// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DBPFSharp
{
    internal sealed class CompressionDirectory : IList<CompressionDirectoryEntry>
    {
        private readonly List<CompressionDirectoryEntry> entries;

        public CompressionDirectory() : this(0)
        {
        }

        public CompressionDirectory(int count)
        {
            this.entries = new List<CompressionDirectoryEntry>(count);
        }

        public CompressionDirectoryEntry this[int index]
        {
            get => entries[index];
            set => entries[index] = value;
        }

        public int Count => entries.Count;

        public int Capacity
        {
            get => this.entries.Capacity;
            set => this.entries.Capacity = value;
        }

        bool ICollection<CompressionDirectoryEntry>.IsReadOnly => false;

        public ReadOnlyCollection<CompressionDirectoryEntry> AsReadOnly() => entries.AsReadOnly();

        public void Add(CompressionDirectoryEntry item) => entries.Add(item);

        public void Clear() => entries.Clear();

        public bool Contains(DBPFIndexEntry index)
        {
            ArgumentNullException.ThrowIfNull(index);

            return Find(index.Type, index.Group, index.Instance) != null;
        }

        public bool Contains(CompressionDirectoryEntry item) => entries.Contains(item);

        public void CopyTo(CompressionDirectoryEntry[] array, int arrayIndex) => entries.CopyTo(array, arrayIndex);

        public CompressionDirectoryEntry? Find(uint type, uint group, uint instance)
        {
            for (int i = 0; i < this.entries.Count; i++)
            {
                CompressionDirectoryEntry entry = this.entries[i];
                if (entry.Type == type && entry.Group == group && entry.Instance == instance)
                {
                    return entry;
                }
            }

            return null;
        }

        public IEnumerator<CompressionDirectoryEntry> GetEnumerator() => entries.GetEnumerator();

        public int IndexOf(CompressionDirectoryEntry item) => entries.IndexOf(item);

        public void Insert(int index, CompressionDirectoryEntry item) => entries.Insert(index, item);

        public bool Remove(CompressionDirectoryEntry item) => entries.Remove(item);

        public void RemoveAt(int index) => entries.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)entries).GetEnumerator();
    }
}
