namespace Marvin.Communication.Sockets
{
    internal class ClientConnectedState : ClientStateBase
    {
        public ClientConnectedState(TcpClientConnection context, StateMap stateMap) : base(context, stateMap, BinaryConnectionState.Connected)
        {
        }

        public override void Send(BinaryMessage message)
        {
            Context.ExecuteSend(message);
        }

        public override void Disconnect()
        {
            NextState(StateDisconnecting);
            Context.Disconnect();
        }

        public override void ConnectionClosed()
        {
            Context.Disconnect();
            NextState(StateConnecting);
        }
    }
}