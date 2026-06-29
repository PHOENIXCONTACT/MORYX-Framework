// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Processes.Endpoints.Extensions;

namespace Moryx.ControlSystem.Processes.Endpoints.EventHandlers;

internal class ProcessHolderEventHandlers
{
    public static EventHandler<IResource> OnResourceRemoved(EventHandler<Process> processChanged, EventHandler groupChanged)
    {
        return (_, e) =>
        {
            if (e is IProcessHolderPosition position)
            {
                position.ProcessChanged -= processChanged;
            }
            else if (e is ProcessHolderGroup group)
            {
                group.Changed -= groupChanged;
            }
        };
    }

    public static EventHandler<IResource> OnResourceAdded(EventHandler groupChanged, Action<ProcessHolderGroupModel> broadcast)
    {
        return (_, e) =>
        {
            if (e is ProcessHolderGroup group)
            {
                broadcast(group.ToDto());
                group.Changed += groupChanged;
            }
            else if (e is IProcessHolderPosition position)
            {
                SendUpdate(broadcast, position);
            }
        };
    }

    public static void SendUpdate(Action<ProcessHolderGroupModel> broadcast, IProcessHolderPosition position)
    {
        var resource = position as Resource;
        var parentCategory = resource?.ParentCategory();
        if (parentCategory is null)
        {
            return;
        }

        if (parentCategory == Category.ProcessHolderGroup)
        {
            var parentGroup = (resource.Parent as IProcessHolderGroup)?.ToDto();
            if (parentGroup == null)
            {
                return;
            }

            broadcast(parentGroup);
        }
        else
        {
            var group = ProcessHolderMappers.ModelFromParent(resource);
            var positions = resource.Parent?.Children
                .OfType<IProcessHolderPosition>()
                .Select((p, index) => p.ToDto(index, group.Id))
                ?? [];
            group.Positions.AddRange(positions);
            broadcast(group);
        }
    }

    public static EventHandler OnGroupChanged(Action<ProcessHolderGroupModel> broadcast)
    {
        return (obj, e) =>
        {
            if (obj is not IProcessHolderGroup group)
            {
                return;
            }

            broadcast(group.ToDto());
        };
    }

    public static EventHandler<Process> OnProcessChanged(Action<ProcessHolderGroupModel> broadcast)
    {
        return (obj, _) =>
        {
            switch (obj)
            {
                case IProcessHolderPosition position:
                    SendUpdate(broadcast, position);
                    break;
                case IProcessHolderGroup group:
                    broadcast(group.ToDto());
                    break;
            }
        };
    }

    public static EventHandler OnResetExecuted(Action<ProcessHolderGroupModel> broadcast)
    {
        return (obj, e) =>
        {
            if (obj is not IProcessHolderPosition position)
            {
                return;
            }

            SendUpdate(broadcast, position);
        };
    }
}
