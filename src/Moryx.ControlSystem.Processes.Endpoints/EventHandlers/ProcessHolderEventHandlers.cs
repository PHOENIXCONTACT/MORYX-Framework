// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Channels;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Processes.Endpoints.Extensions;

namespace Moryx.ControlSystem.Processes.Endpoints.EventHandlers;
internal class ProcessHolderEventHandlers
{

    public static EventHandler<IResource> OnResourceRemoved(EventHandler<IProcess> processChanged, EventHandler groupChanged)
    {
        return new EventHandler<IResource>((obj, e) =>
        {
            if (e is IProcessHolderPosition position)
            {
                position.ProcessChanged -= processChanged;
            }
            else if (e is ProcessHolderGroup group)
            {
                group.Changed -= groupChanged;
            }
        });
    }

    public static EventHandler<IResource> OnResourceAdded(
        Channel<ProcessHolderGroupChangedEventArg> groupChannel,
        EventHandler groupChanged)
    {
        return new EventHandler<IResource>((obj, e) =>
        {
            if (e is ProcessHolderGroup group)
            {
                groupChannel.Writer.TryWrite(new ProcessHolderGroupChangedEventArg(group.ToDto()));
                group.Changed += groupChanged;
            }
            else if (e is IProcessHolderPosition position)
            {
                SendUpdate(groupChannel, position);
            }
        });
    }

    public static void SendUpdate(
        Channel<ProcessHolderGroupChangedEventArg> groupChannel,
        IProcessHolderPosition position)
    {
        var resource = position as Resource;
        var parentCategory = resource?.ParentCategory();
        if (parentCategory is null)
        {
            return;
        }

        if (parentCategory == Category.ProcessHolderGroup)
        {
            groupChannel.Writer.TryWrite(new ProcessHolderGroupChangedEventArg((resource.Parent as IProcessHolderGroup).ToDto()));
        }
        else
        {
            var group = ProcessHolderMappers.ModelFromParent(resource);
            var positions = resource.Parent.Children
                .OfType<IProcessHolderPosition>()
                .Select((p, index) => p.ToDto(index, group.Id));
            group.Positions.AddRange(positions);
            groupChannel.Writer.TryWrite(new ProcessHolderGroupChangedEventArg(group));
        }
    }

    public static EventHandler OnGroupChanged(Channel<ProcessHolderGroupChangedEventArg> groupChannel)
    {
        return new EventHandler((obj, e) =>
        {
            if (obj is not IProcessHolderGroup group)
            {
                return;
            }

            groupChannel.Writer.TryWrite(new ProcessHolderGroupChangedEventArg(group.ToDto()));
        });
    }

    public static EventHandler<IProcess> OnProcessChanged(
        Channel<ProcessHolderGroupChangedEventArg> groupChannel)
    {
        return new EventHandler<IProcess>((obj, process) =>
        {
            if (obj is IProcessHolderPosition position)
            {
                SendUpdate(groupChannel, position);
            }
            else if (obj is IProcessHolderGroup group)
            {
                groupChannel.Writer.TryWrite(new ProcessHolderGroupChangedEventArg(group.ToDto()));
            }
        });
    }

    public static EventHandler OnResetExecuted(
        Channel<ProcessHolderGroupChangedEventArg> groupChannel)
    {
        return new EventHandler((obj, e) =>
        {
            var position = obj as IProcessHolderPosition;
            if (position is null)
            {
                return;
            }

            SendUpdate(groupChannel, position);
        });
    }
}
