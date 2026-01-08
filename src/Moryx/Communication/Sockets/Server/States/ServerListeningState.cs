// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Communication.Sockets;

internal class ServerListeningState : ServerStateBase
{
    public ServerListeningState(TcpListenerConnection context, StateMap stateMap) : base(context, stateMap, BinaryConnectionState.AttemptingConnection)
    {
    }

    public override void ConnectionAssigned(TcpTransmission transmission, BinaryMessage message)
    {
        Context.ExecuteAssignConnection(transmission);
        NextState(StateConnected);
        if (message != null)
            Context.PublishInitialMessage(message);
    }

    public override void Close()
    {
        Context.Unregister();
        NextState(StateNotListening);
    }
}