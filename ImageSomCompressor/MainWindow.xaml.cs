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
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
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
            var openFileDialog = new OpenFileDialog
            {
                Filter = "1.JPeg Image|*.jpg|2.Bitmap Image|*.bmp",
                Title = "Please select an image file."
            };
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var bitmap = new Bitmap(openFileDialog.FileName);
                (DataContext as ImageSomCompressorDataContext).Image = bitmap;
                PrintImageOnGui(bitmap);
            }
        }

        private void OnBtnTrainClick(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as ImageSomCompressorDataContext;
            lattice = new Lattice(dataContext.Width, dataContext.Height, InputDimension, dataContext.NumberOfIterations,
                dataContext.LearningRate);
            dataContext.ProgressBar = 0;
            var input = (DataContext as ImageSomCompressorDataContext).Image.ToVectors().ToArray();
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

        private void OnBtnCompressClick(object sender, RoutedEventArgs e)
        {
            var image = (DataContext as ImageSomCompressorDataContext).Image;
            var input = (DataContext as ImageSomCompressorDataContext).Image.ToVectors().ToArray();
            var result = lattice.GenerateResult(input);
            var bitmap = MakeBitmap(result, image.Height, image.Width);
            (DataContext as ImageSomCompressorDataContext).Image = bitmap;
            PrintImageOnGui(bitmap);
        }
    }
}