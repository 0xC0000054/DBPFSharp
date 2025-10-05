// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.IO;

namespace DBPFSharp.FileFormat.Exemplar.Properties
{
    internal static class ExemplarPropertyHeader
    {
        internal readonly ref struct BinaryHeaderData(uint id,
                                                      ExemplarPropertyDataType dataType,
                                                      int repCount)
        {
            internal readonly uint id = id;
            internal readonly ExemplarPropertyDataType dataType = dataType;
            internal readonly int count = repCount;
        }

        internal readonly ref struct TextHeaderData(uint id,
                                                    ExemplarPropertyDataType dataType,
                                                    int repCount,
                                                    ReadOnlySpan<char> data)
        {
            internal readonly uint id = id;
            internal readonly ExemplarPropertyDataType dataType = dataType;
            internal readonly int count = repCount;
            internal readonly ReadOnlySpan<char> data = data;
        }

        internal static BinaryHeaderData ReadBinary(BinaryReader reader)
        {
            uint propertyID = reader.ReadUInt32();
            ExemplarPropertyDataType dataType = (ExemplarPropertyDataType)reader.ReadUInt16();
            ExemplarPropertyKeyType keyType = (ExemplarPropertyKeyType)reader.ReadUInt16();

            // Skip the unused byte.
            reader.BaseStream.Position++;

            int repCount = keyType switch
            {
                ExemplarPropertyKeyType.SingleValue => 1,
                ExemplarPropertyKeyType.Array => reader.ReadInt32(),
                _ => throw new DBPFException($"Unknown {nameof(ExemplarPropertyKeyType)} value."),
            };

            return new BinaryHeaderData(propertyID, dataType, repCount);
        }

        internal static TextHeaderData ReadText(ReadOnlySpan<char> span)
        {
            const string DataStartMarker = "}=";

            if (span.Length <= 11 || span[10] != ':')
            {
                throw new DBPFException("Invalid text exemplar property entry.");
            }

            uint propertyID = TextExemplarUtil.ParseHexNumberUInt32(span[..10]);

            ReadOnlySpan<char> data = span[11..];

            int dataStartIndex = data.IndexOf(DataStartMarker);

            if (dataStartIndex == -1)
            {
                throw new DBPFException("Invalid text exemplar property entry.");
            }

            data = data[(dataStartIndex + DataStartMarker.Length)..];

            Span<Range> ranges = stackalloc Range[3];

            int count = data.Split(ranges, ':');

            if (count != 3)
            {
                throw new DBPFException("Invalid text exemplar property entry.");
            }

            ExemplarPropertyDataType dataType = GetDataTypeFromText(data[ranges[0]]);
            int repCount = TextExemplarUtil.ParseHexNumberSInt32(data[ranges[1]]);

            if (repCount == 0)
            {
                // A repetition count of zero is treated as an array with one item.
                repCount = 1;
            }

            data = data[ranges[2]];

            if (!data.StartsWith("{", StringComparison.Ordinal)
                || !data.EndsWith("}", StringComparison.Ordinal))
            {
                throw new DBPFException("Invalid text exemplar property entry.");
            }

            data = data.TrimStart('{').TrimEnd('}');

            return new TextHeaderData(propertyID, dataType, repCount, data);
        }

        internal static void WriteBinary(BinaryWriter writer, uint id, ExemplarPropertyDataType dataType, int repCount)
        {
            // Strings are always arrays.
            bool isArray = dataType == ExemplarPropertyDataType.String || repCount > 1;

            writer.Write(id);
            writer.Write((ushort)dataType);
            writer.Write((ushort)(isArray ? ExemplarPropertyKeyType.Array : ExemplarPropertyKeyType.SingleValue));

            writer.Write((byte)0); // Unused byte or padding

            if (isArray)
            {
                writer.Write(repCount);
            }
        }

        private static ExemplarPropertyDataType GetDataTypeFromText(ReadOnlySpan<char> text)
        {
            if (text.Equals("Bool", StringComparison.Ordinal))
            {
                return ExemplarPropertyDataType.Boolean;
            }
            else if (text.Equals("Uint8", StringComparison.Ordinal))
            {
                return ExemplarPropertyDataType.UInt8;
            }
            else if (text.Equals("Uint16", StringComparison.Ordinal))
            {
                return ExemplarPropertyDataType.UInt16;
            }
            else if (text.Equals("Uint32", StringComparison.Ordinal))
            {
                return ExemplarPropertyDataType.UInt32;
            }
            else if (text.Equals("Sint32", StringComparison.Ordinal))
            {
                return ExemplarPropertyDataType.SInt32;
            }
            else if (text.Equals("Sint64", StringComparison.Ordinal))
            {
                return ExemplarPropertyDataType.SInt64;
            }
            else if (text.Equals("Float32", StringComparison.Ordinal))
            {
                return ExemplarPropertyDataType.Float32;
            }
            else if (text.Equals("String", StringComparison.Ordinal))
            {
                return ExemplarPropertyDataType.String;
            }
            else
            {
                throw new DBPFException($"Unknown text exemplar property type: {text.ToString()}.");
            }
        }
    }
}
