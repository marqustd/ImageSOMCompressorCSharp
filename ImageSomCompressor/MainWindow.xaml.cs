using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using ImageSomCompressor.Core.Som.Lattice;
using ImageSomCompressor.Core.Som.Vector;
using Vector = ImageSomCompressor.Core.Som.Vector.Vector;

namespace ImageSomCompressor
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ImageSomCompressorDataContext();
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
                PrintImageOnGui(bitmap);
            }
        }

        private void OnBtnTrainClick(object sender, RoutedEventArgs e)
        {
            var inputVector = new Vector { 2, 2 };

            IVector[] input =
            {
                inputVector,
                inputVector,
                inputVector,
                inputVector,
                inputVector,
                inputVector,
                inputVector,
                inputVector,
                inputVector,
                inputVector
            };
            var som = new Lattice(3, 3, inputVector.Count, 100, 0.5);
            som.Train(input);
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