// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.Communication.Sockets.IntegrationTests;

public class ConnectionBuffer<TMessage> where TMessage : BinaryMessage
{
    public ConnectionBuffer()
    {
        Received = [];
        LastStateChangeEvents = [];
    }

    public IBinaryConnection Connection { get; set; }

    public List<TMessage> Received { get; }

    public int Id { get; set; }

    public List<BinaryConnectionState> LastStateChangeEvents { get; }
}