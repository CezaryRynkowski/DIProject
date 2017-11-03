﻿using System;
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
            var tempBitmap = new Bitmap(_image);
            var resultBitmap = (Bitmap)tempBitmap.Clone();
            for (var i = 0; i < resultBitmap.Width; i++)
            {
                for (var j = 0; j < resultBitmap.Height; j++)
                {
                    var c = resultBitmap.GetPixel(i, j);
                    resultBitmap.SetPixel(i, j, Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                }
            }
            return ReturnBitmap(resultBitmap);
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