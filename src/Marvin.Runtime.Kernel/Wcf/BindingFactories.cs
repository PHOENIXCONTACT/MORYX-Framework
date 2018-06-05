using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Static factory to do the Wcf bindings.
    /// </summary>
    public static class BindingFactory
    {
        /// <summary>
        /// Creates a new basic http binding with our default configuration.
        /// Most parts are set to nearly int max, timeouts are set to 5 min.
        /// Encoding is Text and TransferMode is bufferd.
        /// </summary>
        /// <param name="requiresAuthentification">Is authentification required?</param>
        /// <returns>The new created binding.</returns>
        public static Binding CreateBasicHttpBinding(bool requiresAuthentification)
        {
            var basicHttpBinding = new BasicHttpBinding
            {
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferPoolSize = int.MaxValue,
                CloseTimeout = TimeSpan.FromSeconds(30),
                OpenTimeout = TimeSpan.FromSeconds(30),
                ReceiveTimeout = TimeSpan.FromSeconds(30),
                SendTimeout = TimeSpan.FromSeconds(30),
                MessageEncoding = WSMessageEncoding.Text,
                TransferMode = TransferMode.Buffered,
                UseDefaultWebProxy = true,
                ReaderQuotas =
                {
                    MaxStringContentLength = int.MaxValue,
                    MaxArrayLength = int.MaxValue,
                    MaxBytesPerRead = int.MaxValue,
                    MaxNameTableCharCount = int.MaxValue
                }
            };


            if (requiresAuthentification)
            {
                basicHttpBinding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
            }

            return basicHttpBinding;
        }

        /// <summary>
        /// Creates a new web http binding with our default configuration.
        /// Most values are set to nearly int max, UseDefaultWebProxy = true and security is none.
        /// </summary>
        /// <returns>A new web http binding.</returns>
        public static Binding CreateWebHttpBinding()
        {
            var webHttpBinding = new WebHttpBinding
            {
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferPoolSize = int.MaxValue,
                CloseTimeout = TimeSpan.FromSeconds(30),
                OpenTimeout = TimeSpan.FromSeconds(30),
                ReceiveTimeout = TimeSpan.FromSeconds(30),
                SendTimeout = TimeSpan.FromSeconds(30),
                UseDefaultWebProxy = true,
                ReaderQuotas =
                {
                    MaxStringContentLength = int.MaxValue,
                    MaxArrayLength = int.MaxValue,
                    MaxBytesPerRead = int.MaxValue,
                    MaxNameTableCharCount = int.MaxValue
                },
                Security =
                {
                    Mode = WebHttpSecurityMode.None
                }
            };

            return webHttpBinding;
        }

        /// <summary>
        /// Creates a new net tcp binding with our default configuration.
        /// Most values are set to nearly int max, timeouts are set to 5 min.
        /// </summary>
        /// <param name="requiresAuthentification">Is authentification required?</param>
        /// <returns>A new net tcp binding.</returns>
        public static Binding CreateNetTcpBinding(bool requiresAuthentification)
        {
            var binding = new NetTcpBinding
            {
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferPoolSize = int.MaxValue,
                CloseTimeout = TimeSpan.FromSeconds(30),
                OpenTimeout = TimeSpan.FromSeconds(30),
                SendTimeout = TimeSpan.FromSeconds(30),
                ReceiveTimeout = TimeSpan.MaxValue,
                ReaderQuotas =
                {
                    MaxStringContentLength = int.MaxValue,
                    MaxArrayLength = int.MaxValue,
                    MaxBytesPerRead = int.MaxValue,
                    MaxNameTableCharCount = int.MaxValue
                },
                Security =
                {
                    Mode = requiresAuthentification ? SecurityMode.Transport : SecurityMode.None
                }
            };

            return binding;
        }
    }
}
