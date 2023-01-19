// Copyright (c) 2023 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DBPFSharp
{
    internal sealed class DBPFIndexCollection : IList<DBPFIndexEntry>
    {
        private readonly List<DBPFIndexEntry> entries;

        public DBPFIndexCollection() : this(0)
        {
        }

        public DBPFIndexCollection(int count)
        {
            this.entries = new List<DBPFIndexEntry>(count);
        }

        public DBPFIndexEntry this[int index]
        {
            get => this.entries[index];
            set => this.entries[index] = value;
        }

        public int Count => this.entries.Count;

        public bool IsReadOnly => false;

        public void Add(DBPFIndexEntry item)
        {
            this.entries.Add(item);
        }

        public ReadOnlyCollection<DBPFIndexEntry> AsReadOnly() => new(this.entries);

        public void Clear()
        {
            this.entries.Clear();
        }

        public bool Contains(DBPFIndexEntry item)
        {
            return this.entries.Contains(item);
        }

        public void CopyTo(DBPFIndexEntry[] array, int arrayIndex)
        {
            this.entries.CopyTo(array, arrayIndex);
        }

        public DBPFIndexEntry? Find(uint type, uint group, uint instance)
        {
            for (int i = 0; i < this.entries.Count; i++)
            {
                DBPFIndexEntry index = this.entries[i];
                if (index.Type == type && index.Group == group && index.Instance == instance)
                {
                    return index;
                }
            }

            return null;
        }

        public IEnumerator<DBPFIndexEntry> GetEnumerator()
        {
            return this.entries.GetEnumerator();
        }

        public int IndexOf(DBPFIndexEntry item)
        {
            ArgumentNullException.ThrowIfNull(item);

            return IndexOf(item.Type, item.Group, item.Instance);
        }

        public int IndexOf(uint type, uint group, uint instance) => IndexOf(type, group, instance, 0);

        public int IndexOf(uint type, uint group, uint instance, int startIndex)
        {
            if (startIndex >= 0 && startIndex < this.entries.Count)
            {
                for (int i = startIndex; i < this.entries.Count; i++)
                {
                    DBPFIndexEntry index = this.entries[i];
                    if (index.Type == type && index.Group == group && index.Instance == instance)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public void Insert(int index, DBPFIndexEntry item)
        {
            this.entries.Insert(index, item);
        }

        public bool Remove(DBPFIndexEntry item)
        {
            return this.entries.Remove(item);
        }

        public void RemoveAll(Predicate<DBPFIndexEntry> predicate) => this.entries.RemoveAll(predicate);


        public void RemoveAt(int index)
        {
            this.entries.RemoveAt(index);
        }

        /// <summary>
        /// Sorts the collection in ascending order by the file location.
        /// </summary>
        public void SortByLocation() => this.entries.Sort(IndexLocationComparer.Instance);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.entries).GetEnumerator();
        }

        private sealed class IndexLocationComparer : IComparer<DBPFIndexEntry>
        {
            private IndexLocationComparer()
            {
            }

            public static IndexLocationComparer Instance { get; } = new IndexLocationComparer();

            public int Compare(DBPFIndexEntry? x, DBPFIndexEntry? y)
            {
                if (ReferenceEquals(x, y))
                {
                    return 0;
                }

                if (x is null)
                {
                    return -1;
                }

                if (y is null)
                {
                    return 1;
                }

                if (x.Location < y.Location)
                {
                    return -1;
                }
                else if (x.Location > y.Location)
                {
                    return 1;
                }

                // The file locations should never be equal.
                return 0;
            }
        }

    }
}
