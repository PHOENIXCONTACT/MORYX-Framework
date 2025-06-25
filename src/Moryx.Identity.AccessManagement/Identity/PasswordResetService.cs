using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement.Identity
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly MoryxIdentitiesDbContext _dbContext;

        public PasswordResetService(MoryxIdentitiesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PasswordReset> GetPasswordReset(string userId)
        {
            return await _dbContext.PasswordResets.FirstOrDefaultAsync(pr => pr.UserId == userId);
        }

        public async Task<PasswordReset> GeneratePasswordReset(string userId)
        {
            var passwordReset = new PasswordReset
            {
                UserId = userId,
                ExpiryTime = DateTime.UtcNow.AddMinutes(30),
                ResetToken = RandomString()
            };

            _dbContext.PasswordResets.Add(passwordReset);
            await _dbContext.SaveChangesAsync();
            return passwordReset;
        }

        public async Task RemovePasswordReset(PasswordReset passwordReset)
        {
            _dbContext.PasswordResets.Remove(passwordReset);
            await _dbContext.SaveChangesAsync();
        }

        private static string RandomString(int length = 5)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[RandomNumberGenerator.GetInt32(s.Length)]).ToArray());
        }
    }
}