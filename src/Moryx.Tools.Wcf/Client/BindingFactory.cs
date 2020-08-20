using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using Moryx.Communication;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Creates Bindings for WCF clients.
    /// </summary>
    public static class BindingFactory
    {
        /// <summary>
        /// Creates the default binding by type of the binding.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="requiresAuthentication">if set to <c>true</c> [requires authentication].</param>
        /// <param name="proxyConfig">The proxy configuration.</param>
        /// <returns></returns>
        internal static Binding CreateDefault(BindingType type, bool requiresAuthentication, IProxyConfig proxyConfig)
        {
            switch (type)
            {
                case BindingType.NetTcp:
                    return CreateDefaultNetTcpBinding();
                case BindingType.BasicHttp:
                    return CreateDefaultBasicHttpBinding(requiresAuthentication, proxyConfig);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Creates a default NetTcp binding.
        /// </summary>
        /// <returns>The binding</returns>
        public static NetTcpBinding CreateDefaultNetTcpBinding()
        {
            return new NetTcpBinding(SecurityMode.None)
            {
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferPoolSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,

                SendTimeout = TimeSpan.FromSeconds(30),
                ReceiveTimeout = TimeSpan.MaxValue,
                CloseTimeout = TimeSpan.FromSeconds(30),
                OpenTimeout = TimeSpan.FromSeconds(30),

                Security = new NetTcpSecurity { Mode = SecurityMode.None }
            };
        }

        /// <summary>
        /// Creates a default BasicHttp binding including proxy configuration as configured for this factory.
        /// </summary>
        /// <param name="requiresAuthentication">If <c>true</c>, set the security mode to <c>BasicHttpSecurityMode.TransportCredentialOnly</c>,
        ///     otherwise <c>BasicHttpSecurityMode.None</c> will be used.</param>
        /// <param name="proxyConfig">An optional proxy configuration.</param>
        /// <returns>The binding</returns>
        public static BasicHttpBinding CreateDefaultBasicHttpBinding(bool requiresAuthentication, IProxyConfig proxyConfig)
        {
            var binding = new BasicHttpBinding
            {
                MaxBufferSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue,
                ReceiveTimeout = TimeSpan.FromSeconds(30),
                SendTimeout = TimeSpan.FromSeconds(30),
                OpenTimeout = TimeSpan.FromSeconds(30),
                CloseTimeout = TimeSpan.FromSeconds(30),
                ReaderQuotas =
                {
                    MaxArrayLength = int.MaxValue,
                    MaxStringContentLength = int.MaxValue,
                    MaxBytesPerRead = int.MaxValue
                },
                Security =
                {
                    Mode = requiresAuthentication ?  BasicHttpSecurityMode.TransportCredentialOnly : BasicHttpSecurityMode.None,
                    Transport =
                    {
                        ClientCredentialType = HttpClientCredentialType.Ntlm,
                        ProxyCredentialType = HttpProxyCredentialType.Ntlm,
                    },
                    Message =
                    {
                        ClientCredentialType = BasicHttpMessageCredentialType.UserName,
                    }

                },
                UseDefaultWebProxy = false,
            };

            if (proxyConfig != null && proxyConfig.EnableProxy)
            {
                binding.UseDefaultWebProxy = proxyConfig.UseDefaultWebProxy;

                if (!string.IsNullOrEmpty(proxyConfig.Address) && proxyConfig.Port != 0)
                {
                    var proxyUrl = $"http://{proxyConfig.Address}:{proxyConfig.Port}";

                    binding.ProxyAddress = new Uri(proxyUrl);
                }
            }

            return binding;
        }

        /// <summary>
        /// Creates BasicHttp binding as needed to connect to SAP services.
        /// </summary>
        /// <returns>The binding</returns>
        public static BasicHttpBinding CreateSapBinding()
        {
            BasicHttpBinding binding = new BasicHttpBinding
            {
                TextEncoding = new UTF8Encoding(),
                ReaderQuotas = new XmlDictionaryReaderQuotas(),
                Security =
                {
                    Mode = BasicHttpSecurityMode.TransportCredentialOnly,
                    Transport =
                    {
                        ClientCredentialType = HttpClientCredentialType.Basic,
                        ProxyCredentialType = HttpProxyCredentialType.Basic,
                    },
                    Message =
                    {
                        ClientCredentialType = BasicHttpMessageCredentialType.UserName,
                    }

                },
                UseDefaultWebProxy = false,
            };

            return binding;
        }
    }
}