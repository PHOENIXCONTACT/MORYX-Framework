// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Interface for handler classes used to distribute mixed type messages to specialized handler methods
    /// </summary>
    public interface IHandlerMap<in T>
    {
        /// <summary>
        /// Distribution message handler
        /// </summary>
        void Handle(T message);

        /// <summary>
        /// Event handler for events of type <see cref="EventHandler{T}"/> that uses the <see cref="IHandlerMap{T}"/>
        /// </summary>
        void ReceivedHandler(object sender, T message);
    }
}
