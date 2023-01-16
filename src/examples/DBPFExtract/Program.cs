// Copyright (c) 2023 Nicholas Hayes
// SPDX-License-Identifier: MIT

using DBPFSharp;
using System;
using System.IO;

namespace DBPFextract
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CommandLineArgs commandLine = new(args);

                DBPFFile file = new(commandLine.InputFile!);

                Console.WriteLine("{0} contains {1} entries ({2} compressed).",
                                  Path.GetFileName(commandLine.InputFile),
                                  file.Header.Entries,
                                  file.CompressionDirectory.Count);

                if (file.Header.Entries > 0)
                {
                    uint type, group, instance;

                    if (commandLine.HaveTGI)
                    {
                        type = commandLine.Type;
                        group = commandLine.Group;
                        instance = commandLine.Instance;
                    }
                    else
                    {
                        DBPFIndexEntry indexEntry = file.Index[0];

                        type = indexEntry.Type;
                        group = indexEntry.Group;
                        instance = indexEntry.Instance;
                    }

                    DBPFEntry item = file.GetEntry(type, group, instance);

                    Console.WriteLine("TGI = 0x{0:X8},0x{1:X8},0x{2:X8}, IsCompressed = {3}",
                                      type,
                                      group,
                                      instance,
                                      item.IsCompressed);

                    byte[] decoded = item.GetUncompressedData();

                    Console.WriteLine("Uncompressed size = {0}", decoded.Length);

                    if (!string.IsNullOrWhiteSpace(commandLine.OutputFile))
                    {
                        File.WriteAllBytes(commandLine.OutputFile, decoded);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}