using Microsoft.Extensions.Configuration;
using PersonalWebsite.Models;
using System.Security.Cryptography;
using System.Text;

namespace PersonalWebsite.Authorization
{
    public class AuthorizationManager
    {
        public bool Authorized { get; private set; }

        private readonly IConfiguration configuration;
        private readonly SHA256 hasher;

        public AuthorizationManager(IConfiguration configuration)
        {
            this.configuration = configuration;
            hasher = SHA256.Create();
            Authorized = false;
        }

        public void SignIn(string password)
        {
            var passwordBytes = Encoding.ASCII.GetBytes(password);
            var passwordHash = hasher.ComputeHash(passwordBytes);
            var passwordHashAsString = System.Convert.ToBase64String(passwordHash);
            var expectedPasswordHashAsString = configuration["LoginDetails:PasswordHash"];
            if (passwordHashAsString == expectedPasswordHashAsString)
            {
                Authorized = true;
            }
        }
    }
}
