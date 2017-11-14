using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DIProject.Controllers
{
    internal class ImageProcessingWithBitLocksService
    {
        public static Bitmap Invert(Bitmap image)
        {
            int w = image.Width;
            int h = image.Height;
            BitmapData srcData = image.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int bytes = srcData.Stride * srcData.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];
            Marshal.Copy(srcData.Scan0, buffer, 0, bytes);
            image.UnlockBits(srcData);
            int current = 0;
            int cChannels = 3;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    current = y * srcData.Stride + x * 4;
                    for (int c = 0; c < cChannels; c++)
                    {
                        result[current + c] = (byte)(255 - buffer[current + c]);
                    }
                    result[current + 3] = 255;
                }
            }
            Bitmap resImg = new Bitmap(w, h);
            BitmapData resData = resImg.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, resData.Scan0, bytes);
            resImg.UnlockBits(resData);
            return resImg;
        }

        public static Image SetGrayscale(Bitmap image)
        {
            //Lock bitmap's bits to system memory
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            BitmapData bmpData = image.LockBits(rect, ImageLockMode.ReadWrite, image.PixelFormat);

            //Scan for the first line
            IntPtr ptr = bmpData.Scan0;

            //Declare an array in which your RGB values will be stored
            int bytes = Math.Abs(bmpData.Stride) * image.Height;
            byte[] rgbValues = new byte[bytes];

            //Copy RGB values in that array
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            for (int i = 0; i < rgbValues.Length; i += 3)
            {
                //Set RGB values in a Array where all RGB values are stored
                byte gray = (byte)(rgbValues[i] * .21 + rgbValues[i + 1] * .71 + rgbValues[i + 2] * .071);
                rgbValues[i] = rgbValues[i + 1] = rgbValues[i + 2] = gray;
            }

            //Copy changed RGB values back to bitmap
            Marshal.Copy(rgbValues, 0, ptr, bytes);

            //Unlock the bits
            image.UnlockBits(bmpData);
            return image;
        }

        public static Image Resize(Bitmap image, int newWidth, int newHeight)
        {
            var newImage = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage((Image)newImage);
            g.InterpolationMode = InterpolationMode.High;
            g.DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }

        public static Image SetContrast(Bitmap image, double contrast)
        {
            BitmapData sourceData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            image.UnlockBits(sourceData);
            double contrastLevel = Math.Pow((100.0 + contrast) / 100.0, 2);

            double blue = 0;
            double green = 0;
            double red = 0;

            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                blue = ((((pixelBuffer[k] / 255.0) - 0.5) * contrastLevel) + 0.5) * 255.0;
                green = ((((pixelBuffer[k + 1] / 255.0) - 0.5) * contrastLevel) + 0.5) * 255.0;
                red = ((((pixelBuffer[k + 2] / 255.0) - 0.5) * contrastLevel) + 0.5) * 255.0;

                if (blue > 255)
                    blue = 255;
                else if (blue < 0)
                    blue = 0;

                if (green > 255)
                    green = 255;
                else if (green < 0)
                    green = 0;

                if (red > 255)
                    red = 255;
                else if (red < 0)
                    red = 0;

                pixelBuffer[k] = (byte)blue;
                pixelBuffer[k + 1] = (byte)green;
                pixelBuffer[k + 2] = (byte)red;
            }
            Bitmap resultBitmap = new Bitmap(image.Width, image.Height);
            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(pixelBuffer, 0, resultData.Scan0, pixelBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        public static Image SetBrightness(Bitmap image, int brightness)
        {
            var bright = (float)brightness;
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            System.Drawing.Imaging.BitmapData bmpData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, image.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * image.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            float correctionFactortemp = bright;
            if (bright < 0)
            {
                correctionFactortemp = 1 + bright;
            }
            for (int counter = 1; counter < rgbValues.Length; counter++)
            {
                float color = (float)rgbValues[counter];
                if (bright < 0)
                {
                    color *= (int)correctionFactortemp;
                }
                else
                {
                    color = (255 - color) * bright + color;
                }
                rgbValues[counter] = (byte)color;
            }
            Marshal.Copy(rgbValues, 0, ptr, bytes);
            image.UnlockBits(bmpData);
            return image;
        }

        public static Image SetGamma(Bitmap image, double brightness, double c = 1d)
        {
            brightness = brightness / 100.0;
            int width = image.Width;
            int height = image.Height;
            BitmapData srcData = image.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int bytes = srcData.Stride * srcData.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];
            Marshal.Copy(srcData.Scan0, buffer, 0, bytes);
            image.UnlockBits(srcData);
            int current = 0;
            int cChannels = 3;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    current = y * srcData.Stride + x * 4;
                    for (int i = 0; i < cChannels; i++)
                    {
                        double range = (double)buffer[current + i] / 255;
                        double correction = c * Math.Pow(range, brightness);
                        result[current + i] = (byte)(correction * 255);
                    }
                    result[current + 3] = 255;
                }
            }
            Bitmap resImg = new Bitmap(width, height);
            BitmapData resData = resImg.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, resData.Scan0, bytes);
            resImg.UnlockBits(resData);
            return resImg;
        }
    }
}