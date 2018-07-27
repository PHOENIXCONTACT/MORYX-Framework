using System.Threading.Tasks;

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

        public override Task SendAsync(BinaryMessage message)
        {
            return Context.ExecuteSendAsync(message);
        }

        public override void Reconnect(int delayMs)
        {
            if (delayMs > 0)
            {
                Context.CleanupTransmission();
                NextState(StateReconnecting);
                Context.ScheduleConnectTimer(delayMs);
            }
            else
            {
                Context.CleanupTransmission();
                NextState(StateListening);
                Context.Register();
            }
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