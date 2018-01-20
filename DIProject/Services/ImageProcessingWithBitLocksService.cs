using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DIProject.Controllers
{
    public static class ImageProcessingWithBitLocksService
    {
        private static double[,] xSobel => new double[,]
        {
            { -1, 0, 1 },
            { -2, 0, 2 },
            { -1, 0, 1 }
        };

        //Sobel operator kernel for vertical pixel changes
        private static double[,] ySobel => new double[,]
        {
            {  1,  2,  1 },
            {  0,  0,  0 },
            { -1, -2, -1 }
        };

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
            var g = Graphics.FromImage(newImage);
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

        public static Image EdgeDetection(Bitmap image)
        {
            var grayscale = true;
            var xkernel = xSobel;
            var ykernel = ySobel;
            var width = image.Width;
            var height = image.Height;

            var srcData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bytes = srcData.Stride * srcData.Height;
            var pixelBuffer = new byte[bytes];
            var resultBuffer = new byte[bytes];

            var srcScan0 = srcData.Scan0;

            Marshal.Copy(srcScan0, pixelBuffer, 0, bytes);

            image.UnlockBits(srcData);

            if (grayscale == true)
            {
                float rgb = 0;
                for (int i = 0; i < pixelBuffer.Length; i += 4)
                {
                    rgb = pixelBuffer[i] * .21f;
                    rgb += pixelBuffer[i + 1] * .71f;
                    rgb += pixelBuffer[i + 2] * .071f;
                    pixelBuffer[i] = (byte)rgb;
                    pixelBuffer[i + 1] = pixelBuffer[i];
                    pixelBuffer[i + 2] = pixelBuffer[i];
                    pixelBuffer[i + 3] = 255;
                }
            }

            var xr = 0.0;
            var xg = 0.0;
            var xb = 0.0;
            var yr = 0.0;
            var yg = 0.0;
            var yb = 0.0;
            var rt = 0.0;
            var gt = 0.0;
            var bt = 0.0;

            var filterOffset = 1;
            var calcOffset = 0;
            var byteOffset = 0;

            for (var OffsetY = filterOffset; OffsetY < height - filterOffset; OffsetY++)
            {
                for (var OffsetX = filterOffset; OffsetX < width - filterOffset; OffsetX++)
                {
                    xr = xg = xb = yr = yg = yb = 0;
                    rt = gt = bt = 0.0;
                    byteOffset = OffsetY * srcData.Stride + OffsetX * 4;

                    for (var filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (var filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset + filterX * 4 + filterY * srcData.Stride;
                            xb += pixelBuffer[calcOffset] * xkernel[filterY + filterOffset, filterX + filterOffset];
                            xg += pixelBuffer[calcOffset + 1] * xkernel[filterY + filterOffset, filterX + filterOffset];
                            xr += pixelBuffer[calcOffset + 2] * xkernel[filterY + filterOffset, filterX + filterOffset];
                            yb += pixelBuffer[calcOffset] * ykernel[filterY + filterOffset, filterX + filterOffset];
                            yg += pixelBuffer[calcOffset + 1] * ykernel[filterY + filterOffset, filterX + filterOffset];
                            yr += pixelBuffer[calcOffset + 2] * ykernel[filterY + filterOffset, filterX + filterOffset];
                        }
                    }
                    bt = Math.Sqrt((xb * xb) + (yb * yb));
                    gt = Math.Sqrt((xg * xg) + (yg * yg));
                    rt = Math.Sqrt((xr * xr) + (yr * yr));

                    if (bt > 255) bt = 255;
                    else if (bt < 0) bt = 0;
                    if (gt > 255) gt = 255;
                    else if (gt < 0) gt = 0;
                    if (rt > 255) rt = 255;
                    else if (rt < 0) rt = 0;

                    resultBuffer[byteOffset] = (byte)(bt);
                    resultBuffer[byteOffset + 1] = (byte)(gt);
                    resultBuffer[byteOffset + 2] = (byte)(rt);
                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            Bitmap resultImage = new Bitmap(width, height);
            BitmapData resultData = resultImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultImage.UnlockBits(resultData);

            return resultImage;
        }

        public static double[,] GaussianBlur(int lenght, double weight)
        {
            var kernel = new double[lenght, lenght];
            double kernelSum = 0;
            var foff = (lenght - 1) / 2;
            var distance = 0.0;
            var constant = 1d / (2 * Math.PI * weight * weight);
            for (var y = -foff; y <= foff; y++)
            {
                for (var x = -foff; x <= foff; x++)
                {
                    distance = (y * y + x * x) / (2 * weight * weight);
                    kernel[y + foff, x + foff] = constant * Math.Exp(-distance);
                    kernelSum += kernel[y + foff, x + foff];
                }
            }
            for (var y = 0; y < lenght; y++)
            {
                for (var x = 0; x < lenght; x++)
                    kernel[y, x] = kernel[y, x] * 1d / kernelSum;
            }
            return kernel;
        }

        public static Image GaussianBlur(Bitmap image)
        {
            var width = image.Width;
            var height = image.Height;
            var kernel = GaussianBlur(5, 1.1);
            var srcData = image.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bytes = srcData.Stride * srcData.Height;
            var buffer = new byte[bytes];
            var result = new byte[bytes];
            Marshal.Copy(srcData.Scan0, buffer, 0, bytes);
            image.UnlockBits(srcData);
            var colorChannels = 3;
            var rgb = new double[colorChannels];
            var foff = (kernel.GetLength(0) - 1) / 2;
            var kcenter = 0;
            var kpixel = 0;
            for (var y = foff; y < height - foff; y++)
            {
                for (var x = foff; x < width - foff; x++)
                {
                    for (var c = 0; c < colorChannels; c++)
                    {
                        rgb[c] = 0.0;
                    }
                    kcenter = y * srcData.Stride + x * 4;
                    for (var fy = -foff; fy <= foff; fy++)
                    {
                        for (var fx = -foff; fx <= foff; fx++)
                        {
                            kpixel = kcenter + fy * srcData.Stride + fx * 4;
                            for (var c = 0; c < colorChannels; c++)
                                rgb[c] += buffer[kpixel + c] * kernel[fy + foff, fx + foff];
                        }
                    }
                    for (var c = 0; c < colorChannels; c++)
                    {
                        if (rgb[c] > 255) rgb[c] = 255;
                        else if (rgb[c] < 0) rgb[c] = 0;
                    }
                    for (var c = 0; c < colorChannels; c++)
                        result[kcenter + c] = (byte)rgb[c];
                    result[kcenter + 3] = 255;
                }
            }
            var resultImage = new Bitmap(width, height);
            var resultData = resultImage.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, resultData.Scan0, bytes);
            resultImage.UnlockBits(resultData);
            return resultImage;
        }

        public static Bitmap HorizontalFlip(Bitmap img)
        {
            int w = img.Width;
            int h = img.Height;
            BitmapData sd = img.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int bytes = sd.Stride * sd.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];
            Marshal.Copy(sd.Scan0, buffer, 0, bytes);

            int current, flipped = 0;
            for (int y = 0; y < h; y++)
            {
                for (int x = 4; x < w; x++)
                {
                    current = y * sd.Stride + x * 4;
                    flipped = y * sd.Stride + (w - x) * 4;
                    for (int i = 0; i < 3; i++)
                    {
                        result[flipped + i] = buffer[current + i];
                    }
                    result[flipped + 3] = 255;
                }
            }
            Bitmap resimg = new Bitmap(w, h);
            BitmapData rd = resimg.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, rd.Scan0, bytes);
            img.UnlockBits(sd);
            resimg.UnlockBits(rd);
            return resimg;
        }
        public static Bitmap HistEq(this Bitmap img)
        {
            int w = img.Width;
            int h = img.Height;
            BitmapData sd = img.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int bytes = sd.Stride * sd.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];
            Marshal.Copy(sd.Scan0, buffer, 0, bytes);
            img.UnlockBits(sd);
            int current = 0;
            double[] pn = new double[256];
            for (int p = 0; p < bytes; p += 4)
            {
                pn[buffer[p]]++;
            }
            for (int prob = 0; prob < pn.Length; prob++)
            {
                pn[prob] /= (w * h);
            }
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    current = y * sd.Stride + x * 4;
                    double sum = 0;
                    for (int i = 0; i < buffer[current]; i++)
                    {
                        sum += pn[i];
                    }
                    for (int c = 0; c < 3; c++)
                    {
                        result[current + c] = (byte)Math.Floor(255 * sum);
                    }
                    result[current + 3] = 255;
                }
            }
            Bitmap res = new Bitmap(w, h);
            BitmapData rd = res.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, rd.Scan0, bytes);
            res.UnlockBits(rd);
            return res;
        }

    }
}