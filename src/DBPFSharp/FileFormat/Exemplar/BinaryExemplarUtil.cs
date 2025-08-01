// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System.IO;

namespace DBPFSharp.FileFormat.Exemplar
{
    internal static class BinaryExemplarUtil
    {
        public static TGI ReadTGI(BinaryReader reader)
        {
            uint type = reader.ReadUInt32();
            uint group = reader.ReadUInt32();
            uint instance = reader.ReadUInt32();

            return new TGI(type, group, instance);
        }

        public static void WriteTGI(BinaryWriter writer, in TGI tgi)
        {
            writer.Write(tgi.Type);
            writer.Write(tgi.Group);
            writer.Write(tgi.Instance);
        }
    }
}
