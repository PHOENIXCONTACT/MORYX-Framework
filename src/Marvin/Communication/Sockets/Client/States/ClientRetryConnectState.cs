namespace Marvin.Communication.Sockets
{
    internal class ClientRetryConnectState : ClientStateBase
    {
        public ClientRetryConnectState(TcpClientConnection context, StateMap stateMap) : base(context, stateMap, BinaryConnectionState.AttemptingConnection)
        {
        }

        public override void ScheduledConnectTimerElapsed()
        {
            NextState(StateConnecting);
        }

        public override void Disconnect()
        {
            NextState(StateDisconnected);
        }
    }
}
