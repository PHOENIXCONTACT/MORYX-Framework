// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text;

namespace Moryx.Communication.Sockets.IntegrationTests.DelimiterProtocol
{
    public class HtmlInterpreter : DelimitedMessageInterpreter
    {
        private static HtmlInterpreter _instance;

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static HtmlInterpreter Instance => _instance ?? (_instance = new HtmlInterpreter());

        /// <summary>
        /// 1 MB read buffer size for each connection
        /// </summary>
        protected override int BufferSize => 1048576;

        /// <summary>
        /// Number of bytes to read in each iteration
        /// </summary>
        protected override int ReadSize => 10240;

        /// <summary>
        /// Byte sequence for start of message
        /// </summary>
        protected override byte[] StartDelimiter => Encoding.UTF8.GetBytes("<html>");

        /// <summary>
        /// Byte sequence for end of message
        /// </summary>
        protected override byte[] EndDelimiter => Encoding.UTF8.GetBytes("</html>");
    }
}
