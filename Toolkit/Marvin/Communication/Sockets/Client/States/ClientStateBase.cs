using System;
using System.Net.Sockets;
using Marvin.StateMachines;

namespace Marvin.Communication.Sockets
{
    internal abstract class ClientStateBase : StateBase<TcpClientConnection>
    {
        public BinaryConnectionState Current { get; }

        protected ClientStateBase(TcpClientConnection context, StateMap stateMap, BinaryConnectionState connectionState) : base(context, stateMap)
        {
            Current = connectionState;
        }

        public virtual void Connect()
        {
        }

        public virtual void Disconnect()
        {
        }

        public virtual void ConnectionCallback(IAsyncResult ar, TcpClient tcpClient)
        {
        }

        public virtual void Send(BinaryMessage message)
        {
            InvalidState();
        }

        public virtual void ConnectionClosed()
        {
        }

        [StateDefinition(typeof(ClientConnectedState))]
        protected const int StateConnected = 10;

        [StateDefinition(typeof(ClientConnectingState))]
        protected const int StateConnecting = 20;

        [StateDefinition(typeof(ClientDisconnectedState), IsInitial = true)]
        protected const int StateDisconnected = 30;

        [StateDefinition(typeof(ClientDisconnectingState))]
        protected const int StateDisconnecting = 40;

        
    }
}