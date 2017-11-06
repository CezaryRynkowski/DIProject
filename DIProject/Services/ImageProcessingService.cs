using System.Drawing;

namespace DIProject.Services
{
    public abstract class ImageProcessingService
    {
        public static Bitmap Invert(Bitmap image)
        {
            for (var i = 0; i < image.Width; i++)
            {
                for (var j = 0; j < image.Height; j++)
                {
                    var c = image.GetPixel(i, j);
                    image.SetPixel(i, j, Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                }
            }
            return image;
        }

        public static Bitmap SetGrayscale(Bitmap image)
        {
            for (var i = 0; i < image.Width; i++)
            {
                for (var j = 0; j < image.Height; j++)
                {
                    var c = image.GetPixel(i, j);
                    var gray = (byte)(.299 * c.R + .587 * c.G + .114 * c.B);
                    image.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                }
            }
            return image;
        }

        public static Bitmap SetBrightness(Bitmap image, int brightness)
        {
            if (brightness < -255) brightness = -255;
            if (brightness > 255) brightness = 255;
            Color c;
            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    c = image.GetPixel(i, j);
                    int cR = c.R + brightness;
                    int cG = c.G + brightness;
                    int cB = c.B + brightness;

                    if (cR < 0) cR = 1;
                    if (cR > 255) cR = 255;

                    if (cG < 0) cG = 1;
                    if (cG > 255) cG = 255;

                    if (cB < 0) cB = 1;
                    if (cB > 255) cB = 255;

                    image.SetPixel(i, j, Color.FromArgb((byte)cR, (byte)cG, (byte)cB));
                }
            }
            return image;
        }
    }
}