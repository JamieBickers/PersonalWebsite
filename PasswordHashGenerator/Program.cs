using System;
using System.Security.Cryptography;
using System.Text;

namespace PasswordHashGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var hasher = SHA256.Create();
            Console.WriteLine("Enter the new password: ");
            var newPassword = Console.ReadLine();
            var passwordBytes = Encoding.ASCII.GetBytes(newPassword);
            var passwordHash = hasher.ComputeHash(passwordBytes);
            var passwordHashAsString = System.Convert.ToBase64String(passwordHash);
            Console.WriteLine(passwordHashAsString);
        }
    }
}

