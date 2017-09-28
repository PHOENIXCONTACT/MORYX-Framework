using System.Collections.ObjectModel;
using Marvin.Modules;

namespace Marvin.Runtime.Base
{
    internal class ServerNotificationCollection : ObservableCollection<IModuleNotification>, INotificationCollection
    {
    }
}