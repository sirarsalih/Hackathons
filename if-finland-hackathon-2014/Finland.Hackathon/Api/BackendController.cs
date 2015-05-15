using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Finland.Hackathon.ClaimsServiceReference;

namespace Finland.Hackathon.Api
{
    public class BackendController : ApiController
    {
        public JsonResult Get()
        {
            var jsonResult = new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    claims = new[]
                    {
                        new {
							caption = "Caution, be attentive", 
							text = "This is a spot that at this time of year and time has a lot of collisions. Be extra attentive for the next 500m.",
							triggerStop = true,
							latitude = "60.29449", 
							longitude = "24.95972"
						},
                        new {
							caption = "Attention", 
							text ="The area you are going to there is reported more than the usual amount of theft. Do you want us to find a safer parking spot with a less than 10 min walk to the destination?", 
							triggerStop = false,
							latitude = "60.178180000000005", 
							longitude = "24.950070000000004"
						}
                    }
                }
            };
            return jsonResult;
        }

        public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
        {
            var jsonString = await request.Content.ReadAsStringAsync();
            return new HttpResponseMessage(HttpStatusCode.Created);
        }
    }
}
