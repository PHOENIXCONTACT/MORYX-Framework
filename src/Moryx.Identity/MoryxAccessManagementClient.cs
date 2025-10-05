// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moryx.Identity.Models;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Moryx.Identity
{
    /// <summary>
    /// Represents a client for accessing the MoryxAccessManagement.
    /// </summary>
    public class MoryxAccessManagementClient
    {
        // Best practice for dealing with the lifetime of an httpClient according
        // to https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines
        private static readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError().WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        private static readonly SocketsHttpHandler _socketHandler = new()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(60),
            UseCookies = false,
            Proxy = new WebProxy() { BypassProxyOnLocal = true }
        };
        private static readonly PolicyHttpMessageHandler _pollyHandler = new(_retryPolicy)
        {
            InnerHandler = _socketHandler
        };
        private static HttpClient _identityClient;

        private readonly IMemoryCache _memoryCache;
        private readonly ILogger _logger;
        private static readonly SemaphoreSlim _semaphore = new(1, 1);

        private readonly IOptionsMonitor<MoryxIdentityOptions> _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoryxAccessManagementClient"/>.
        /// </summary>
        /// <inheritdoc />
        public MoryxAccessManagementClient(IOptionsMonitor<MoryxIdentityOptions> options, IMemoryCache memoryCache, ILogger logger, HttpClient identityClient = null)
        {
            _identityClient = identityClient ?? _identityClient ?? new(_pollyHandler);
            if (_identityClient.BaseAddress == null)
            {
                _identityClient.BaseAddress = new Uri(options.CurrentValue.BaseAddress);
            }

            _memoryCache = memoryCache;
            _options = options;
            _logger = logger;
        }

        /// <summary>
        /// Refreshes the authentication tokens.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>The refreshed authentication tokens.</returns>
        public async Task<AuthResult> GetRefreshedTokens(string token, string refreshToken)
        {
            try
            {
                await _semaphore.WaitAsync();

                var result = await _memoryCache.GetOrCreateAsync<AuthResult>(token + refreshToken, async cache =>
                {
                    cache.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                    return await GetRefreshedTokensAsync(token, refreshToken);
                });

                _logger?.LogDebug("Retrieved a new token");
                return result;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<AuthResult> GetRefreshedTokensAsync(string token, string refreshToken)
        {
            var uri = _options.CurrentValue.RefreshTokenUri;

            using var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Add("Cookie",
                $"{MoryxIdentityDefaults.JWT_COOKIE_NAME}={token}; {MoryxIdentityDefaults.REFRESH_TOKEN_COOKIE_NAME}={refreshToken}");

            HttpResponseMessage result;
            try
            {
                result = await _identityClient.SendAsync(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger?.LogCritical(ex, "Failed to request new token at the IAM server");
                return new AuthResult() { Token = token, RefreshToken = refreshToken, Success = false };
            }

            if (!result.IsSuccessStatusCode)
            {
                _logger?.LogDebug("Failed to request new token at the IAM server: {result}", result);
                return new AuthResult() { Token = token, RefreshToken = refreshToken, Success = false };
            }

            var deserializedResult = await JsonSerializer.DeserializeAsync<AuthResult>(await result.Content.ReadAsStreamAsync());
            if (deserializedResult.Success)
            {
                _logger?.LogDebug("Successfully refreshed a token");
            }
            else
            {
                _logger?.LogDebug("Server denied refresh request: {errors}", deserializedResult?.Errors.FirstOrDefault());
            }

            return deserializedResult;
        }

        /// <summary>
        /// Retrieves the permissions that match the <paramref name="requiredPolicy"/> filter.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="requiredPolicy">The required policy.</param>
        /// <returns>The permissions associated with <paramref name="token"/>.</returns>
        public async Task<IEnumerable<string>> GetPermissions(string token, string refreshToken, string requiredPolicy = "")
        {
            var uri = requiredPolicy == "" ? _options.CurrentValue.RequestUri : _options.CurrentValue.RequestUri + "?filter=" + requiredPolicy;

            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("Cookie",
                $"{MoryxIdentityDefaults.JWT_COOKIE_NAME}={token}; {MoryxIdentityDefaults.REFRESH_TOKEN_COOKIE_NAME}={refreshToken}");

            HttpResponseMessage result;
            try
            {
                result = await _identityClient.SendAsync(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger?.LogCritical(ex, "Failed to verify permission {permission}", requiredPolicy);
                return null;
            }

            if (!result.IsSuccessStatusCode)
            {
                _logger?.LogDebug("Failed to verify permission {permission}: {result}", requiredPolicy, result);
                return null;
            }

            return await JsonSerializer.DeserializeAsync<IEnumerable<string>>(await result.Content.ReadAsStreamAsync());
        }
    }
}

