// Copyright (c) 2023, 2025, 2026 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;

namespace DBPFSharp
{
    internal ref struct SpanAsciiTextReader
    {
        private readonly ReadOnlySpan<byte> bytes;
        private int offset;

        public SpanAsciiTextReader(ReadOnlySpan<byte> bytes)
        {
            this.bytes = bytes;
            this.offset = 0;
        }

        public ReadOnlySpan<byte> ReadLine()
        {
            ReadOnlySpan<byte> line;

            ReadOnlySpan<byte> remaining = this.bytes[this.offset..];

            int indexOfNewLine = remaining.IndexOfAny((byte)'\r', (byte)'\n');

            if (indexOfNewLine == -1)
            {
                line = remaining;
                this.offset += remaining.Length;
            }
            else
            {
                line = remaining[..indexOfNewLine];

                byte matchedChar = remaining[indexOfNewLine];
                this.offset += indexOfNewLine + 1;

                if (matchedChar == (byte)'\r')
                {                
                    // If we found '\r', consume any immediately following '\n'.
                    if (this.offset < this.bytes.Length && this.bytes[this.offset] == (byte)'\n')
                    {
                        this.offset++;
                    }
                }
            }

            return line;
        }
    }
}
