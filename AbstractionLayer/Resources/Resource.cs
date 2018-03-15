using System;
using System.Runtime.Serialization;
using Marvin.Logging;
using Marvin.Modules;
using Marvin.Serialization;
using Marvin.StateMachines;

namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Base class for all resources to reduce boilerplate code
    /// </summary>
    [DataContract]
    public abstract class Resource : ILoggingComponent, IResource, IPlugin, IPersistentObject
    {
        #region Dependencies

        /// <summary>
        /// Logger for this resource
        /// </summary>
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Type controller to construct new resources
        /// </summary>
        public IResourceCreator Creator { get; set; }

        #endregion

        /// 
        public long Id { get; set; }

        ///
        public string Name { get; set; }

        /// 
        public string LocalIdentifier { get; set; }

        ///
        public string GlobalIdentifier { get; set; }

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

        ///
        public virtual void Initialize()
        {
            Logger = Logger?.GetChild(Name, GetType());
        }

        ///
        public virtual void Start()
        {
        }

        /// <summary>
        /// Method invoked to stop a resource instances execution
        /// </summary>
        public virtual void Stop()
        {
        }

        /// 
        public void Dispose()
        {
            // Remove type controller reference, just to make sure
            Creator = null;

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
        /// Resource specific implementation of Dispose instead of overriding <see cref="Dispose"/>.
        /// This ensures, that developers do not accidently forget to call <code>base.Dispose()</code> and
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
        /// Event raised when the resource was modified and the changes should be
        /// written to the data storage
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Descriptor to provide access to this resource. Either the descriptor or 
        /// its properties and methods need to flagged with <see cref="EditorVisibleAttribute"/> 
        /// </summary>
        public virtual object Descriptor => this;

        /// <summary>
        /// Return Id, name and type of the resource
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Id}:{Name} ({GetType().Name})";
        }
 
    }
}