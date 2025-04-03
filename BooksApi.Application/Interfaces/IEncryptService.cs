using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.Interfaces
{
    public interface IEncryptService
    {
        string HashPassword(string password, string email);
        bool VerifyPassword(string hashedPassword, string enteredPassword, string email);
    }
}
