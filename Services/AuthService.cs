using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Data;
using GymManagementSystem.Models;

namespace GymManagementSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string ErrorMessage)> RegisterAsync(string fullName, string email, string password)
        {
            // Check if email already exists
            var existingAdmin = await _context.Admins.AnyAsync(a => a.Email == email);
            if (existingAdmin)
            {
                return (false, "Kayıt işlemi gerçekleştirilemedi. Lütfen bilgilerinizi kontrol edin");
            }

            var admin = new Admin
            {
                FullName = fullName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Admin",
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return (true, string.Empty);
        }

        public async Task<Admin?> ValidateLoginAsync(string email, string password)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == email);

            if (admin == null)
                return null;

            if (!admin.IsActive)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash))
                return null;

            return admin;
        }

        public async Task<bool> AdminExistsAsync()
        {
            return await _context.Admins.AnyAsync();
        }
    }
}
