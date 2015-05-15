using System.Web.Mvc;

namespace Finland.Hackathon.Controllers
{
    public class ClaimServiceController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Finland Hackathon";

            return View();
        }
    }
}
