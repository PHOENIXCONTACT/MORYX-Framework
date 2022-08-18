// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moryx.Logging;
using Newtonsoft.Json;

namespace Moryx.Communication.Endpoints
{
    /// <summary>
    /// Base class to connect to an web api service hosted by the runtime
    /// </summary>
    public abstract class WebServiceConnectorBase : IWebServiceConnector
    {
        private readonly IVersionServiceManager _endpointService;
        private bool _isAvailable;

        /// <summary>
        /// Name of the service interface to connect to
        /// </summary>
        public abstract string ServiceName { get; }

        /// <summary>
        /// Logger of the connector
        /// </summary>
        protected IModuleLogger Logger { get; }

        /// <summary>
        /// Gets the current client version of the client
        /// </summary>
        protected abstract string ClientVersion { get; }

        /// <summary>
        /// Underlying http client used for the communication
        /// </summary>
        protected HttpClient HttpClient { get; private set; }

        /// <inheritdoc />
        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                _isAvailable = value;
                AvailabilityChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Create connector with bare connection settings and Logger
        /// </summary>
        protected WebServiceConnectorBase(string host, int port, IProxyConfig proxyConfig, IModuleLogger logger)
        {
            _endpointService = new WebVersionServiceManager(proxyConfig, host, port);
            Logger = logger;
        }

        private CultureInfo _myCulture;

        /// <inheritdoc />
        public void Start()
        {
            _myCulture = Thread.CurrentThread.CurrentUICulture;

            TryFetchEndpoint();
        }

        private void TryFetchEndpoint()
        {
            _endpointService.ServiceEndpointsAsync(ServiceName)
                .ContinueWith(EvaluateResponse).ConfigureAwait(false);
        }

        private async Task EvaluateResponse(Task<Endpoint[]> resp)
        {
            //Try again or dispose old client
            if (resp.Status != TaskStatus.RanToCompletion || resp.Result.Length == 0)
            {
                Logger.Log(LogLevel.Warning, "Failed to read endpoints.");
                await CallbackAndTryFetch(ConnectionState.FailedTry);
                return;
            }

            // Parse endpoint url
            var endpoint = resp.Result.FirstOrDefault();
            if (endpoint == null || string.IsNullOrEmpty(endpoint.Address))
            {
                Logger.Log(LogLevel.Error, "Endpoint for {0} has empty address: {1}",
                    ServiceName, endpoint?.Address);

                await CallbackAndTryFetch(ConnectionState.FailedTry);
                return;
            }

            var clientVersion = Version.Parse(ClientVersion);
            var serverVersion = Version.Parse(endpoint.Version);

            // Compare version
            if (VersionCompare.ClientMatch(serverVersion, clientVersion))
            {
                // Create HttpClient
                var proxyConfig = (_endpointService as IProxyConfigAccess)?.ProxyConfig;
                HttpClient = HttpClientBuilder.GetClient(endpoint.Address, proxyConfig);

                if (!string.IsNullOrEmpty(_myCulture.IetfLanguageTag))
                    HttpClient.DefaultRequestHeaders.AcceptLanguage
                        .Add(new StringWithQualityHeaderValue(_myCulture.IetfLanguageTag));

                IsAvailable = true;

                await ConnectionCallback(ConnectionState.Success);
            }
            else
            {
                Logger.Log(LogLevel.Error, "Version mismatch: Client: {0} - Server: {1}",
                    clientVersion, serverVersion);
                await CallbackAndTryFetch(ConnectionState.VersionMismatch);
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            HttpClient?.Dispose();
        }

        /// <summary>
        /// Executes the callback and starts the fetching after a defined delay
        /// </summary>
        private async Task CallbackAndTryFetch(ConnectionState connectionState)
        {
            await ConnectionCallback(connectionState);
            await Task.Delay(1000);
            TryFetchEndpoint();
        }

        /// <summary>
        /// Action which is called if the client gets connected or failed
        /// </summary>
        public virtual Task ConnectionCallback(ConnectionState connectionState)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Send a GET request to the specified Url as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Url the request is sent to.</param>
        /// <returns>The result as typed object</returns>
        protected async Task<T> GetAsync<T>(string url)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("Client not available!");

            var response = await HttpClient.GetAsync(url);
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseContent);
                case HttpStatusCode.NotFound:
                case HttpStatusCode.NoContent:
                    return default;
                default:
                    throw new WebException($"StatusCode: {response.StatusCode} ({(int)response.StatusCode}) - Check server log!");
            }
        }

        /// <summary>
        /// Send a POST request to the specified Url as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Url the request is sent to.</param>
        /// <param name="payload">The HTTP request content sent to the server.</param>
        /// <returns>The result as typed object</returns>
        protected async Task<T> PostAsync<T>(string url, object payload)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("Client not available!");

            var payloadString = string.Empty;
            if (payload != null)
                payloadString = JsonConvert.SerializeObject(payload);

            var response = await HttpClient.PostAsync(url, new StringContent(payloadString, Encoding.UTF8, "text/json"));
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseContent);
                case HttpStatusCode.NoContent:
                    return default;
                default:
                    throw new WebException($"StatusCode: {response.StatusCode} ({(int)response.StatusCode}) - Check server log!");
            }
        }

        /// <summary>
        /// Send a PUT request to the specified Url as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Url the request is sent to.</param>
        /// <param name="payload">The HTTP request content sent to the server.</param>
        /// <returns>The result as typed object</returns>
        protected async Task<T> PutAsync<T>(string url, object payload)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("Client not available!");

            var payloadString = string.Empty;
            if (payload != null)
                payloadString = JsonConvert.SerializeObject(payload);

            var response = await HttpClient.PutAsync(url, new StringContent(payloadString, Encoding.UTF8, "text/json"));
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseContent);
                case HttpStatusCode.NoContent:
                    return default;
                default:
                    throw new WebException($"StatusCode: {response.StatusCode} ({(int)response.StatusCode}) - Check server log!");
            }
        }

        /// <summary>
        /// Send a DELETE request to the specified Url as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Url the request is sent to.</param>
        /// <returns><c>true</c> if the result is <see cref="HttpStatusCode.OK"/></returns>
        protected async Task<bool> DeleteAsync(string url)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("Client not available!");

            var response = await HttpClient.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Send a DELETE request to the specified Url as an asynchronous operation.
        /// </summary>
        /// <param name="url">The Url the request is sent to.</param>
        /// <returns>The result as typed object</returns>
        protected async Task<T> DeleteAsync<T>(string url)
        {
            if (!IsAvailable)
                throw new InvalidOperationException("Client not available!");

            var response = await HttpClient.DeleteAsync(url);
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Accepted:
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseContent);
                case HttpStatusCode.NoContent:
                    return default;
                default:
                    throw new WebException($"StatusCode: {response.StatusCode} ({(int)response.StatusCode}) - Check server log!");
            }
        }

        /// <inheritdoc />
        public event EventHandler AvailabilityChanged;
    }
}