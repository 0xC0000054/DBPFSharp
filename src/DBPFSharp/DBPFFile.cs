// Copyright (c) 2023 Nicholas Hayes
// SPDX-License-Identifier: MIT

using DBPFSharp.Properties;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace DBPFSharp
{
    /// <summary>
    /// Encapsulates a DBPF file.
    /// </summary>
    public sealed class DBPFFile : IDisposable
    {
        private const uint CompressionDirectoryType = 0xe86b1eef;
        private const uint CompressionDirectoryGroup = 0xe86b1eef;
        private const uint CompressionDirectoryInstance = 0x286b1f03;

        private DBPFIndexCollection indices;
        private CompressionDirectory compressionDirectory;
        private Stream? stream;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DBPFFile"/> class.
        /// </summary>
        public DBPFFile()
        {
            this.Header = new DBPFHeader();
            this.indices = new DBPFIndexCollection();
            this.compressionDirectory = new CompressionDirectory();
            this.FileName = null;
            this.IsDirty = false;
            this.stream = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DBPFFile"/> class and loads the specified path.
        /// </summary>
        /// <param name="path">The path of the file to load.</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="DBPFException">
        /// The DBPF format version is not supported.
        /// -or-
        /// The size of the entry table is invalid.
        /// -or
        /// The header identifier does not equal DBPF.
        /// </exception>
        /// <exception cref="FileNotFoundException">The file specified in <paramref name="path"/> was not found.</exception>
        public DBPFFile(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            this.FileName = path;

            bool loaded = false;
            this.stream = new FileStream(path, FileMode.Open, FileAccess.Read);

            try
            {
                this.Header = new DBPFHeader(stream);

                int entryCount = checked((int)Header.Entries);

                this.indices = new DBPFIndexCollection(entryCount);
                this.compressionDirectory = new CompressionDirectory();

                this.stream.Seek(Header.IndexLocation, SeekOrigin.Begin);

                for (int i = 0; i < entryCount; i++)
                {
                    uint type = stream.ReadUInt32();
                    uint group = stream.ReadUInt32();
                    uint instance = stream.ReadUInt32();
                    uint location = stream.ReadUInt32();
                    uint size = stream.ReadUInt32();

                    this.indices.Add(new DBPFIndexEntry(type, group, instance, location, size));
                }

                DBPFIndexEntry? compressionDirectoryIndex = this.indices.Find(CompressionDirectoryType, CompressionDirectoryGroup, CompressionDirectoryInstance);
                if (compressionDirectoryIndex != null)
                {
                    stream.Seek(compressionDirectoryIndex.Location, SeekOrigin.Begin);

                    int recordCount = (int)(compressionDirectoryIndex.FileSize / CompressionDirectoryEntry.SizeOf);
                    compressionDirectory.Capacity = recordCount;

                    for (int i = 0; i < recordCount; i++)
                    {
                        uint type = stream.ReadUInt32();
                        uint group = stream.ReadUInt32();
                        uint instance = stream.ReadUInt32();
                        uint uncompressedSize = stream.ReadUInt32();

                        this.compressionDirectory.Add(new CompressionDirectoryEntry(type, group, instance, uncompressedSize));
                    }
                }

                this.indices.SortByLocation();
                loaded = true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // Close the stream if an Exception was thrown when the file was being loaded.
                if (!loaded && this.stream != null)
                {
                    this.stream.Dispose();
                    this.stream = null;
                }
            }
        }

        /// <summary>
        /// Gets the name of the loaded file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string? FileName { get; private set; }

        /// <summary>
        /// Gets the compression directory.
        /// </summary>
        /// <value>
        /// A collection containing the compression directory entries.
        /// </value>
        public ReadOnlyCollection<CompressionDirectoryEntry> CompressionDirectory => this.compressionDirectory.AsReadOnly();

        /// <summary>
        /// Gets the <see cref="DBPFHeader"/> of the file.
        /// </summary>
        /// <value>
        /// The file header.
        /// </value>
        public DBPFHeader Header { get; private set; }

        /// <summary>
        /// Gets the <see cref="DBPFIndexEntry"/> collection from the file.
        /// </summary>
        /// <value>
        /// A collection containing the file index entries.
        /// </value>
        public ReadOnlyCollection<DBPFIndexEntry> Index => this.indices.AsReadOnly();

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirty { get; private set; }

        /// <summary>
        /// Add an entry to the file.
        /// </summary>
        /// <param name="type">he TGI group id of the file entry.</param>
        /// <param name="group">The TGI group id of the file entry.</param>
        /// <param name="instance">The TGI instance id of the file entry.</param>
        /// <param name="data">The item data.</param>
        /// <param name="compress">
        /// <see langword="true"/> if the data should be compressed; otherwise, <see langword="false"/>.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null</exception>
        /// <exception cref="ObjectDisposedException">The method is called after the file has been closed.</exception>
        public void Add(uint type, uint group, uint instance, byte[] data, bool compress)
        {
            ArgumentNullException.ThrowIfNull(data);
            VerifyNotDisposed();

            DBPFEntry item = new(data, compress);

            this.indices.Add(new DBPFIndexEntry(type, group, instance, item));

            this.IsDirty = true;
        }

        /// <summary>
        /// Closes the current file.
        /// </summary>
        public void Close() => Dispose();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;

                if (this.stream != null)
                {
                    this.stream.Dispose();
                    this.stream = null;
                }

                this.FileName = null;
            }
        }

        /// <summary>
        /// Loads an entry from the file.
        /// </summary>
        /// <param name="type">The TGI type id to load.</param>
        /// <param name="group">The TGI group id to load.</param>
        /// <param name="instance">The TGI instance id to load.</param>
        /// <returns>The loaded file entry.</returns>
        /// <exception cref="DBPFException">The specified entry does not exist in the file</exception>
        /// <exception cref="ObjectDisposedException">The method is called after the file has been disposed.</exception>
        public DBPFEntry GetEntry(uint type, uint group, uint instance)
        {
            VerifyNotDisposed();

            DBPFIndexEntry? index = this.indices.Find(type, group, instance);

            if (index is null)
            {
                throw new DBPFException(Resources.SpecifiedIndexDoesNotExist);
            }

            DBPFEntry? entry = index.Entry;

            if (entry is null)
            {
                this.stream!.Seek(index.Location, SeekOrigin.Begin);
                byte[] data = GC.AllocateUninitializedArray<byte>(checked((int)index.FileSize));

                stream.ReadExactly(data, 0, data.Length);

                bool compressed = this.compressionDirectory.Contains(index);

                entry = new DBPFEntry(data, compressed);
            }

            return entry;
        }

        /// <summary>
        /// Removes the specified file from the file
        /// </summary>
        /// <param name="type">The TGI type id to remove</param>
        /// <param name="group">The TGI group id to remove</param>
        /// <param name="instance">The TGI instance id to remove</param>
        /// <exception cref="ObjectDisposedException">The method is called after the file has been closed.</exception>
        public void Remove(uint type, uint group, uint instance)
        {
            VerifyNotDisposed();

            int index = this.indices.IndexOf(type, group, instance);

            if (index != -1)
            {
                // Loop to remove any additional files with the same TGI, this should never happen but check anyway.
                do
                {
                    this.indices[index].IndexState = DatIndexState.Deleted;
                    index = this.indices.IndexOf(type, group, instance, index + 1);
                }
                while (index >= 0);

                this.IsDirty = true;
            }
        }

        /// <summary>
        /// Saves the currently loaded file
        /// </summary>
        /// <exception cref="InvalidOperationException">The current file is not loaded from an existing file.</exception>
        /// <exception cref="ObjectDisposedException">The method is called after the file has been closed.</exception>
        public void Save()
        {
            if (string.IsNullOrEmpty(this.FileName))
            {
                throw new InvalidOperationException(Resources.NotAnExistingFile);
            }

            Save(this.FileName);
        }

        /// <summary>
        /// Saves the file to the specified fileName.
        /// </summary>
        /// <param name="fileName">The fileName to save as</param>
        /// <exception cref="ArgumentNullException"><paramref name="fileName" /> is null.</exception>
        /// <exception cref="ObjectDisposedException">The method is called after the file has been closed.</exception>
        public void Save(string fileName)
        {
            ArgumentNullException.ThrowIfNull(fileName);
            VerifyNotDisposed();

            string saveFileName = fileName;
            if (fileName.Equals(this.FileName, StringComparison.OrdinalIgnoreCase) && this.stream != null)
            {
                // When overwriting an existing file, we save to a temporary file first and then use File.Copy to overwrite it if the save was successful.
                saveFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            }

            using (FileStream output = new(saveFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                DBPFHeader head = this.Header;

                head.Save(output);

                TrimDeletedItems();

                DBPFIndexCollection saveIndices = new(this.indices.Count + 2);
                CompressionDirectory compDirs = new();
                long location = 0;
                uint size = 0;

                for (int i = 0; i < this.indices.Count; i++)
                {
                    DBPFIndexEntry index = indices[i];
                    DatIndexState state = index.IndexState;

                    switch (state)
                    {
                        case DatIndexState.New:
                        case DatIndexState.Modified:

                            DBPFEntry entry = index.Entry!;

                            location = output.Position;
                            size = entry.Save(output);
                            if (entry.IsCompressed)
                            {
                                compDirs.Add(new CompressionDirectoryEntry(index.Type, index.Group, index.Instance, entry.RawDataLength));
                            }

                            break;
                        case DatIndexState.Normal:

                            location = output.Position;
                            size = index.FileSize;

#if DEBUG
                            System.Diagnostics.Debug.WriteLine(string.Format("Existing file Index: {0} Type: 0x{1:X8}", i, index.Type));
#endif
                            this.stream!.Seek(index.Location, SeekOrigin.Begin);

                            int dataSize = checked((int)size);

                            byte[] buffer = new byte[dataSize];

                            stream.ReadExactly(buffer, 0, dataSize);

                            output.Write(buffer, 0, dataSize);

                            if (this.compressionDirectory != null)
                            {
                                CompressionDirectoryEntry? compressionDirectoryEntry = this.compressionDirectory.Find(index.Type, index.Group, index.Instance);
                                if (compressionDirectoryEntry != null)
                                {
                                    compDirs.Add(compressionDirectoryEntry);
                                }
                            }
                            break;

                        case DatIndexState.Deleted:
                        default:
                            // Unknown entry state or deleted file.
#if DEBUG
                            System.Diagnostics.Debug.WriteLine(string.Format("Index # {0} has unsupported state {0}.\n", i, state));
#endif
                            continue;
                    }

                    saveIndices.Add(new DBPFIndexEntry(index.Type,
                                                       index.Group,
                                                       index.Instance,
                                                       checked((uint)location),
                                                       size));
                }

                if (compDirs.Count > 0)
                {
                    location = output.Position;

                    int count = compDirs.Count;
                    for (int i = 0; i < count; i++)
                    {
                        compDirs[i].Save(output);
                    }

                    size = (uint)(compDirs.Count * CompressionDirectoryEntry.SizeOf);
                    saveIndices.Add(new DBPFIndexEntry(CompressionDirectoryType,
                                                       CompressionDirectoryGroup,
                                                       CompressionDirectoryInstance,
                                                       checked((uint)location),
                                                       size));
                }

                uint entryCount = (uint)saveIndices.Count;
                location = output.Position;
                size = entryCount * DBPFIndexEntry.SizeOf;
                for (int i = 0; i < saveIndices.Count; i++)
                {
                    saveIndices[i].Save(output);
                }
                head.Entries = entryCount;
                head.IndexSize = size;
                head.IndexLocation = (uint)location;
                head.DateModified = DateTimeOffset.Now;

                output.Position = 0L;
                head.Save(output);

                saveIndices.SortByLocation();

                this.Header = head;
                this.indices = saveIndices;
                this.compressionDirectory = compDirs;
                this.FileName = fileName;

                this.IsDirty = false;
            }

            if (!saveFileName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
            {
                // Close the old file and copy the new file in its place.
                this.stream?.Dispose();
                this.stream = null;

                File.Copy(saveFileName, fileName, overwrite: true);
                File.Delete(saveFileName);

                // Open the new file to prevent a NullRefrenceException if GetEntry is called.
                this.stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            }
        }

        /// <summary>
        /// Trims the deleted items from the entry collection.
        /// </summary>
        private void TrimDeletedItems()
        {
            if (this.indices.Count > 0)
            {
                this.indices.RemoveAll(new Predicate<DBPFIndexEntry>(index => (index.IndexState == DatIndexState.Deleted || index.Type == CompressionDirectoryType)));
            }
        }

        private void VerifyNotDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(DBPFFile));
            }
        }
    }
}
