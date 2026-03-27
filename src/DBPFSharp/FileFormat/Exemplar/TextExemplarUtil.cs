// Copyright (c) 2023, 2025, 2026 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DBPFSharp.FileFormat.Exemplar
{
    internal static class TextExemplarUtil
    {
        private const byte ArrayDelimiter = (byte)',';

        internal static TGI ParseParentCohort(ReadOnlySpan<byte> line)
        {
            ReadOnlySpan<byte> Prefix = "ParentCohort=Key:{"u8;

            if (!line.StartsWith(Prefix) || !line.EndsWith((byte)'}'))
            {
                throw new DBPFException("Invalid text exemplar ParentCohort property.");
            }

            ReadOnlySpan<byte> data = line.Slice(Prefix.Length, line.Length - Prefix.Length - 1);

            Span<Range> ranges = stackalloc Range[3];

            if (!Split(data, ArrayDelimiter, ranges))
            {
                throw new DBPFException("Invalid text exemplar ParentCohort property.");
            }

            // Text exemplars use the order: group, instance, type.

            uint group = ParseHexNumberUInt32(data[ranges[0]]);
            uint instance = ParseHexNumberUInt32(data[ranges[1]]);
            uint type = ParseHexNumberUInt32(data[ranges[2]]);

            return new TGI(type, group, instance);
        }

        internal static int ParsePropertyCount(ReadOnlySpan<byte> line)
        {
            ReadOnlySpan<byte> Prefix = "PropCount="u8;

            if (!line.StartsWith(Prefix))
            {
                throw new DBPFException("Invalid text exemplar PropCount property.");
            }

            return ParseHexNumberSInt32(line[Prefix.Length..]);
        }

        internal static List<bool> ParseBooleanArray(ReadOnlySpan<byte> span, int count)
        {
            List<bool> result = new(count);

            if (span.IsEmpty)
            {
                result.Add(false);
            }
            else
            {
                if (count > 1)
                {
                    foreach (var segment in span.Split(ArrayDelimiter))
                    {
                        result.Add(ParseBoolean(span[segment]));
                    }
                }
                else
                {
                    result.Add(ParseBoolean(span));
                }
            }

            return result;
        }

        internal static List<float> ParseFloat32Array(ReadOnlySpan<byte> span, int count)
        {
            List<float> result = new(count);

            if (span.IsEmpty)
            {
                result.Add(0);
            }
            else
            {
                if (count > 1)
                {
                    foreach (var segment in span.Split(ArrayDelimiter))
                    {
                        result.Add(ParseFloat32(span[segment]));
                    }
                }
                else
                {
                    result.Add(ParseFloat32(span));
                }
            }

            return result;
        }

        internal static List<int> ParseSInt32Array(ReadOnlySpan<byte> span, int count)
        {
            List<int> result = new(count);

            if (span.IsEmpty)
            {
                result.Add(0);
            }
            else
            {
                if (count > 1)
                {
                    foreach (var segment in span.Split(ArrayDelimiter))
                    {
                        result.Add(ParseHexNumberSInt32(span[segment]));
                    }
                }
                else
                {
                    result.Add(ParseHexNumberSInt32(span));
                }
            }

            return result;
        }

        internal static List<long> ParseSInt64Array(ReadOnlySpan<byte> span, int count)
        {
            List<long> result = new(count);

            if (span.IsEmpty)
            {
                result.Add(0);
            }
            else
            {
                if (count > 1)
                {
                    foreach (var segment in span.Split(ArrayDelimiter))
                    {
                        result.Add(ParseHexNumberSInt64(span[segment]));
                    }
                }
                else
                {
                    result.Add(ParseHexNumberSInt64(span));
                }
            }

            return result;
        }

        internal static List<byte> ParseUInt8Array(ReadOnlySpan<byte> span, int count)
        {
            List<byte> result = new(count);

            if (span.IsEmpty)
            {
                result.Add(0);
            }
            else
            {
                if (count > 1)
                {
                    foreach (var segment in span.Split(ArrayDelimiter))
                    {
                        result.Add(ParseHexNumberUInt8(span[segment]));
                    }
                }
                else
                {
                    result.Add(ParseHexNumberUInt8(span));
                }
            }

            return result;
        }

        internal static List<ushort> ParseUInt16Array(ReadOnlySpan<byte> span, int count)
        {
            List<ushort> result = new(count);

            if (span.IsEmpty)
            {
                result.Add(0);
            }
            else
            {
                if (count > 1)
                {
                    foreach (var segment in span.Split(ArrayDelimiter))
                    {
                        result.Add(ParseHexNumberUInt16(span[segment]));
                    }
                }
                else
                {
                    result.Add(ParseHexNumberUInt16(span));
                }
            }

            return result;
        }

        internal static List<uint> ParseUInt32Array(ReadOnlySpan<byte> span, int count)
        {
            List<uint> result = new(count);

            if (span.IsEmpty)
            {
                result.Add(0);
            }
            else
            {
                if (count > 1)
                {
                    foreach (var segment in span.Split(ArrayDelimiter))
                    {
                        result.Add(ParseHexNumberUInt32(span[segment]));
                    }
                }
                else
                {
                    result.Add(ParseHexNumberUInt32(span));
                }
            }

            return result;
        }

        internal static bool ParseBoolean(ReadOnlySpan<byte> span)
        {
            if (!Utf8Parser.TryParse(span, out bool value, out int _))
            {
                throw new FormatException($"Failed to parse the text exemplar value {Encoding.ASCII.GetString(span)} as a Boolean.");
            }

            return value;
        }

        internal static float ParseFloat32(ReadOnlySpan<byte> span)
        {
            return float.Parse(span,
                               NumberStyles.AllowThousands | NumberStyles.Float,
                               CultureInfo.InvariantCulture);
        }

        internal static int ParseHexNumberSInt32(ReadOnlySpan<byte> span)
        {
            return int.Parse(StripHexPrefix(span),
                             NumberStyles.HexNumber,
                             CultureInfo.InvariantCulture);
        }

        internal static long ParseHexNumberSInt64(ReadOnlySpan<byte> span)
        {
            return long.Parse(StripHexPrefix(span),
                              NumberStyles.HexNumber,
                              CultureInfo.InvariantCulture);
        }

        internal static byte ParseHexNumberUInt8(ReadOnlySpan<byte> span)
        {
            return byte.Parse(StripHexPrefix(span),
                              NumberStyles.HexNumber,
                              CultureInfo.InvariantCulture);
        }
        internal static ushort ParseHexNumberUInt16(ReadOnlySpan<byte> span)
        {
            return ushort.Parse(StripHexPrefix(span),
                                NumberStyles.HexNumber,
                                CultureInfo.InvariantCulture);
        }

        internal static uint ParseHexNumberUInt32(ReadOnlySpan<byte> span)
        {
            return uint.Parse(StripHexPrefix(span),
                              NumberStyles.HexNumber,
                              CultureInfo.InvariantCulture);
        }

        internal static bool Split(ReadOnlySpan<byte> source, byte separator, Span<Range> destination)
        {
            int index = 0;

            foreach (Range range in source.Split(separator))
            {
                if (index >= destination.Length)
                {
                    return false;
                }

                destination[index++] = range;
            }

            return index == destination.Length;
        }

        private static ReadOnlySpan<byte> StripHexPrefix(ReadOnlySpan<byte> span)
        {
            ReadOnlySpan<byte> result = span;

            if (span.Length > 2 && span[0] == (byte)'0')
            {
                if (span[1] is ((byte)'x') or ((byte)'X'))
                {
                    result = span[2..];
                }
            }

            return result;
        }
    }
}
