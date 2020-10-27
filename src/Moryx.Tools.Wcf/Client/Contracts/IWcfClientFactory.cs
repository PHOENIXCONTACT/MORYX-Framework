// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Public API of a WcfClientFactory.
    /// </summary>
    public interface IWcfClientFactory : IDisposable
    {
        /// <summary>
        /// Retrieves information about the current state of all WCF clients managed byy this factory.
        /// </summary>
        IEnumerable<WcfClientInfo> ClientInfos { get; }

        /// <summary>
        /// The Id used by all NetTcp clients managed by this factory to subscribe to the service.
        /// </summary>
        string ClientId { get; }

        /// <summary>
        /// Gets the client information by the identifier.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        WcfClientInfo GetClientById(long clientId);

        /// <summary>
        /// Raised when a client is disconnected from its service. The service's name is passed to the event handler.
        /// </summary>
        event EventHandler<string> ClientDisconnected;

        /// <summary>
        /// Raised when a client is connected to its service. The service's name is passed to the event handler.
        /// </summary>
        event EventHandler<string> ClientConnected;

        /// <summary>
        /// Raised when a client's state changes. The client's WcfClientInfo object is passed to the event handler.
        /// </summary>
        event EventHandler<WcfClientInfo> ClientInfoChanged;

        /// <summary>
        /// Creates a new WCF client.
        /// </summary>
        /// <typeparam name="T">The client implementation provided by the WCF service reference.</typeparam>
        /// <typeparam name="TK">The client interface provided by the WCF service reference.</typeparam>
        /// <param name="config">The client's configuration</param>
        /// <param name="callback">A callback to inform about changes of the client's state.</param>
        /// <returns>A unique ID identifying this client</returns>
        long Create<T, TK>(ClientConfig config, Action<ConnectionState, T> callback)
            where T : ClientBase<TK>
            where TK : class;

        /// <summary>
        /// Creates a new WCF client.
        /// </summary>
        /// <typeparam name="T">The client implementation provided by the WCF service reference.</typeparam>
        /// <typeparam name="TK">The client interface provided by the WCF service reference.</typeparam>
        /// <param name="config">The client's configuration</param>
        /// <param name="callbackService">For NetTcp clients a callback object for WCF callback can be passed here. For BasicHttp clients, this argument is ignored.</param>
        /// <param name="callback">A callback to inform about changes of the client's state.</param>
        /// <returns>A unique ID identifying this client</returns>
        long Create<T, TK>(ClientConfig config, object callbackService, Action<ConnectionState, T> callback)
            where T : ClientBase<TK>
            where TK : class;

        /// <summary>
        /// Creates a new WCF client.
        /// </summary>
        /// <typeparam name="T">The client implementation provided by the WCF service reference.</typeparam>
        /// <typeparam name="TK">The client interface provided by the WCF service reference.</typeparam>
        /// <param name="config">The client's configuration</param>
        /// <param name="callback">A callback to inform about changes of the client's state.</param>
        /// <param name="binding">The binding to be used for the new client.</param>
        /// <returns>A unique ID identifying this client</returns>
        long Create<T, TK>(ClientConfig config, Action<ConnectionState, T> callback, Binding binding)
            where T : ClientBase<TK>
            where TK : class;

        /// <summary>
        /// Creates a new WCF client.
        /// </summary>
        /// <typeparam name="T">The client implementation provided by the WCF service reference.</typeparam>
        /// <typeparam name="TK">The client interface provided by the WCF service reference.</typeparam>
        /// <param name="config">The client's configuration</param>
        /// <param name="callbackService">For NetTcp clients a callback object for WCF callback can be passed here. For BasicHttp clients, this argument is ignored.</param>
        /// <param name="callback">A callback to inform about changes of the client's state.</param>
        /// <param name="binding">The binding to be used for the new client.</param>
        /// <returns>A unique ID identifying this client</returns>
        long Create<T, TK>(ClientConfig config, object callbackService, Action<ConnectionState, T> callback, Binding binding)
            where T : ClientBase<TK>
            where TK : class;

        /// <summary>
        /// Creates a new WCF client.
        /// </summary>
        /// <typeparam name="T">The client implementation provided by the WCF service reference.</typeparam>
        /// <typeparam name="TK">The client interface provided by the WCF service reference.</typeparam>
        /// <param name="clientVersion">The version of the client a dotted quad (eg. 1.0.0.0)</param>
        /// <param name="callback">A callback to inform about changes of the client's state.</param>
        /// <returns>A unique ID identifying this client</returns>
        long Create<T, TK>(string clientVersion, Action<ConnectionState, T> callback)
            where T : ClientBase<TK>
            where TK : class;

        /// <summary>
        /// Creates a new WCF client.
        /// </summary>
        /// <typeparam name="T">The client implementation provided by the WCF service reference.</typeparam>
        /// <typeparam name="TK">The client interface provided by the WCF service reference.</typeparam>
        /// <param name="clientVersion">The version of the client a dotted quad (eg. 1.0.0.0)</param>
        /// <param name="callbackService">For NetTcp clients a callback object for WCF callback can be passed here. For BasicHttp clients, this argument is ignored.</param>
        /// <param name="callback">A callback to inform about changes of the client's state.</param>
        /// <returns>A unique ID identifying this client</returns>
        long Create<T, TK>(string clientVersion, object callbackService, Action<ConnectionState, T> callback)
            where T : ClientBase<TK>
            where TK : class;

        /// <summary>
        /// Creates a new WCF client.
        /// </summary>
        /// <typeparam name="T">The client implementation provided by the WCF service reference.</typeparam>
        /// <typeparam name="TK">The client interface provided by the WCF service reference.</typeparam>
        /// <param name="clientVersion">The version of the client a dotted quad (eg. 1.0.0.0)</param>
        /// <param name="callback">A callback to inform about changes of the client's state.</param>
        /// <param name="binding">The binding to be used for the new client.</param>
        /// <returns>A unique ID identifying this client</returns>
        long Create<T, TK>(string clientVersion, Action<ConnectionState, T> callback, Binding binding)
            where T : ClientBase<TK>
            where TK : class;

        /// <summary>
        /// Creates a new WCF client.
        /// </summary>
        /// <typeparam name="T">The client implementation provided by the WCF service reference.</typeparam>
        /// <typeparam name="TK">The client interface provided by the WCF service reference.</typeparam>
        /// <param name="clientVersion">The version of the client a dotted quad (eg. 1.0.0.0)</param>
        /// <param name="callbackService">For NetTcp clients a callback object for WCF callback can be passed here. For BasicHttp clients, this argument is ignored.</param>
        /// <param name="callback">A callback to inform about changes of the client's state.</param>
        /// <param name="binding">The binding to be used for the new client.</param>
        /// <returns>A unique ID identifying this client</returns>
        long Create<T, TK>(string clientVersion, object callbackService, Action<ConnectionState, T> callback, Binding binding)
            where T : ClientBase<TK>
            where TK : class;

        /// <summary>
        /// Destroys a WCF client
        /// </summary>
        /// <param name="clientId">The client's ID as returned by CreateClient()</param>
        /// <exception cref="InvalidOperationException">If the ID is unknown.</exception>
        void Destroy(long clientId);
    }
}
