using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using QueueManager.Models;
using QueueManager.Repositories;

namespace QueueManager.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository = new();

        public User? Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var user = _userRepository.GetByUsername(username.Trim());

            if (user == null)
                return null;

            return PasswordHasher.VerifyPassword(password, user.PasswordHash)
                ? user
                : null;
        }

        public bool Register(string username, string password, UserRole role = UserRole.User)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            username = username.Trim();

            if (_userRepository.UsernameExists(username))
                return false;

            var user = new User
            {
                Username = username,
                PasswordHash = PasswordHasher.HashPassword(password),
                Role = role,
                CreatedAt = DateTime.Now
            };

            _userRepository.Add(user);
            return true;
        }

        public void CreateDefaultUser()
        {
            const string username = "user";
            const string password = "user123";

            if (_userRepository.UsernameExists(username))
                return;

            Register(username, password, UserRole.User);

            AppLogger.Warning(
                "Utworzono domyślne konto użytkownika: user.");
        }

        public void CreateDefaultAdmin()
        {
            const string username = "admin";
            const string password = "admin123";

            if (_userRepository.UsernameExists(username))
                return;

            Register(username, password, UserRole.Admin);

            AppLogger.Warning(
                "Utworzono domyślne konto administratora: admin.");
        }
    }
}