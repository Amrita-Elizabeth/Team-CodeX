using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using OCRProject.Services;

class Program
{
    static void Main(string[] args)
    {
        string inputFolderPath = @"C:\Users\ASUS\OneDrive\Desktop\OCR\InputImages";
        string outputFolderPath = Path.GetFullPath(@"C:\Users\ASUS\OneDrive\Desktop\OCR\ProcessedImages");

        // Ensure output directory exists
        Directory.CreateDirectory(outputFolderPath);
        Console.WriteLine($"Output folder path: {outputFolderPath}");

        // Load images from the input folder
        var images = ImageLoader.LoadImages(inputFolderPath).ToList();
        Console.WriteLine($"Loaded {images.Count} images for processing.");

        var preprocessor = new Preprocessor();

        foreach (var imageData in images)
        {
            Console.WriteLine($"Processing image: {imageData.FileName}");
            string baseFileName = Path.GetFileNameWithoutExtension(imageData.FileName);
            Dictionary<string, Bitmap> preprocessedImages = new Dictionary<string, Bitmap>();

            // Apply preprocessing methods
            preprocessedImages["Grayscale"] = preprocessor.ConvertToGrayscale(imageData.Image);
            preprocessedImages["Thresholded"] = preprocessor.ApplyThresholding(preprocessedImages["Grayscale"], 150);
            preprocessedImages["AdaptiveContrast"] = preprocessor.AdjustContrastDynamically(imageData.Image);

            // Auto-detect and correct skew (deskewing)
            float detectedAngle = preprocessor.DetectSkewAngle(imageData.Image);
            preprocessedImages["AutoRotated"] = preprocessor.RotateImage(imageData.Image, detectedAngle);
            Console.WriteLine($"Detected skew angle: {detectedAngle}° - Applied correction.");

            // Save processed images and extract text for each method
            foreach (var entry in preprocessedImages)
            {
                string imagePath = Path.Combine(outputFolderPath, $"{baseFileName}_{entry.Key}.jpg");
                entry.Value.Save(imagePath);
                Console.WriteLine($"Saved {entry.Key} image at: {imagePath}");

                SaveExtractedText(imagePath, $"{baseFileName}_{entry.Key}.txt", outputFolderPath);
            }
        }

        Console.WriteLine("OCR extraction completed for all images.");
    }

    /// <summary>
    /// Extracts text from an image and saves it to a separate file.
    /// </summary>
    private static void SaveExtractedText(string imagePath, string textFileName, string outputFolderPath)
    {
        Console.WriteLine($"Extracting text from {Path.GetFileName(imagePath)}...");
        (string extractedText, float confidence) = OcrProcessor.ExtractTextWithConfidence(imagePath);

        // Log OCR confidence levels
        if (string.IsNullOrWhiteSpace(extractedText))
        {
            Console.WriteLine($"⚠️ OCR failed for {imagePath} (Confidence: {confidence}%)");
            extractedText = $"OCR extraction failed. Confidence: {confidence}%. Please check image quality.";
        }
        else
        {
            Console.WriteLine($"✅ OCR Success for {imagePath} (Confidence: {confidence}%)");
        }

        // Save extracted text
        string textFilePath = Path.Combine(outputFolderPath, textFileName);
        File.WriteAllText(textFilePath, extractedText);
        Console.WriteLine($"Extracted text saved to: {textFilePath}");
    }
}
