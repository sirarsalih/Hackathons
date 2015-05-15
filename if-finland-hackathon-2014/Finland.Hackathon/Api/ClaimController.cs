using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Finland.Hackathon.ClaimsServiceReference;
using Newtonsoft.Json;

namespace Finland.Hackathon.Api
{
    public class ClaimController : ApiController
    {
        public JsonResult Get(int i)
        {
            var service = new ClaimsServiceClient();
            var locations = service.GetLocationPacksByData(i);
            var jsonResult = new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    locations = JsonConvert.SerializeObject(locations)
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
