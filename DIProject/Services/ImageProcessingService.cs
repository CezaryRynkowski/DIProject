using System;
using System.Drawing;
using DIProject.Models;

namespace DIProject.Services
{
    public static class ImageProcessingService
    {
        public static Bitmap Invert(Bitmap image)
        {
            var tempImage = image;
            for (var i = 0; i < tempImage.Width; i++)
            {
                for (var j = 0; j < tempImage.Height; j++)
                {
                    var c = tempImage.GetPixel(i, j);
                    tempImage.SetPixel(i, j, Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                }
            }
            return tempImage;
        }

        public static Bitmap SetGrayscale(Bitmap image)
        {
            var tempImage = image;
            for (var i = 0; i < tempImage.Width; i++)
            {
                for (var j = 0; j < tempImage.Height; j++)
                {
                    var c = tempImage.GetPixel(i, j);
                    var gray = (byte)(.299 * c.R + .587 * c.G + .114 * c.B);
                    tempImage.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                }
            }
            return tempImage;
        }

        public static Bitmap SetBrightness(Bitmap image, int brightness)
        {
            var tempImage = image;
            if (brightness < -255) brightness = -255;
            if (brightness > 255) brightness = 255;
            for (var i = 0; i < tempImage.Width; i++)
            {
                for (var j = 0; j < tempImage.Height; j++)
                {
                    var c = tempImage.GetPixel(i, j);
                    var cR = c.R + brightness;
                    var cG = c.G + brightness;
                    var cB = c.B + brightness;

                    if (cR < 0) cR = 1;
                    if (cR > 255) cR = 255;

                    if (cG < 0) cG = 1;
                    if (cG > 255) cG = 255;

                    if (cB < 0) cB = 1;
                    if (cB > 255) cB = 255;

                    tempImage.SetPixel(i, j, Color.FromArgb((byte)cR, (byte)cG, (byte)cB));
                }
            }
            return tempImage;
        }

        public static Bitmap SetColorFilter(Bitmap image, ColorFilterTypes colorFilterType)
        {
            var tempImage = image;
            for (var i = 0; i < tempImage.Width; i++)
            {
                for (var j = 0; j < tempImage.Height; j++)
                {
                    var c = tempImage.GetPixel(i, j);
                    var nPixelR = 0;
                    var nPixelG = 0;
                    var nPixelB = 0;
                    switch (colorFilterType)
                    {
                        case ColorFilterTypes.Red:
                            nPixelR = c.R;
                            nPixelG = c.G - 255;
                            nPixelB = c.B - 255;
                            break;
                        case ColorFilterTypes.Green:
                            nPixelR = c.R - 255;
                            nPixelG = c.G;
                            nPixelB = c.B - 255;
                            break;
                        case ColorFilterTypes.Blue:
                            nPixelR = c.R - 255;
                            nPixelG = c.G - 255;
                            nPixelB = c.B;
                            break;
                    }

                    nPixelR = Math.Max(nPixelR, 0);
                    nPixelR = Math.Min(255, nPixelR);

                    nPixelG = Math.Max(nPixelG, 0);
                    nPixelG = Math.Min(255, nPixelG);

                    nPixelB = Math.Max(nPixelB, 0);
                    nPixelB = Math.Min(255, nPixelB);

                    tempImage.SetPixel(i, j, Color.FromArgb((byte)nPixelR, (byte)nPixelG, (byte)nPixelB));
                }
            }
            return tempImage;
        }

        public static Bitmap SetContrast(Bitmap image, double contrast)
        {
            var tempImage = image;
            if (contrast < -100) contrast = -100;
            if (contrast > 100) contrast = 100;
            contrast = (100.0 + contrast) / 100.0;
            contrast *= contrast;
            for (var i = 0; i < tempImage.Width; i++)
            {
                for (var j = 0; j < tempImage.Height; j++)
                {
                    var c = tempImage.GetPixel(i, j);
                    var pR = c.R / 255.0;
                    pR -= 0.5;
                    pR *= contrast;
                    pR += 0.5;
                    pR *= 255;
                    if (pR < 0) pR = 0;
                    if (pR > 255) pR = 255;

                    var pG = c.G / 255.0;
                    pG -= 0.5;
                    pG *= contrast;
                    pG += 0.5;
                    pG *= 255;
                    if (pG < 0) pG = 0;
                    if (pG > 255) pG = 255;

                    var pB = c.B / 255.0;
                    pB -= 0.5;
                    pB *= contrast;
                    pB += 0.5;
                    pB *= 255;
                    if (pB < 0) pB = 0;
                    if (pB > 255) pB = 255;

                    tempImage.SetPixel(i, j, Color.FromArgb((byte)pR, (byte)pG, (byte)pB));
                }
            }
            return tempImage;
        }

        public static Bitmap Resize(Bitmap image, int newWidth, int newHeight)
        {
            if (newWidth != 0 && newHeight != 0)
            {
                var temp = image;
                var bmap = new Bitmap(newWidth, newHeight, temp.PixelFormat);

                var nWidthFactor = (double)temp.Width / newWidth;
                var nHeightFactor = (double)temp.Height / newHeight;

                double fx, fy, nx, ny;
                int cx, cy, fr_x, fr_y;
                byte nRed, nGreen, nBlue;
                byte bp1, bp2;

                var color1 = new Color();
                var color2 = new Color();
                var color3 = new Color();
                var color4 = new Color();

                for (int x = 0; x < bmap.Width; ++x)
                {
                    for (int y = 0; y < bmap.Height; ++y)
                    {

                        fr_x = (int)Math.Floor(x * nWidthFactor);
                        fr_y = (int)Math.Floor(y * nHeightFactor);
                        cx = fr_x + 1;
                        if (cx >= temp.Width) cx = fr_x;
                        cy = fr_y + 1;
                        if (cy >= temp.Height) cy = fr_y;
                        fx = x * nWidthFactor - fr_x;
                        fy = y * nHeightFactor - fr_y;
                        nx = 1.0 - fx;
                        ny = 1.0 - fy;

                        color1 = temp.GetPixel(fr_x, fr_y);
                        color2 = temp.GetPixel(cx, fr_y);
                        color3 = temp.GetPixel(fr_x, cy);
                        color4 = temp.GetPixel(cx, cy);

                        // Blue
                        bp1 = (byte)(nx * color1.B + fx * color2.B);
                        bp2 = (byte)(nx * color3.B + fx * color4.B);
                        nBlue = (byte)(ny * bp1 + fy * bp2);

                        // Green
                        bp1 = (byte)(nx * color1.G + fx * color2.G);
                        bp2 = (byte)(nx * color3.G + fx * color4.G);
                        nGreen = (byte)(ny * bp1 + fy * bp2);

                        // Red
                        bp1 = (byte)(nx * color1.R + fx * color2.R);
                        bp2 = (byte)(nx * color3.R + fx * color4.R);
                        nRed = (byte)(ny * bp1 + fy * bp2);

                        bmap.SetPixel(x, y, Color.FromArgb(255, nRed, nGreen, nBlue));
                    }
                }
                return bmap;
            }
            return null;
        }

        public static Bitmap RotateFlip(Bitmap image, RotateFlipType rotateFlipType)
        {
            image.RotateFlip(rotateFlipType);
            return image;
        }
    }
}