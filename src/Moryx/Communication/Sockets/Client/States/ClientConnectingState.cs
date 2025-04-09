// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net.Sockets;

namespace Moryx.Communication.Sockets
{
    internal class ClientConnectingState : ClientStateBase
    {
        public ClientConnectingState(TcpClientConnection context, StateMap stateMap) : base(context, stateMap, BinaryConnectionState.AttemptingConnection)
        {
        }

        public override void OnEnter()
        {
            Context.Connect();
        }

        public override void ConnectionCallback(IAsyncResult ar, TcpClient tcpClient)
        {
            try
            {
                tcpClient.EndConnect(ar);
                Context.Connected();
                NextState(StateConnected);
            }
            catch (Exception)
            {
                if (Context.Config.RetryWaitMs > 0)
                {
                    NextState(StateRetryConnect);
                    Context.ScheduleConnectTimer(Context.Config.RetryWaitMs);
                }
                else
                {
                    Context.Connect();
                }
            }
        }

        public override void Disconnect()
        {
            NextState(StateDisconnected);
            Context.CloseClient();
        }
    }
}
