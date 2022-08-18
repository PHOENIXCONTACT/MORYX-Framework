using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Moryx.Model
{
    /// <summary>
    /// Register components necessary for 
    /// </summary>
    public static class ModelServiceCollectionExtension
    {
        /// <summary>
        /// Link MORYX kernel to the service collection
        /// </summary>
        public static void AddMoryxModels(this IServiceCollection serviceCollection)
        {
            // Register config manager
            serviceCollection.AddSingleton<IDbContextManager, DbContextManager>();
        }
    }
}
