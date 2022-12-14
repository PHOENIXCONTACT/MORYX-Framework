// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moryx.AbstractionLayer.Recipes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Moryx.AbstractionLayer.Products.Endpoints
{
    /// <summary>
    /// Provides extension methods to add a SignalR hub to the service collection and map its path within the application
    /// </summary>
    public static class HubMiddlewareExtension
    {
        /// <summary>
        /// Adds a background service to the <paramref name="services"/> which feeds information
        /// from the <see cref="IProductManagementModification"/> to all clients connected to the SignalR hub.
        /// </summary>
        public static void AddMoryxProductManagementHub(this IServiceCollection services)
        {
            services.AddHostedService<ProductManagementHubService>();
        }

        /// <summary>
        /// Registers the SignalR hub endpoint
        /// </summary>
        public static void UseMoryxProductManagementHub(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ProductManagementHub>("/api/moryx/products/hub");
            });
        }
    }

    /// <summary>
    /// Defines the BackgroundService for listening to events from the <see cref="IProductManagementModification"/> and feeding
    /// them to the <see cref="ProductManagementHub"/>.
    /// </summary>

    public class ProductManagementHubService : BackgroundService
    {
        private readonly IProductManagement _productManagement;
        private readonly IHubContext<ProductManagementHub> _hubContext;
        private readonly ProductConverter _converter;

        public ProductManagementHubService(IProductManagement productManagement,
            IHubContext<ProductManagementHub> hubContext)
        {
            _productManagement = productManagement ?? throw new ArgumentNullException(nameof(productManagement));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _converter = new ProductConverter(_productManagement);
        }

        /// <summary>
        /// Prepares the BackgroundService to subscribe and unsubscribe events from the <see cref="_productManagement"/>.
        /// The actual task to run in the BackgroundService is empty as we do not actually calculate anything.
        /// </summary>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _productManagement.TypeChanged += TypeChanged;
            _productManagement.RecipeChanged += RecipeChanged;

            stoppingToken.Register(() =>
            {
                _productManagement.TypeChanged -= TypeChanged;
                _productManagement.RecipeChanged -= RecipeChanged;
            });

            return Task.CompletedTask;
        }

        private async void RecipeChanged(object sender, IRecipe e)
        {
            await _hubContext.Clients.All.SendAsync(nameof(_productManagement.RecipeChanged), ProductConverter.ConvertRecipe(e));
        }

        private async void TypeChanged(object sender, IProductType e)
        {
            await _hubContext.Clients.All.SendAsync(nameof(_productManagement.TypeChanged), _converter.ConvertProduct(e, false));
        }
    }
}
