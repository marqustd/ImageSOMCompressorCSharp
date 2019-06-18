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
using ImageSomCompressor.Core;
using ImageSomCompressor.Core.Som.Extensions;
using ImageSomCompressor.Core.Som.Lattice;
using ImageSomCompressor.Core.Som.Vector;
using MessageBox = System.Windows.Forms.MessageBox;

namespace ImageSomCompressor
{
    public partial class MainWindow : Window
    {
        private const int INPUT_DIMENSION = 3; //cuz of RGB
        private readonly BackgroundWorker _backgroundWorker;
        private readonly DispatcherTimer _dispatcherTimer = new DispatcherTimer();
        private readonly ImageSomCompressorDataContext _dataContext;
        private ILattice _lattice;

        public MainWindow()
        {
            InitializeComponent();
            _dataContext = new ImageSomCompressorDataContext();
            DataContext = _dataContext;
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
            if (e.Cancelled)
            {
                MessageBox.Show("Learning has been canceled");
            }
            else if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else
            {
                MessageBox.Show("Learning has ended");
                _dataContext.Stopwatch.Stop();
                _dispatcherTimer.Stop();
            }
        }

        private void OnDispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!_dataContext.Stopwatch.IsRunning)
            {
                return;
            }

            var ts = _dataContext.Stopwatch.Elapsed;
            _dataContext.CurrentTime = $"{ts.Minutes:00}:{ts.Seconds:00}";
        }

        private void SetStartValues()
        {
            _dataContext.Width = 3;
            _dataContext.Height = 3;
            _dataContext.LearningRate = 0.5d;
            _dataContext.NumberOfIterations = 30;
        }

        private void OnBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;

            var input = (IVector[]) e.Argument;

            _lattice.Train(input, worker);
        }

        private void OnBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _dataContext.ProgressBar = e.ProgressPercentage;
            _dataContext.CurrentIteration =
                $"{(int) ((float) e.ProgressPercentage / 100 * _dataContext.NumberOfIterations)}/{_dataContext.NumberOfIterations}";
        }

        private void OnBtnLoadClick(object sender, RoutedEventArgs e)
        {
            SetStartValues();
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JPeg Image|*.jpg|PNG Image|*.png|Bitmap Image|*.bmp",
                Title = "Please select an image file."
            };
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _dataContext.IsProcessingEnable = false;
                var bitmap = new Bitmap(openFileDialog.FileName);
                _dataContext.OriginalImage = bitmap;
                PrintImageOnGui(bitmap);
                _dataContext.IsProcessingEnable = true;
            }
        }

        private void OnBtnTrainClick(object sender, RoutedEventArgs e)
        {
            _lattice = new Lattice(_dataContext.Width, _dataContext.Height, INPUT_DIMENSION,
                _dataContext.NumberOfIterations,
                _dataContext.LearningRate);
            _dataContext.ProgressBar = 0;
            _dataContext.Stopwatch = Stopwatch.StartNew();
            _dispatcherTimer.Start();
            var input = _dataContext.OriginalImage.ToVectors().ToArray();
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
            var image = _dataContext.OriginalImage;
            var input = _dataContext.OriginalImage.ToVectors().ToArray();
            var result = _lattice.GenerateResult(input);
            var bitmap = result.ToBitmap(image.Height, image.Width);
            _dataContext.ChangedImage = bitmap;
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
                Filter = "SomCompressed|*.som|JPeg Image|*.jpg|PNG Image|*.png|Bitmap Image|*.bmp",
                Title = "Please select an image file.",
                FileName = "Result"
            };
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var bitmap = _dataContext.ChangedImage;
                switch (saveFileDialog.FilterIndex)
                {
                    case 1:
                        var input = _dataContext.OriginalImage.ToVectors().ToArray();
                        new FileHelper().SaveToFile(saveFileDialog.FileName, _lattice.GenerateResultBytes(input), _lattice.Neurons, bitmap.Width,
                            bitmap.Height);
                        break;
                    case 2:
                        bitmap.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                        break;
                    case 3:
                        bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
                        break;
                    case 4:
                        bitmap.Save(saveFileDialog.FileName, ImageFormat.Bmp);
                        break;
                }
            }
        }
    }
}