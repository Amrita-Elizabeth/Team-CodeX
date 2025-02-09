using System;
using Tesseract;

namespace OCRProject.Services
{
    public static class OcrProcessor
    {
        public static (string, float) ExtractTextWithConfidence(string imagePath)
        {
            try
            {
                using (var engine = new TesseractEngine(@"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ProjectOCR\tessdata", "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(imagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            float confidence = page.GetMeanConfidence() * 100;
                            string text = page.GetText();
                            return (text, confidence);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OCR Error for {imagePath}: {ex.Message}");
                return (string.Empty, 0);
            }
        }
    }
}
