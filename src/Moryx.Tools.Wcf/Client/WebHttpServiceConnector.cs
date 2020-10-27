using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moryx.Communication;
using Moryx.Serialization;
using Newtonsoft.Json;

namespace Moryx.Tools.Wcf
{
    /// <summary>
    /// Base class to connect 
    /// </summary>
    public abstract class WebHttpServiceConnector : IHttpServiceConnector
    {
        private IVersionServiceManager _endpointService;

        private HttpClient _httpClient;

        private bool _isAvailable;

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
        /// Name of the service interface to connect to
        /// </summary>
        public abstract string ServiceName { get; }

        /// <summary>
        /// Create connector using the client factories version service
        /// </summary>
        protected WebHttpServiceConnector(IWcfClientFactory clientFactory)
        {
            _endpointService = ((BaseWcfClientFactory)clientFactory).VersionService;
        }

        /// <summary>
        /// Create connector with bare connection settings
        /// </summary>
        protected WebHttpServiceConnector(string host, int port, IProxyConfig proxyConfig)
        {
            _endpointService = new VersionServiceManager(proxyConfig, host, port);
        }

        /// <inheritdoc />
        public void Start()
        {
            TryFetchEndpoint();
        }

        private void TryFetchEndpoint()
        {
            _endpointService.ServiceEndpointsAsync(ServiceName)
                .ContinueWith(EvaluateResponse);
        }

        private void EvaluateResponse(Task<Endpoint[]> resp)
        {
            //Try again or dispose old client
            if (resp.Status != TaskStatus.RanToCompletion || resp.Result.Length == 0)
            {
                TryFetchEndpoint();
                return;
            }

            // Parse endpoint url
            var endpoint = resp.Result.FirstOrDefault(e => e.Binding == ServiceBindingType.WebHttp)?.Address;
            if (string.IsNullOrEmpty(endpoint))
            {
                TryFetchEndpoint();
                return;
            }

            // Create new base address client
            _httpClient = new HttpClient { BaseAddress = new Uri(endpoint) };

            IsAvailable = true;

            Task.Run(OnConnect);
        }

        /// <inheritdoc />
        public void Stop()
        {
            _httpClient?.Dispose();
        }

        /// <summary>
        /// Initial action to perform when client connected
        /// </summary>
        public virtual Task OnConnect()
        {
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Get data from URL
        /// </summary>
        protected async Task<T> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<T>(response);
        }

        /// <summary>
        /// Post data to URL
        /// </summary>
        protected async Task<T> PostAsync<T>(string url, object payload)
        {
            var payloadString = string.Empty;
            if (payload != null)
                payloadString = JsonConvert.SerializeObject(payload);

            var response = await _httpClient.PostAsync(url, new StringContent(payloadString, Encoding.UTF8, "text/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        /// <summary>
        /// Put new data on endpoint
        /// </summary>
        protected async Task<T> PutAsync<T>(string url, object payload)
        {
            var payloadString = string.Empty;
            if (payload != null)
                payloadString = JsonConvert.SerializeObject(payload);

            var response = await _httpClient.PutAsync(url, new StringContent(payloadString, Encoding.UTF8, "text/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        /// <summary>
        /// Delete on endpoint
        /// </summary>
        protected async Task<bool> DeleteAsync(string url)
        {
            var response = await _httpClient.DeleteAsync(url);
            return response.StatusCode == HttpStatusCode.OK;
        }

        /// <inheritdoc />
        public event EventHandler AvailabilityChanged;
    }
}