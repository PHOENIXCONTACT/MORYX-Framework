using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Marvin.AbstractionLayer.UI;
using Marvin.Container;
using Marvin.Resources.UI.Interaction.ResourceInteraction;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// Component selector for resource view models
    /// </summary>
    [Plugin(LifeCycle.Singleton)]
    internal class ResourceDetailsComponentSelector : DetailsComponentSelector<IResourceDetails, IResourceController>
    {
        public ResourceDetailsComponentSelector(IContainer container, IResourceController controller) : base(container, controller)
        {
        }

        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            var groupType = arguments.FirstOrDefault() as string;
            // If no name was given use the default type
            if (groupType == null)
                return Registrations[DetailsConstants.DefaultType];

            // Directly return the default view if it is known
            if (Registrations.ContainsKey(groupType))
                return Registrations[groupType];

            // Start from the current type going upwards the type tree looking for a custom ui
            var typeModel = FindType(groupType, Controller.TypeTree);
            while (typeModel != null)
            {
                if (Registrations.ContainsKey(typeModel.Name))
                    return Registrations[typeModel.Name];
                typeModel = typeModel.BaseType;
            }

            // If all failed use the default
            return Registrations[DetailsConstants.DefaultType];
        }

        private static ResourceTypeModel FindType(string searchedType, List<ResourceTypeModel> typeBranch)
        {
            foreach (var typeModel in typeBranch)
            {
                if (typeModel.Name == searchedType)
                    return typeModel;

                var subtypeModel = FindType(searchedType, typeModel.DerivedTypes);
                if (subtypeModel != null)
                    return subtypeModel;
            }

            return null;
        }
    }
}