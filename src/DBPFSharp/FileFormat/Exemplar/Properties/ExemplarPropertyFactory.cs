// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.IO;

namespace DBPFSharp.FileFormat.Exemplar.Properties
{
    internal static class ExemplarPropertyFactory
    {
        internal static ExemplarProperty Create(BinaryReader reader)
        {
            ExemplarPropertyHeader.BinaryHeaderData header = ExemplarPropertyHeader.ReadBinary(reader);

            return CreateBinaryProperty(header.id, header.dataType, reader, header.count);
        }

        internal static ExemplarProperty Create(ReadOnlySpan<char> line)
        {
            ExemplarPropertyHeader.TextHeaderData header = ExemplarPropertyHeader.ReadText(line);

            return CreateTextProperty(header.id, header.dataType, header.data, header.count);
        }

        private static ExemplarProperty CreateBinaryProperty(uint id,
                                                             ExemplarPropertyDataType dataType,
                                                             BinaryReader reader,
                                                             int count)
        {
            return dataType switch
            {
                ExemplarPropertyDataType.Boolean => new ExemplarPropertyBoolean(id, reader, count),
                ExemplarPropertyDataType.UInt8 => new ExemplarPropertyUInt8(id, reader, count),
                ExemplarPropertyDataType.UInt16 => new ExemplarPropertyUInt16(id, reader, count),
                ExemplarPropertyDataType.UInt32 => new ExemplarPropertyUInt32(id, reader, count),
                ExemplarPropertyDataType.SInt32 => new ExemplarPropertySInt32(id, reader, count),
                ExemplarPropertyDataType.SInt64 => new ExemplarPropertySInt64(id, reader, count),
                ExemplarPropertyDataType.Float32 => new ExemplarPropertyFloat32(id, reader, count),
                ExemplarPropertyDataType.String => new ExemplarPropertyString(id, reader, count),
                _ => throw new InvalidOperationException($"Unsupported {nameof(ExemplarPropertyDataType)} value: {dataType}."),
            };
        }

        private static ExemplarProperty CreateTextProperty(uint id,
                                                           ExemplarPropertyDataType dataType,
                                                           ReadOnlySpan<char> data,
                                                           int count)
        {
            return dataType switch
            {
                ExemplarPropertyDataType.Boolean => new ExemplarPropertyBoolean(id, TextExemplarUtil.ParseBooleanArray(data, count)),
                ExemplarPropertyDataType.UInt8 => new ExemplarPropertyUInt8(id, TextExemplarUtil.ParseUInt8Array(data, count)),
                ExemplarPropertyDataType.UInt16 => new ExemplarPropertyUInt16(id, TextExemplarUtil.ParseUInt16Array(data, count)),
                ExemplarPropertyDataType.UInt32 => new ExemplarPropertyUInt32(id, TextExemplarUtil.ParseUInt32Array(data, count)),
                ExemplarPropertyDataType.SInt32 => new ExemplarPropertySInt32(id, TextExemplarUtil.ParseSInt32Array(data, count)),
                ExemplarPropertyDataType.SInt64 => new ExemplarPropertySInt64(id, TextExemplarUtil.ParseSInt64Array(data, count)),
                ExemplarPropertyDataType.Float32 => new ExemplarPropertyFloat32(id, TextExemplarUtil.ParseFloat32Array(data, count)),
                ExemplarPropertyDataType.String => new ExemplarPropertyString(id, TextExemplarUtil.ParseString(data)),
                _ => throw new InvalidOperationException($"Unsupported {nameof(ExemplarPropertyDataType)} value: {dataType}."),
            };
        }
    }
}
