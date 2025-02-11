﻿using System;
using System.Drawing;
using System.Drawing.Imaging;

public class GrayscaleProcessing
{
    // Method to convert an image to grayscale
    public Bitmap ConvertToGrayscale(Bitmap originalImage)
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

        return grayImage;
    }
}
