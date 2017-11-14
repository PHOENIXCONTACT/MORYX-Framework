namespace Marvin.Communication.Sockets
{
    internal class ServerConnectedState: ServerStateBase
    {
        public ServerConnectedState(TcpListenerConnection context, StateMap stateMap) : base(context, stateMap, BinaryConnectionState.Connected)
        {
        }

        public override void Send(BinaryMessage message)
        {
            Context.ExecuteSend(message);
        }

        public override void Close()
        {
            NextState(StateNotListening);
            Context.CleanupTransmission();
        }

        public override void ConnectionLost()
        {
            Context.CleanupTransmission();
            NextState(StateListening);
            Context.Register();
        }
    }
}