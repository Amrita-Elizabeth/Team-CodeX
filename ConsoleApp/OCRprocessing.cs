using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Tesseract;

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
                    using (var pix = PixConverter.ToPix(image))
                    {
                        using (var page = engine.Process(pix))
                        {
                            return page.GetText();
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

