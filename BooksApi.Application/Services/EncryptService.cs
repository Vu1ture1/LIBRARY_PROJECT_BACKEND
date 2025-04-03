using BooksApi.Application.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.Services
{
    public class EncryptService : IEncryptService
    {
        public string HashPassword(string password, string email)
        {
            byte[] salt = Encoding.UTF8.GetBytes(email);

            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashedPassword;
        }
        public bool VerifyPassword(string hashedPassword, string enteredPassword, string email)
        {
            byte[] salt = Encoding.UTF8.GetBytes(email);

            string enteredHashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return enteredHashedPassword == hashedPassword;
        }
    }
}