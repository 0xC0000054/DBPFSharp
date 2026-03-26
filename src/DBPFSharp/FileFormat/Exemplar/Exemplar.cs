// Copyright (c) 2023, 2025, 2026 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;

namespace DBPFSharp.FileFormat.Exemplar
{
    /// <summary>
    /// Represents an exemplar.
    /// </summary>
    /// <seealso cref="ExemplarBase" />
    public sealed class Exemplar : ExemplarBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Exemplar"/> class.
        /// </summary>
        public Exemplar()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Exemplar"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        public Exemplar(byte[] data) : base(data)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Exemplar"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public Exemplar(ReadOnlySpan<byte> data) : base(data)
        {
        }

        private protected override ReadOnlySpan<byte> BinaryHeaderSignature => "EQZB1###"u8;

        private protected override ReadOnlySpan<byte> TextHeaderSignature => "EQZT1###"u8;
    }
}
