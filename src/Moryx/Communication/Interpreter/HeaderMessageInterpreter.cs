// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Communication
{
    /// <summary>
    /// Interpreter for messages with header
    /// </summary>
    public abstract class HeaderMessageInterpreter<THeader> : IMessageInterpreter
        where THeader : IBinaryHeader, new()
    {
        /// <summary>
        /// Size of the header
        /// </summary>
        protected abstract int HeaderSize { get; }

        /// <summary>
        /// Footed at the end of the message
        /// </summary>
        protected virtual byte[] Footer => BinaryMessage.EmptyBytes;

        /// <summary>
        /// Message footer
        /// </summary>
        private int FooterSize => Footer.Length;

        /// <summary>
        /// Create transmission for first communication
        /// </summary>
        /// <returns></returns>
        public IReadContext CreateContext()
        {
            return new HeaderMessageContext<THeader>
            {
                ReadSize = HeaderSize,
                ReadBuffer = new byte[HeaderSize]
            };
        }

        /// <summary>
        /// Serialize the message into its binary representation
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public byte[] SerializeMessage(BinaryMessage message)
        {
            var headerMessage = (BinaryMessage<THeader>)message;

            // Get header and payload
            var header = headerMessage.Header.ToBytes();
            var payload = headerMessage.Payload;

            // Create full message
            var bytes = new byte[header.Length + payload.Length + FooterSize];
            Buffer.BlockCopy(header, 0, bytes, 0, header.Length);
            Buffer.BlockCopy(headerMessage.Payload, 0, bytes, header.Length, payload.Length);
            Buffer.BlockCopy(Footer, 0, bytes, header.Length + payload.Length, FooterSize);

            return bytes;
        }

        /// <summary>
        /// Method called by the connection once the bytes were read
        /// </summary>
        /// <param name="context">Current transmission</param>
        /// <param name="readBytes">Bytes that were read by the socket</param>
        /// <param name="publishCompleteMessage">Delegate to publish a complete message read from the byte stream</param>
        public ByteReadResult ProcessReadBytes(IReadContext context, int readBytes, Action<BinaryMessage> publishCompleteMessage)
        {
            var headerContext = (HeaderMessageContext<THeader>)context;
            headerContext.CurrentIndex += readBytes;
            switch (headerContext.State)
            {
                case ParsingState.ReadingHeader:
                    var diff = HeaderSize - headerContext.CurrentIndex;
                    if (diff > 0)
                    {
                        headerContext.ReadSize = diff;
                    }
                    else
                    {
                        // Header complete
                        headerContext.Header = new THeader();
                        headerContext.Header.FromBytes(headerContext.ReadBuffer);

                        // Prepare message for payload
                        var payloadLength = headerContext.Header.PayloadLength + FooterSize;
                        if (payloadLength > 0)
                            PrepareContext(headerContext, payloadLength, ParsingState.ReadingPayload);
                        else
                        {
                            publishCompleteMessage(ToMessage(headerContext));
                            PrepareContext(headerContext, HeaderSize, ParsingState.ReadingHeader);
                        }
                    }
                    break;
                case ParsingState.ReadingPayload:
                    diff = (headerContext.Header.PayloadLength + FooterSize) - headerContext.CurrentIndex;
                    if (diff > 0)
                    {
                        headerContext.ReadSize = diff;
                    }
                    else
                    {
                        // Create and publish message
                        var message = ToMessage(headerContext);
                        publishCompleteMessage(message);

                        PrepareContext(headerContext, HeaderSize, ParsingState.ReadingHeader);
                    }
                    break;
            }

            return ByteReadResult.KeepReading;
        }

        private static void PrepareContext(HeaderMessageContext<THeader> context, int size, ParsingState nextState)
        {
            // Reset context for next state
            context.CurrentIndex = 0;
            context.ReadSize = size;
            if (context.ReadBuffer.Length < size) // Only increase read buffer if it is too small
                context.ReadBuffer = new byte[size];

            context.State = nextState;
        }


        /// <summary>
        /// Create incoming message from transmission
        /// </summary>
        private static BinaryMessage ToMessage(HeaderMessageContext<THeader> headerContext)
        {
            var includesPayload = headerContext.State == ParsingState.ReadingPayload;
            var payloadLength = includesPayload ? headerContext.Header.PayloadLength : 0;

            var payload = includesPayload ? new byte[payloadLength] : BinaryMessage.EmptyBytes;
            if (includesPayload)
                Buffer.BlockCopy(headerContext.ReadBuffer, 0, payload, 0, payloadLength);

            return new BinaryMessage<THeader>(headerContext.Header, payload);
        }

        /// <summary>
        /// Create error response for a faulty message
        /// </summary>
        public virtual bool ErrorResponse(IReadContext context, out byte[] lastWill)
        {
            lastWill = BinaryMessage.EmptyBytes;
            return false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        protected virtual bool Equals(HeaderMessageInterpreter<THeader> other)
        {
            // For the binary connection port assignment it is
            // important to distinguish between two instances of IMessageInterpreter
            // For this case, header interpreter of the same header are considered equal 
            // unless a derived protocoll provides different implementations for versions of the header
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
            return Equals((HeaderMessageInterpreter<THeader>)obj);
        }
    }
}
