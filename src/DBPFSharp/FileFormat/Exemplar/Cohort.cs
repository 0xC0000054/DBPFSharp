// Copyright (c) 2023, 2025, 2026 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;

namespace DBPFSharp.FileFormat.Exemplar
{
    /// <summary>
    /// Represents a cohort.
    /// </summary>
    /// <seealso cref="ExemplarBase" />
    public sealed class Cohort : ExemplarBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cohort"/> class.
        /// </summary>
        public Cohort()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cohort"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        /// <excpetion cref="DBPFException"></excpetion>
        public Cohort(byte[] data) : base(data)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cohort"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public Cohort(ReadOnlySpan<byte> data) : base(data)
        {
        }

        private protected override ReadOnlySpan<byte> BinaryHeaderSignature => "CQZB1###"u8;

        private protected override ReadOnlySpan<byte> TextHeaderSignature => "CQZT1###"u8;
    }
}
