using System;
using System.Net.Sockets;

namespace Marvin.Communication.Sockets
{
    internal class ClientConnectingState : ClientStateBase
    {
        public ClientConnectingState(TcpClientConnection context, StateMap stateMap) : base(context, stateMap, BinaryConnectionState.AttemptingConnection)
        {
        }

        public override void OnEnter()
        {
            Context.Connect();
        }

        public override void ConnectionCallback(IAsyncResult ar, TcpClient tcpClient)
        {
            try
            {
                tcpClient.EndConnect(ar);
                Context.Connected();
                NextState(StateConnected);
            }
            catch (Exception)
            {
                Context.RetryConnect();
            }
        }

        public override void Disconnect()
        {
            NextState(StateDisconnected);
            Context.StopConnect();
        }
    }
}