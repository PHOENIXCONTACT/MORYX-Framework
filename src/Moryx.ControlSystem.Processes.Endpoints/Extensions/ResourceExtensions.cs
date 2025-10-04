// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

using Moryx.AbstractionLayer.Resources;

namespace Moryx.ControlSystem.Processes.Endpoints.Extensions;
internal static class ResourceExtensions
{
    public static Category ParentCategory(this Resource resource)
    {
        var parentType = resource.Parent?.GetType();
        var parentCategory = parentType.IsAssignableTo(typeof(IProcessHolderGroup)) ?
                Category.ProcessHolderGroup : Category.ParentResource;
        return parentCategory;
    }
}
