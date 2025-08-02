// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Globalization;

namespace DBPFSharp.FileFormat.Exemplar
{
    internal static class TextExemplarUtil
    {
        private const char ArrayDelimiter = ',';

        internal static TGI ParseParentCohort(ReadOnlySpan<char> line)
        {
            const string Prefix = "ParentCohort=Key:{";
            const string Suffix = "}";

            if (!line.StartsWith(Prefix) || !line.EndsWith(Suffix))
            {
                throw new DBPFException("Invalid text exemplar ParentCohort property.");
            }

            ReadOnlySpan<char> data = line.Slice(Prefix.Length, line.Length - Prefix.Length - 1);

            Span<Range> ranges = stackalloc Range[3];

            int count = data.Split(ranges, ArrayDelimiter);

            if (count != 3)
            {
                throw new DBPFException("Invalid text exemplar ParentCohort property.");
            }

            // Text exemplars use the order: group, instance, type.

            uint group = ParseHexNumberUInt32(data[ranges[0]]);
            uint instance = ParseHexNumberUInt32(data[ranges[1]]);
            uint type = ParseHexNumberUInt32(data[ranges[2]]);

            return new TGI(type, group, instance);
        }

        internal static int ParsePropertyCount(ReadOnlySpan<char> line)
        {
            const string Prefix = "PropCount=";

            if (!line.StartsWith(Prefix))
            {
                throw new DBPFException("Invalid text exemplar PropCount property.");
            }

            return ParseHexNumberSInt32(line[Prefix.Length..]);
        }

        internal static List<bool> ParseBooleanArray(ReadOnlySpan<char> span, int count)
        {
            List<bool> result = new(count);

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

            return result;
        }

        internal static List<float> ParseFloat32Array(ReadOnlySpan<char> span, int count)
        {
            List<float> result = new(count);

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

            return result;
        }

        internal static List<int> ParseSInt32Array(ReadOnlySpan<char> span, int count)
        {
            List<int> result = new(count);

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

            return result;
        }

        internal static List<long> ParseSInt64Array(ReadOnlySpan<char> span, int count)
        {
            List<long> result = new(count);

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

            return result;
        }

        internal static List<byte> ParseUInt8Array(ReadOnlySpan<char> span, int count)
        {
            List<byte> result = new(count);

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

            return result;
        }

        internal static List<ushort> ParseUInt16Array(ReadOnlySpan<char> span, int count)
        {
            List<ushort> result = new(count);

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

            return result;
        }

        internal static List<uint> ParseUInt32Array(ReadOnlySpan<char> span, int count)
        {
            List<uint> result = new(count);

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

            return result;
        }

        internal static bool ParseBoolean(ReadOnlySpan<char> span)
        {
            return bool.Parse(span);
        }

        internal static float ParseFloat32(ReadOnlySpan<char> span)
        {
            return float.Parse(span,
                               NumberStyles.AllowThousands | NumberStyles.Float,
                               CultureInfo.InvariantCulture);
        }

        internal static int ParseHexNumberSInt32(ReadOnlySpan<char> span)
        {
            return int.Parse(StripHexPrefix(span),
                             NumberStyles.HexNumber,
                             CultureInfo.InvariantCulture);
        }

        internal static long ParseHexNumberSInt64(ReadOnlySpan<char> span)
        {
            return long.Parse(StripHexPrefix(span),
                              NumberStyles.HexNumber,
                              CultureInfo.InvariantCulture);
        }

        internal static byte ParseHexNumberUInt8(ReadOnlySpan<char> span)
        {
            return byte.Parse(StripHexPrefix(span),
                              NumberStyles.HexNumber,
                              CultureInfo.InvariantCulture);
        }
        internal static ushort ParseHexNumberUInt16(ReadOnlySpan<char> span)
        {
            return ushort.Parse(StripHexPrefix(span),
                                NumberStyles.HexNumber,
                                CultureInfo.InvariantCulture);
        }

        internal static uint ParseHexNumberUInt32(ReadOnlySpan<char> span)
        {
            return uint.Parse(StripHexPrefix(span),
                              NumberStyles.HexNumber,
                              CultureInfo.InvariantCulture);
        }

        internal static string ParseString(ReadOnlySpan<char> span)
        {
            return span.Trim('"').ToString();
        }

        private static ReadOnlySpan<char> StripHexPrefix(ReadOnlySpan<char> span)
        {
            return span.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? span[2..] : span;
        }
    }
}
