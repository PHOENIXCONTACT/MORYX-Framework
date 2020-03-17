// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Runtime.Serialization;
using Marvin.Logging;
using Marvin.Modules;
using Marvin.Serialization;

namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Base class for all resources to reduce boilerplate code
    /// </summary>
    [DataContract]
    public abstract class Resource : ILoggingComponent, IResource, IInitializablePlugin, IDisposable, IPersistentObject
    {
        #region Dependencies

        /// <summary>
        /// Logger for this resource
        /// </summary>
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Reference to the graph for interaction
        /// </summary>
        public IResourceGraph Graph { get; set; }

        #endregion

        /// <summary>Database key of the resource</summary>
        public long Id { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <summary>
        /// Description of this resource
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Descriptor to provide access to this resource. Either the descriptor or 
        /// its properties and methods need to flagged with <see cref="EditorVisibleAttribute"/> 
        /// </summary>
        public virtual object Descriptor => this;

        /// <summary>
        /// Parent resource of this resource
        /// </summary>
        [ResourceReference(ResourceRelationType.ParentChild, ResourceReferenceRole.Source)]
        public Resource Parent { get; set; }

        /// <summary>
        /// All children of this resource
        /// </summary>
        [ResourceReference(ResourceRelationType.ParentChild, ResourceReferenceRole.Target)]
        public IReferences<Resource> Children { get; set; }

        /// <inheritdoc />
        void IInitializable.Initialize()
        {
            Logger = Logger?.GetChild(Name, GetType());
            OnInitialize();
        }

        /// <summary>
        /// Resource specific implementation of <see cref="IInitializable.Initialize"/>
        /// </summary>
        protected virtual void OnInitialize()
        {
        }

        /// <inheritdoc />
        void IPlugin.Start()
        {
            OnStart();
        }

        /// <summary>
        /// Resource specific implementation of <see cref="IPlugin.Start"/>
        /// </summary>
        protected virtual void OnStart()
        {
        }

        /// <inheritdoc />
        void IPlugin.Stop()
        {
            OnStop();
        }

        /// <summary>
        /// Resource specific implementation of <see cref="IPlugin.Stop"/>
        /// </summary>
        protected virtual void OnStop()
        {
        }

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            // Remove graph reference, just to make sure
            Graph = null;

            try
            {
                OnDispose();
            }
            catch (Exception e)
            {
                Logger?.LogException(LogLevel.Error, e, "Failed to dispose resource {0}-{1}", Id, Name);
            }
        }

        /// <summary>
        /// Resource specific implementation of Dispose instead of overriding <see cref="IDisposable.Dispose"/>.
        /// This ensures, that developers do not accidentally forget to call <code>base.Dispose()</code> and
        /// create a memory leak
        /// </summary>
        protected virtual void OnDispose()
        {
        }

        /// <summary>
        /// Inform the resource management, that this instance was modified 
        /// and trigger saving the current state to storage
        /// </summary>
        protected void RaiseResourceChanged()
        {
            // This is only null during boot, when the resource manager populates the object
            Changed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Return Id, name and type of the resource
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Id}:{Name} ({GetType().Name})";
        }

        /// <summary>
        /// Event raised when the resource was modified and the changes should be
        /// written to the data storage
        /// </summary>
        public event EventHandler Changed;
    }
}
