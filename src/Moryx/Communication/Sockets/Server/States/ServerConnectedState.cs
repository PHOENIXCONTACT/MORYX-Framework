// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;

namespace Moryx.Communication.Sockets
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
                NextState(StateReconnecting);
                Context.CleanupTransmission();
                Context.ScheduleConnectTimer(delayMs);
            }
            else
            {
                NextState(StateListening);
                Context.CleanupTransmission();
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
