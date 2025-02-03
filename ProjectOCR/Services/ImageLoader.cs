using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace OCRProject.Services
{
    // Class to store image data with its filename
    public class ImageData
    {
        public string FileName { get; set; }    // Name of the file (e.g., image1.jpg)
        public Bitmap Image { get; set; }       // Actual image in Bitmap format
    }

    public static class ImageLoader
    {
        // Method to load all images from a specified folder
        public static List<ImageData> LoadImages(string folderPath)
        {
            // Supported image extensions
            var supportedExtensions = new[] { ".jpg", ".jpeg", ".png" };

            // Get all image files with supported extensions
            var imageFiles = Directory.GetFiles(folderPath, "*.*")
                                      .Where(file => supportedExtensions.Contains(Path.GetExtension(file).ToLower()))
                                      .ToArray();

            var images = new List<ImageData>();

            foreach (var file in imageFiles)
            {
                try
                {
                    // Load the image as a Bitmap
                    Bitmap image = new Bitmap(file);

                    // Add image data to the list
                    images.Add(new ImageData
                    {
                        FileName = Path.GetFileName(file),
                        Image = image
                    });

                    Console.WriteLine($"Loaded image: {file}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading image {file}: {ex.Message}");
                }
            }

            return images;
        }
    }
}
