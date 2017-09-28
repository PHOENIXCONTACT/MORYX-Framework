using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Marvin.Container;

namespace Marvin.Runtime.Diagnostics.ModuleFailureReporter
{
    [Plugin(LifeCycle.Singleton, typeof(IMailClient))]
    internal class MailClient : IMailClient
    {
        private MailClientConfig _config;
        private MailAddress _sender;
        private readonly Dictionary<MailMessage, Action<bool>> _waitingCallbacks = new Dictionary<MailMessage, Action<bool>>();
   
        /// <summary>
        /// Internal smtp client
        /// </summary>
        private SmtpClient _client;
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
        }

        /// <summary>
        /// Deliver mail
        /// </summary>
        public void SendMail(Mail mail, Action<bool> resultCallback)
        {
            var recipient = new MailAddress(mail.Recipients.First());
            var message = new MailMessage(_sender, recipient)
            {
                Subject = mail.Subject,
                Body = mail.MessageBody,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8
            };
            if(mail.AttachmentPath != null)
                message.Attachments.Add(new Attachment(mail.AttachmentPath));
            foreach (var address in mail.Recipients.Skip(1))
            {
                message.CC.Add(address);
            }

            _waitingCallbacks[message] = resultCallback;
            _client.SendAsync(message, message);
        }

        /// <summary>
        /// Initialize mail client
        /// </summary>
        /// <param name="clientConfig">Config to use</param>
        public void Initialize(MailClientConfig clientConfig)
        {
            _config = clientConfig;
            _client = new SmtpClient(clientConfig.MailServer, clientConfig.Port);
            _client.SendCompleted += SendMailCompleted;
            _sender = new MailAddress(clientConfig.SenderAddress, clientConfig.Sender);
        }

        private void SendMailCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            var mailMessage = asyncCompletedEventArgs.UserState as MailMessage;
            // Unknown mail or faulty object
            if (mailMessage == null || !_waitingCallbacks.ContainsKey(mailMessage))
                return;

            Action<bool> callback = null;
            lock (_waitingCallbacks)
            {
                callback = _waitingCallbacks[mailMessage];
                _waitingCallbacks.Remove(mailMessage);
                // dispose maessage and attachment, otherwise it will be write-protected (open handle)
                // see: http://stackoverflow.com/questions/7367701/closing-file-attached-to-email-system-net-mail
                mailMessage.Dispose();
            }
        
            if(callback != null)
                callback(asyncCompletedEventArgs.Error == null);
        }
    }
}
