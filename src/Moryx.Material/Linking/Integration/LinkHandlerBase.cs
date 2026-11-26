// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Logging;
using Moryx.Modules;

namespace Moryx.Material.Linking.Integration;

/// <summary>
/// Base class of an integration's <see cref="ILinkHandler{TReference}"/> resolving business objects and
/// executing the configured <see cref="LinkingHook{TReference, TBusinessObject}"/>s in both phases.
/// </summary>
/// <typeparam name="TReference">Type of reference the handled links point to.</typeparam>
/// <typeparam name="TBusinessObject">Type of business object resolved for the reference.</typeparam>
/// <typeparam name="THookConfig">Type of the hook plugin configurations.</typeparam>
public abstract class LinkHandlerBase<TReference, TBusinessObject, THookConfig> : ILinkHandler<TReference>, ILoggingComponent
    where TReference : Reference
    where TBusinessObject : class
    where THookConfig : IPluginConfig
{
    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <inheritdoc />
    public IModuleLogger Logger { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    /// <summary>
    /// Pool of the managed references.
    /// </summary>
    protected abstract IReferencePool<TReference> Pool { get; }

    /// <summary>
    /// Configurations of the hooks to execute, in order.
    /// </summary>
    protected abstract IEnumerable<THookConfig> HookConfigs { get; }
    #endregion

    /// <summary>
    /// Creates the hook for <paramref name="config"/>.
    /// </summary>
    protected abstract Task<ILinkingHook> CreateHookAsync(THookConfig config, IMaterialContainer container,
        LinkingRequest<TReference> request, ValidationContext validationContext, TBusinessObject? target,
        TBusinessObject? previous, CancellationToken cancellationToken);

    /// <summary>
    /// Destroys a hook created by <see cref="CreateHookAsync"/>.
    /// </summary>
    protected abstract void DestroyHook(ILinkingHook hook);

    /// <summary>
    /// Resolves the managed reference and business object for an unresolved <paramref name="target"/>.
    /// </summary>
    /// <returns>The managed reference and the business object, which is <c>null</c> if it does not exist.</returns>
    protected abstract Task<(TReference Reference, TBusinessObject? BusinessObject)> ResolveAsync(TReference target,
        CancellationToken cancellationToken);

    /// <summary>
    /// Loads the business object of <paramref name="reference"/>, or <c>null</c> if it does not exist.
    /// </summary>
    protected abstract Task<TBusinessObject?> LoadAsync(TReference reference, CancellationToken cancellationToken);

    /// <inheritdoc />
    public async Task<LinkingResponse<TReference>> ValidateAsync(IMaterialContainer container, LinkingRequest<TReference> request,
        CancellationToken cancellationToken)
    {
        var validation = new ValidationContext();
        TReference? resolved = null;

        try
        {
            TBusinessObject? target = null;
            if (request.Target is not null)
            {
                (resolved, target) = await ResolveAsync(request.Target, cancellationToken);
                if (target is null)
                {
                    validation.AddError($"{request.Target} could not be resolved.");
                }
            }

            var previous = request.Previous is null ? null : await LoadAsync(request.Previous, cancellationToken);
            await ExecuteHooksAsync(container, request, validation, target, previous, applyPhase: false, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Logger.LogError(ex, "Error during linking request handling for container {Id} - {Name}", container.Id, container.Name);
            validation.AddError(ex);
        }

        return new LinkingResponse<TReference>(validation, resolved);
    }

    /// <inheritdoc />
    public async Task OnAppliedAsync(IMaterialContainer container, LinkingRequest<TReference> request, LinkingResponse<TReference> response,
        CancellationToken cancellationToken)
    {
        var applied = request.IsUnlink ? null : response.Reference ?? request.Target;

        try
        {
            // Update pool bookkeeping: the link switched from its previous reference to the applied one
            if (!request.Previous.ValueEquals(applied))
            {
                if (applied is not null)
                {
                    await Pool.AcquireAsync(applied, cancellationToken);
                }

                Pool.Release(request.Previous);
            }

            var target = applied is null ? null : await LoadAsync(applied, cancellationToken);
            var previous = request.Previous is null ? null : await LoadAsync(request.Previous, cancellationToken);
            await ExecuteHooksAsync(container, request, response.Context, target, previous, applyPhase: true, cancellationToken);

            // TODO: Lineage related handling. Record the link / unlink lineage event of the applied request.
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Logger.LogError(ex, "Error during link applied handling for container {Id} - {Name}", container.Id, container.Name);
            throw;
        }
    }

    private async Task ExecuteHooksAsync(IMaterialContainer container, LinkingRequest<TReference> request, ValidationContext validation,
        TBusinessObject? target, TBusinessObject? previous, bool applyPhase, CancellationToken cancellationToken)
    {
        foreach (var config in HookConfigs)
        {
            ILinkingHook? hook = null;
            try
            {
                hook = await CreateHookAsync(config, container, request, validation, target, previous, cancellationToken);
                if (hook is not LinkingHook<TReference, TBusinessObject> typedHook)
                {
                    validation.AddError($"Configured hook '{config.PluginName}' could not be created as " +
                        $"{nameof(LinkingHook<,>)} for {typeof(TReference).Name}.");
                    continue;
                }

                if (applyPhase)
                {
                    await typedHook.HandleLinkAppliedAsync(cancellationToken);
                }
                else
                {
                    await typedHook.HandleLinkRequestAsync(cancellationToken);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Logger.LogError(ex, "Hook {hook} threw during {phase}", config.PluginName, applyPhase ? "apply" : "request");
                validation.AddError(ex, hook?.GetType() ?? typeof(ILinkingHook));
            }
            finally
            {
                if (hook is not null)
                {
                    DestroyHook(hook);
                }
            }
        }
    }
}
