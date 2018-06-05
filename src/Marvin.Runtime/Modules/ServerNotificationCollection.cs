using System.Collections.ObjectModel;
using Marvin.Modules;

namespace Marvin.Runtime.Modules
{
    internal class ServerNotificationCollection : ObservableCollection<IModuleNotification>, INotificationCollection
    {
    }
}