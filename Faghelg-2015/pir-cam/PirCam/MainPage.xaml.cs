using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
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
        //Webcam variables
        private MediaCapture _mediaCap = new MediaCapture();

        public MainPage()
        {
            InitializeComponent();

            InitilizeWebcam();

            string connectionString =
                "Endpoint=sb://iteraphotobooth.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=wQYYVYvzgZmeWxTF+Z3nWLzBQ7j0YrY8RK47QEbsDH4=";

            SubscriptionClient subscriptionClient =
                SubscriptionClient.CreateFromConnectionString
                    (connectionString, "PhotoTopic", "AllMessages");

            // Configure the callback options.
            OnMessageOptions options = new OnMessageOptions {AutoComplete = false};

            TopicClient client =
                TopicClient.CreateFromConnectionString(connectionString, "PhotoTopic");

            subscriptionClient.OnMessage((message) =>
            {
                try
                {
                    TakePicture();
                    client.Send(message);
                }
                catch (Exception ex)
                {
                    new MessageDialog(ex.Message).ShowAsync();
                }
            }, options);
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
                //var bitmap = new BitmapImage();
                //bitmap.SetSource(photoStream);

                BitmapImage bitmap = null;
                this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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

                this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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

        private async void MediaCapture_Failed(MediaCapture currentCaptureObject, MediaCaptureFailedEventArgs currentFailure)
        {
            await new MessageDialog(currentFailure.Message).ShowAsync();
        }

        private async void InitilizeWebcam()
        {
            try
            {
                //initialize the WebCam via MediaCapture object
                _mediaCap = new MediaCapture();
                await _mediaCap.InitializeAsync();

                // Set callbacks for any possible failure in TakePicture() logic
                _mediaCap.Failed += MediaCapture_Failed;
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        private void Execute_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            TakePicture();
        }
    }
}
