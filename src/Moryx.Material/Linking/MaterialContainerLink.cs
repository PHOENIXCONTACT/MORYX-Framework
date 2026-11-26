// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Material.Linking.Integration;
using Newtonsoft.Json;

namespace Moryx.Material.Linking;

// TODO: Add custom entry encoder to have proper serialization for the UI
/// <summary>
/// Non-generic base of <see cref="MaterialContainerLink{TReference}"/> allowing to enumerate the links of a container.
/// <list type="bullet">
/// <item>Links are declared as get-only properties on a <see cref="MaterialContainer"/>, which discovers them
/// automatically (see <see cref="MaterialContainer.Links"/>).</item>
/// <item> A container can declare any number of links to any number and type of business objects, e.g. one to an
/// order and one to a product type.</item>
/// </list>
/// </summary>
/// <remarks>
/// <see cref="MaterialContainerLink"/> provides an abstraction between the implementations of
/// <see cref="MaterialContainer"/> and <see cref="Reference"/>s. It prevents the resources to be able to setup the
/// relations to its references and keeps the module-provided <see cref="ILinkHandler{TReference}"/> from being
/// known to the resource, while making the two-step linking process a reusable bit of code.
/// </remarks>
[DataContract]
public abstract class MaterialContainerLink
{
    // TODO: Remove and hand-in a delegate for rais changed
    /// <summary>
    /// Container declaring this link. Set by the <see cref="MaterialContainer"/>.
    /// </summary>
    internal MaterialContainer? Owner { get; set; }

    /// <summary>
    /// Currently linked reference, if any.
    /// </summary>
    public abstract Reference? CurrentReference { get; }

    /// <summary>
    /// Whether a request is stored until an integration attaches to this link.
    /// </summary>
    public abstract bool HasPendingRequest { get; }

    /// <summary>
    /// Notifies the owning container about changes that must be persisted.
    /// </summary>
    protected void RaiseChanged() => Owner?.RaiseLinkChanged();
}

