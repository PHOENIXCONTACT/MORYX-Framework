namespace Marvin.Communication.Sockets
{
    internal class ServerNotListeningState: ServerStateBase
    {
        public ServerNotListeningState(TcpListenerConnection context, StateMap stateMap) : base(context, stateMap, BinaryConnectionState.Disconnected)
        {
        }

        public override void Open()
        {
            NextState(StateListening);
            Context.Register();
        }
    }
}