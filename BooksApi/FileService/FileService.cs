using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BooksApi.FileService
{
    public class FileService : IFileService
    {
        private IWebHostEnvironment environment;
        public FileService(IWebHostEnvironment environment) {
            this.environment = environment;
        }
        public async Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions)
        {
            if (imageFile == null)
            {
                //throw new ArgumentNullException(nameof(imageFile));
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
                throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");
            }

            var fileName = $"{Guid.NewGuid().ToString()}{ext}";
            var fileNameWithPath = Path.Combine(path, fileName);
            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await imageFile.CopyToAsync(stream);
            return fileName;
        }
        public void DeleteFile(string fileNameWithExtension)
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
            File.Delete(path);
        }
    }
}



