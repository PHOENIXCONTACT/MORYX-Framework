// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Communication;

namespace Moryx.AbstractionLayer.Drivers.Plc
{
    /// <summary>
    /// Driver for programmable logic controllers that support object communication
    /// </summary>
    [Obsolete("This will be removed soon, use IMessageDriver instead")]
    public interface IPlcDriver : IDriver, IPlcCommunication
    {
        /// <summary>
        /// Register a message hock at the plc driver
        /// </summary>
        /// <param name="hook">Instance of a message hook</param>
        void Register(IBinaryMessageHook hook);

        /// <summary>
        /// Access a filtered connection sharing the same socket
        /// </summary>
        IPlcCommunication this[string identifier] { get; }
    }

    /// <summary>
    /// Extension of the <see cref="IPlcDriver"/> interface that allows changing serializer
    /// </summary>
    /// <typeparam name="THeader"></typeparam>
    [Obsolete("This will be removed soon, use IMessageDriver instead")]
    public interface IPlcDriver<THeader> : IPlcDriver
        where THeader : IBinaryHeader, new()
    {
        /// <summary>
        /// Serializer used by this plc driver instance
        /// </summary>
        IByteSerializer<THeader> Serializer { get; set; }
    }
}
