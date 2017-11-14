using System;
using Marvin.Modules;

namespace Marvin.Runtime.Base
{
    internal class WarningNotification : IModuleNotification
    {
        private readonly Action<WarningNotification> _confirmationDelegate;

        public WarningNotification(Action<WarningNotification> confirmationDelegate, Exception ex)
            : this(confirmationDelegate, ex, ex.Message)
        {
        }

        public WarningNotification(Action<WarningNotification> confirmationDelegate, Exception ex, string message)
        {
            _confirmationDelegate = confirmationDelegate;
            Type = NotificationType.Warning;
            Timestamp = DateTime.Now;
            Exception = ex;
            Message = message;
        }

        public bool Confirm()
        {
            _confirmationDelegate(this);
            return true;
        }

        public NotificationType Type { get; }

        public DateTime Timestamp { get; }

        public string Message { get; }

        public Exception Exception { get; }
    }
}
