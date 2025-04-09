// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.StateMachines;

namespace Moryx.Communication.Sockets
{
    internal abstract class ServerStateBase : StateBase<TcpListenerConnection>
    {
        public BinaryConnectionState CurrentState { get; }

        protected ServerStateBase(TcpListenerConnection context, StateMap stateMap, BinaryConnectionState connectionState) : base(context, stateMap)
        {
            CurrentState = connectionState;
        }

        public virtual void Open()
        {
        }

        public virtual void Reconnect(int delayMs)
        {
        }

        public virtual void Close()
        {
        }

        public virtual void ConnectionAssigned(TcpTransmission transmission, BinaryMessage message)
        {
        }

        public virtual void ConnectionLost()
        {
        }

        public virtual void ScheduledConnectTimerElapsed()
        {
        }

        public virtual void Send(BinaryMessage message)
        {
            InvalidState();
        }

        public virtual Task SendAsync(BinaryMessage message)
        {
            return InvalidStateAsync();
        }

        [StateDefinition(typeof(ServerConnectedState))]
        protected const int StateConnected = 10;

        [StateDefinition(typeof(ServerListeningState))]
        protected const int StateListening = 20;

        [StateDefinition(typeof(ServerNotListeningState), IsInitial = true)]
        protected const int StateNotListening = 30;

        [StateDefinition(typeof(ServerReconnectingState))]
        protected const int StateReconnecting = 40;

    }
}
