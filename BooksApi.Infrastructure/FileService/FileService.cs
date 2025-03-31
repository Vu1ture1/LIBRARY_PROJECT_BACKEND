using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using BooksApi.Domain.Exceptions.BookExceptions;
using BooksApi.Application.Interfaces;

namespace BooksApi.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private IWebHostEnvironment environment;
        public FileService(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }
        public async Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions, CancellationToken cancellationToken)
        {
            if (imageFile == null)
            {
                return "Default.jpg";
            }

            var contentPath = environment.WebRootPath;
            var path = Path.Combine(contentPath, "Images");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var ext = Path.GetExtension(imageFile.FileName);

            if (!allowedFileExtensions.Contains(ext))
            {
                throw new InvalidFileFormatException();
            }

            var fileName = $"{Guid.NewGuid().ToString()}{ext}";
            var fileNameWithPath = Path.Combine(path, fileName);
            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await imageFile.CopyToAsync(stream, cancellationToken);
            return fileName;
        }
        public void DeleteFile(string fileNameWithExtension, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(fileNameWithExtension))
            {
                throw new ArgumentNullException(nameof(fileNameWithExtension));
            }

            var contentPath = environment.WebRootPath;
            var path = Path.Combine(contentPath, $"Images", fileNameWithExtension);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Invalid file path");
            }

            cancellationToken.ThrowIfCancellationRequested();

            File.Delete(path);
        }
    }
}

