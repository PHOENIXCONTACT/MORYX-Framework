// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moryx.Identity.AccessManagement.Data;
using Moryx.Identity.AccessManagement.Models;
using Moryx.Identity.AccessManagement.Settings;
using Moryx.Identity.Models;

namespace Moryx.Identity.AccessManagement
{
    /// <inheritdoc/>
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly MoryxUserManager _userManager;
        private readonly IPermissionManager _permissionManager;
        private readonly MoryxIdentitiesDbContext _dbContext;
        private readonly JwtSecurityTokenHandler _jwtTokenHandler;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Creates a new token service.
        /// </summary>
        /// <param name="jwtSettings">The settings used for the JWT token creation.</param>
        /// <param name="userManager">The user manager providing the users for the token creation.</param>
        /// <param name="permissionManager">The permission manager providing the permisions to generate claims for.</param>
        /// <param name="tokenValidationParameters">The parameters used for token validation.</param>
        /// <param name="dbContext">The database context for the MORYX AccessManagement</param>
        public TokenService(JwtSettings jwtSettings,
            MoryxUserManager userManager,
            IPermissionManager permissionManager,
            TokenValidationParameters tokenValidationParameters,
            MoryxIdentitiesDbContext dbContext,
            IConfiguration configuration)
        {
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _userManager = userManager;
            _permissionManager = permissionManager;
            _dbContext = dbContext;
            _jwtTokenHandler = new JwtSecurityTokenHandler();
            _configuration = configuration;
        }

        /// <inheritdoc/>
        public async Task<AuthResult> GenerateToken(MoryxUser user)
        {
            var claims = new List<Claim> {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(ClaimTypes.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.Id)
            };

            if (!string.IsNullOrEmpty(user.Firstname))
                claims.Add(new Claim(ClaimTypes.GivenName, user.Firstname));

            if (!string.IsNullOrEmpty(user.LastName))
                claims.Add(new Claim(ClaimTypes.Surname, user.LastName));

            // Add roles of the user
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
            claims.AddRange(roleClaims);

            // Generate key
            var securityKey = _tokenValidationParameters.IssuerSigningKey;
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtSettings.ExpirationInMinutes));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Audience = _jwtSettings.Issuer,
                Issuer = _jwtSettings.Issuer,
                Expires = expires,
                SigningCredentials = credentials
            };

            var token = _jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = _jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                IsUsed = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(Convert.ToDouble(_jwtSettings.RefreshTokenExpirationInDays)),
                IsRevoked = false,
                Token = RandomString(25) + Guid.NewGuid()
            };

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new AuthResult
            {
                Token = jwtToken,
                Success = true,
                RefreshToken = refreshToken.Token,
                Domain = _configuration["CookieDomain"]
            };
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Claim>> GetAllPermissionClaims(IList<string> roles)
        {
            var allPermissions = new List<Permission>();
            foreach (var role in roles)
            {
                var permissions = await _permissionManager.FindForRole(role);
                allPermissions.AddRange(permissions);
            }

            var distinctPermissions = allPermissions
                .Select(p => p.Name).Distinct();

            var permissionClaims = distinctPermissions.Select(p => new Claim("permission", p));
            return permissionClaims;
        }

        /// <inheritdoc/>
        public async Task<AuthResult> VerifyAndGenerateRefreshToken(TokenRequest tokenRequest)
        {
            var validateLifetime = _tokenValidationParameters.ValidateLifetime;
            try
            {
                // Bypass exception by temporary validating expired tokens as well
                _tokenValidationParameters.ValidateLifetime = false;

                // This validation function will make sure that the token meets the validation parameters
                // and its an actual jwt token not just a random string
                var principal = _jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);

                // Now we need to check if the token has a valid security algorithm
                var jwtSecurityToken = validatedToken as JwtSecurityToken;
                var result = jwtSecurityToken?.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                if (result == false)
                    return FailureResult($"The token uses invalid security algorithm {jwtSecurityToken.Header.Alg}.", tokenRequest);

                var jtiClaim = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti);
                if (jtiClaim == null)
                    return FailureResult($"The token does not contain an ID.", tokenRequest);

                var expiryClaim = principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp);
                var expiryParsingSuccess = long.TryParse(expiryClaim?.Value, out var utcExpiryDate);
                if (expiryClaim == null || !expiryParsingSuccess)
                    return FailureResult($"The token does not contain a valid timestamp.", tokenRequest);

                var expDate = UnixTimeStampToDateTime(utcExpiryDate);
                if (expDate > DateTime.UtcNow)
                    return FailureResult("Cannot refresh this token since it has not expired yet.", tokenRequest);

                var storedRefreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);
                if (storedRefreshToken == null)
                    return FailureResult("The token does not exist.", tokenRequest);

                if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                    return FailureResult("The token has expired, user needs to login again.", tokenRequest);

                if (storedRefreshToken.IsUsed)
                    return FailureResult("The refresh token has already been used.", tokenRequest);

                if (storedRefreshToken.IsRevoked)
                    return FailureResult("The token has been revoked.", tokenRequest);

                if (storedRefreshToken.JwtId != jtiClaim.Value)
                    return FailureResult("The token does not matched the stored token for this user.", tokenRequest);

                storedRefreshToken.IsUsed = true;
                _dbContext.RefreshTokens.Update(storedRefreshToken);
                await _dbContext.SaveChangesAsync();

                var dbUser = await _userManager.FindByIdAsync(storedRefreshToken.UserId);
                return await GenerateToken(dbUser);
            }
            catch
            {
                return FailureResult("An unexpected error occured while verifying the token.", tokenRequest);
            }
            finally
            {
                // Change to original value
                _tokenValidationParameters.ValidateLifetime = validateLifetime;
            }
        }

        private static AuthResult FailureResult(string errorMessage, TokenRequest tokenRequest)
        {
            return new AuthResult()
            {
                Success = false,
                Errors = new List<string>() { errorMessage },
                Token = tokenRequest.Token,
                RefreshToken = tokenRequest.RefreshToken
            };
        }

        /// <inheritdoc />
        public async Task InvalidateRefreshToken(string token)
        {
            var securityToken = _jwtTokenHandler.ReadJwtToken(token);
            var refreshToken = await _dbContext.RefreshTokens.Where(token => token.JwtId == securityToken.Id).SingleOrDefaultAsync();

            if (refreshToken == null)
                return;
            _dbContext.RefreshTokens.Remove(refreshToken);
            await _dbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task InvalidateRefreshTokens(MoryxUser user)
        {
            var refreshTokens = await _dbContext.RefreshTokens.Where(token => token.UserId == user.Id).ToListAsync();
            _dbContext.RefreshTokens.RemoveRange(refreshTokens);
            await _dbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public bool IsTokenValid(string token)
        {
            try
            {
                _jwtTokenHandler.ValidateToken(token, _tokenValidationParameters,
                    out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[RandomNumberGenerator.GetInt32(s.Length)]).ToArray());
        }

        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }
    }
}

