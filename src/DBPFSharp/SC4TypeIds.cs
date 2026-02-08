// Copyright (c) 2023, 2025, 2026 Nicholas Hayes
// SPDX-License-Identifier: MIT

namespace DBPFSharp
{
    /// <summary>
    /// Constants for resource type identifiers used in SimCity 4.
    /// </summary>
    public static class SC4TypeIds
    {
        /// <summary>
        /// The Cohort resource type.
        /// </summary>
        public const uint Cohort = 0x05342861;
        
        /// <summary>
        /// A common image format, BMP, JPEG, PNG TGA, etc.
        /// </summary>
        /// <remarks>
        /// The FSH image format has its own type id, but
        /// other common image formats share this one.
        /// </remarks>
        public const uint CommonImageFormat = 0x856DDBAC;

        /// <summary>
        /// The Cursor resource type.
        /// </summary>
        public const uint Cursor = 0xAA5C3144;

        /// <summary>
        /// The DBPF resource type.
        /// </summary>
        public const uint DBPF = 0x6A5B7BF5;

        /// <summary>
        /// The Effect Directory resource type.
        /// </summary>
        public const uint EffectDirectory = 0xEA5118B0;

        /// <summary>
        /// The Exemplar resource type.
        /// </summary>
        public const uint Exemplar = 0x6534284A;

        /// <summary>
        /// The FSH image resource type.
        /// </summary>
        public const uint FSH = 0x7AB50E44;

        /// <summary>
        /// The Hit audio track.
        /// </summary>
        public const uint HitTrack = 0x0B8D821A;

        /// <summary>
        /// The Hit audio track list.
        /// </summary>
        public const uint HitTrackList = 0x7B1ACFCD;

        /// <summary>
        /// The Hit audio track logic object.
        /// </summary>
        public const uint HitTrackLogicObject = 0x9D796DB4;

        /// <summary>
        /// The key configuration resource type.
        /// </summary>
        public const uint KeyConfig = 0xA2E3D533;

        /// <summary>
        /// The LTEXT string localization resource.
        /// </summary>
        /// <remarks>
        /// This type id is also used by the LEV, WAV, and XA formats.
        /// </remarks>
        public const uint LTEXT = 0x2026960B;

        /// <summary>
        /// The Lua script resource type.
        /// </summary>
        public const uint Lua = 0xCA63E2A3;

        /// <summary>
        /// The mad cow movie resource type.
        /// </summary>
        public const uint MadCowMovie = 0x0A8B0E70;

        /// <summary>
        /// The path information resource type.
        /// </summary>
        public const uint PathInfo = 0x296678F7;

        /// <summary>
        /// The SimGlide 3D (S3D) model resource type.
        /// </summary>
        public const uint S3D = 0x5AD0E817;

        /// <summary>
        /// The sprite animation resource type.
        /// </summary>
        public const uint SpriteAnimation = 0x29A5D1EC;

        /// <summary>
        /// The sprite animation table resource type.
        /// </summary>
        public const uint SpriteAnimationTable = 0x09ADCD75;

        /// <summary>
        /// An unspecified format.
        /// </summary>
        /// <remarks>
        /// This type id is shared by AB, INI, UI, and possibly others.
        /// </remarks>
        public const uint Unspecified = 0x00000000;
    }
}
