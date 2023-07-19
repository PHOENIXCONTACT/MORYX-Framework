using Microsoft.Extensions.DependencyInjection;
using Moryx.Container;

namespace Moryx.Notifications
{
    /// <summary>
    /// <see cref="IContainer"/> extensions to register and handle notifications
    /// </summary>
    public static class NotificationExtensions
    {
        /// <summary>
        /// Registers all components to handling notifications in the current container
        /// </summary>
        public static IServiceCollection RegisterNotifications(this IServiceCollection services)
        {
            var adapter = new NotificationAdapter();

            services.AddSingleton<INotificationAdapter>(adapter);
            services.AddSingleton<INotificationSourceAdapter>(adapter);

            return services;
        }
    }
}