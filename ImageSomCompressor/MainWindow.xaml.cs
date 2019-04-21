using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using ImageSomCompressor.Core.Som.Extensions;
using ImageSomCompressor.Core.Som.Lattice;
using ImageSomCompressor.Core.Som.Vector;

namespace ImageSomCompressor
{
    public partial class MainWindow : Window
    {
        private const int InputDimension = 3; //cuz of RGB
        private readonly BackgroundWorker backgroundWorker;
        private ILattice lattice;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ImageSomCompressorDataContext();
            lattice = new Lattice(3, 3, InputDimension, 100, 0.5);
            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
        }

        private void SetStartValues()
        {
            var dataContext = DataContext as ImageSomCompressorDataContext;
            dataContext.Width = 3;
            dataContext.Height = 3;
            dataContext.LearningRate = 0.5d;
            dataContext.NumberOfIterations = 30;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;

            var input = (IVector[]) e.Argument;

            lattice.Train(input, worker);
        }

        private Bitmap MakeBitmap(IEnumerable<IVector> input, int height, int width)
        {
            var array = input.ToArray();
            var bitmap = new Bitmap(width, height);
            var i = 0;
            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    var node = array[i++];
                    bitmap.SetPixel(x, y, Color.FromArgb((int) node[0], (int) node[1], (int) node[2]));
                }
            }

            return bitmap;
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            (DataContext as ImageSomCompressorDataContext).ProgressBar = e.ProgressPercentage;
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
                var bitmap = new Bitmap(openFileDialog.FileName);
                (DataContext as ImageSomCompressorDataContext).OriginalImage = bitmap;
                PrintImageOnGui(bitmap);
            }
        }

        private void OnBtnTrainClick(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as ImageSomCompressorDataContext;
            lattice = new Lattice(dataContext.Width, dataContext.Height, InputDimension, dataContext.NumberOfIterations,
                dataContext.LearningRate);
            dataContext.ProgressBar = 0;
            var input = (DataContext as ImageSomCompressorDataContext).OriginalImage.ToVectors().ToArray();
            backgroundWorker.RunWorkerAsync(input);
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
            var dataContext = (DataContext as ImageSomCompressorDataContext);
            var image = dataContext.OriginalImage;
            var input = dataContext.OriginalImage.ToVectors().ToArray();
            var result = lattice.GenerateResult(input);
            var bitmap = MakeBitmap(result, image.Height, image.Width);
            dataContext.ChangedImage = bitmap;
            PrintImageOnGui(bitmap);
        }

        private void OnBtnCompressClick(object sender, RoutedEventArgs e)
        {
            ShowChangedImage();
        }

        private void OnBtnSaveImageClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog()
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
                        bitmap.Save(saveFileDialog.FileName,ImageFormat.Jpeg);
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