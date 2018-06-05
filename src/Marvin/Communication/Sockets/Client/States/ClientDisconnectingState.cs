namespace Marvin.Communication.Sockets
{
    internal class ClientDisconnectingState : ClientStateBase
    {
        public ClientDisconnectingState(TcpClientConnection context, StateMap stateMap) : base(context, stateMap, BinaryConnectionState.Connected)
        {
        }

        public override void ConnectionClosed()
        {
            NextState(StateDisconnected);
        }
    }
}