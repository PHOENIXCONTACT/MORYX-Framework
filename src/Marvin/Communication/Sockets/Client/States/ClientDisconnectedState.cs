using System;
using System.Net.Sockets;

namespace Marvin.Communication.Sockets
{
    internal class ClientDisconnectedState : ClientStateBase
    {
        public ClientDisconnectedState(TcpClientConnection context, StateMap stateMap) : base(context, stateMap, BinaryConnectionState.Disconnected)
        {
        }

        public override void Connect()
        {
            NextState(StateConnecting);
        }

        public override void ConnectionCallback(IAsyncResult ar, TcpClient tcpClient)
        {
            
        }
    }
}