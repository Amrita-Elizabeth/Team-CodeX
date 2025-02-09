using System;
using System.IO;
using System.Drawing;
using System.Linq;
using OCRProject.Services;

class Program
{
    static void Main(string[] args)
    {
        string inputFolderPath = @"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ProjectOCR\InputImages";
        string outputFolderPath = Path.GetFullPath(@"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ProjectOCR\ProcessedImages");

        Directory.CreateDirectory(outputFolderPath);
        Console.WriteLine($"Output folder path: {outputFolderPath}");

        var images = ImageLoader.LoadImages(inputFolderPath)
            .Where(img => Path.GetFileNameWithoutExtension(img.FileName).Equals("sample1", StringComparison.OrdinalIgnoreCase) ||
                          Path.GetFileNameWithoutExtension(img.FileName).Equals("sample2", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Console.WriteLine($"Loaded {images.Count} images for processing.");
        foreach (var img in images)
        {
            Console.WriteLine($"Image loaded: {img.FileName}");
        }

        var preprocessor = new Preprocessor();

        foreach (var imageData in images)
        {
            Console.WriteLine($"Processing image: {imageData.FileName}");
            string baseFileName = Path.GetFileNameWithoutExtension(imageData.FileName);

            // 1. Grayscale Conversion
            var grayImage = preprocessor.ConvertToGrayscale(imageData.Image);
            string grayImagePath = Path.Combine(outputFolderPath, $"{baseFileName}_grayscale.jpg");
            grayImage.Save(grayImagePath);
            Console.WriteLine($"Saved grayscale image at: {grayImagePath}");
            SaveExtractedText(grayImagePath, $"{baseFileName}_grayscale.txt", outputFolderPath);

            // 2. Thresholding
            var thresholdedImage = preprocessor.ApplyThresholding(grayImage, 150);  // Try different threshold values
            string thresholdImagePath = Path.Combine(outputFolderPath, $"{baseFileName}_threshold.jpg");
            thresholdedImage.Save(thresholdImagePath);
            Console.WriteLine($"Saved thresholded image at: {thresholdImagePath}");
            SaveExtractedText(thresholdImagePath, $"{baseFileName}_threshold.txt", outputFolderPath);

            // 3. Contrast Adjustment
            var contrastImage = preprocessor.AdjustContrast(imageData.Image, 50);
            string contrastImagePath = Path.Combine(outputFolderPath, $"{baseFileName}_contrast.jpg");
            contrastImage.Save(contrastImagePath);
            Console.WriteLine($"Saved contrast adjusted image at: {contrastImagePath}");
            SaveExtractedText(contrastImagePath, $"{baseFileName}_contrast.txt", outputFolderPath);

            // 4. Rotation (Deskewing)
            var rotatedImage = preprocessor.RotateImage(imageData.Image, 15);
            string rotatedImagePath = Path.Combine(outputFolderPath, $"{baseFileName}_rotated.jpg");
            rotatedImage.Save(rotatedImagePath);
            Console.WriteLine($"Saved rotated image at: {rotatedImagePath}");
            SaveExtractedText(rotatedImagePath, $"{baseFileName}_rotated.txt", outputFolderPath);
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

        // If text is empty, log a warning
        if (string.IsNullOrWhiteSpace(extractedText))
        {
            Console.WriteLine($"⚠️ OCR failed for {imagePath} (Confidence: {confidence}%)");
            extractedText = $"OCR extraction failed. Confidence: {confidence}%. Please check image quality.";
        }
        else
        {
            Console.WriteLine($"✅ OCR Success for {imagePath} (Confidence: {confidence}%)");
        }

        string textFilePath = Path.Combine(outputFolderPath, textFileName);
        File.WriteAllText(textFilePath, extractedText);
        Console.WriteLine($"Extracted text saved to: {textFilePath}");
    }
}
