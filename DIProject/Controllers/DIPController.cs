using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web.Mvc;

namespace DIProject.Controllers
{
    public class DIPController : Controller
    {
        private static Image _image;

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload()
        {
            if (Request.Files.Count > 0)
            {
                foreach (string fil in Request.Files)
                {
                    var httpPostedFileBase = Request.Files[fil];
                    if (httpPostedFileBase != null)
                        _image = Image.FromStream(httpPostedFileBase.InputStream, true, true);
                }
                var imageBytes = ImageToByteArray(_image);
                return Content(ImageBytesToBase64(imageBytes));
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [HttpPost]
        public ActionResult Invert()
        {
            var temp = new Bitmap(_image);
            var bmap = (Bitmap)temp.Clone();
            for (var i = 0; i < bmap.Width; i++)
            {
                for (var j = 0; j < bmap.Height; j++)
                {
                    var c = bmap.GetPixel(i, j);
                    bmap.SetPixel(i, j, Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                }
            }
            return ReturnBitmap(bmap);
        }

        [HttpPost]
        public ActionResult SetGrayscale()
        {
            var temp = (Bitmap)_image;
            var bmap = (Bitmap)temp.Clone();
            for (var i = 0; i < bmap.Width; i++)
            {
                for (var j = 0; j < bmap.Height; j++)
                {
                    var c = bmap.GetPixel(i, j);
                    var gray = (byte)(.299 * c.R + .587 * c.G + .114 * c.B);
                    bmap.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                }
            }
            return ReturnBitmap(bmap);
        }

        private ContentResult ReturnBitmap(Image bitmap)
        {
            _image = bitmap;
            var imageBytes = ImageToByteArray(bitmap);
            var base64 = ImageBytesToBase64(imageBytes);
            return Content(base64);
        }

        private static byte[] ImageToByteArray(Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        private static string ImageBytesToBase64(byte[] imageBytes)
        {
            return Convert.ToBase64String(imageBytes);
        }
    }
}