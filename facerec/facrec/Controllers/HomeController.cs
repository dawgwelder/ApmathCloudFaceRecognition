using System;
using System.Xml;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using FaceDetection;


namespace facrec.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [HttpGet]
        public ActionResult Index()
        {
            return View();

        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            if (upload != null)
            {
                
                string fileName = System.IO.Path.GetFileName(upload.FileName);
               
                //upload.SaveAs(Server.MapPath("~/images/" + fileName)); функция загрузки на диск
                //var bitmap = new Bitmap(Server.MapPath("~/images/" + fileName)); вроде нахер не нужно
                var CascadeFacePath = Server.MapPath("../images/haarcascade_frontalface_default.xml"); // загружаем xml - в этом месте ошибка
                var CascadeEyePath = Server.MapPath("../images/haarcascade_eye.xml"); // загружаем xml - в этом месте ошибка
                
                Bitmap bmp = new Bitmap(Image.FromStream(upload.InputStream, true, true));
                Image <Bgr, Byte> image = new Image<Bgr, Byte>(bmp); // вкладываем пришедшеее на сервер изображение в переменную
                long detectionTime;
                List<Rectangle> faces = new List<Rectangle>();
                List<Rectangle> eyes = new List<Rectangle>();

                //The cuda cascade classifier doesn't seem to be able to load "haarcascade_frontalface_default.xml" file in this release
                //disabling CUDA module for now
                bool tryUseCuda = false;

                DetectFace.Detect(
                  image.Mat, CascadeFacePath, CascadeEyePath,
                  faces, eyes,
                  tryUseCuda,
                  out detectionTime);

                foreach (Rectangle face in faces)
                    CvInvoke.Rectangle(image, face, new Bgr(Color.Red).MCvScalar, 2);
                foreach (Rectangle eye in eyes)
                    CvInvoke.Rectangle(image, eye, new Bgr(Color.Blue).MCvScalar, 2);


                var picPath = Server.MapPath("../Content/" + upload.FileName);
                image.Save(picPath);

                ViewBag.PicPath = "/Content/" + upload.FileName;
            }
            return View("index");
        }
    }
   
    
    
}