using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ImageSomCompressor.Core.Som.Extensions;
using ImageSomCompressor.Core.Som.Lattice;
using ImageSomCompressor.Core.Som.Vector;

namespace ImageSomCompressor
{
    public partial class MainWindow : Window
    {
        private const int INPUT_DIMENSION = 3; //cuz of RGB
        private readonly BackgroundWorker _backgroundWorker;
        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        private ILattice _lattice;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ImageSomCompressorDataContext();
            _lattice = new Lattice(3, 3, INPUT_DIMENSION, 100, 0.5);
            _backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _backgroundWorker.DoWork += OnBackgroundWorker_DoWork;
            _backgroundWorker.ProgressChanged += OnBackgroundWorker_ProgressChanged;
            _backgroundWorker.RunWorkerCompleted += OnBackgroundWorker_ProgressChanged_Complete;

            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1);
            _dispatcherTimer.Tick += OnDispatcherTimer_Tick;
        }

        private void OnBackgroundWorker_ProgressChanged_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            var dataContext = DataContext as ImageSomCompressorDataContext;
            if (e.Cancelled)
            {
                //canceled
            }
            else if (e.Error != null)
            {
                //error
            }
            else
            {
                dataContext.Stopwatch.Stop();
                _dispatcherTimer.Stop();
            }
        }

        private void OnDispatcherTimer_Tick(object sender, EventArgs e)
        {
            var dataContext = DataContext as ImageSomCompressorDataContext;
            if (!dataContext.Stopwatch.IsRunning)
            {
                return;
            }

            var ts = dataContext.Stopwatch.Elapsed;
            dataContext.CurrentTime = $"{ts.Minutes:00}:{ts.Seconds:00}";
        }

        private void SetStartValues()
        {
            var dataContext = DataContext as ImageSomCompressorDataContext;
            dataContext.Width = 3;
            dataContext.Height = 3;
            dataContext.LearningRate = 0.5d;
            dataContext.NumberOfIterations = 30;
        }

        private void OnBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;

            var input = (IVector[]) e.Argument;

            _lattice.Train(input, worker);
        }

        private void OnBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var dataContext = DataContext as ImageSomCompressorDataContext;
            dataContext.ProgressBar = e.ProgressPercentage;
            dataContext.CurrentIteration =
                $"{(int) ((float) e.ProgressPercentage / 100 * dataContext.NumberOfIterations)}/{dataContext.NumberOfIterations}";
        }

        private void OnBtnLoadClick(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as ImageSomCompressorDataContext;
            SetStartValues();
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JPeg Image|*.jpg|PNG Image|*.png|Bitmap Image|*.bmp",
                Title = "Please select an image file."
            };
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dataContext.IsProcessingEnable = false;
                var bitmap = new Bitmap(openFileDialog.FileName);
                dataContext.OriginalImage = bitmap;
                PrintImageOnGui(bitmap);
                dataContext.IsProcessingEnable = true;
            }
        }

        private void OnBtnTrainClick(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as ImageSomCompressorDataContext;
            _lattice = new Lattice(dataContext.Width, dataContext.Height, INPUT_DIMENSION,
                dataContext.NumberOfIterations,
                dataContext.LearningRate);
            dataContext.ProgressBar = 0;
            dataContext.Stopwatch = Stopwatch.StartNew();
            _dispatcherTimer.Start();
            var input = (DataContext as ImageSomCompressorDataContext).OriginalImage.ToVectors().ToArray();
            _backgroundWorker.RunWorkerAsync(input);
        }

        private void PrintImageOnGui(Image image)
        {
            using (var memory = new MemoryStream())
            {
                image.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    (DataContext as ImageSomCompressorDataContext).ImageSource = bitmapImage;
                }));
            }
        }

        private void ShowChangedImage()
        {
            var dataContext = DataContext as ImageSomCompressorDataContext;
            var image = dataContext.OriginalImage;
            var input = dataContext.OriginalImage.ToVectors().ToArray();
            var result = _lattice.GenerateResult(input);
            var bitmap = result.ToBitmap(image.Height, image.Width);
            dataContext.ChangedImage = bitmap;
            PrintImageOnGui(bitmap);
        }

        private void OnBtnCompressClick(object sender, RoutedEventArgs e)
        {
            ShowChangedImage();
        }

        private void OnBtnSaveImageClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JPeg Image|*.jpg|PNG Image|*.png|Bitmap Image|*.bmp",
                Title = "Please select an image file.",
                FileName = "Result"
            };
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var bitmap = (DataContext as ImageSomCompressorDataContext).ChangedImage;
                switch (saveFileDialog.FilterIndex)
                {
                    case 1:
                        bitmap.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                        break;
                    case 2:
                        bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
                        break;
                    case 3:
                        bitmap.Save(saveFileDialog.FileName, ImageFormat.Bmp);
                        break;
                }
            }
        }
    }
}