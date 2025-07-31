// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Runtime.Serialization;

namespace DBPFSharp
{
    /// <summary>
    /// The exception that is thrown when an error occurs with a DBPF file.
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    public sealed class DBPFException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DBPFException"/> class.
        /// </summary>
        public DBPFException() : base("A DBPFException has occurred")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DBPFException"/> class with the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public DBPFException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DBPFException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public DBPFException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
