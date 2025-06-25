using System.Threading.Tasks;
using Moryx.Identity.AccessManagement.Data;

namespace Moryx.Identity.AccessManagement.Identity
{
    public interface IPasswordResetService
    {
        Task<PasswordReset> GetPasswordReset(string userId);

        Task<PasswordReset> GeneratePasswordReset(string userId);

        Task RemovePasswordReset(PasswordReset passwordReset);
    }
}
