// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using System.Reflection;
using Moryx.AbstractionLayer.Processes;
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
        var visualization = attribute is not null
            ? new VisualizationModel { Category = Category.ProcessHolderGroup, Icon = attribute.Icon }
            : null;

        return new ProcessHolderGroupModel
        {
            Id = group.Id,
            Name = group.Name,
            Visualization = visualization,
            Positions = group.Positions.Select((x, i) => x.ToDto(i, group.Id)).ToList()
        };
    }

    public static ProcessHolderPositionModel ToDto(this IProcessHolderPosition position, int index, long groupId)
    {
        var process = (position as ProcessHolderPosition)?.Process;
        var currentActivity = process?.CurrentActivity() == null
            ? string.Empty
            : $"{process.CurrentActivity().Id} - {process.CurrentActivity().GetType().GetDisplayName() ?? process.CurrentActivity().GetType().Name}";
        return new ProcessHolderPositionModel
        {
            Id = position.Id,
            Position = index,
            GroupId = groupId,
            Name = position.Name,
            IsEmpty = position.IsEmpty(),
            Activity = string.IsNullOrEmpty(currentActivity) ? "-" : currentActivity,
            Process = position is ProcessHolderPosition { CurrentProcess: > 0 } processHolderPosition
                ? processHolderPosition.CurrentProcess.ToString(CultureInfo.InvariantCulture)
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
    public static IEnumerable<ProcessHolderGroupModel> ToDto(this IEnumerable<IProcessHolderGroup> groups)
    {
        List<ProcessHolderGroupModel> groupModels = [];
        foreach (var group in groups)
        {
            var groupDto = group.ToDto();
            //In case the .ToDto converted the positions already
            if (groupDto.Positions.Count == 0)
            {
                groupDto.Positions.AddRange(group.Positions.Select((p, index) => p.ToDto(index, group.Id)));
            }
            groupModels.AddIfNotExists(groupDto);
        }
        return groupModels.OrderBy(x => x.Name);
    }

    public static IEnumerable<ProcessHolderGroupModel> ToDto(this Dictionary<Resource, List<IProcessHolderPosition>> groups) =>
        groups.Select(group =>
        {
            var groupDto = ModelFromParent(group.Key);
            for (var index = 0; index < group.Value.Count; index++)
            {
                var dto = group.Value[index].ToDto(index, groupDto.Id);
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
            ? new VisualizationModel { Category = parentCategory, Icon = parentAttribute.Icon }
            : null;

        return new ProcessHolderGroupModel
        {
            Id = resource.Parent?.Id ?? -1,
            Name = resource.Parent?.Name ?? "?",
            Visualization = groupVisualization
        };
    }
}
