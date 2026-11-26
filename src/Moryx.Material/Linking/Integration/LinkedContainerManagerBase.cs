// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Moryx.Logging;
using Moryx.Material.Facade;
using Moryx.Material.States;
using Moryx.Modules;

namespace Moryx.Material.Linking.Integration;

/// <summary>
/// Base class of an integration's component tracking the <see cref="MaterialContainerLink{TReference}"/>s of all registered
/// containers. It attaches the integration's <see cref="ILinkHandler{TReference}"/> to each link, processes pending
/// requests and substitutes linked references with managed ones.
/// </summary>
/// <typeparam name="TReference">Type of reference the managed links point to.</typeparam>
public abstract class LinkedContainerManagerBase<TReference> : IAsyncPlugin, ILoggingComponent
    where TReference : Reference
{
    private readonly ConcurrentDictionary<long, IMaterialContainer> _containers = [];
    private readonly ConcurrentDictionary<Task, byte> _running = [];
    private CancellationTokenSource _stopping = new();

    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <inheritdoc />
    public IModuleLogger Logger { get; set; }

    /// <summary>
    /// Material management facade providing the containers.
    /// </summary>
    public IMaterialManagement MaterialManagement { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    /// <summary>
    /// Handler attached to the managed links.
    /// </summary>
    protected abstract ILinkHandler<TReference> Handler { get; }

    /// <summary>
    /// Pool of the managed references.
    /// </summary>
    protected abstract IReferencePool<TReference> Pool { get; }
    #endregion

    // TODO: What if a container has multiple links/references for an business object type?
    /// <summary>
    /// Returns the link managed by this integration, or <c>null</c> if <paramref name="container"/> does not declare one.
    /// </summary>
    protected abstract MaterialContainerLink<TReference>? GetLink(IMaterialContainer container);

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _stopping = new CancellationTokenSource();
        MaterialManagement.ContainerStateChanged += OnContainerStateChanged;

        var containers = MaterialManagement
            .GetContainers(c => c.State is not StateClassification.Deregistered && GetLink(c) is not null);
        // TODO: Might be parallel? Otherwise a blocking hook can block the start of the module
        foreach (var container in containers)
        {
            await TryAddAsync(container, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        MaterialManagement.ContainerStateChanged -= OnContainerStateChanged;

        await _stopping.CancelAsync();
        await Task.WhenAll(_running.Keys);
        _stopping.Dispose();

        foreach (var container in _containers.Values)
        {
            Detach(container);
        }
        _containers.Clear();
    }

    private void OnContainerStateChanged(object? sender, ContainerStateChangedEventArgs e)
    {
        if (GetLink(e.Container) is null)
        {
            return;
        }

        if (e.NewStateInformation is DeregisteredStateInformation)
        {
            if (_containers.TryRemove(e.Container.Id, out var removed))
            {
                Detach(removed);
                // TODO: Lineage related handling - record an unlink lineage event when a linked container is removed.
            }
        }
        else
        {
            // The event source must not be blocked while resolving references or processing pending requests
            Track(TryAddAsync(e.Container, _stopping.Token));
        }
    }

    private void Track(Task task)
    {
        if (task.IsCompleted)
        {
            return;
        }

        _running.TryAdd(task, 0);
        _ = task.ContinueWith(t => _running.TryRemove(t, out _), TaskScheduler.Default);
    }

    private async Task TryAddAsync(IMaterialContainer container, CancellationToken cancellationToken)
    {
        var link = GetLink(container);
        if (link is null || !_containers.TryAdd(container.Id, container))
        {
            return;
        }

        try
        {
            if (link.Reference is { } linked)
            {
                var managed = await Pool.AcquireAsync(linked, cancellationToken);
                if (!ReferenceEquals(managed, linked))
                {
                    link.Substitute(managed);
                }
            }

            var result = await link.AttachAsync(container, Handler, cancellationToken);
            if (result is { Status: LinkingStatus.Rejected })
            {
                Logger.LogWarning("Pending link request of container {Id} - {Name} was rejected: {errors}", container.Id, container.Name,
                    string.Join("; ", result.Context.Entries.Where(e => e.Severity == ValidationSeverity.Error).Select(e => e.Text)));
            }
        }
        catch (OperationCanceledException)
        {
            // Module is stopping, the pending request stays stored on the link
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to attach to the link of container {Id} - {Name}", container.Id, container.Name);
        }
    }

    private void Detach(IMaterialContainer container)
    {
        var link = GetLink(container);
        if (link is null)
        {
            return;
        }

        link.Detach(Handler);

        // Release our hold on the pooled reference
        Pool.Release(link.Reference);
    }
}
