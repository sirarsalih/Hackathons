using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace SignalRServer.Controllers
{
    public class SendController : Controller
    {
        // GET: Send
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ContentResult TakePicture()
        {
            // Create the topic if it does not exist already.
            string connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];

            var namespaceManager =
                NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.TopicExists("PhotoTopic"))
            {
                namespaceManager.CreateTopic("PhotoTopic");
            }

            // ----------------------------------------------------------------------------------------

            if (!namespaceManager.SubscriptionExists("PhotoTopic", "AllMessages"))
            {
                namespaceManager.CreateSubscription("PhotoTopic", "AllMessages");
            }

            if (!namespaceManager.SubscriptionExists("PhotoTopic", "PictureTaken"))
            {
                namespaceManager.CreateSubscription("PhotoTopic", "PictureTaken");
            }

            // ----------------------------------------------------------------------------------------

            TopicClient client =
                TopicClient.CreateFromConnectionString(connectionString, "PhotoTopic");
            
            BrokeredMessage message = new BrokeredMessage();
            // Send message to the topic.
            client.Send(message);

            SubscriptionClient subscriptionClient =
                SubscriptionClient.CreateFromConnectionString
                    (connectionString, "PhotoTopic", "PictureTaken");

            // Configure the callback options.
            OnMessageOptions options = new OnMessageOptions();
            options.AutoComplete = false;
            options.AutoRenewTimeout = TimeSpan.FromMinutes(1);

            subscriptionClient.OnMessage((incMessage) =>
            {
                try
                {
                    incMessage.Complete();
                }
                catch (Exception ex)
                {
                    incMessage.Abandon();
                }
            }, options);

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
            MemoryStream target = new MemoryStream();
            imageFile.InputStream.CopyTo(target);
            byte[] imageBytes = target.ToArray();
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
    }
}