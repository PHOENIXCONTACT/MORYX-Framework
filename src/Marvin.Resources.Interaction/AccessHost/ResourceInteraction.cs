using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Marvin.AbstractionLayer.Resources;
using Marvin.Container;
using Marvin.Resources.Interaction.Converter;
using Marvin.Serialization;

namespace Marvin.Resources.Interaction
{
    /// <seealso cref="IResourceInteraction"/>
    [Plugin(LifeCycle.Singleton, typeof(IResourceInteraction))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ResourceInteraction : IResourceInteraction
    {
        #region Dependency Injection

        /// <summary>
        /// Injected by castle.
        /// </summary>
        public ICustomSerialization Serialization { get; set; }

        /// <summary>
        /// Factory to create resource instances
        /// </summary>
        public IResourceGraph Graph { get; set; }

        /// <summary>
        /// Type controller for type trees and construction
        /// </summary>
        public IResourceTypeTree TypeController { get; set; }

        #endregion

        /// <inheritdoc />
        public ResourceTypeModel[] GetTypeTree()
        {
            return TypeController.RootTypes.Select(t => ResourceToModelConverter.ConvertType(t, Serialization)).ToArray();
        }

        /// <inheritdoc />
        public ResourceModel[] GetResourceTree()
        {
            var converter = new ResourceToModelConverter(Graph, TypeController, Serialization);
            var resourceTree = Graph.GetResources<Resource>(r => r.Parent == null).Select(converter.ConvertResource).ToArray();
            return resourceTree;
        }

        /// <inheritdoc />
        public ResourceModel GetDetails(long id, int depth = 1)
        {
            var converter = new ResourceToModelConverter(Graph, TypeController, Serialization);

            //Additionally load workpieces 
            var resource = Graph.Get(id);
            var model = converter.GetDetails(resource, depth);

            return model;
        }

        /// <inheritdoc />
        public Entry InvokeMethod(long id, MethodEntry methodModel)
        {
            var resource = Graph.Get(id);
            return EntryConvert.InvokeMethod(resource, methodModel, Serialization);
        }

        /// <inheritdoc />
        public ResourceModel Create(string resourceType, MethodEntry constructor = null)
        {
            var converter = new ResourceToModelConverter(Graph, TypeController, Serialization);

            var resource = Graph.Instantiate(resourceType);
            if (constructor != null)
                EntryConvert.InvokeMethod(resource, constructor, Serialization);

            var model = converter.GetDetails(resource, int.MaxValue);
            model.Methods = new MethodEntry[0]; // Reset methods because the can not be invoked on new objects

            return model;
        }

        /// <inheritdoc />
        public ResourceModel Save(ResourceModel model)
        {
            var converter = new ModelToResourceConverter(Graph, Serialization);

            var resourcesToSave = new HashSet<Resource>();
            // Get or create resource
            converter.FromModel(model, resourcesToSave);

            // Save all created or altered resources
            foreach (var resourceToSave in resourcesToSave)
            {
                Graph.Save(resourceToSave);
            }

            return model;
        }

        /// <inheritdoc />
        public bool Remove(long id)
        {
            var resource = Graph.Get(id);
            return Graph.Destroy(resource);
        }
    }
}