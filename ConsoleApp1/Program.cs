﻿using System;
using System.Drawing;
using System.Drawing.Imaging;  // Required for System.Drawing.Imaging.ImageFormat
using System.IO;
using System.Linq;
using Tesseract;

// Alias the conflicting ImageFormat
using ImagingImageFormat = System.Drawing.Imaging.ImageFormat;  // Alias for System.Drawing.Imaging.ImageFormat
using TessImageFormat = Tesseract.ImageFormat;                  // Alias for Tesseract.ImageFormat

class Program
{
    static void Main(string[] args)
    {
        // Set the input and output folder paths
        string inputFolderPath = @"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ConsoleApp1\InputImages";
        string outputFolderPath = @"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ConsoleApp1\ProceesedImages";

        // Create the output folder if it doesn't exist
        if (!Directory.Exists(outputFolderPath))
        {
            Directory.CreateDirectory(outputFolderPath);
        }

        // Get all image files with specified extensions from the input folder
        string[] imageFiles = Directory.GetFiles(inputFolderPath, "*.*")
            .Where(file => file.ToLower().EndsWith(".jpg") ||
                           file.ToLower().EndsWith(".jpeg") ||
                           file.ToLower().EndsWith(".png"))
            .ToArray();

        foreach (string file in imageFiles)
        {
            // Load the image
            Image image = Image.FromFile(file);
            Console.WriteLine($"Processing image: {file}");

            // Preprocess the image (convert to grayscale)
            Bitmap grayImage = ConvertToGrayscale((Bitmap)image);

            // Save the preprocessed image to the output folder
            string outputFilePath = Path.Combine(outputFolderPath, Path.GetFileName(file));
            grayImage.Save(outputFilePath, ImagingImageFormat.Jpeg);  // Save as JPEG format
            Console.WriteLine($"Saved processed image: {outputFilePath}");

            // Extract text using Tesseract OCR
            string extractedText = ExtractTextFromImage(outputFilePath);
            Console.WriteLine($"Extracted Text from {Path.GetFileName(file)}:\n{extractedText}\n");
        }
    }

    // Method to convert an image to grayscale
    public static Bitmap ConvertToGrayscale(Bitmap originalImage)
    {
        Bitmap grayImage = new Bitmap(originalImage.Width, originalImage.Height);
        using (Graphics g = Graphics.FromImage(grayImage))
        {
            // Create grayscale color matrix
            var colorMatrix = new ColorMatrix(new float[][]
            {
                new float[] { 0.3f, 0.3f, 0.3f, 0, 0 },
                new float[] { 0.59f, 0.59f, 0.59f, 0, 0 },
                new float[] { 0.11f, 0.11f, 0.11f, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { 0, 0, 0, 0, 1 }
            });

            var attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);

            // Apply the color matrix to convert the image to grayscale
            g.DrawImage(originalImage,
                        new Rectangle(0, 0, originalImage.Width, originalImage.Height),
                        0, 0, originalImage.Width, originalImage.Height,
                        GraphicsUnit.Pixel, attributes);
        }

            // Apply binarization
            for (int x = 0; x < grayImage.Width; x++)
            { 
            for (int y = 0; y < grayImage.Height; y++)
            {
                Color pixelColor = grayImage.GetPixel(x, y);
                int brightness = (int)((pixelColor.R + pixelColor.G + pixelColor.B) / 3);
                grayImage.SetPixel(x, y, brightness < 128 ? Color.Black : Color.White);
            }
            }

        return grayImage;

    }

    // Method to extract text from an image using Tesseract OCR
    public static string ExtractTextFromImage(string imagePath)
    {
        using (var engine = new TesseractEngine(@"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ConsoleApp1\tessdata", "eng", EngineMode.Default))
        {
            using (var img = Pix.LoadFromFile(imagePath))
            {
                using (var page = engine.Process(img))
                {
                    return page.GetText();
                }
            }
        }
    }
}
