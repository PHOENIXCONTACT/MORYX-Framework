// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Moryx.TestTools.SystemTest.Clients
{
    public class TestWebClientBase
    {
        private readonly string _baseUrl;
        private readonly JsonSerializerSettings _serializerSettings;

        public TestWebClientBase(string baseUrl)
        {
            _baseUrl = baseUrl;
            _serializerSettings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        }

        public string CreateQueryString(KeyValuePair<string, string>[] queryVariables)
        {
            return string.Join("&", queryVariables.Select(queryVariable => $"{queryVariable.Key}={queryVariable.Value}"));
        }

        public T Get<T>(string endpoint, params KeyValuePair<string, string>[] queryVariables)
        {
            var query = CreateQueryString(queryVariables);

            using (var wc = new WebClient())
            {
                return JsonConvert.DeserializeObject<T>(wc.DownloadString($"{_baseUrl}{endpoint}?{query}"), _serializerSettings);
            }
        }

        public async Task<T> GetAsync<T>(string endpoint, params KeyValuePair<string, string>[] queryVariables)
        {
            var query = CreateQueryString(queryVariables);

            using (var wc = new WebClient())
            {
                var result = await wc.DownloadStringTaskAsync($"{_baseUrl}{endpoint}?{query}");
                return JsonConvert.DeserializeObject<T>(result, _serializerSettings);
            }
        }

        public T PostAsJson<T>(string endpoint, object payload)
        {
            return SendAsJson<T>(endpoint, HttpMethod.Post.Method, payload);
        }

        public void PostAsJson(string endpoint, object payload)
        {
            SendAsJson(endpoint, HttpMethod.Post.Method, payload);
        }

        public Task<T> PostAsJsonAsync<T>(string endpoint, object payload)
        {
            return SendAsJsonAsync<T>(endpoint, HttpMethod.Post.Method, payload);
        }

        public Task PostAsJsonAsync(string endpoint, object payload)
        {
            return SendAsJsonAsync(endpoint, HttpMethod.Post.Method, payload);
        }

        public T PutAsJson<T>(string endpoint, object payload)
        {
            return SendAsJson<T>(endpoint, HttpMethod.Put.Method, payload);
        }

        public void PutAsJson(string endpoint, object payload)
        {
            SendAsJson(endpoint, HttpMethod.Put.Method, payload);
        }

        public Task<T> PutAsJsonAsync<T>(string endpoint, object payload)
        {
            return SendAsJsonAsync<T>(endpoint, HttpMethod.Put.Method, payload);
        }

        public Task PutAsJsonAsync(string endpoint, object payload)
        {
            return SendAsJsonAsync(endpoint, HttpMethod.Put.Method, payload);
        }

        public T DeleteAsJson<T>(string endpoint, object payload)
        {
            return SendAsJson<T>(endpoint, HttpMethod.Delete.Method, payload);
        }

        public void DeleteAsJson(string endpoint, object payload)
        {
            SendAsJson(endpoint, HttpMethod.Delete.Method, payload);
        }

        public Task<T> DeleteAsJsonAsync<T>(string endpoint, object payload)
        {
            return SendAsJsonAsync<T>(endpoint, HttpMethod.Delete.Method, payload);
        }

        public Task DeleteAsJsonAsync(string endpoint, object payload)
        {
            return SendAsJsonAsync(endpoint, HttpMethod.Delete.Method, payload);
        }

        private T SendAsJson<T>(string endpoint, string method, object payload)
        {
            using (var wc = new WebClient())
            {
                if(payload != null)
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";

                return JsonConvert.DeserializeObject<T>(wc.UploadString($"{_baseUrl}{endpoint}", method, SerializePayload(payload)));
            }
        }

        private void SendAsJson(string endpoint, string method, object payload)
        {
            using (var wc = new WebClient())
            {
                if (payload != null)
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";

                var payloadJson = SerializePayload(payload);
                wc.UploadString($"{_baseUrl}{endpoint}", method, payloadJson);
            }
        }

        private async Task<T> SendAsJsonAsync<T>(string endpoint, string method, object payload)
        {
            using (var wc = new WebClient())
            {
                if (payload != null)
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";

                var result = await wc.UploadStringTaskAsync($"{_baseUrl}{endpoint}", method, SerializePayload(payload));
                return JsonConvert.DeserializeObject<T>(result);
            }
        }

        private async Task SendAsJsonAsync(string endpoint, string method, object payload)
        {
            using (var wc = new WebClient())
            {
                if (payload != null)
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";

                await wc.UploadStringTaskAsync($"{_baseUrl}{endpoint}", method, SerializePayload(payload));
            }
        }

        private string SerializePayload(object payload)
        {
            return payload != null ? JsonConvert.SerializeObject(payload, _serializerSettings) : "";
        }
    }
}
