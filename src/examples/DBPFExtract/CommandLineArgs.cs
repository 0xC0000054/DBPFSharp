// Copyright (c) 2023 Nicholas Hayes
// SPDX-License-Identifier: MIT

using Mono.Options;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DBPFextract
{
    internal sealed class CommandLineArgs
    {
        public CommandLineArgs(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                HaveTGI = false;
                string tgi = string.Empty;
                string outputFile = string.Empty;

                OptionSet options = new()
                {
                    { "TGI=", (string value) => tgi = value },
                    { "o|output-file=", (string value) => outputFile = value }
                };

                List<string> remainingArgs = options.Parse(args);

                if (!string.IsNullOrWhiteSpace(tgi))
                {
                    string[] parts = tgi.Split(',');

                    if (parts.Length == 3
                        && ParseHexNumber(parts[0], out uint type)
                        && ParseHexNumber(parts[1], out uint group)
                        && ParseHexNumber(parts[2], out uint instance))
                    {
                        Type = type;
                        Group = group;
                        Instance = instance;
                        HaveTGI = true;
                    }
                    else
                    {
                        throw new ArgumentException("Invalid TGI string. It should be 3 hexadecimal numbers separated by commas.");
                    }
                }

                OutputFile = outputFile;

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

        public bool HaveTGI { get; }

        public string? InputFile { get; }

        public string? OutputFile { get; }

        private static bool ParseHexNumber(ReadOnlySpan<char> chars, out uint value)
        {
            if (chars.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                chars = chars.Slice(2);
            }

            return uint.TryParse(chars, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
        }
    }
}
