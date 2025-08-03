// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using DBPFSharp;
using DBPFSharp.FileFormat;
using DBPFSharp.FileFormat.Exemplar;

namespace ResourceParsing
{
    internal class Program
    {
        private const uint CohortTypeID = 0x05342861;
        private const uint ExemplarTypeID = 0x6534284A;
        private const uint LTEXTTypeID = 0x2026960B;

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

                    DBPFIndexEntry? exemplarEntry = index.FirstOrDefault(i => i.Type is CohortTypeID or ExemplarTypeID);

                    if (exemplarEntry != null)
                    {
                        ParseExemplar(file, exemplarEntry);
                    }
                    else
                    {
                        Console.WriteLine("{0} does not contain any cohort or exemplar records.");
                    }

                    DBPFIndexEntry? ltextEntry = index.FirstOrDefault(i => i.Type is LTEXTTypeID);

                    if (ltextEntry != null)
                    {
                        ParseLTEXT(file, ltextEntry);
                    }
                    else
                    {
                        Console.WriteLine("{0} does not contain any LTEXT records.");
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
            if (indexEntry.Type is not CohortTypeID and not ExemplarTypeID)
            {
                Console.WriteLine("0x{0:X8}, 0x{1:X8}, 0x{2:X8} is not a cohort or exemplar.",
                                  indexEntry.Type,
                                  indexEntry.Group,
                                  indexEntry.Instance);
                return;
            }

            try
            {
                DBPFEntry entry = file.GetEntry(indexEntry);
                byte[] data = entry.GetUncompressedData();

                Exemplar exemplar = new(data);

                string type = exemplar.IsCohort ? "Cohort" : "Exemplar";

                Console.WriteLine("{0} 0x{1:X8}, 0x{2:X8}, 0x{3:X8} has {4} properties.",
                                  type,
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
            if (indexEntry.Type is not LTEXTTypeID)
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
                byte[] data = entry.GetUncompressedData();

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
