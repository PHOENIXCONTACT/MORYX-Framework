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

        public override void Reconnect(int delayMs)
        {
            if (delayMs > 0)
            {
                NextState(StateReconnecting);
                Context.ScheduleConnectTimer(delayMs);
            }
            else
            {
                NextState(StateListening);
                Context.Register();
            }
        }
    }
}