using System;
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
        private readonly BackgroundWorker backgroundWorker;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ImageSomCompressorDataContext();
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

            var som = new Lattice(3, 3, input.FirstOrDefault().Count, 100, 0.5);
            som.Train(input, worker);
        }

        // This event handler updates the progress.
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
    }
}