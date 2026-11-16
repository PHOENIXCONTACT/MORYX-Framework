// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;

// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG

namespace Moryx.ControlSystem.Processes.Endpoints.Extensions;

internal static class ProcessHolderPositionsExtensions
{
    public static Dictionary<Resource, List<IProcessHolderPosition>> GetUngroupedPositions(this IEnumerable<IProcessHolderPosition> positions)
    {
        var result = new Dictionary<Resource, List<IProcessHolderPosition>>();
        foreach (var position in positions)
        {
            if (position is not Resource resource || resource.ParentCategory() == Category.ProcessHolderGroup)
            {
                continue;
            }

            if (result.TryGetValue(resource, out var list))
            {
                list.Add(position);
            }
            else
            {
                result.Add(resource, [position]);
            }
        }
        return result;
    }
}
