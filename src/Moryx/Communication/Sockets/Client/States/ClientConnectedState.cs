// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;

namespace Moryx.Communication.Sockets
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

        public override Task SendAsync(BinaryMessage message)
        {
            return Context.ExecuteSendAsync(message);
        }

        public override void Reconnect(int delayMs)
        {
            Context.ReconnectDelayMs = delayMs;
            NextState(StateReconnecting);
        }

        public override void Disconnect()
        {
            NextState(StateDisconnecting);
        }

        public override void ConnectionClosed()
        {
            Context.Disconnect();
            NextState(StateConnecting);
        }

        public override void ScheduledConnectTimerElapsed()
        {
            NextState(StateConnecting);
        }
    }
}
