using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ECommerceAPI.Helpers
{
    /// <summary>
    /// Helper class to manage image uploads, deletions, and access in the application.
    /// </summary>
    public class UploadImagesHelper
    {
        private readonly IWebHostEnvironment _environment;

        public UploadImagesHelper(IWebHostEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        /// <summary>
        /// Uploads an image to a specific folder and returns the saved file name.
        /// </summary>
        public async Task<string> UploadImageAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file.");

            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension))
                throw new ArgumentException("File must have an extension.");

            var fileName = Guid.NewGuid() + extension;
            var folderPath = Path.Combine(_environment.WebRootPath, "images", folderName);
            var filePath = Path.Combine(folderPath, fileName);

            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return GetPublicImageUrl(fileName, folderName);
            }
            catch (Exception ex)
            {
                throw new IOException("An error occurred while uploading the image.", ex);
            }
        }

        /// <summary>
        /// Returns the physical file path of an image.
        /// </summary>
        public string GetImagePath(string fileName, string folderName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Invalid file name.");

            var folderPath = Path.Combine(_environment.WebRootPath, "images", folderName);
            return Path.Combine(folderPath, fileName);
        }

        /// <summary>
        /// Returns a public URL to access the image.
        /// </summary>
        public string GetPublicImageUrl(string fileName, string folderName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Invalid file name.");

            return $"/images/{folderName}/{fileName}";
        }

        /// <summary>
        /// Deletes an image using its full path.
        /// </summary>
        public void DeleteImage(string filePath)
        {
            try
            {
                filePath = Path.Combine(_environment.WebRootPath, filePath);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to delete the image.", ex);
            }
        }

        /// <summary>
        /// Deletes an image using its name and folder.
        /// </summary>
        //public void DeleteImage(string fileName, string folderName)
        //{
        //    var filePath = GetImagePath(fileName, folderName);
        //    DeleteImage(filePath);
        //}

        /// <summary>
        /// Checks if an image exists by full path.
        /// </summary>
        public bool ImageExists(string filePath)
        {
            filePath = Path.Combine(_environment.WebRootPath, filePath);
            return File.Exists(filePath);
        }
    }
}
