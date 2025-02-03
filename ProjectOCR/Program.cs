using System;
using System.IO;
using OCRProject.Services;

class Program
{
    static void Main(string[] args)
    {
        // Set the input and output folder paths
        string inputFolderPath = @"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ConsoleApp1\InputImages";
        string outputFolderPath = @"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ConsoleApp1\ProcessedImages";
        string textOutputFolderPath = @"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ConsoleApp1\ExtractedText";

        // Create the output folders if they don't exist
        Directory.CreateDirectory(outputFolderPath);
        Directory.CreateDirectory(textOutputFolderPath);

        // Load images using ImageLoader service
        var images = ImageLoader.LoadImages(inputFolderPath);

        // Create an instance of the Preprocessor (Grayscale Processing)
        var preprocessor = new Preprocessor();

        foreach (var imageData in images)
        {
            Console.WriteLine($"Processing image: {imageData.FileName}");

            // Preprocess the image (convert to grayscale)
            var grayImage = preprocessor.ConvertToGrayscale(imageData.Image);

            // Save the preprocessed image to the output folder
            string outputFilePath = Path.Combine(outputFolderPath, imageData.FileName);
            grayImage.Save(outputFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            Console.WriteLine($"Saved processed image: {outputFilePath}");

            // Extract text using OCRProcessor
            string extractedText = OcrProcessor.ExtractText(outputFilePath);
            Console.WriteLine($"Extracted Text from {imageData.FileName}:\n{extractedText}\n");

            // Save extracted text to a new text file
            string textFileName = Path.GetFileNameWithoutExtension(imageData.FileName) + ".txt";
            string textFilePath = Path.Combine(textOutputFolderPath, textFileName);
            File.WriteAllText(textFilePath, extractedText);
            Console.WriteLine($"Saved extracted text to: {textFilePath}\n");
        }
    }
}
