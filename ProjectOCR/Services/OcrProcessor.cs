//using System;
//using Tesseract;

//namespace OCRProject.Services
//{
//    public static class OcrProcessor
//    {
//        public static string ExtractText(string imagePath)
//        {
//            using (var engine = new TesseractEngine(@"C:\Users\ASUS\OneDrive\Desktop\Team_CodeX\Team_CodeX_2024-25\Team-CodeX\ConsoleApp1\tessdata", "eng", EngineMode.Default))
//            {
//                using (var img = Pix.LoadFromFile(imagePath))
//                {
//                    using (var page = engine.Process(img))
//                    {
//                        return page.GetText();
//                    }
//                }
//            }
//        }
//    }
//}

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Tesseract;

// Alias to resolve ambiguity
using ImagingImageFormat = System.Drawing.Imaging.ImageFormat;

namespace OCRProject.Services
{
    public class OcrProcessor
    {
        private readonly string _tessDataPath;

        public OcrProcessor(string tessDataPath)
        {
            _tessDataPath = tessDataPath;
        }

        public string ExtractText(Bitmap image)
        {
            try
            {
                using (var engine = new TesseractEngine(_tessDataPath, "eng", EngineMode.Default))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        image.Save(memoryStream, ImagingImageFormat.Png);
                        memoryStream.Position = 0;

                        using (var pix = Pix.LoadFromMemory(memoryStream.ToArray()))
                        {
                            using (var page = engine.Process(pix))
                            {
                                return page.GetText();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ExtractText: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
