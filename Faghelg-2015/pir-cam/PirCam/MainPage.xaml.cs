using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using ppatierno.AzureSBLite.Messaging;

// https://www.hackster.io/katpurz/motion-sensor-surveillance-camera-using-azure-cloud-storage-89550a
// http://ms-iot.github.io/content/en-US/win10/samples/WebCamSample.htm
// https://github.com/katpurz/Win10IoTMotionSurveillanceCamera
// http://raspi.tv/wp-content/uploads/2014/07/Raspberry-Pi-GPIO-pinouts.png
// http://stackoverflow.com/questions/26620523/how-do-i-take-a-photo-with-the-correct-rotation-aspect-ratio-in-windows-phone-8

namespace PirCam
{
    public sealed partial class MainPage
    {
        //Cam variables
        private MediaCapture _mediaCap = new MediaCapture();
        private const string ServiceBusConnectionString =
            "Endpoint=sb://iteraphotobooth.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=wQYYVYvzgZmeWxTF+Z3nWLzBQ7j0YrY8RK47QEbsDH4=";
        private const string ServiceBusQueueName = "PhotoQueue";
#if DEBUG
        private const string SignalRServerUrl = "http://localhost:55199/Send/SendImage";
#else
        private const string SignalRServerUrl = "http://iteraphotobooth.azurewebsites.net/Send/SendImage";
#endif

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
                var settings = new MediaCaptureInitializationSettings();

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
            var stream = new InMemoryRandomAccessStream();

            try
            {
                await _mediaCap.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

                var photoOrientation = ConvertOrientationToPhotoOrientation(SimpleOrientation.Rotated90DegreesCounterclockwise);

                var photoFile = await KnownFolders.PicturesLibrary.CreateFileAsync("Photo.jpeg", CreationCollisionOption.GenerateUniqueName);

                await ReencodeAndSavePhotoAsync(stream, photoOrientation, photoFile);
                
                var imageBytes = await ShowPhotoOnScreenThenDeleteAsync(photoFile);

                PostToServerAsync(imageBytes);
            }
            catch (Exception ex)
            {
                new MessageDialog(ex.Message).ShowAsync();
            }
        }

        private static void PostToServerAsync(byte[] imageBytes)
        {
            using (var httpClient = new HttpClient(new HttpClientHandler {UseDefaultCredentials = true}))
            {
                var mfdc = new MultipartFormDataContent
                {
                    {new StreamContent(content: new MemoryStream(imageBytes)), "Photo", "Photo.jpeg"}
                };
                var result = httpClient.PostAsync(SignalRServerUrl, mfdc).Result;
                var resultContent = result.Content.ReadAsStringAsync().Result;
            }
        }

        private async Task<byte[]> ShowPhotoOnScreenThenDeleteAsync(StorageFile photoFile)
        {
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

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { CaptureImage.Source = bitmap; });

            var t = Task.Factory.StartNew(() => File.Delete(photoFile.Path));
            t.Wait();
            return imageBytes;
        }

        private async Task ReencodeAndSavePhotoAsync(IRandomAccessStream stream, PhotoOrientation photoOrientation, StorageFile photoFile)
        {
            using (var inputStream = stream)
            {
                var decoder = await BitmapDecoder.CreateAsync(inputStream);

                using (var outputStream = await photoFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateForTranscodingAsync(outputStream, decoder);

                    var properties = new BitmapPropertySet { { "System.Photo.Orientation", new BitmapTypedValue(photoOrientation, PropertyType.UInt16) } };

                    await encoder.BitmapProperties.SetPropertiesAsync(properties);
                    await encoder.FlushAsync();
                }
            }
        }

        private static PhotoOrientation ConvertOrientationToPhotoOrientation(SimpleOrientation orientation)
        {
            switch (orientation)
            {
                case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    return PhotoOrientation.Rotate90;
                case SimpleOrientation.Rotated180DegreesCounterclockwise:
                    return PhotoOrientation.Rotate180;
                case SimpleOrientation.Rotated270DegreesCounterclockwise:
                    return PhotoOrientation.Rotate270;
                case SimpleOrientation.NotRotated:
                default:
                    return PhotoOrientation.Normal;
            }
        }
    }
}
