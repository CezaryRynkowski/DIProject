using System;
using System.Drawing;
using System.Net;
using System.Web.Mvc;
using DIProject.Helpers;

namespace DIProject.Controllers
{
    public class DIPLockBitsController : Controller
    {
        private static Image _image;
        private ImageProcessingWithBitLocksService _imageProcessingWithBitLocksService = new ImageProcessingWithBitLocksService();

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.type = "lockbits";
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
                var imageBytes = ImageHelper.ImageToByteArray(_image);
                return Content(ImageHelper.ImageBytesToBase64(imageBytes));
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        [HttpPost]
        public ActionResult Invert()
        {
            return ReturnBitmap(ImageProcessingWithBitLocksService.Invert((Bitmap)_image), false);
        }

        [HttpPost]
        public ActionResult SetGrayscale()
        {
            return ReturnBitmap(ImageProcessingWithBitLocksService.SetGrayscale((Bitmap)_image), false);
        }

        [HttpPost]
        public ActionResult SetBrightness(int brightness)
        {
            return ReturnBitmap(ImageProcessingWithBitLocksService.SetBrightness((Bitmap)_image, brightness), false);
        }

        [HttpPost]
        public ActionResult SetGamma(int brightness)
        {
            return ReturnBitmap(ImageProcessingWithBitLocksService.SetGamma((Bitmap)_image, Convert.ToDouble(brightness)), false);
        }

        private ContentResult ReturnBitmap(Image bitmap, bool overrideCurrentImage)
        {
            if (overrideCurrentImage)
                _image = bitmap;
            var imageBytes = ImageHelper.ImageToByteArray(bitmap);
            var base64 = ImageHelper.ImageBytesToBase64(imageBytes);
            return Content(base64);
        }
    }
}