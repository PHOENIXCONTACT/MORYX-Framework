// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text;

namespace Moryx.Serialization
{
    /// <summary>
    /// Bit converter that writes directly to a given byte array
    /// </summary>
    public static class InlineConverter
    {
        /// <summary>
        /// Includes a short into an array of bytes
        /// </summary>
        public static unsafe void Include(short value, byte[] bytes, ref int index)
        {
            fixed (byte* b = bytes)
                *((short*)(b + index)) = value;
            index += 2;
        }

        /// <summary>
        /// Includes a short into an array of bytes
        /// </summary>+
        public static unsafe void Include(ushort value, byte[] bytes, ref int index)
        {
            fixed (byte* b = bytes)
                *((ushort*)(b + index)) = value;
            index += 2;
        }

        /// <summary>
        /// Includes an int into an array of bytes
        /// </summary>
        public static unsafe void Include(int value, byte[] bytes, ref int index)
        {
            fixed (byte* b = bytes)
                *((int*)(b + index)) = value;
            index += 4;
        }

        /// <summary>
        /// Includes an int into an array of bytes
        /// </summary>
        public static unsafe void Include(uint value, byte[] bytes, ref int index)
        {
            fixed (byte* b = bytes)
                *((uint*)(b + index)) = value;
            index += 4;
        }

        /// <summary>
        /// Includes a long into an array of bytes
        /// </summary>
        public static unsafe void Include(long value, byte[] bytes, ref int index)
        {
            fixed (byte* b = bytes)
                *((long*)(b + index)) = value;
            index += 8;
        }

        /// <summary>
        /// Includes a long into an array of bytes
        /// </summary>
        public static unsafe void Include(ulong value, byte[] bytes, ref int index)
        {
            fixed (byte* b = bytes)
                *((ulong*)(b + index)) = value;
            index += 8;
        }

        /// <summary>
        /// Includes an int into an array of bytes
        /// </summary>
        public static unsafe void Include(float value, byte[] bytes, ref int index)
        {
            fixed (byte* b = bytes)
                *((float*)(b + index)) = value;
            index += 4;
        }

        /// <summary>
        /// Includes a long into an array of bytes
        /// </summary>
        public static unsafe void Include(double value, byte[] bytes, ref int index)
        {
            fixed (byte* b = bytes)
                *((double*)(b + index)) = value;
            index += 8;
        }

        private static readonly Encoding Encoder = new ASCIIEncoding();
        /// <summary>
        /// Read string from byte array
        /// </summary>
        public static string ReadString(bool padded, byte[] bytes, int index, int maxLength)
        {
            int length;
            if (padded)
            {
                // For padded strings we must find the end first
                for (length = 0; length < maxLength; length++)
                {
                    if (bytes[index + length] == 0)
                        break;
                }
            }
            else
            {
                length = maxLength;
            }

            return Encoder.GetString(bytes, index, length);
        }
    }
}
