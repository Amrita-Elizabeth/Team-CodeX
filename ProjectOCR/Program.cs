//using System;
//using System.IO;
//using OCRProject.Services;

//class Program
//{
//    static void Main(string[] args)
//    {
//        // Set the input and output folder paths
//        string inputFolderPath = @"C:\Users\Molly\Documents\Project\Team-CodeX\ProjectOCR\Image_input";
//        string outputFolderPath = @"C:\Users\Molly\Documents\Project\Team-CodeX\ProjectOCR\Processed_data";
//        string textOutputFolderPath = @"C:\Users\Molly\Documents\Project\Team-CodeX\ProjectOCR\ExtractedText";

//        // Create the output folders if they don't exist
//        Directory.CreateDirectory(outputFolderPath);
//        Directory.CreateDirectory(textOutputFolderPath);

//        // Load images using ImageLoader service
//        var images = ImageLoader.LoadImages(inputFolderPath);

//        // Create an instance of the Preprocessor (Grayscale Processing)
//        var preprocessor = new Preprocessor();

//        foreach (var imageData in images)
//        {
//            Console.WriteLine($"Processing image: {imageData.FileName}");

//            // Preprocess the image (convert to grayscale)
//            var grayImage = preprocessor.ConvertToGrayscale(imageData.Image);

//            // Save the preprocessed image to the output folder
//            string outputFilePath = Path.Combine(outputFolderPath, imageData.FileName);
//            grayImage.Save(outputFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
//            Console.WriteLine($"Saved processed image: {outputFilePath}");

//            // Extract text using OCRProcessor
//            string extractedText = OcrProcessor.ExtractText(outputFilePath);
//            Console.WriteLine($"Extracted Text from {imageData.FileName}:\n{extractedText}\n");

//            // Save extracted text to a new text file
//            string textFileName = Path.GetFileNameWithoutExtension(imageData.FileName) + ".txt";
//            string textFilePath = Path.Combine(textOutputFolderPath, textFileName);
//            File.WriteAllText(textFilePath, extractedText);
//            Console.WriteLine($"Saved extracted text to: {textFilePath}\n");
//        }
//    }
//}
//using System;
//using System.IO;
//using OCRProject.Services;

//class Program
//{
//    static void Main(string[] args)
//    {
//        string inputFolderPath = @"C:\Users\Molly\Documents\Project\Team-CodeX\ProjectOCR\Image_input";
//        string outputFolderPath = @"C:\Users\Molly\Documents\Project\Team-CodeX\ProjectOCR\Processed_data";
//        string textOutputFolderPath = @"C:\Users\Molly\Documents\Project\Team-CodeX\ProjectOCR\ExtractedText";

//        Directory.CreateDirectory(outputFolderPath);
//        Directory.CreateDirectory(textOutputFolderPath);

//        var images = ImageLoader.LoadImages(inputFolderPath);
//        var preprocessor = new Preprocessor();

//        foreach (var imageData in images)
//        {
//            Console.WriteLine($"Processing image: {imageData.FileName}");

//            // Apply preprocessing methods sequentially
//            var grayImage = preprocessor.ConvertToGrayscale(imageData.Image);
//            var binaryImage = preprocessor.ApplyThresholding(grayImage);
//            var sharpenedImage = preprocessor.ApplySharpening(binaryImage);

//            // Save the final preprocessed image
//            string processedFilePath = Path.Combine(outputFolderPath, imageData.FileName);
//            sharpenedImage.Save(processedFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
//            Console.WriteLine($"Saved processed image: {processedFilePath}");

//            // Extract text using OCRProcessor
//            string extractedText = OcrProcessor.ExtractText(processedFilePath);
//            Console.WriteLine($"Extracted Text from {imageData.FileName}:\n{extractedText}\n");

//            // Save extracted text to a new text file
//            string textFileName = Path.GetFileNameWithoutExtension(imageData.FileName) + ".txt";
//            string textFilePath = Path.Combine(textOutputFolderPath, textFileName);
//            File.WriteAllText(textFilePath, extractedText);
//            Console.WriteLine($"Saved extracted text to: {textFilePath}\n");
//        }
//    }
//}
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