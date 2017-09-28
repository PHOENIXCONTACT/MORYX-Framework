using System;

namespace Marvin.Runtime.Diagnostics.ModuleFailureReporter
{
    /// <summary>
    /// Interface for a mail client
    /// </summary>
    public interface IMailClient : IDisposable
    {
        /// <summary>
        /// Deliver mail
        /// </summary>
        void SendMail(Mail mail, Action<bool> result);

        /// <summary>
        /// Initialize mail client
        /// </summary>
        /// <param name="clientConfig">Config to use</param>
        void Initialize(MailClientConfig clientConfig);
    }
}
