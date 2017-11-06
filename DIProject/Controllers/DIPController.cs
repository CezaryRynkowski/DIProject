using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web.Mvc;
using DIProject.Services;

namespace DIProject.Controllers
{
    public class DIPController : Controller
    {
        private static Image _image;
        private ImageProcessingService _imageProcessing;

        public DIPController(ImageProcessingService imageProcessing)
        {
            _imageProcessing = imageProcessing;
        }

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
            return ReturnBitmap(ImageProcessingService.Invert((Bitmap)_image), false);
        }

        [HttpPost]
        public ActionResult SetGrayscale()
        {
            return ReturnBitmap(ImageProcessingService.SetGrayscale((Bitmap)_image), false);
        }

        [HttpPost]
        public ActionResult SetBrightness(int brightness)
        {
            return ReturnBitmap(ImageProcessingService.SetBrightness((Bitmap)_image, brightness), false);
        }

        private ContentResult ReturnBitmap(Image bitmap, bool overrideCurrentImage)
        {
            if (overrideCurrentImage)
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