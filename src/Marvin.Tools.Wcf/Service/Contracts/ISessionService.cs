// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ServiceModel;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract]
    public interface ISessionService
    {
        /// <summary>
        /// Unique ClientId
        /// </summary>
        string ClientId { get; }

        /// <summary>
        /// Register Client on Servicehost
        /// </summary>
        /// <param name="clientId"></param>
        [OperationContract]
        void Subscribe(string clientId);

        /// <summary>
        /// Clients may send Heartbeat() calls 
        /// </summary>
        /// <param name="beat">Unique id of this heart beat used to identify the call</param>
        [OperationContract]
        long Heartbeat(long beat);

        /// <summary>
        /// Unregister Client from Servicehost
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Unsubscribe();

        /// <summary>
        /// Close this session connection
        /// </summary>
        void Close();
    }
}
