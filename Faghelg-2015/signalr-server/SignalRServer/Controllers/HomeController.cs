using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignalRServer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("ImageCarousel");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        public ActionResult ImageCarousel()
        {
            ViewBag.ImageContents = GetAllImages();
            return View();
        }

        [Authorize(Users = "sirar")]
        public ActionResult ImageCarouselAdmin()
        {
            ViewBag.ImageContents = GetAllImages();
            return View();
        }

        public IEnumerable<ImageContent> GetAllImages()
        {
            IEnumerable<ImageContent> imageContents;
            using (var dbContext = new iteraphotobooth_dbEntities())
            {
                imageContents = dbContext.GetAll();
            }
            return imageContents.Reverse();
        }
    }
}