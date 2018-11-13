using System.Collections.Generic;

namespace Marvin.Communication.Sockets.IntegrationTests
{
    public class ConnectionBuffer<TMessage> where TMessage : BinaryMessage
    {
        public ConnectionBuffer()
        {
            Received = new List<TMessage>();
            LastStateChangeEvents = new List<BinaryConnectionState>();
        }

        public IBinaryConnection Connection { get; set; }

        public List<TMessage> Received { get; }

        public int Id { get; set; }

        public List<BinaryConnectionState> LastStateChangeEvents { get; }
    }
}
