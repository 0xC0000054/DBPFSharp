// Copyright (c) 2023, 2025, 2026 Nicholas Hayes
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
                                                    ReadOnlySpan<byte> data)
        {
            internal readonly uint id = id;
            internal readonly ExemplarPropertyDataType dataType = dataType;
            internal readonly int count = repCount;
            internal readonly ReadOnlySpan<byte> data = data;
        }

        internal static BinaryHeaderData ReadBinary(ref SpanBinaryReader reader)
        {
            uint propertyID = reader.ReadUInt32();
            ExemplarPropertyDataType dataType = (ExemplarPropertyDataType)reader.ReadUInt16();
            ExemplarPropertyKeyType keyType = (ExemplarPropertyKeyType)reader.ReadUInt16();

            // Skip the unused byte.
            reader.Position++;

            int repCount = keyType switch
            {
                ExemplarPropertyKeyType.SingleValue => 1,
                ExemplarPropertyKeyType.Array => reader.ReadInt32(),
                _ => throw new DBPFException($"Unknown {nameof(ExemplarPropertyKeyType)} value."),
            };

            return new BinaryHeaderData(propertyID, dataType, repCount);
        }

        internal static TextHeaderData ReadText(ReadOnlySpan<byte> span)
        {
            ReadOnlySpan<byte> DataStartMarker = "}="u8;

            if (span.Length <= 11 || span[10] != ':')
            {
                throw new DBPFException("Invalid text exemplar property entry.");
            }

            uint propertyID = TextExemplarUtil.ParseHexNumberUInt32(span[..10]);

            ReadOnlySpan<byte> data = span[11..];

            int dataStartIndex = data.IndexOf(DataStartMarker);

            if (dataStartIndex == -1)
            {
                throw new DBPFException("Invalid text exemplar property entry.");
            }

            data = data[(dataStartIndex + DataStartMarker.Length)..];

            Span<Range> ranges = stackalloc Range[3];

            if (!TextExemplarUtil.Split(data, (byte)':', ranges))
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

            if (!data.StartsWith((byte)'{')
                || !data.EndsWith((byte)'}'))
            {
                throw new DBPFException("Invalid text exemplar property entry.");
            }

            data = data.TrimStart((byte)'{').TrimEnd((byte)'}');

            if (dataType == ExemplarPropertyDataType.String)
            {
                // Trim the double quotes from the string data.
                data = data.Trim((byte)'"');
            }

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

        private static ExemplarPropertyDataType GetDataTypeFromText(ReadOnlySpan<byte> text)
        {
            if (text.SequenceEqual("Bool"u8))
            {
                return ExemplarPropertyDataType.Boolean;
            }
            else if (text.SequenceEqual("Uint8"u8))
            {
                return ExemplarPropertyDataType.UInt8;
            }
            else if (text.SequenceEqual("Uint16"u8))
            {
                return ExemplarPropertyDataType.UInt16;
            }
            else if (text.SequenceEqual("Uint32"u8))
            {
                return ExemplarPropertyDataType.UInt32;
            }
            else if (text.SequenceEqual("Sint32"u8))
            {
                return ExemplarPropertyDataType.SInt32;
            }
            else if (text.SequenceEqual("Sint64"u8))
            {
                return ExemplarPropertyDataType.SInt64;
            }
            else if (text.SequenceEqual("Float32"u8))
            {
                return ExemplarPropertyDataType.Float32;
            }
            else if (text.SequenceEqual("String"u8))
            {
                return ExemplarPropertyDataType.String;
            }
            else
            {
                throw new DBPFException($"Unknown text exemplar property type: {System.Text.Encoding.ASCII.GetString(text)}.");
            }
        }
    }
}
