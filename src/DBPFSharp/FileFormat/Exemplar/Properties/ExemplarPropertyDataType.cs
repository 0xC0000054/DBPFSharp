// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

namespace DBPFSharp.FileFormat.Exemplar.Properties
{
    /// <summary>
    /// The data type of an exemplar property.
    /// </summary>
    public enum ExemplarPropertyDataType : ushort
    {
        /// <summary>
        /// The data type is invalid.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// The data type is a Boolean.
        /// </summary>
        Boolean = 0xB00,

        /// <summary>
        /// The data type is an unsigned 8-bit integer.
        /// </summary>
        UInt8 = 0x100,

        /// <summary>
        /// The data type is an unsigned 16-bit integer.
        /// </summary>
        UInt16 = 0x200,

        /// <summary>
        /// The data type is an unsigned 32-bit integer.
        /// </summary>
        UInt32 = 0x300,

        /// <summary>
        /// The data type is an signed 32-bit integer.
        /// </summary>
        SInt32 = 0x700,

        /// <summary>
        /// The data type is an signed 64-bit integer.
        /// </summary>
        SInt64 = 0x800,

        /// <summary>
        /// The data type is a 32-bit floating point value.
        /// </summary>
        Float32 = 0x900,

        /// <summary>
        /// The data type is a string.
        /// </summary>
        String = 0xC00
    }
}
