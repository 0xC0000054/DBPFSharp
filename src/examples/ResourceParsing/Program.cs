// Copyright (c) 2023, 2025, 2026 Nicholas Hayes
// SPDX-License-Identifier: MIT

using DBPFSharp;
using DBPFSharp.FileFormat;
using DBPFSharp.FileFormat.Exemplar;

namespace ResourceParsing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: ResourceParsing <DBPF file>");
                return;
            }

            try
            {
                string input = args[0];

                using (DBPFFile file = new(input))
                {
                    var index = file.Index;

                    DBPFIndexEntry? exemplarEntry = index.FirstOrDefault(i => i.Type is SC4TypeIds.Exemplar);

                    if (exemplarEntry != null)
                    {
                        ParseExemplar(file, exemplarEntry);
                    }
                    else
                    {
                        Console.WriteLine("{0} does not contain any cohort or exemplar records.", input);
                    }

                    DBPFIndexEntry? ltextEntry = index.FirstOrDefault(i => i.Type is SC4TypeIds.LTEXT);

                    if (ltextEntry != null)
                    {
                        ParseLTEXT(file, ltextEntry);
                    }
                    else
                    {
                        Console.WriteLine("{0} does not contain any LTEXT records.", input);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void ParseExemplar(DBPFFile file, DBPFIndexEntry indexEntry)
        {
            if (indexEntry.Type is not SC4TypeIds.Exemplar)
            {
                Console.WriteLine("0x{0:X8}, 0x{1:X8}, 0x{2:X8} is not an exemplar.",
                                  indexEntry.Type,
                                  indexEntry.Group,
                                  indexEntry.Instance);
                return;
            }

            try
            {
                DBPFEntry entry = file.GetEntry(indexEntry);
                ReadOnlySpan<byte> data = entry.GetUncompressedDataAsSpan();

                Exemplar exemplar = new(data);

                Console.WriteLine("Exemplar 0x{0:X8}, 0x{1:X8}, 0x{2:X8} has {3} properties.",
                                  indexEntry.Type,
                                  indexEntry.Group,
                                  indexEntry.Instance,
                                  exemplar.Properties.Count);

            }
            catch (DBPFException ex)
            {
                Console.WriteLine("Error when parsing 0x{0:X8}, 0x{1:X8}, 0x{2:X8}: {3}",
                                  indexEntry.Type,
                                  indexEntry.Group,
                                  indexEntry.Instance,
                                  ex.Message);
            }
        }

        public static void ParseLTEXT(DBPFFile file, DBPFIndexEntry indexEntry)
        {
            if (indexEntry.Type is not SC4TypeIds.LTEXT)
            {
                Console.WriteLine("0x{0:X8}, 0x{1:X8}, 0x{2:X8} is not a LTEXT.",
                                  indexEntry.Type,
                                  indexEntry.Group,
                                  indexEntry.Instance);
                return;
            }

            try
            {
                DBPFEntry entry = file.GetEntry(indexEntry);
                ReadOnlySpan<byte> data = entry.GetUncompressedDataAsSpan();

                LTEXT ltext = new(data);

                Console.WriteLine("0x{0:X8}, 0x{1:X8}, 0x{2:X8} text: {3}.",
                                  indexEntry.Type,
                                  indexEntry.Group,
                                  indexEntry.Instance,
                                  ltext.Value);

            }
            catch (DBPFException ex)
            {
                Console.WriteLine("Error when parsing 0x{0:X8}, 0x{1:X8}, 0x{2:X8}: {3}",
                                  indexEntry.Type,
                                  indexEntry.Group,
                                  indexEntry.Instance,
                                  ex.Message);
            }
        }
    }
}
