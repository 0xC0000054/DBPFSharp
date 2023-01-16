// Copyright (c) 2023 Nicholas Hayes
// SPDX-License-Identifier: MIT

using DBPFSharp;
using System;
using System.IO;

namespace DBPFcreate
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CommandLineArgs commandLine = new(args);

                byte[] inputFile = File.ReadAllBytes(commandLine.InputFile!);

                using (DBPFFile file = new())
                {
                    file.Add(commandLine.Type,
                             commandLine.Group,
                             commandLine.Instance,
                             inputFile,
                             commandLine.Compress);

                    file.Save(commandLine.OutputFile!);
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