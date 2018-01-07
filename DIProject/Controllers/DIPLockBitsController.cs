using System;
using System.Drawing;
using System.Net;
using System.Web.Mvc;
using DIProject.Helpers;
using DIProject.Services;

namespace DIProject.Controllers
{
    public class DIPLockBitsController : Controller
    {
        private static Image _image;

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
            return ReturnBitmap(ImageProcessingWithBitLocksService.Invert((Bitmap)_image));
        }

        [HttpPost]
        public ActionResult SetGrayscale()
        {
            return ReturnBitmap(ImageProcessingWithBitLocksService.SetGrayscale((Bitmap)_image));
        }

        [HttpPost]
        public ActionResult SetBrightness(int brightness)
        {
            return ReturnBitmap(ImageProcessingWithBitLocksService.SetBrightness((Bitmap)_image, brightness));
        }

        [HttpPost]
        public ActionResult SetContrast(int contrast)
        {
            return ReturnBitmap(ImageProcessingWithBitLocksService.SetContrast((Bitmap)_image, Convert.ToDouble(contrast)));
        }

        [HttpPost]
        public ActionResult Resize(int newWidth, int newHeight)
        {
            return ReturnBitmap(ImageProcessingWithBitLocksService.Resize((Bitmap)_image, newWidth, newHeight));
        }

        [HttpPost]
        public ActionResult RotateFlip(string rotateFlip)
        {
            RotateFlipType rotateFlipType;
            Enum.TryParse(rotateFlip, true, out rotateFlipType);
            return ReturnBitmap(ImageProcessingService.RotateFlip((Bitmap)_image, rotateFlipType));
        }

        [HttpPost]
        public ActionResult EdgeDetection()
        {
            return ReturnBitmap(ImageProcessingWithBitLocksService.EdgeDetection((Bitmap) _image));
        }

        [HttpPost]
        public ActionResult GaussianBlur()
        {
            return ReturnBitmap(ImageProcessingWithBitLocksService.GaussianBlur((Bitmap) _image));
        }

        private ContentResult ReturnBitmap(Image bitmap, bool overrideCurrentImage = true)
        {
            if (overrideCurrentImage)
                _image = bitmap;
            var imageBytes = ImageHelper.ImageToByteArray(bitmap);
            var base64 = ImageHelper.ImageBytesToBase64(imageBytes);
            return Content(base64);
        }
    }
}