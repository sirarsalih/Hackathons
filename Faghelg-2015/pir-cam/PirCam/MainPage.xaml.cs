using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using ppatierno.AzureSBLite.Messaging;

// https://www.hackster.io/katpurz/motion-sensor-surveillance-camera-using-azure-cloud-storage-89550a
// http://ms-iot.github.io/content/en-US/win10/samples/WebCamSample.htm
// https://github.com/katpurz/Win10IoTMotionSurveillanceCamera
// http://raspi.tv/wp-content/uploads/2014/07/Raspberry-Pi-GPIO-pinouts.png

namespace PirCam
{
    public sealed partial class MainPage
    {
        //Cam variables
        private MediaCapture _mediaCap = new MediaCapture();
        private const string ServiceBusConnectionString =
            "Endpoint=sb://iteraphotobooth.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=wQYYVYvzgZmeWxTF+Z3nWLzBQ7j0YrY8RK47QEbsDH4=";
        private const string ServiceBusQueueName = "PhotoQueue";

        public MainPage()
        {
            InitializeComponent();

            InitilizeCam();

            InitializeServiceBusAndHandleMessage();
        }

        private void InitializeServiceBusAndHandleMessage()
        {
            var client = QueueClient.CreateFromConnectionString(ServiceBusConnectionString, ServiceBusQueueName);
            var options = new OnMessageOptions {AutoComplete = false};
            client.OnMessage((message) =>
            {
                try
                {
                    TakePicture();
                    // Remove message from queue.
                    message.Complete();
                }
                catch (Exception)
                {
                    // Indicates a problem, dispose message.
                    message.Dispose();
                }
            }, options);
        }

        private async void InitilizeCam()
        {
            try
            {
                //initialize the WebCam via MediaCapture object
                _mediaCap = new MediaCapture();
                var settings = new Windows.Media.Capture.MediaCaptureInitializationSettings();

                var cams = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

                // If more than 1 cam, then choose the front if available
                var foundFront = false;
                if (cams.Count > 1)
                {
                    foreach (var cam in cams)
                    {
                        var location = cam.EnclosureLocation;
                        if (location == null || location.Panel != Panel.Front) continue;
                        settings.VideoDeviceId = cam.Id;
                        await _mediaCap.InitializeAsync(settings);
                        foundFront = true;
                        break;
                    }
                    if (!foundFront)
                    {
                        await _mediaCap.InitializeAsync();
                    }
                }
                else
                {
                    await _mediaCap.InitializeAsync();
                }

                // Set callbacks for any possible failure in TakePicture() logic
                _mediaCap.Failed += MediaCapture_Failed;
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        private async void MediaCapture_Failed(MediaCapture currentCaptureObject, MediaCaptureFailedEventArgs currentFailure)
        {
            await new MessageDialog(currentFailure.Message).ShowAsync();
        }

        private void Execute_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            TakePicture();
        }

        private async void TakePicture()
        {
            try
            {
                //gets a reference to the file we're about to write a picture into
                var photoFile = await KnownFolders.PicturesLibrary.CreateFileAsync("#¤¤#.jpg", CreationCollisionOption.ReplaceExisting);

                //use the MediaCapture object to stream captured photo to a file
                var imageProperties = ImageEncodingProperties.CreateJpeg();
                await _mediaCap.CapturePhotoToStorageFileAsync(imageProperties, photoFile);

                //show photo onscreen
                IRandomAccessStream photoStream = await photoFile.OpenReadAsync();

                BitmapImage bitmap = null;
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                 {
                     bitmap = new BitmapImage();
                     bitmap.SetSource(photoStream);
                 });

                var v = Task.Factory.StartNew(() => File.ReadAllBytes(photoFile.Path));
                v.Wait();
                var imageBytes = v.Result;

                using (var httpClient = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true }))
                {
                    var mfdc = new MultipartFormDataContent
                    {
                        {new StreamContent(content: new MemoryStream(imageBytes)), "Image", "Image.jpeg"}
                    };

#if DEBUG
                    var result = httpClient.PostAsync("http://localhost:55199/Send/SendImage", mfdc).Result;
#else
                    var result = httpClient.PostAsync("http://iteraphotobooth.azurewebsites.net/Send/SendImage", mfdc).Result;
#endif
                    var resultContent = result.Content.ReadAsStringAsync().Result;
                }

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                 {
                     CaptureImage.Source = bitmap;
                 });

                var t = Task.Factory.StartNew(() => File.Delete(photoFile.Path));
                t.Wait();
            }
            catch (Exception ex)
            {
                new MessageDialog(ex.Message).ShowAsync();
            }
        }
    }
}
