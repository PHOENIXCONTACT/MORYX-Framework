// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using System.Text;

namespace Marvin.Communication.Sockets.IntegrationTests
{
    public class SystemTestHeader : IBinaryHeader
    {
        public static string TestString = "There is a theory which states that if ever anyone discovers exactly what " +
                                          "the Universe is for and why it is here, it will instantly disappear and be replaced by " +
                                          "something even more bizarre and inexplicable. There is another theory which states that this " +
                                          "has already happened.";

        public SystemTestHeader()
        {
            HeaderString = TestString;
        }

        public int ClientIdx { get; set; }

        public string HeaderString { get; private set; }

        public bool HasStartAndStopString => false;

        public byte[] BlockStart { get; set; }

        public byte[] BlockEnd { get; set; }

        public int PayloadLength { get; set; }

        public void FromBytes(byte[] headerBytes)
        {
            var index = 0;
            FromBytes(headerBytes, ref index);
        }

        public void FromBytes(byte[] headerBytes, ref int index)
        {
            ClientIdx = BitConverter.ToInt32(headerBytes, index);
            index += 4;
            PayloadLength = BitConverter.ToInt32(headerBytes, index);
            index += 4;
            var testStringLength = Encoding.Unicode.GetByteCount(TestString);
            HeaderString = Encoding.Unicode.GetString(headerBytes.Skip(index).Take(testStringLength).ToArray());
            index += testStringLength;
        }

        /// <summary>
        /// Binary size of the object
        /// </summary>
        public int Size { get; private set; }

        public byte[] ToBytes()
        {
            var idx = BitConverter.GetBytes(ClientIdx);
            var pl = BitConverter.GetBytes(PayloadLength);
            var str = Encoding.Unicode.GetBytes(HeaderString);
            var result = new byte[idx.Length + pl.Length + str.Length];
            idx.CopyTo(result, 0);
            pl.CopyTo(result, idx.Length);
            str.CopyTo(result, idx.Length + pl.Length);
            return result;
        }
    }
}
