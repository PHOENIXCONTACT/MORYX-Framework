using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text.RegularExpressions;
using Marvin.AbstractionLayer.Resources;
using Marvin.Container;
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
        public IResourceManager Manager { get; set; }

        /// <summary>
        /// Type controller for type trees and construction
        /// </summary>
        public IResourceTypeTree TypeController { get; set; }

        #endregion

        /// <summary>
        /// Type cache to avoid redundant conversions AND make use of WCFs "IsReference" feature
        /// </summary>
        private readonly Dictionary<string, ResourceTypeModel> _typeCache = new Dictionary<string, ResourceTypeModel>();

        /// <inheritdoc />
        public ResourceTypeModel[] GetTypeTree()
        {
            return TypeController.RootTypes.Select(ConvertType).ToArray();
        }

        /// <inheritdoc />
        public ResourceModel[] GetResourceTree()
        {
            return Manager.GetRoots().Select(ConvertResource).ToArray();
        }

        private ResourceReferenceModel[] ChildrenOnly(Resource current)
        {
            // Get the children reference
            var childrenProperty = current.GetType().GetProperty(nameof(Resource.Children));
            // Find possible overrides on the children reference
            var referenceOverrides = GetReferenceOverrides(current.GetType().GetProperties());

            var model = ConvertReference(current, childrenProperty, referenceOverrides);
            model.Targets = current.Children.Select(ConvertResource).ToList();

            return new[] { model };
        }

        private ResourceModel ConvertResource(Resource resource)
        {
            return new ResourceModel
            {
                Id = resource.Id,
                Name = resource.Name,
                LocalIdentifier = resource.LocalIdentifier,
                GlobalIdentifier = resource.GlobalIdentifier,
                Description = resource.Description,
                Type = resource.GetType().Name,
                References = ChildrenOnly(resource)
            };
        }

        /// <inheritdoc />
        public ResourceModel GetDetails(long id, int depth = 1)
        {
            //Additionally load workpieces 
            var resource = Manager.Get(id);
            var model = GetDetails(resource, depth);
            return model;
        }

        /// <inheritdoc />
        public Entry InvokeMethod(long id, MethodEntry methodModel)
        {
            var resource = Manager.Get(id);
            return EntryConvert.InvokeMethod(resource, methodModel, Serialization);
        }

        /// <inheritdoc />
        public ResourceModel Create(string resourceType, long parentResourceId, MethodEntry constructor = null)
        {
            var resource = Manager.Create(resourceType);
            if (constructor != null)
                EntryConvert.InvokeMethod(resource, constructor);

            var model = GetDetails(resource, int.MaxValue);
            model.ParentId = parentResourceId;
            model.Methods = new MethodEntry[0]; // Reset methods because the can not be invoked on new objects

            return model;
        }

        /// <inheritdoc />
        public ResourceModel Save(ResourceModel model)
        {
            // Get or create resource
            var resource = FromModel(model);

            // Add new resource to its parent
            if (model.ParentId > 0 && resource.Parent == null)
            {
                // Find new parent and establish the reference
                var parent = Manager.Get(model.ParentId);
                resource.Parent = parent;
                parent.Children.Add(resource);
            }

            Manager.Save(resource);

            return model;
        }

        /// <inheritdoc />
        /// 
        public bool Start(long id)
        {
            var resource = Manager.Get(id);
            return Manager.Start(resource);
        }

        /// <inheritdoc />
        public bool Reset(long id)
        {
            var resource = Manager.Get(id);
            return Manager.Start(resource) && Manager.Stop(resource);
        }

        /// <inheritdoc />
        public bool Stop(long id)
        {
            var resource = Manager.Get(id);
            return Manager.Stop(resource);
        }

        /// <inheritdoc />
        public bool Remove(long id)
        {
            var resource = Manager.Get(id);
            return Manager.Destroy(resource);
        }

        #region GetDetails

        /// <summary>
        /// Recursive function to load resource and its children
        /// </summary>
        /// <param name="current">Current instance</param>
        /// <param name="depth"></param>
        /// <returns></returns>
        private ResourceModel GetDetails(Resource current, int depth)
        {
            // Extract model
            return new ResourceModel
            {
                Id = current.Id,
                ParentId = current.Parent?.Id ?? 0,
                Name = current.Name,
                LocalIdentifier = current.LocalIdentifier,
                GlobalIdentifier = current.GlobalIdentifier,
                Description = current.Description,

                // Use simplified type reference
                Type = current.GetType().Name,

                // Recursively read children and references
                References = ConvertReferences(current, depth),

                // Properties and methods are read from the descriptor
                // This can be the resource itself or a dedicated object
                Properties = EntryConvert.EncodeObject(current.Descriptor, Serialization).ToList(),
                Methods = EntryConvert.EncodeMethods(current.Descriptor, Serialization).ToArray()
            };
        }

        #endregion

        #region Convert References

        /// <summary>
        /// Convert all references of a resource
        /// </summary>
        private ResourceReferenceModel[] ConvertReferences(Resource current, int depth)
        {
            var properties = current.GetType().GetProperties();
            // Find all reference properties on the object
            var referenceProperties = GetReferences(properties);
            // Find all properties that define a type override of existing references
            var referenceOverrides = GetReferenceOverrides(properties);

            // Convert all references to DTOs
            return referenceProperties.Select(prop => ConvertReference(current, prop, referenceOverrides, depth)).ToArray();
        }

        /// <summary>
        /// Get all references 
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetReferences(IEnumerable<PropertyInfo> properties)
        {
            var referenceProperties = (from prop in properties
                                       let propType = prop.PropertyType
                                       // Find all properties referencing a resource a collection of resources
                                       where prop.CanWrite && Attribute.IsDefined(prop, typeof(ResourceReferenceAttribute))
                                       // Exclude read only properties, because they are simple type overrides of other references
                                       // Filter parent to break recursion
                                       where prop.Name != nameof(Resource.Parent)
                                       select prop).ToList();
            return referenceProperties;
        }


        /// <summary>
        /// Get all reference overrides from a resources properties
        /// </summary>
        private static Dictionary<string, List<Type>> GetReferenceOverrides(IEnumerable<PropertyInfo> properties)
        {
            var referenceOverrides = (from prop in properties
                                      let overrideAtt = prop.GetCustomAttribute<ReferenceOverrideAttribute>()
                                      where overrideAtt != null
                                      let targetType = typeof(IEnumerable<IResource>).IsAssignableFrom(prop.PropertyType)
                                        ? EntryConvert.ElementType(prop.PropertyType) : prop.PropertyType
                                      group targetType by overrideAtt.Source into g
                                      select new { g.Key, overrides = g.ToList() }).ToDictionary(v => v.Key, v => v.overrides);
            return referenceOverrides;
        }

        /// <summary>
        /// Convert a property referencing another resource into a <see cref="ResourceReferenceModel"/>
        /// </summary>
        private ResourceReferenceModel ConvertReference(Resource current, PropertyInfo property, IDictionary<string, List<Type>> overrides, int depth = 0)
        {
            var attribute = property.GetCustomAttribute<ResourceReferenceAttribute>();

            // Create reference model from property information and optional attribute
            var referenceModel = new ResourceReferenceModel
            {
                Name = property.Name,
                Description = property.GetCustomAttribute<DescriptionAttribute>()?.Description,

                Role = attribute.Role,
                RelationType = attribute.RelationType,
                IsCollection = typeof(IEnumerable<IResource>).IsAssignableFrom(property.PropertyType)
            };

            // Get type constraints
            Type targetType = property.PropertyType;
            if (referenceModel.IsCollection)
                targetType = EntryConvert.ElementType(targetType);
            var typeConstraints = MergeTypeConstraints(property, targetType, overrides);
            referenceModel.SupportedTypes = SupportedTypes(typeConstraints);

            // Only load possible targets if we are supposed to
            if (depth <= 0)
                return referenceModel;

            // Load possible targets from the full set of resources
            referenceModel.PossibleTargets = MatchingInstances(typeConstraints).ToList();

            // We can not load targets if we do not have any
            referenceModel.Targets = new List<ResourceModel>();
            var value = property.GetValue(current);
            if (value == null)
                return referenceModel;

            // Convert referenced resource objects and possible instance types 
            var referenceTargets = referenceModel.IsCollection ? (IEnumerable<IResource>)value : new[] { (IResource)value };
            foreach (Resource resource in referenceTargets)
            {
                var target = GetDetails(resource, depth - 1);
                referenceModel.Targets.Add(target);
                // Possible targets must always include the current target, even if its not part of the tree
                if (referenceModel.PossibleTargets.All(pt => pt.Id != target.Id))
                    referenceModel.PossibleTargets.Add(target);
            }

            return referenceModel;
        }

        /// <summary>
        /// Determine all type constraints for this reference property. Type constraints are created by the usage
        /// of <see cref="ReferenceOverrideAttribute"/> on readonly properties to limit allowed types for the referenced property
        /// </summary>
        private static ICollection<Type> MergeTypeConstraints(PropertyInfo property, Type targetType, IDictionary<string, List<Type>> referenceOverrides)
        {
            // If there are no overrides the only limitation is the target type
            if (!referenceOverrides.ContainsKey(property.Name))
                return new[] { targetType };

            // Otherwise find all types that limit the reference type without redundancies. This means eliminating all types
            // represented by another type
            var myOverrides = referenceOverrides[property.Name];
            return myOverrides.Concat(new[] { targetType })
                .Where(type => !myOverrides.Any(over => over != type && type.IsAssignableFrom(over))).ToList();
        }

        /// <summary>
        /// Convert the supported types 
        /// </summary>
        private ResourceTypeModel[] SupportedTypes(ICollection<Type> typeConstraints)
        {
            return TypeController.SupportedTypes(typeConstraints).Select(ConvertType).ToArray();
        }

        /// <summary>
        /// Updates the references of a resource
        /// </summary>
        private void UpdateReferences(Resource instance, ResourceModel model)
        {
            var type = instance.GetType();
            foreach (var reference in model.References)
            {
                // Skip references that were not set
                if (reference.Targets == null)
                    continue;

                var property = type.GetProperty(reference.Name);
                if (reference.IsCollection)
                {
                    var collection = ((IReferenceCollection)property.GetValue(instance)).UnderlyingCollection;
                    // Add new items and update existing ones
                    foreach (var targetModel in reference.Targets)
                    {
                        Resource target;
                        if (targetModel.Id == 0 || collection.All(r => r.Id != targetModel.Id))
                        {
                            // New reference added to the collection
                            target = FromModel(targetModel);
                            collection.Add(target);
                        }
                        else
                        {
                            // Element already exists in the collection
                            target = (Resource)collection.First(r => r.Id == targetModel.Id);
                            FromModel(targetModel, target);
                        }
                    }
                    // Remove deleted items
                    var targetIds = reference.Targets.Select(t => t.Id).Distinct().ToArray();
                    var deletedItems = collection.Where(r => !targetIds.Contains(r.Id)).ToArray();
                    foreach (var deletedItem in deletedItems)
                        collection.Remove(deletedItem);
                }
                else
                {
                    var targetModel = reference.Targets.FirstOrDefault();
                    var value = (Resource)property.GetValue(instance);

                    Resource target;
                    if (targetModel == null)
                        target = null;
                    else if (targetModel.Id == value?.Id)
                        target = FromModel(targetModel, value);
                    else
                        target = FromModel(targetModel);

                    if (target != value)
                        property.SetValue(instance, target);
                }
            }
        }

        private Resource FromModel(ResourceModel model, Resource resource = null)
        {
            // Only fetch resource object if it was not given
            if (resource == null)
            {
                // Get or create resource
                resource = model.Id == 0
                    ? Manager.Create(model.Type)
                    : Manager.Get(model.Id);
            }

            // Copy standard properties
            resource.Name = model.Name;
            resource.LocalIdentifier = model.LocalIdentifier;
            resource.GlobalIdentifier = model.GlobalIdentifier;
            resource.Description = model.Description;

            // Copy extended properties
            if (model.Properties != null)
                EntryConvert.UpdateInstance(resource.Descriptor, model.Properties, Serialization);

            // Set all other references
            if (model.References != null)
                UpdateReferences(resource, model);

            return resource;
        }

        /// <summary>
        /// Convert <see cref="IResourceTypeNode"/> to <see cref="ResourceTypeModel"/> without converting a type twice
        /// </summary>
        private ResourceTypeModel ConvertType(IResourceTypeNode node)
        {
            return ConvertType(node, null);
        }

        /// <summary>
        /// Convert <see cref="IResourceTypeNode"/> to <see cref="ResourceTypeModel"/> without converting a type twice
        /// </summary>
        private ResourceTypeModel ConvertType(IResourceTypeNode node, ResourceTypeModel baseType)
        {
            var resType = node.ResourceType;

            if (_typeCache.ContainsKey(node.Name))
                return _typeCache[node.Name];

            var typeModel = new ResourceTypeModel
            {
                Creatable = node.Creatable,
                Name = Regex.Replace(node.Name, @"`\d", string.Empty), // Remove generic arguments from type name
                BaseType = baseType,

                // Read display name of the type
                DisplayName = resType.GetCustomAttribute<DisplayNameAttribute>(false)?.DisplayName,

                // Read description of the type
                Description = resType.GetCustomAttribute<DescriptionAttribute>(false)?.Description,

                // Convert resource constructors
                Constructors = node.Constructors.Select(ctr => EntryConvert.EncodeMethod(ctr, Serialization)).ToArray()
            };

            typeModel.DerivedTypes = node.DerivedTypes.Select(t => ConvertType(t, typeModel)).ToArray();

            _typeCache[node.Name] = typeModel;

            return typeModel;
        }

        /// <summary>
        /// Find all resources of the tree that are possible instances for a reference of that type
        /// </summary>
        private IEnumerable<ResourceModel> MatchingInstances(ICollection<Type> typeConstraints)
        {
            var matches = new List<Resource>();

            foreach (var root in Manager.GetRoots())
            {
                IncludeMatchingInstance(root, typeConstraints, matches);
            }

            return matches.Select(r => new ResourceModel
            {
                Id = r.Id,
                Name = r.Name,
                Type = r.GetType().Name,
                LocalIdentifier = r.LocalIdentifier,
                GlobalIdentifier = r.GlobalIdentifier,
                Description = r.Description
            });
        }

        /// <summary>
        /// Include this resource into the collection of matches if it is an instance of the desired type
        /// </summary>
        private static void IncludeMatchingInstance(Resource current, ICollection<Type> typeConstraints, ICollection<Resource> currentMatches)
        {
            if (typeConstraints.All(tc => tc.IsInstanceOfType(current)))
                currentMatches.Add(current);

            foreach (var child in current.Children)
            {
                IncludeMatchingInstance(child, typeConstraints, currentMatches);
            }
        }

        #endregion
    }
}