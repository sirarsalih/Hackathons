using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace SignalRServer.Controllers
{
    public class SendController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult TakePicture()
        {
            var connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            var qd = new QueueDescription("PhotoQueue")
            {
                MaxSizeInMegabytes = 1024,
                DefaultMessageTimeToLive = new TimeSpan(0, 1, 0)
            };

            if (!namespaceManager.QueueExists("PhotoQueue"))
            {
                namespaceManager.CreateQueue(qd);
            }

            var client = QueueClient.CreateFromConnectionString(connectionString, "PhotoQueue");

            client.Send(new BrokeredMessage());
            return new ContentResult();
        }

        [HttpPost]
        public ContentResult SendMessage(string messageText)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ImageHub>();
            context.Clients.All.addNewMessageToPage(messageText);

            return null;
        }

        [HttpPost]
        public HttpStatusCodeResult SendImage()
        {
            var imageFile = Request.Files[0];
            var ms = new MemoryStream();
            imageFile.InputStream.CopyTo(ms);
            var image = Image.FromStream(ms);

            DerotateIfRotatedWrongly(image);

            byte[] imageBytes;
            using (ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                imageBytes = ms.ToArray();
            }
            
            var imageContent = new ImageContent()
            {
                ImageDateTime = DateTime.Now,
                ImageData = imageBytes
            };
            using (var dbContext = new iteraphotobooth_dbEntities())
            {
                dbContext.Add(imageContent);
                dbContext.SaveChanges();
            }
            var context = GlobalHost.ConnectionManager.GetHubContext<ImageHub>();
            context.Clients.All.addNewImageToPage(imageContent.ImageId);
            return new HttpStatusCodeResult(200);
        }

        public FileResult ShowImage(int imageId)
        {
            ImageContent imageContent;
            using (var dbContext = new iteraphotobooth_dbEntities())
            {
                imageContent = dbContext.GetById(imageId);
            }

            return File(imageContent.ImageData, System.Net.Mime.MediaTypeNames.Image.Jpeg);
        }

        private static void DerotateIfRotatedWrongly(Image image)
        {
            if (image.PropertyIdList.Contains(0x0112))
            {
                int rotationValue = image.GetPropertyItem(0x0112).Value[0];
                switch (rotationValue)
                {
                    case 1: // landscape, do nothing
                        break;

                    case 8: // rotated 90 right
                        // de-rotate:
                        image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;

                    case 3: // bottoms up
                        // de-rotate:
                        image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;

                    case 6: // rotated 90 left
                        // de-rotate:
                        image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                }
            }
        }
    }
}