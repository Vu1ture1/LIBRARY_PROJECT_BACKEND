using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksApi.Application.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions, CancellationToken cancellationToken);
        void DeleteFile(string fileNameWithExtension, CancellationToken cancellationToken);
    }
}
