using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Moryx.Identity.AccessManagement;
using Moryx.Identity.AccessManagement.Controllers;
using Moryx.Identity.AccessManagement.Data;
using Moryx.Identity.AccessManagement.Identity;
using Moryx.Identity.AccessManagement.Models;
using Moryx.Identity.Models;
using NUnit.Framework;

namespace Moryx.Identity.IdentityServer.Controllers.Tests.Identity
{
    public class LoginControllerTests
    {
        private Mock<MoryxUserManager> _moryxUserManagerMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IPasswordResetService> _pwResetServiceMock;
        private LoginController _loginController;

        [SetUp]
        public void Setup()
        {
            _moryxUserManagerMock = new Mock<MoryxUserManager>(
                Mock.Of<IUserStore<MoryxUser>>(), null, null, null, null, null, null, null, null);
            _tokenServiceMock = new Mock<ITokenService>();
            _mapperMock = new Mock<IMapper>();
            _configurationMock = new Mock<IConfiguration>();
            _pwResetServiceMock = new Mock<IPasswordResetService>();

            var moryxUser = new MoryxUser { Email = "test@test.com", UserName = "test", Id = Guid.NewGuid().ToString() };
            _moryxUserManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(moryxUser);
            var claims = new List<Claim> {
                new(JwtRegisteredClaimNames.Sub, moryxUser.Id),
                new(ClaimTypes.Name, moryxUser.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, moryxUser.Id)
            };

            var rsaProvider = new RSACryptoServiceProvider(512);
            SecurityKey securityKey = new RsaSecurityKey(rsaProvider);
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Audience = string.Empty,
                Issuer = string.Empty,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = signingCredentials
            };
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            _tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<MoryxUser>())).ReturnsAsync(new AuthResult { Success = true, Token = jwtToken, RefreshToken = jwtToken });
            _moryxUserManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<MoryxUser>(), It.IsAny<string>())).ReturnsAsync(true);
            _moryxUserManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(moryxUser);

            _loginController = new LoginController(_moryxUserManagerMock.Object, _tokenServiceMock.Object, _mapperMock.Object, _configurationMock.Object, _pwResetServiceMock.Object);
        }

        [Test]
        public async Task UserLoginWithoutCallbackUrlRedirectsToIndexOfHome()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            _loginController.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _loginController.Login(new UserLoginModel(), string.Empty) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("Index"));
                Assert.That(result.ControllerName, Is.EqualTo("Home"));
            });
        }

        [Test]
        public async Task UserLoginWithCallbackUrlRedirectsToCallbackUrl()
        {
            // Arrange
            var testUrl = "testUrl";
            var httpContext = new DefaultHttpContext();
            var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("returnUrl", testUrl);
            httpContext.Request.QueryString = new QueryString("?" + queryString.ToString());
            _loginController.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _loginController.Login(new UserLoginModel(), testUrl) as RedirectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Url, Is.EqualTo(testUrl));
        }

        [Test]
        public async Task UserCanOnlyChangeTheirOwnPassword([Values(true, false)] bool maliciouslyChangeOtherPw)
        {
            // Arrange 
            var randomResetToken = Guid.NewGuid().ToString();
            var randomUserName = Guid.NewGuid().ToString();
            var randomNewPassword = Guid.NewGuid().ToString();

            var pwReset = new PasswordReset()
            {
                ExpiryTime = DateTime.UtcNow.AddMinutes(2),
                ResetToken = maliciouslyChangeOtherPw ? Guid.NewGuid().ToString() : randomResetToken,
                UserId = maliciouslyChangeOtherPw ? Guid.NewGuid().ToString() : randomUserName,
            };
            _pwResetServiceMock.Setup(p => p.GetPasswordReset(It.IsAny<string>())).Returns(Task.FromResult(pwReset));

            var resetModel = new MoryxUserResetPasswordModel()
            {
                UserName = randomUserName,
                ResetToken = randomResetToken,
                NewPassword = randomNewPassword,
            };
            _moryxUserManagerMock
                .Setup(mock => mock.RemovePasswordAsync(It.IsAny<MoryxUser>())).Verifiable();
            _moryxUserManagerMock
                .Setup(mock => mock.AddPasswordAsync(It.IsAny<MoryxUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success))
                .Verifiable();

            // Act
            await _loginController.ResetPassword(resetModel);

            // Assert
            if (maliciouslyChangeOtherPw)
            {
                _moryxUserManagerMock.Verify(mock => mock.RemovePasswordAsync(It.IsAny<MoryxUser>()), Times.Never());
                _moryxUserManagerMock.Verify(mock => mock.AddPasswordAsync(It.IsAny<MoryxUser>(), It.IsAny<string>()), Times.Never());
            }
            else
            {
                _moryxUserManagerMock.Verify(mock => mock.RemovePasswordAsync(It.IsAny<MoryxUser>()), Times.Once());
                _moryxUserManagerMock.Verify(mock => mock.AddPasswordAsync(It.IsAny<MoryxUser>(), randomNewPassword), Times.Once());
            }
        }
    }
}