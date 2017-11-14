using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web.Mvc;
using DIProject.Helpers;
using DIProject.Models;
using DIProject.Services;

namespace DIProject.Controllers
{
    public class DIPController : Controller
    {
        private static Image _image;
        private ImageProcessingService _imageProcessing = new ImageProcessingService();

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.type = "setpixels";
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

        [HttpPost]
        public ActionResult SetColorFilter(string colorFilter)
        {
            ColorFilterTypes colorFilterType;
            Enum.TryParse(colorFilter, true, out colorFilterType);
            return colorFilterType == ColorFilterTypes.None ? Content(ImageHelper.ImageBytesToBase64(ImageHelper.ImageToByteArray(_image))) : ReturnBitmap(ImageProcessingService.SetColorFilter((Bitmap)_image, colorFilterType), false);
        }

        [HttpPost]
        public ActionResult SetContrast(int contrast)
        {
            return ReturnBitmap(ImageProcessingService.SetContrast((Bitmap)_image, Convert.ToDouble(contrast)), false);
        }

        [HttpPost]
        public ActionResult Resize(int newWidth, int newHeight)
        {
            return ReturnBitmap(ImageProcessingService.Resize((Bitmap)_image, newWidth, newHeight), false);
        }

        [HttpPost]
        public ActionResult RotateFlip(string rotateFlip)
        {
            RotateFlipType rotateFlipType;
            Enum.TryParse(rotateFlip, true, out rotateFlipType);
            return ReturnBitmap(ImageProcessingService.RotateFlip((Bitmap)_image, rotateFlipType), false);
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