// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Recipes;
using Moryx.Factory;
using Moryx.Tools;

namespace Moryx.ControlSystem.Processes.Endpoints.Extensions;
internal static class ProcessHolderMappers
{

    public static ProcessHolderGroupModel ToDto(this IProcessHolderGroup group)
    {
        var attribute = group.GetType().GetCustomAttribute<EntryVisualizationAttribute>();
        var visualization = attribute is not null ?
            new VisualizationModel { Category = Category.ProcessHolderGroup, Icon = attribute.Icon } :
            null;
        return new ProcessHolderGroupModel
        {
            Id = group.Id,
            Name = group.Name,
            Visualization = visualization,
            Positions = [.. group.Positions.Select((x, i) => x.ToDto(i, group.Id))]
        };
    }

    public static ProcessHolderPositionModel ToDto(
        this IProcessHolderPosition position, int index, long groupId)
    {
        var process = (position as ProcessHolderPosition)?.Process;
        var CurrentActivity = process?.CurrentActivity() == null
            ? string.Empty
            : $"{process?.CurrentActivity().Id} - {process?.CurrentActivity().GetType().GetDisplayName() ?? process?.CurrentActivity().GetType().Name}";
        return new ProcessHolderPositionModel
        {
            Id = position.Id,
            Position = index,
            GroupId = groupId,
            Name = position.Name,
            IsEmpty = position.IsEmpty(),
            Activity = string.IsNullOrEmpty(CurrentActivity) ? "-" : CurrentActivity,
            Process = position is ProcessHolderPosition pos && pos.CurrentProcess > 0
                ? (position as ProcessHolderPosition)?.CurrentProcess.ToString(CultureInfo.InvariantCulture)
                : "-",
            Order = position.Process?.Recipe.GetOrderOperationString() ?? "-",
        };
    }

    /// <summary>
    /// Given a list of <see cref="IProcessHolderPosition"/>, return a list of <see cref="ProcessHolderGroupModel"/>
    /// based on the Parent -> Child or ProcessHolderGroup -> ProcessHolderPositions relationships.
    /// </summary>
    /// <param name="groups"></param>
    /// <returns></returns>
    public static IEnumerable<ProcessHolderGroupModel> ToDto(
        this IEnumerable<IProcessHolderGroup> groups)
    {
        List<ProcessHolderGroupModel> groupModels = [];
        foreach (var group in groups)
        {
            var groupDto = group.ToDto();
            //In case the .ToDto converted the positions already
            if (!groupDto.Positions.Any())
            {
                groupDto.Positions.AddRange(group.Positions.Select((p, index) => p.ToDto(index, group.Id)));
            }
            groupModels.AddIfNotExists(groupDto);
        }
        return groupModels.OrderBy(x => x.Name);
    }

    public static IEnumerable<ProcessHolderGroupModel> ToDto(
        this Dictionary<Resource, IEnumerable<IProcessHolderPosition>> groups)
        => groups.Select((group) =>
        {
            var groupDto = ModelFromParent(group.Key);
            for (var index = 0; index < group.Value.Count(); index++)
            {
                var dto = group.Value
                          .ElementAt(index)
                          .ToDto(index, groupDto.Id);
                groupDto.Positions.Add(dto);
            }
            return groupDto;
        }).OrderBy(x => x.Name);

    /// <summary>
    /// Returns a new <see cref="ProcessHolderGroupModel"/> based on the Parent of the resource
    /// </summary>
    /// <param name="resource">The whose parent will be used to create the model</param>
    /// <returns>The group model</returns>
    public static ProcessHolderGroupModel ModelFromParent(Resource resource)
    {
        var parentCategory = resource.ParentCategory();
        var parentAttribute = resource.Parent?.GetType().GetCustomAttribute<EntryVisualizationAttribute>();
        var groupVisualization = parentAttribute is not null
            ? new VisualizationModel
            {
                Category = parentCategory,
                Icon = parentAttribute.Icon
            }
            : null;
        return new ProcessHolderGroupModel
        {
            Id = resource.Parent?.Id ?? -1,
            Name = resource.Parent?.Name ?? "?",
            Visualization = groupVisualization
        };
    }
}
