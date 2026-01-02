
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Net.Http;

namespace Moryx.Grpc
{
    /// <summary>
    /// Extension methods for ClientConfig
    /// </summary>
    public static class ClientConfigExtensions
    {
        /// <summary>
        /// With a given <paramref name="config"/> creates an Address/URL
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string CreateAddress(ClientConfig config)
        {
            var uriBuilder = new UriBuilder()
            {
                Host = config.Host,
                Port = config.Port,
                Scheme = config.UseTls ? "https" : "http"
            };
            return uriBuilder.Uri.ToString();
        }

        /// <summary>
        /// WIth a given <paramref name="config"/> creates a channel options
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static GrpcChannelOptions CreateChannelOptions(ClientConfig config)
        {

            if (config.UseTls)
            {
                // Make sure to use the certificate from the provided path
                // Adding it to the (windows) certificates store did not work
                // Maybe this can be improved in the future
                var httpClientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                    {
#if NET8_0_OR_GREATER
                        chain!.ChainPolicy.TrustMode = System.Security.Cryptography.X509Certificates.X509ChainTrustMode.CustomRootTrust;
                        chain.ChainPolicy.CustomTrustStore.Add(new System.Security.Cryptography.X509Certificates.X509Certificate2(config.CertPath!));
#endif
                        return chain.Build(cert!);
                    }
                };
                return new GrpcChannelOptions
                {
                    HttpHandler = httpClientHandler,
                };
            }

            return new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Insecure,
            };
        }
    }
}
