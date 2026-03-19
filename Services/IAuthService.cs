using GymManagementSystem.Models;

namespace GymManagementSystem.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string ErrorMessage)> RegisterAsync(string fullName, string email, string password);
        Task<Admin?> ValidateLoginAsync(string email, string password);
        Task<bool> AdminExistsAsync();
    }
}
