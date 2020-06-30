// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.Container;
using Marvin.Logging;

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// Base class of a WebService Manager
    /// </summary>
    public abstract class ServiceManager<T> : IServiceManager, ILoggingComponent
        where T : class, ISessionService
    {
        /// <summary>
        /// Module logger injected by Castle
        /// </summary>
        [UseChild("WcfService")]
        public IModuleLogger Logger { get; set; }

        private readonly List<T> _services = new List<T>();

        /// <summary>
        /// Thread safe snapshot of the currently subscribed clients
        /// </summary>
        protected IEnumerable<T> Services
        {
            get
            {
                lock (_services)
                    return _services.ToArray();
            }
        }

        /// <see cref="IServiceManager.Register(ISessionService)"/>
        void IServiceManager.Register(ISessionService service)
        {
            T casted = service as T;
            if (casted == null)
                throw new ArgumentException("Type of service does not match managed type", nameof(service));

            lock (_services)
            {
                _services.Add(casted);
            }

            Initialize(casted);
        }

        /// <summary>
        /// Initialize the new connection
        /// </summary>
        /// <param name="connection">New connection</param>
        protected virtual void Initialize(T connection)
        {

        }

        /// <see cref="IServiceManager.Unregister(ISessionService)"/>
        void IServiceManager.Unregister(ISessionService connection)
        {
            T casted = connection as T;
            if (casted == null)
                return;

            lock (_services)
            {
                _services.Remove(casted);
            }
        }

        /// <summary>
        /// Get all connections with this id
        /// </summary>
        protected T[] this[string clientId]
        {
            get { return _services.Where(c => c.ClientId == clientId).ToArray(); }
        }

        /// <summary>
        /// Invoke push method on this client
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="instanceCall"></param>
        protected void Push(string clientId, Action<T> instanceCall)
        {
            foreach (var client in this[clientId])
            {
                instanceCall(client);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var service in Services)
            {
                service.Close();
            }

            lock(_services)
                _services.Clear();
        }
    }
}
