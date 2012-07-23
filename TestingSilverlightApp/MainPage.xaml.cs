using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TestingSilverlightApp.ImageService;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media.Imaging;

namespace TestingSilverlightApp
{
    public partial class MainPage : UserControl
    {
        CaptureSource _captureSource;
        ImageProcessingServiceClient _client = new ImageProcessingServiceClient();
        
        AppMode _appMode;

        public MainPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs eargs)
        {
            // Create capturesource and use the default video capture device 
            _captureSource = new CaptureSource();
            _captureSource.VideoCaptureDevice = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();
            _captureSource.CaptureImageCompleted += new EventHandler<CaptureImageCompletedEventArgs>(captureCompleted);

            _client.RecognizeFromOctoCompleted += new EventHandler<RecognizeFromOctoCompletedEventArgs>(_client_RecognizeFromOctoCompleted);
            _client.GetTrainedLabelsCompleted += new EventHandler<GetTrainedLabelsCompletedEventArgs>(_client_GetTrainedLabelsCompleted);
            _client.AddToOctoSetCompleted += new EventHandler<AddToOctoSetCompletedEventArgs>(_client_AddToOctoSetCompleted);
            _client.GetTrainedLabelsAsync();
        }

        void _client_AddToOctoSetCompleted(object sender, AddToOctoSetCompletedEventArgs e)
        {
            busyIndicator.IsBusy = false;
            if (e.Result)
            {
                TextBoxInfo.Text = "Face added to trainging set";
            }
            else
            {
                TextBoxInfo.Text = "Error while adding face";
            }
        }

        void _client_GetTrainedLabelsCompleted(object sender, GetTrainedLabelsCompletedEventArgs e)
        {
            StringBuilder  builder = new StringBuilder();
            foreach(var name in e.Result){
                builder.AppendLine(name);
            }
            OctoListTextBlock.Text = builder.ToString();
        }

        void _client_RecognizeFromOctoCompleted(object sender, RecognizeFromOctoCompletedEventArgs e)
        {
            busyIndicator.IsBusy = false;
            TextBoxInfo.Text = e.Result;
            
        }

        private void captureCompleted(object sender, CaptureImageCompletedEventArgs e)
        {
            var image = e.Result;
            var resized = image.Resize(320, 240, WriteableBitmapExtensions.Interpolation.Bilinear);

            //resized.ForEach((x, y, c) => Color.FromArgb(c.A, (byte)(c.R * 0.3),(byte)( c.G * 0.59),(byte) (c.B * 0.11)));
            

            var imgCollection = new ObservableCollection<int>(resized.Pixels);
            switch(_appMode)
            {
                case AppMode.RecognitionOcto:
                    _client.RecognizeFromOctoAsync(imgCollection, resized.PixelWidth);
                    break;
                case AppMode.TrainingOcto:
                    _client.AddToOctoSetAsync(imgCollection, resized.PixelWidth,TextBoxLabel.Text);
                    break;
            }
           
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs eargs)
        {
            try
            {
                // Start capturing
                if (_captureSource.State != CaptureState.Started)
                {
                    // Create video brush and fill the WebcamVideo rectangle with it
                    var vidBrush = new VideoBrush();
                    vidBrush.Stretch = Stretch.Uniform;
                    vidBrush.SetSource(_captureSource);
                    WebcamVideo.Fill = vidBrush;

                    // Ask user for permission and start the capturing
                    if (CaptureDeviceConfiguration.RequestDeviceAccess())
                    {
                        _captureSource.Start();
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                TextBoxInfo.Text = "Web Cam already started - if not, I can't find it...";
            }
            catch (Exception)
            {
                TextBoxInfo.Text = "Could not start web cam, do you have one?";
            }
        }

        private void ButtonRecognizeOcto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                busyIndicator.BusyContent = "Imaged captured & send to server";
                busyIndicator.IsBusy = true;
                _appMode = AppMode.RecognitionOcto;
                _captureSource.CaptureImageAsync();
            }
            catch (InvalidOperationException ex)
            {
                TextBoxInfo.Text = "Start the web cam first";
                busyIndicator.IsBusy = false;
            }
            catch (Exception ex)
            {
                TextBoxInfo.Text = "Undetermined exception";
                busyIndicator.IsBusy = false;
            }        
        }

        private void ButtonAddOcto_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(TextBoxLabel.Text))
            {
                TextBoxInfo.Text = "You have to specify the label!";
                return;
            }
            try
            {
                busyIndicator.IsBusy = true;
                busyIndicator.BusyContent = "Imaged captured & send to server to be added to training set";
                _appMode = AppMode.TrainingOcto;
                _captureSource.CaptureImageAsync();
            }
            catch (InvalidOperationException ex)
            {
                TextBoxInfo.Text = "Start the web cam first";
                busyIndicator.IsBusy = false;
            }
            catch (Exception ex)
            {
                busyIndicator.IsBusy = false;
            }
        }

    }
}
