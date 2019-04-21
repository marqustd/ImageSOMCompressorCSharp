using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using ImageSomCompressor.Annotations;

namespace ImageSomCompressor
{
    internal class ImageSomCompressorDataContext : INotifyPropertyChanged
    {
        private int height;
        private Bitmap image;
        private BitmapImage imageSource;
        private double learningRate;
        private int numberOfIterations;
        private double progressBar;

        private int width;

        public int Width
        {
            get => width;
            set
            {
                width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        public int Height
        {
            get => height;
            set
            {
                height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        public int NumberOfIterations
        {
            get => numberOfIterations;
            set
            {
                numberOfIterations = value;
                OnPropertyChanged(nameof(NumberOfIterations));
            }
        }

        public double LearningRate
        {
            get => learningRate;
            set
            {
                learningRate = value;
                OnPropertyChanged(nameof(LearningRate));
            }
        }

        public double ProgressBar
        {
            get => progressBar;
            set
            {
                progressBar = value;
                OnPropertyChanged(nameof(ProgressBar));
            }
        }

        public BitmapImage ImageSource
        {
            get => imageSource;
            set
            {
                imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        public Bitmap Image
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}