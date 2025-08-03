// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using System;
using System.Text;

namespace DBPFSharp.FileFormat
{
    /// <summary>
    /// Represents a LTEXT (localized text) record.
    /// </summary>
    /// <seealso cref="FileFormat" />
    public sealed class LTEXT : FileFormat
    {
        private static readonly Lazy<UTF8Encoding> UTF8 = new(() => new UTF8Encoding(encoderShouldEmitUTF8Identifier: false,
                                                                                     throwOnInvalidBytes: true));
        private static readonly Lazy<UnicodeEncoding> UTF16LE = new(() => new UnicodeEncoding(bigEndian: false,
                                                                                              byteOrderMark: false,
                                                                                              throwOnInvalidBytes: true));
        /// <summary>
        /// Initializes a new instance of the <see cref="LTEXT"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        public LTEXT(string value)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));

            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LTEXT"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        /// <exception cref="DBPFException">An error occurred when decoding the LTEXT data.</exception>
        public LTEXT(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));

            this.Value = Decode(data);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; }

        /// <summary>
        /// Encodes the data to a byte array.
        /// </summary>
        /// <returns>
        /// The encoded data.
        /// </returns>
        /// <exception cref="DBPFException">An error occurred when encoding the data.</exception>
        public override byte[] Encode()
        {
            try
            {
                string value = this.Value;

                if (value.Length > Header.TextMaxLength)
                {
                    throw new DBPFException("The string is too long to encode as a LTEXT record.");
                }

                Encoding encoding = UTF16LE.Value;

                int encodedTextLengthInBytes = encoding.GetByteCount(value);

                byte[] encodedData = new byte[Header.SizeOf + encodedTextLengthInBytes];
                Span<byte> encodedDataAsSpan = encodedData;

                Header.Write(encodedDataAsSpan, value.Length, FileEncoding.UTF16LE);

                int bytesWritten = encoding.GetBytes(value, encodedDataAsSpan[Header.SizeOf..]);

                if (bytesWritten != encodedTextLengthInBytes)
                {
                    throw new DBPFException("Failed to encode the LTEXT string data.");
                }

                return encodedData;
            }
            catch (EncoderFallbackException ex)
            {
                throw new DBPFException("Failed to encode the LTEXT string data.", ex);
            }
        }

        private static string Decode(ReadOnlyMemory<byte> data)
        {
            (int textLength, FileEncoding encoding) = Header.Read(data.Span);

            string result = string.Empty;

            if (textLength > 0)
            {
                try
                {
                    ReadOnlyMemory<byte> text = data[Header.SizeOf..];

                    result = string.Create(textLength, (text, encoding), static (chars, state) =>
                    {
                        int decodedChars = state.encoding switch
                        {
                            // We treat the active Windows code page as US-ASCII.
                            FileEncoding.ActiveCodePage => Encoding.ASCII.GetChars(state.text.Span, chars),
                            FileEncoding.UTF8 => UTF8.Value.GetChars(state.text.Span, chars),
                            FileEncoding.UTF16LE => UTF16LE.Value.GetChars(state.text.Span, chars),
                            _ => throw new DBPFException($"Unsupported LTEXT encoding: 0x{(byte)state.encoding:X2}."),
                        };

                        if (decodedChars != chars.Length)
                        {
                            throw new DBPFException("Failed to decode the LTEXT string data.");
                        }
                    });
                }
                catch (DecoderFallbackException ex)
                {
                    throw new DBPFException("Failed to decode the LTEXT string data.", ex);
                }
            }

            return result;
        }

        private enum FileEncoding : byte
        {
            /// <summary>
            /// The active Windows code page.
            /// </summary>
            ActiveCodePage = 0,

            /// <summary>
            /// UTF-8
            /// </summary>
            /// <remarks>
            /// UTF-8 is SC4's internal text encoding, but LTEXT
            /// files typically use UTF-16LE.
            /// </remarks>
            UTF8 = 8,

            /// <summary>
            /// UTF-16 with little endian byte order.
            /// </summary>
            /// <remarks>
            /// Most if not all of the game's LTEXT files use this.
            /// </remarks>
            UTF16LE = 16
        }

        private static class Header
        {
            internal const int SizeOf = 4;
            internal const int TextMaxLength = 0xFF_FF;

            /// <summary>
            /// Reads the LTEXT header.
            /// </summary>
            /// <param name="bytes">The LTEXT record bytes.</param>
            /// <returns>The decoded header data.</returns>
            /// <exception cref="DBPFException">The LTEXT file is invalid.</exception>
            internal static (int textLength, FileEncoding encoding) Read(ReadOnlySpan<byte> bytes)
            {
                if (bytes.Length < SizeOf)
                {
                    throw new DBPFException("The LTEXT file is invalid.");
                }

                // The LTEXT header is a packed little endian UInt32 value.
                // The text length is stored in the bottom 3 bytes as a little endian UInt24, with
                // the upper byte containing the encoding.

                int textLength = bytes[0] | (bytes[1] << 8) | (bytes[2] << 16);
                FileEncoding encoding = (FileEncoding)(bytes[3]);

                return (textLength, encoding);
            }

            /// <summary>
            /// Writes the LTEXT header to the specified destination.
            /// </summary>
            /// <param name="destination">The destination.</param>
            /// <param name="textLength">The text length in characters.</param>
            /// <param name="encoding">The encoding.</param>
            /// <exception cref="DBPFException">
            /// The LTEXT header buffer must be at least 4 bytes.
            /// or
            /// The string is too long to encode as a LTEXT record.
            /// </exception>
            internal static void Write(Span<byte> destination, int textLength, FileEncoding encoding)
            {
                if (destination.Length < SizeOf)
                {
                    throw new DBPFException("The LTEXT header buffer must be at least 4 bytes.");
                }

                if (textLength > TextMaxLength)
                {
                    throw new DBPFException("The string is too long to encode as a LTEXT record.");
                }

                // The LTEXT header is a packed little endian UInt32 value.
                // The text length is stored in the bottom 3 bytes as a little endian UInt24, with
                // the upper byte containing the encoding.

                destination[0] = (byte)(textLength & 0xFF);
                destination[1] = (byte)((textLength >> 8) & 0xFF);
                destination[2] = (byte)((textLength >> 16) & 0xFF);
                destination[3] = (byte)encoding;
            }
        }
    }
}
