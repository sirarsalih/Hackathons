using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Mvc;

namespace Itera.Fagdag.November.Controllers
{
    public class ResourceController : Controller
    {
        [HttpGet]
        public ActionResult GetFile(string imageName)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceNames  = assembly.GetManifestResourceNames();
                var imageFullName = resourceNames.FirstOrDefault(x => x.Substring(36).EndsWith(imageName));
                if (!string.IsNullOrEmpty(imageFullName))
                {
                    using (var stream = assembly.GetManifestResourceStream(imageFullName))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            return new FileContentResult(memoryStream.ToArray(), "image/JPEG");
                        }
                    }
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }
    }
}