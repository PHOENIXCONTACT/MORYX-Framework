namespace Marvin.Communication.Sockets
{
    internal class ClientDisconnectedState : ClientStateBase
    {
        public ClientDisconnectedState(TcpClientConnection context, StateMap stateMap) : base(context, stateMap, BinaryConnectionState.Disconnected)
        {
        }

        public override void Reconnect(int delayMs)
        {
            if (delayMs > 0)
            {
                NextState(StateRetryConnect);
                Context.ScheduleConnectTimer(delayMs);
            }
            else
            {
                NextState(StateConnecting);
            }
        }

        public override void Connect()
        {
            NextState(StateConnecting);
        }

        public override void Disconnect()
        {
        }
    }
}