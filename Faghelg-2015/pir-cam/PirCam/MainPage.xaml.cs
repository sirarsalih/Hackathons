using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
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
        private MediaCapture _mediaCapture;
        private const string ServiceBusConnectionString =
            "Endpoint=sb://iteraphotobooth.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=wQYYVYvzgZmeWxTF+Z3nWLzBQ7j0YrY8RK47QEbsDH4=";
        private const string ServiceBusQueueName = "PhotoQueue";
        private readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler {UseDefaultCredentials = true});
        private bool _foundFrontCam;
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

        private async void InitilizeCam()
        {
            try
            {
                //initialize the WebCam via MediaCapture object
                _mediaCapture = new MediaCapture();
                var settings = new MediaCaptureInitializationSettings();

                var cams = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

                // If more than 1 cam, then choose the front if available
                if (cams.Count > 1)
                {
                    foreach (var cam in cams)
                    {
                        var location = cam.EnclosureLocation;
                        if (location == null || location.Panel != Panel.Front) continue;
                        settings.VideoDeviceId = cam.Id;
                        await _mediaCapture.InitializeAsync(settings);
                        _foundFrontCam = true;
                        break;
                    }
                    if (!_foundFrontCam)
                    {
                        await _mediaCapture.InitializeAsync();
                    }
                }
                else
                {
                    await _mediaCapture.InitializeAsync();
                }

                // Set callbacks for any possible failure in TakePicture() logic
                _mediaCapture.Failed += MediaCapture_Failed;
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
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
                await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

                var photoOrientation = _foundFrontCam ? PhotoOrientation.Rotate90 : PhotoOrientation.Normal;

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

        private void PostToServerAsync(byte[] imageBytes)
        {
            var mfdc = new MultipartFormDataContent
            {
                {new StreamContent(content: new MemoryStream(imageBytes)), "Photo", "Photo.jpeg"}
            };
            var result = _httpClient.PostAsync(SignalRServerUrl, mfdc).Result;
            var resultContent = result.Content.ReadAsStringAsync().Result;
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
    }
}
