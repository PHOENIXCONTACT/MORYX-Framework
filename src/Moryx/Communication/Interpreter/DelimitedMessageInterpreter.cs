// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication
{
    /// <summary>
    /// Interpreter for messages based on delimiters
    /// </summary>
    public abstract class DelimitedMessageInterpreter : IMessageInterpreter
    {
        /// <summary>
        /// Size of the read buffer
        /// </summary>
        protected abstract int BufferSize { get; }
        /// <summary>
        /// Number of bytes to read in each iteration
        /// </summary>
        protected abstract int ReadSize { get; }

        /// <summary>
        /// Optional byte sequence for start of message
        /// </summary>
        protected virtual byte[] StartDelimiter => BinaryMessage.EmptyBytes;

        /// <summary>
        /// Mandatory end delimiter of each message
        /// </summary>
        protected abstract byte[] EndDelimiter { get; }

        /// <summary>
        /// Create transmission for first communication
        /// </summary>
        /// <returns></returns>
        public IReadContext CreateContext()
        {
            return new DelimitedMessageContext
            {
                ReadBuffer = new byte[BufferSize],
                ReadSize = ReadSize
            };
        }

        /// <summary>
        /// Serialize the message into its binary representation
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public byte[] SerializeMessage(BinaryMessage message)
        {
            return message.Payload;
        }

        /// <summary>
        /// Method called by the connection once the bytes were read
        /// </summary>
        /// <param name="context">Current transmission</param>
        /// <param name="readBytes">Bytes that were read by the socket</param>
        /// <param name="publishCompleteMessage">Delegate to publish a complete message read from the byte stream</param>
        public ByteReadResult ProcessReadBytes(IReadContext context, int readBytes, Action<BinaryMessage> publishCompleteMessage)
        {
            var delimited = (DelimitedMessageContext)context;

            // Search for start
            if (!delimited.StartFound)
            {
                if (delimited.CurrentIndex + readBytes >= StartDelimiter.Length && FindStart(readBytes, delimited))
                {
                    delimited.StartFound = true;
                }
                else
                {
                    delimited.CurrentIndex += readBytes;
                    return ByteReadResult.KeepReading;
                }
            }

            // Search for end
            var header = delimited.MessageStart + StartDelimiter.Length;
            var searchStart = delimited.CurrentIndex > (header + EndDelimiter.Length) ? delimited.CurrentIndex - EndDelimiter.Length : header;
            var searchEnd = (delimited.CurrentIndex + readBytes) - EndDelimiter.Length;
            for (var index = searchStart; index <= searchEnd; index++)
            {
                var segment = new ArraySegment<byte>(delimited.ReadBuffer, index, EndDelimiter.Length);
                if (!EndDelimiter.SequenceEqual(segment))
                    continue;

                // Wrap up message and publish
                delimited.MessageEnd = index + EndDelimiter.Length - 1;
                publishCompleteMessage(ToMessage(delimited));

                var overlap = ResetContext(readBytes, delimited);
                return ProcessReadBytes(delimited, overlap, publishCompleteMessage);
            }

            delimited.CurrentIndex += readBytes;
            return ByteReadResult.KeepReading;
        }

        /// <summary>
        /// Find the begging of the message payload. Possible header before start delimiter will be removed from the message
        /// </summary>
        /// <param name="readBytes">Number of bytes read from the stream</param>
        /// <param name="context">Read context</param>
        /// <returns>True if start was found, otherwise false</returns>
        private bool FindStart(int readBytes, DelimitedMessageContext context)
        {
            if (StartDelimiter.Length == 0)
                return true;

            var searchStart = context.CurrentIndex > StartDelimiter.Length ? context.CurrentIndex - StartDelimiter.Length : 0;
            var searchEnd = (context.CurrentIndex + readBytes) - StartDelimiter.Length;
            // Skip non protocol related boiler plate at message start
            for (var index = searchStart; index <= searchEnd; index++)
            {
                var segment = new ArraySegment<byte>(context.ReadBuffer, index, StartDelimiter.Length);
                if (!StartDelimiter.SequenceEqual(segment))
                    continue;

                context.MessageStart = index;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Move message overlap to front of read buffer and reset the context
        /// </summary>
        /// <param name="readBytes">Number of bytes read from the stream. Used to calculate overlap</param>
        /// <param name="context">Message context</param>
        /// <returns>Overlap bytes - used for recursive stream processing</returns>
        private static int ResetContext(int readBytes, DelimitedMessageContext context)
        {
            // Move shit to front of buffer if we have overlap
            //                  Entire read buffer                   Full first message
            var overlap = (readBytes + context.CurrentIndex) - (context.MessageEnd + 1);
            if (overlap > 0)
                Buffer.BlockCopy(context.ReadBuffer, context.MessageEnd + 1, context.ReadBuffer, 0, overlap);

            // Reset message
            context.StartFound = false;
            context.MessageStart = 0;
            context.CurrentIndex = 0;

            return overlap;
        }

        /// <summary>
        /// Create incoming message from transmission
        /// </summary>>
        private static BinaryMessage ToMessage(IReadContext context)
        {
            var delimited = (DelimitedMessageContext)context;
            var messageLenght = (delimited.MessageEnd - delimited.MessageStart) + 1;

            var buffer = new byte[messageLenght];
            Buffer.BlockCopy(delimited.ReadBuffer, delimited.MessageStart, buffer, 0, messageLenght);
            return new BinaryMessage(buffer);
        }

        /// <summary>
        /// Create error response for a faulty message
        /// </summary>
        public bool ErrorResponse(IReadContext context, out byte[] lastWill)
        {
            lastWill = BinaryMessage.EmptyBytes;
            return false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        protected virtual bool Equals(DelimitedMessageInterpreter other)
        {
            return true;
        }

        /// <inheritdoc />
        public bool Equals(IMessageInterpreter messageInterpreter)
        {
            return Equals((object)messageInterpreter);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DelimitedMessageInterpreter)obj);
        }
    }
}
