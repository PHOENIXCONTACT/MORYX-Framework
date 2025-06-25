using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Moryx.Identity.Tests
{
    public class MoryxIdentityHandlerTests
    {
        private IHostBuilder _hostBuilder;
        private Mock<HttpClientHandler> _httpClientHandlerMock;
        private HttpClient _httpClient;
        private JwtSecurityTokenHandler _jwtTokenHandler;
        private const string _specificPermission = "Need.Specific.Permission";
        private const string _anotherSpecificPermission = "Need.Another.Specific.Permission";

        [SetUp]
        public void Setup()
        {
            _httpClientHandlerMock = new Mock<HttpClientHandler>();
            _httpClient = new HttpClient(_httpClientHandlerMock.Object);
            _jwtTokenHandler = new JwtSecurityTokenHandler();

            _hostBuilder = new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .ConfigureServices(services =>
                        {
                            services.AddMemoryCache();
                            services.AddScoped<HttpClient>(_ => _httpClient);
                            services.Configure<MoryxIdentityOptions>(options =>
                            {
                                options.BaseAddress = "https://localhost";
                            });
                            services.AddAuthentication(MoryxIdentityDefaults.AuthenticationScheme)
                                .AddMoryxIdentity();
                            services.AddAuthorization();
                            services.AddSingleton<IAuthorizationPolicyProvider, MoryxAuthorizationPolicyProvider>();
                            services.AddRouting();
                            services.AddControllers();
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();
                            app.UseAuthentication();
                            app.UseAuthorization();
                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapControllers();
                            });
                        });
                });
        }

        [Test]
        public async Task ShouldAlwaysAllowAccessToEndpointsWithNoAuthorizationAttribute(
            [Values("NoAuthClass/NoAuthMethod", "SpecificAuthClass/AnonymousMethod")] string endpoint)
        {
            var permissionsResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
            };
            SetMockResponse(permissionsResponse);
            HttpRequestMessage request = GetNewRequest(endpoint, withCookie: false);

            var host = await _hostBuilder.StartAsync();
            var response = await host.GetTestClient().SendAsync(request);

            Assert.That(response.IsSuccessStatusCode);
        }

        [Test]
        public async Task ShouldAllowAccessToEndpointsWithoutSpecificPermissionIfUserIsSignedIn(
            [Values("NoAuthClass/AuthMethod", "AuthClass/NoAuthMethod")] string endpoint,
            [Values(false, true)] bool isSignedIn)
        {
            var permissionsResponse = new HttpResponseMessage
            {
                StatusCode = isSignedIn ? HttpStatusCode.OK : HttpStatusCode.Unauthorized,
                Content = new StringContent(@"[]"),
            };
            SetMockResponse(permissionsResponse);
            HttpRequestMessage request = GetNewRequest(endpoint);

            var host = await _hostBuilder.StartAsync();
            var response = await host.GetTestClient().SendAsync(request);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.NotFound));
            if (isSignedIn)
                Assert.That(response.IsSuccessStatusCode);
            else
                Assert.That(response.IsSuccessStatusCode, Is.False);
        }

        [Test]
        public async Task ShouldAllowAccessToEndpointsWithSpecificPermissionIfUserHasPermission(
            [Values("NoAuthClass/SpecificAuthMethod", "AuthClass/SpecificAuthMethod", "SpecificAuthClass/NoAuthMethod", "SpecificAuthClass/AuthMethod")] string endpoint,
            [Values(false, true)] bool hasPermission)
        {
            var permissionsResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(hasPermission ? $"[\"{_specificPermission}\"]" : @"[]"),
            };
            SetMockResponse(permissionsResponse);
            HttpRequestMessage request = GetNewRequest(endpoint);

            var host = await _hostBuilder.StartAsync();
            var response = await host.GetTestClient().SendAsync(request);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.NotFound));
            if (hasPermission)
                Assert.That(response.IsSuccessStatusCode);
            else
                Assert.That(response.IsSuccessStatusCode, Is.False);
        }

        [Test]
        public async Task ShouldAllowAccessToEndpointsWithMultipleSpecificPermissionsIfUserHasAllPermissions(
            [Values("SpecificAuthClass/SpecificAuthMethod")] string endpoint,
            [Values(false, true)] bool hasPermission)
        {
            Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> returnFunction = (HttpRequestMessage request, CancellationToken token) =>
            {
                var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
                var filterValue = query["filter"];
                var responseMessage = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(@"[]")
                };

                if (!hasPermission)
                    return responseMessage;

                if (string.IsNullOrEmpty(filterValue))
                    responseMessage.Content = new StringContent($"[\"{_specificPermission}\", \"{_anotherSpecificPermission}\"]");
                else if (_specificPermission.StartsWith(filterValue))
                    responseMessage.Content = new StringContent($"[\"{_specificPermission}\"]");
                else if (_anotherSpecificPermission.StartsWith(filterValue))
                    responseMessage.Content = new StringContent($"[\"{_anotherSpecificPermission}\"]");
                return responseMessage;
            };

            SetMockResponse(null, returnFunction);
            HttpRequestMessage request = GetNewRequest(endpoint);

            var host = await _hostBuilder.StartAsync();
            var response = await host.GetTestClient().SendAsync(request);

            Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.NotFound));
            if (hasPermission)
                Assert.That(response.IsSuccessStatusCode);
            else
                Assert.That(response.IsSuccessStatusCode, Is.False);
        }

        private HttpRequestMessage GetNewRequest(string endpoint, bool withCookie = true)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/{endpoint}");
            if (withCookie)
            {
                var token = _jwtTokenHandler.CreateToken(new SecurityTokenDescriptor());
                var jwtToken = _jwtTokenHandler.WriteToken(token);

                var jwtCookie = new Cookie(MoryxIdentityDefaults.JWT_COOKIE_NAME, jwtToken);
                var refreshCookie = new Cookie(MoryxIdentityDefaults.REFRESH_TOKEN_COOKIE_NAME, "ExampleCookieValue");
                request.Headers.Add("Cookie", jwtCookie.ToString());
                request.Headers.Add("Cookie", refreshCookie.ToString());
            }
            return request;
        }

        private void SetMockResponse(HttpResponseMessage response, Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> returnFunction = null)
        {
            var requestSetup = _httpClientHandlerMock
                 .Protected()
                 .Setup<Task<HttpResponseMessage>>(
                     "SendAsync",
                     ItExpr.IsAny<HttpRequestMessage>(),
                     ItExpr.IsAny<CancellationToken>()
                 );
            if (returnFunction == null)
                requestSetup.ReturnsAsync(response);
            else
                requestSetup.ReturnsAsync(returnFunction);
        }
    }
}