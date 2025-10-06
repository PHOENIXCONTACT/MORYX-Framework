// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication.Sockets
{
    internal class ClientDisconnectingState : ClientStateBase
    {
        public ClientDisconnectingState(TcpClientConnection context, StateMap stateMap) : base(context, stateMap, BinaryConnectionState.Connected)
        {
        }

        public override void OnEnter()
        {
            Context.Disconnect();
        }

        public override void ConnectionClosed()
        {
            NextState(StateDisconnected);
        }
    }
}
