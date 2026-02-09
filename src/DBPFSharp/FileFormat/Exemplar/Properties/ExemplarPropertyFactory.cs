// Copyright (c) 2023, 2025, 2026 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;

namespace DBPFSharp.FileFormat.Exemplar.Properties
{
    internal static class ExemplarPropertyFactory
    {
        internal static ExemplarProperty CreateFromBinary(ref SpanBinaryReader reader)
        {
            ExemplarPropertyHeader.BinaryHeaderData header = ExemplarPropertyHeader.ReadBinary(ref reader);

            return CreateBinaryProperty(header.id, header.dataType, ref reader, header.count);
        }

        internal static ExemplarProperty CreateFromText(ReadOnlySpan<byte> line)
        {
            ExemplarPropertyHeader.TextHeaderData header = ExemplarPropertyHeader.ReadText(line);

            return CreateTextProperty(header.id, header.dataType, header.data, header.count);
        }

        private static ExemplarProperty CreateBinaryProperty(uint id,
                                                             ExemplarPropertyDataType dataType,
                                                             ref SpanBinaryReader reader,
                                                             int count)
        {
            return dataType switch
            {
                ExemplarPropertyDataType.Boolean => new ExemplarPropertyBoolean(id, ref reader, count),
                ExemplarPropertyDataType.UInt8 => new ExemplarPropertyUInt8(id, ref reader, count),
                ExemplarPropertyDataType.UInt16 => new ExemplarPropertyUInt16(id, ref reader, count),
                ExemplarPropertyDataType.UInt32 => new ExemplarPropertyUInt32(id, ref reader, count),
                ExemplarPropertyDataType.SInt32 => new ExemplarPropertySInt32(id, ref reader, count),
                ExemplarPropertyDataType.SInt64 => new ExemplarPropertySInt64(id, ref reader, count),
                ExemplarPropertyDataType.Float32 => new ExemplarPropertyFloat32(id, ref reader, count),
                ExemplarPropertyDataType.String => new ExemplarPropertyString(id, ref reader, count),
                _ => throw new InvalidOperationException($"Unsupported {nameof(ExemplarPropertyDataType)} value: {dataType}."),
            };
        }

        private static ExemplarProperty CreateTextProperty(uint id,
                                                           ExemplarPropertyDataType dataType,
                                                           ReadOnlySpan<byte> data,
                                                           int count)
        {
            return dataType switch
            {
                ExemplarPropertyDataType.Boolean => new ExemplarPropertyBoolean(id, data, count),
                ExemplarPropertyDataType.UInt8 => new ExemplarPropertyUInt8(id, data, count),
                ExemplarPropertyDataType.UInt16 => new ExemplarPropertyUInt16(id, data, count),
                ExemplarPropertyDataType.UInt32 => new ExemplarPropertyUInt32(id, data, count),
                ExemplarPropertyDataType.SInt32 => new ExemplarPropertySInt32(id, data, count),
                ExemplarPropertyDataType.SInt64 => new ExemplarPropertySInt64(id, data, count),
                ExemplarPropertyDataType.Float32 => new ExemplarPropertyFloat32(id, data, count),
                ExemplarPropertyDataType.String => new ExemplarPropertyString(id, data),
                _ => throw new InvalidOperationException($"Unsupported {nameof(ExemplarPropertyDataType)} value: {dataType}."),
            };
        }
    }
}
