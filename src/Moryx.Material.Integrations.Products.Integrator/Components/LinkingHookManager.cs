// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Products;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Material.Linking;
using ValidationContext = Moryx.Material.Linking.ValidationContext;

namespace Moryx.Material.Integrations.Products.Integrator.Components;

[Component(LifeCycle.Singleton, typeof(ILinkingHookManager))]
internal class LinkingHookManager : ILinkingHookManager, ILoggingComponent
{
    #region Dependencies
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public IModuleLogger Logger { get; set; }

    public IProductManagement ProductManagement { get; set; }

    public IProductLinkingHookFactory HookFactory { get; set; }

    public IProductTypeReferencesPool ReferencesPool { get; set; }

    public ModuleConfig Config { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    #endregion

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public void ProcessLinkingRequested(RequestContext context)
    {
        // TODO: Do we want this? If yes we need cancellation token/module stop handling.
        // Schedule async handling without blocking the event source.
        _ = HandleLinkRequestedAsync(context);
    }

    private async Task HandleLinkRequestedAsync(RequestContext context)
    {
        var request = context.ProductRequest;
        var container = context.Container;
        var validation = new ValidationContext();
        ProductType? productToBeLinked = null;
        ProductTypeReference? referenceToBeLinked = null;

        try
        {
            // Resolve the new product reference if this is a (re)link. The pool checks its
            // existing entries first and only queries the facade when necessary.
            if (!request.IsUnlink && request.ProductIdentity is not null)
            {
                referenceToBeLinked = await ReferencesPool.ResolveAsync(request.ProductIdentity, CancellationToken.None);
                productToBeLinked = (referenceToBeLinked as InternalProductTypeReference)?.ProductType;
                if (productToBeLinked is null)
                {
                    validation.AddError($"Product with identity '{request.ProductIdentity}' could not be resolved.");
                }
            }

            var previouslyLinkedProduct = await ProductManagement.LoadProductFor(request.PreviousProduct);

            // Execute all configured hooks (request phase).
            foreach (var hook in Config.Hooks)
            {
                await ExecuteHook(hook, container, request, validation, productToBeLinked, previouslyLinkedProduct, applyPhase: false);
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error during linking request handling for container {id} - {name}", container.Id, container.Name);
            validation.AddError(ex);
        }

        if (context.ResponseCallback is null)
        {
            Logger?.LogWarning("Material container {id} - {name} requests linking to a product without providing a {callback}",
                container.Id, container.Name, nameof(LinkingRequestEventArgs.ResponseCallback));
            return;
        }

        var response = new LinkingResponse(validation, referenceToBeLinked);
        try
        {
            await context.ResponseCallback(response);
        }
        catch (Exception ex)
        {
            Logger?.Log(LogLevel.Error, ex, "Container failed to handle linking response for {id} - {name}", container.Id, container.Name);
        }
    }

    private async Task ExecuteHook(ProductLinkingHookConfig hookConfig, IMaterialContainer container,
        ProductLinkingRequest request, ValidationContext validation, ProductType? productType, ProductType? previousProductType, bool applyPhase)
    {
        ProductLinkingHook? hook = null;
        try
        {
            hook = await HookFactory.Create(hookConfig, CancellationToken.None) as ProductLinkingHook;
            if (hook is null)
            {
                validation.AddError($"Configured hook '{hookConfig.PluginName}' could not be created.");
                return;
            }

            hook.Container = container;
            hook.Request = request;
            hook.ValidationContext = validation;
            hook.ProductType = productType;
            hook.PreviousProductType = previousProductType;

            if (applyPhase)
            {
                await hook.HandleLinkAppliedAsync(CancellationToken.None);
            }
            else
            {
                await hook.HandleLinkRequestAsync(CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            Logger?.Log(LogLevel.Error, ex, "Hook {0} threw during {1}", hookConfig.PluginName, applyPhase ? "apply" : "request");
            validation.AddError(ex, hook?.GetType() ?? typeof(ILinkingHook));
        }
        finally
        {
            if (hook != null)
            {
                HookFactory.Destroy(hook);
            }
        }
    }

    public void ProcessLinkingApplied(AppliedContext context)
    {
        // TODO: Do we want this? If yes we need cancellation token/module stop handling.
        _ = HandleLinkAppliedAsync(context);
    }

    private async Task HandleLinkAppliedAsync(AppliedContext context)
    {
        var request = context.ProductRequest;
        var container = context.Container;
        var validation = context.Validation;
        var linkedProduct = !request.IsUnlink && request.ProductIdentity is not null
            ? await ProductManagement.LoadProductFor(request.ProductIdentity) : null;
        var previouslyLinkedProduct = await ProductManagement.LoadProductFor(request.PreviousProduct);

        try
        {
            foreach (var hook in Config.Hooks)
            {
                await ExecuteHook(hook, container, request, validation, linkedProduct, previouslyLinkedProduct, applyPhase: true);
            }

            // TODO: Lineage related handling.
            //await RecordLineageAsync(context);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error during link applied handling for container {id} - {name}", container.Id, container.Name);
        }
    }

    // TODO: Lineage related aspects. Persist ProductTypeLinkLineageEvent / ProductTypeUnlinkLineageEvent
    //       once the material management lineage store is available.
    //private async Task RecordLineageAsync(AppliedContext context)
    //{
    //    var request = context.ProductRequest;
    //    var successful = !context.Validation.HasErrors;
    //
    //    if (request.PreviousProduct != null)
    //    {
    //        await MaterialManagement.RecordLineageAsync(new ProductTypeUnlinkLineageEvent
    //        {
    //            ContainerId = context.Container.Id,
    //            ProductIdentity = request.PreviousProduct.ProductIdentity,
    //            Successful = successful,
    //            Description = "Product unlinked from container."
    //        });
    //    }
    //
    //    if (!request.IsUnlink && context.ProductReference != null)
    //    {
    //        await MaterialManagement.RecordLineageAsync(new ProductTypeLinkLineageEvent
    //        {
    //            ContainerId = context.Container.Id,
    //            ProductIdentity = context.ProductReference.ProductIdentity,
    //            Successful = successful,
    //            Description = "Product linked to container."
    //        });
    //    }
    //}
}