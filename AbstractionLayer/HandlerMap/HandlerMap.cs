using System;
using System.Collections.Generic;
using System.Linq;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Base class for 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HandlerMap<T> : IHandlerMap<T>
    {
        private readonly string _handlerName;

        /// <summary>
        /// Initialize handler map with given name for exceptions
        /// </summary>
        /// <param name="handlerName">Name of the handler owning this map</param>
        public HandlerMap(string handlerName)
        {
            _handlerName = handlerName;
        }

        /// <summary>
        /// Map of registered handlers for each message type
        /// </summary>
        private readonly IList<IHandler> _handlers = new List<IHandler>();

        /// <summary>
        /// Add handler to the map
        /// </summary>
        protected void AddHandler(IHandler handler)
        {
            _handlers.Add(handler);
        }

        /// <summary>
        /// Handler used when the key is not present in the map. If this is null an exception is thrown
        /// </summary>
        protected IHandler DefaultHandler { get; set; }

        /// <summary>
        /// Distribution message handler
        /// </summary>
        public void Handle(T message)
        {
            ExecuteOnHandler(null, message);
        }

        /// <summary>
        /// Handle that matches the <see cref="EventHandler{TEventArgs}"/> signature
        /// </summary>
        public void ReceivedHandler(object sender, T message)
        {
            ExecuteOnHandler(sender, message);
        }

        private void ExecuteOnHandler(object sender, T message)
        {
            var handler = _handlers.FirstOrDefault(h => h.CanHandle(message));
            if (handler != null)
            {
                handler.Handle(sender, message);
            }
            else if (DefaultHandler != null)
            {
                DefaultHandler.Handle(sender, message);
            }
            else
            {
                throw new InvalidOperationException($"{_handlerName} can not handle this object!");
            }
        }

        /// <summary>
        /// Register a new handler
        /// </summary>
        /// <param name="handler"></param>
        /// <typeparam name="TArgument">Type of the method argument inferred from delegate</typeparam>
        /// <returns></returns>
        public HandlerMap<T> Register<TArgument>(Action<TArgument> handler)
            where TArgument : T
        {
            AddHandler(new CastHandler<TArgument>(handler));
            return this;
        }

        /// <summary>
        /// Register a new handler
        /// </summary>
        /// <param name="handler"></param>
        /// <typeparam name="TArgument">Type of the method argument inferred from delegate</typeparam>
        /// <returns></returns>
        public HandlerMap<T> Register<TArgument>(Action<object, TArgument> handler)
            where TArgument : T
        {
            AddHandler(new CastEventHandler<TArgument>(handler));
            return this;
        }

        /// <summary>
        /// Default handler if no key matches
        /// </summary>
        public HandlerMap<T> Default(Action<T> handler)
        {
            DefaultHandler = new SimpleHandler(handler);
            return this;
        }

        /// <summary>
        /// Default handler if no key matches
        /// </summary>
        public HandlerMap<T> Default(Action<object, T> handler)
        {
            DefaultHandler = new SimpleEventHandler(handler);
            return this;
        }

        /// <summary>
        /// Interface for all generic handler classes
        /// </summary>
        protected interface IHandler
        {
            /// <summary>
            /// Check if this handler can handle the message
            /// </summary>
            bool CanHandle(T message);

            /// <summary>
            /// Handle it for gods sake
            /// </summary>
            void Handle(object sender, T message);
        }

        /// <summary>
        /// Handler implementation that checks type compliance and casts
        /// objects for the callback
        /// </summary>
        protected struct CastHandler<TArgument> : IHandler
            where TArgument : T
        {
            private readonly Action<TArgument> _handler;

            /// <summary>
            /// Create new <see cref="CastHandler{T}"/> for 
            /// a typed callback.
            /// </summary>
            /// <param name="handler"></param>
            public CastHandler(Action<TArgument> handler)
            {
                _handler = handler;
            }

            /// <inheritdoc />
            public bool CanHandle(T message)
            {
                return message is TArgument;
            }

            /// <inheritdoc />
            public void Handle(object sender, T message)
            {
                _handler((TArgument)message);
            }
        }

        /// <summary>
        /// Handler implementation for event handlers that checks type 
        /// compliance and casts objects for the callback
        /// </summary>
        protected struct CastEventHandler<TArgument> : IHandler
            where TArgument : T
        {
            private readonly Action<object, TArgument> _handler;

            /// <summary>
            /// Create new <see cref="CastHandler{T}"/> for 
            /// a typed callback.
            /// </summary>
            /// <param name="handler"></param>
            public CastEventHandler(Action<object, TArgument> handler)
            {
                _handler = handler;
            }

            /// <inheritdoc />
            public bool CanHandle(T message)
            {
                return message is TArgument;
            }

            /// <inheritdoc />
            public void Handle(object sender, T message)
            {
                _handler(sender, (TArgument)message);
            }
        }

        /// <summary>
        /// Default handler implementation
        /// </summary>
        protected struct SimpleHandler : IHandler
        {
            private readonly Action<T> _handler;

            /// <summary>
            /// Create default handler with delegate
            /// </summary>
            public SimpleHandler(Action<T> handler)
            {
                _handler = handler;
            }

            /// <inheritdoc />
            public bool CanHandle(T message)
            {
                return true;
            }

            /// <inheritdoc />
            public void Handle(object sender, T message)
            {
                _handler(message);
            }
        }

        /// <summary>
        /// Default handler implementation
        /// </summary>
        protected struct SimpleEventHandler : IHandler
        {
            private readonly Action<object, T> _handler;

            /// <summary>
            /// Create default handler with delegate
            /// </summary>
            public SimpleEventHandler(Action<object, T> handler)
            {
                _handler = handler;
            }

            /// <inheritdoc />
            public bool CanHandle(T message)
            {
                return true;
            }

            /// <inheritdoc />
            public void Handle(object sender, T message)
            {
                _handler(sender, message);
            }
        }
    }
}