/// <summary>
/// Link slot of a <see cref="MaterialContainer"/> to a business object represented by <typeparamref name="TReference"/>.
/// Drives the two-phase linking protocol against the <see cref="ILinkHandler{TReference}"/> attached by an integration.
/// </summary>
/// <remarks>
/// Requests made while no handler is attached are stored as pending and processed once a handler attaches.
/// Requests on the same link are processed one after another. Hooks must therefore not issue requests
/// on the link they are validating, as this would deadlock.
/// </remarks>
/// <typeparam name="TReference">Type of reference the link points to.</typeparam>
[DataContract]
public abstract class MaterialContainerLink<TReference> : MaterialContainerLink
    where TReference : Reference
{
    private readonly SemaphoreSlim _sync = new(1, 1);
    private Attachment? _attachment;

    /// <summary>
    /// Currently linked reference, if any.
    /// </summary>
    // Persist the declared type only. Integrations substitute managed subtypes at runtime which must not leak into the persisted data.
    [DataMember, JsonProperty(TypeNameHandling = TypeNameHandling.None)]
    public TReference? Reference { get; private set; }

    /// <summary>
    /// Target of a link request that is stored until an integration attaches to this link.
    /// </summary>
    [DataMember, JsonProperty(TypeNameHandling = TypeNameHandling.None)]
    public TReference? PendingTarget { get; private set; }

    /// <summary>
    /// Whether an unlink request is stored until an integration attaches to this link.
    /// </summary>
    [DataMember]
    public bool IsUnlinkPending { get; private set; }

    /// <inheritdoc />
    public override Reference? CurrentReference => Reference;

    /// <inheritdoc />
    public override bool HasPendingRequest => PendingTarget is not null || IsUnlinkPending;

    /// <summary>
    /// Whether an <see cref="ILinkHandler{TReference}"/> is currently attached.
    /// </summary>
    public bool IsAttached => _attachment is not null;

    /// <summary>
    /// Raised after a request was applied to this link.
    /// </summary>
    public event EventHandler<LinkAppliedEventArgs<TReference>>? Applied;

    /// <summary>
    /// Requests to unlink the currently linked reference.
    /// </summary>
    public Task<LinkingResult> UnlinkAsync(CancellationToken cancellationToken = default)
        => RequestAsync(null, cancellationToken);

    /// <summary>
    /// Requests to link <paramref name="target"/> (or to unlink, if <c>null</c>).
    /// </summary>
    /// <param name="target">Unresolved reference identifying the business object to link.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    protected async Task<LinkingResult> RequestAsync(TReference? target, CancellationToken cancellationToken)
    {
        // Completes synchronously if no other request is running. Callers like resource constructors
        // do not await the returned task, so the pending path below must not yield.
        await _sync.WaitAsync(cancellationToken);
        try
        {
            // Nothing linked: unlinking only withdraws a pending link request
            if (target is null && Reference is null)
            {
                if (!ClearPending())
                {
                    return LinkingResult.Unchanged();
                }

                RaiseChanged();
                return LinkingResult.Applied(new());
            }

            var attachment = _attachment;
            if (attachment is null)
            {
                PendingTarget = target;
                IsUnlinkPending = target is null;
                RaiseChanged();
                return LinkingResult.Pending();
            }

            return await ExecuteAsync(attachment, target, cancellationToken);
        }
        finally
        {
            _sync.Release();
        }
    }

    /// <summary>
    /// Attaches <paramref name="handler"/> to this link and processes a pending request, if any.
    /// </summary>
    /// <param name="container">Container instance passed to the handler</param>
    /// <param name="handler">Handler executing the linking protocol.</param>
    /// <param name="cancellationToken">Token used to cancel processing of a pending request.</param>
    /// <returns>The result of the pending request, or <c>null</c> if there was none.</returns>
    /// <remarks>
    /// This is the only place an attachement is formed and it is only triggered from
    /// <see cref="Integration.LinkedContainerManagerBase"/>, which as a component of integrator modules does only
    /// work on resource proxies. Thereby, handing the container back during the linking process is not an issue.
    /// </remarks>
    internal async Task<LinkingResult?> AttachAsync(IMaterialContainer container, ILinkHandler<TReference> handler,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(container);
        ArgumentNullException.ThrowIfNull(handler);

        var attachment = new Attachment(container, handler);
        _attachment = attachment;
        if (!HasPendingRequest)
        {
            return null;
        }

        await _sync.WaitAsync(cancellationToken);
        try
        {
            // The pending request might have been superseded or the handler replaced in the meantime
            if (!HasPendingRequest || !ReferenceEquals(_attachment, attachment))
            {
                return null;
            }

            return await ExecuteAsync(attachment, PendingTarget, cancellationToken);
        }
        finally
        {
            _sync.Release();
        }
    }

    /// <summary>
    /// Detaches <paramref name="handler"/> from this link if it is the currently attached one.
    /// </summary>
    internal void Detach(ILinkHandler<TReference> handler)
    {
        var current = _attachment;
        if (current is not null && ReferenceEquals(current.Handler, handler))
        {
            Interlocked.CompareExchange(ref _attachment, null, current);
        }
    }

    /// <summary>
    /// Replaces the linked reference with an instance managed by an integration.
    /// </summary>
    /// <param name="reference">Reference that must <see cref="Linking.Reference.Matches"/> the linked one.</param>
    /// <exception cref="InvalidOperationException">If <paramref name="reference"/> identifies a different business object.</exception>
    internal void Substitute(TReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        if (Reference is null || !Reference.Matches(reference))
        {
            throw new InvalidOperationException("Only a reference identifying the linked business object can be substituted. " +
                $"Use the link methods of the {GetType().Name} to change the link.");
        }

        Reference = reference;
    }

    /// <summary>
    /// Implementation of the resource's side for the two-step validation process for linking business objects to
    /// <see cref="IMaterialContainer"/>s.
    /// </summary>
    private async Task<LinkingResult> ExecuteAsync(Attachment attachment, TReference? target, CancellationToken cancellationToken)
    {
        var request = new LinkingRequest<TReference>(target, Reference);
        var response = await attachment.Handler.ValidateAsync(attachment.Container, request, cancellationToken);

        // The request is decided, a stored request is superseded either way
        ClearPending();
        if (!response.IsAllowed)
        {
            RaiseChanged();
            return LinkingResult.Rejected(response.Context);
        }

        Reference = target is null ? null : response.Reference;
        RaiseChanged();

        try
        {
            await attachment.Handler.OnAppliedAsync(attachment.Container, request, response, cancellationToken);
        }
        catch (Exception e)
        {
            response.Context.AddWarning($"Post-processing of the applied link failed: {e.Message}");
        }

        // TODO: Check whether we need to fire this event here or whether it can be done behind the handler in the background.
        // The resource already emits the changed event for that matter
        Applied?.Invoke(this, new LinkAppliedEventArgs<TReference>(attachment.Container, request, response.Context, Reference));
        return LinkingResult.Applied(response.Context);
    }

    private bool ClearPending()
    {
        if (!HasPendingRequest)
        {
            return false;
        }

        PendingTarget = null;
        IsUnlinkPending = false;
        return true;
    }

    private sealed record Attachment(IMaterialContainer Container, ILinkHandler<TReference> Handler);
}
