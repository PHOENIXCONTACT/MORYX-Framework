using System.Collections.Generic;
using System.Linq;
using Moryx.AbstractionLayer.Resources;
using Moryx.Tools;
// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

namespace Moryx.ControlSystem.Processes.Endpoints.Extensions;

internal static class ProcessHolderPositionsExtensions
{

    public static Dictionary<Resource, IEnumerable<IProcessHolderPosition>> GetUngroupedPostions(
        this IEnumerable<IProcessHolderPosition> positions)
    {
        var result = new Dictionary<Resource, IEnumerable<IProcessHolderPosition>>();        
        foreach (var position in positions)
        {
            if (position is Resource resource && !(resource.ParentCategory() == Category.ProcessHolderGroup))
            {
                if (result.TryGetValue(resource, out var list))
                {
                    list.Append(position);
                }
                else
                {
                    result.Add(resource, [position]);
                }
            }
        }
        return result;
    }
}