// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication.Sockets.IntegrationTests.DelimiterProtocol
{
    public class EndDelimiterOnlyInterpreter : DelimitedMessageInterpreter
    {
        public static byte[] TestEndDelimiter = { 1, 3, 3, 7 };
        public const int TestBufferSize = 1024;
        public const int TestReadSize = 100;

        /// <summary>
        /// Size of the read buffer
        /// </summary>
        protected override int BufferSize => TestBufferSize;
        
        /// <summary>
        /// Number of bytes to read in each iteration
        /// </summary>
        protected override int ReadSize => TestReadSize;
        
        /// <summary>
        /// Byte sequence for end of message
        /// </summary>
        protected override byte[] EndDelimiter => TestEndDelimiter;
    }
}
