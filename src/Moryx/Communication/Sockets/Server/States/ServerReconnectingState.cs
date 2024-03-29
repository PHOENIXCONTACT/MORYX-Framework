// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication.Sockets
{
    internal class ServerReconnectingState : ServerStateBase
    {
        public ServerReconnectingState(TcpListenerConnection context, StateMap stateMap) : base(context, stateMap, BinaryConnectionState.AttemptingConnection)
        {
        }

        public override void ScheduledConnectTimerElapsed()
        {
            NextState(StateListening);
            Context.Register();
        }

        public override void Close()
        {
            NextState(StateNotListening);
        }
    }
}
