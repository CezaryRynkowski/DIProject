using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DIProject.Controllers
{
    public static class ImageProcessingWithBitLocksService
    {
        public static Bitmap Invert(Bitmap image)
        {
            var w = image.Width;
            var h = image.Height;

            var srcData = image.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bytes = srcData.Stride * srcData.Height;
            var buffer = new byte[bytes];
            var result = new byte[bytes];

            Marshal.Copy(srcData.Scan0, buffer, 0, bytes);
            image.UnlockBits(srcData);
            var current = 0;
            var cChannels = 3;

            for (var y = 0; y < h; y++)
            {
                for (var x = 0; x < w; x++)
                {
                    current = y * srcData.Stride + x * 4;
                    for (var c = 0; c < cChannels; c++)
                    {
                        result[current + c] = (byte)(255 - buffer[current + c]);
                    }
                    result[current + 3] = 255;
                }
            }
            var resImg = new Bitmap(w, h);
            var resData = resImg.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, resData.Scan0, bytes);
            resImg.UnlockBits(resData);

            return resImg;
        }

        public static Image SetGrayscale(Bitmap image)
        {
            var rect = new Rectangle(0, 0, image.Width, image.Height);
            var bmpData = image.LockBits(rect, ImageLockMode.ReadWrite, image.PixelFormat);
            var ptr = bmpData.Scan0;

            var bytes = Math.Abs(bmpData.Stride) * image.Height;
            var rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            for (var i = 0; i < rgbValues.Length; i += 3)
            {
                var gray = (byte)(rgbValues[i] * .299 + rgbValues[i + 1] * .587 + rgbValues[i + 2] * .114);
                rgbValues[i] = rgbValues[i + 1] = rgbValues[i + 2] = gray;
            }

            Marshal.Copy(rgbValues, 0, ptr, bytes);
            image.UnlockBits(bmpData);

            return image;
        }

        public static Image Resize(Bitmap image, int newWidth, int newHeight)
        {
            var newImage = new Bitmap(newWidth, newHeight);
            var g = Graphics.FromImage((Image)newImage);
            g.InterpolationMode = InterpolationMode.High;
            g.DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }

        public static Image SetContrast(Bitmap image, double contrast)
        {
            var sourceData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            image.UnlockBits(sourceData);
            var contrastLevel = Math.Pow((100.0 + contrast) / 100.0, 2);

            double blue = 0;
            double green = 0;
            double red = 0;

            for (var k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                blue = ((pixelBuffer[k] / 255.0 - 0.5) * contrastLevel + 0.5) * 255.0;
                green = ((pixelBuffer[k + 1] / 255.0 - 0.5) * contrastLevel + 0.5) * 255.0;
                red = ((pixelBuffer[k + 2] / 255.0 - 0.5) * contrastLevel + 0.5) * 255.0;

                if (blue > 255) blue = 255;
                else if (blue < 0) blue = 0;

                if (green > 255) green = 255;
                else if (green < 0) green = 0;

                if (red > 255) red = 255;
                else if (red < 0) red = 0;

                pixelBuffer[k] = (byte)blue;
                pixelBuffer[k + 1] = (byte)green;
                pixelBuffer[k + 2] = (byte)red;
            }
            var resultBitmap = new Bitmap(image.Width, image.Height);
            var resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        public static Image SetBrightness(Bitmap image, int brightness)
        {
            var TempBitmap = image;
            var NewBitmap = new Bitmap(TempBitmap.Width, TempBitmap.Height);
            var NewGraphics = Graphics.FromImage(NewBitmap);
            var FinalValue = brightness / 255.0f;
            float[][] FloatColorMatrix ={

                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 1, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new[] {FinalValue, FinalValue, FinalValue, 1, 1}
            };
            var NewColorMatrix = new ColorMatrix(FloatColorMatrix);
            var Attributes = new ImageAttributes();
            Attributes.SetColorMatrix(NewColorMatrix);
            NewGraphics.DrawImage(TempBitmap, new Rectangle(0, 0, TempBitmap.Width, TempBitmap.Height), 0, 0, TempBitmap.Width, TempBitmap.Height, GraphicsUnit.Pixel, Attributes);
            Attributes.Dispose();
            NewGraphics.Dispose();
            return NewBitmap;
        }
    }
}