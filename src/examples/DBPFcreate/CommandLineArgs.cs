// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using Mono.Options;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DBPFcreate
{
    internal sealed class CommandLineArgs
    {
        public CommandLineArgs(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                Compress = false;
                string tgi = string.Empty;
                string outputFile = string.Empty;
                bool compress = false;

                OptionSet options = new()
                {
                    { "TGI=", (string value) => tgi = value },
                    { "c|compress", (string value) => compress = value != null },
                    { "o|output-file=", (string value) => outputFile = value }
                };

                List<string> remainingArgs = options.Parse(args);

                Compress = compress;

                if (TryParseTGIString(tgi, out uint type, out uint group, out uint instance))
                {
                    Type = type;
                    Group = group;
                    Instance = instance;
                }
                else
                {
                    throw new ArgumentException("Invalid TGI string. It should be 3 hexadecimal numbers separated by commas.");
                }

                if (!string.IsNullOrWhiteSpace(outputFile))
                {
                    OutputFile = outputFile;
                }
                else
                {
                    throw new ArgumentException("Missing the output file path.");
                }

                if (remainingArgs.Count == 1)
                {
                    InputFile = remainingArgs[0];
                }
                else
                {
                    if (remainingArgs.Count > 1)
                    {
                        throw new ArgumentException("Unknown command line argument.");
                    }
                    else
                    {
                        throw new ArgumentException("Missing the input file.");
                    }
                }
            }
        }

        public uint Type { get; }

        public uint Group { get; }

        public uint Instance { get; }

        public bool Compress { get; }

        public string? InputFile { get; }

        public string? OutputFile { get; }

        private static bool TryParseTGIString(string value,
                                              out uint type,
                                              out uint group,
                                              out uint instance)
        {
            type = 0;
            group = 0;
            instance = 0;

            bool result = false;

            if (!string.IsNullOrWhiteSpace(value))
            {
                string[] parts = value.Split(',');

                result = parts.Length == 3
                      && ParseHexNumber(parts[0], out type)
                      && ParseHexNumber(parts[1], out group)
                      && ParseHexNumber(parts[2], out instance);
            }

            return result;
        }

        private static bool ParseHexNumber(ReadOnlySpan<char> chars, out uint value)
        {
            // TryParse returns false if the hexadecimal number starts with a 0x or 0X prefix.
            if (chars.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                chars = chars.Slice(2);
            }

            return uint.TryParse(chars, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
        }
    }
}
