using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using ImageSomCompressor.Annotations;

namespace ImageSomCompressor
{
    internal class ImageSomCompressorDataContext : INotifyPropertyChanged
    {
        private Bitmap _changedImage;
        private string _currentIteration;
        private string _currentTime;
        private int _height;
        private BitmapImage _imageSource;
        private bool _isProcessingEnable;
        private double _learningRate;
        private int _numberOfIterations;
        private Bitmap _originalImage;
        private double _progressBar;
        private Stopwatch _stopwatch;
        private int _width;

        public bool IsProcessingEnable
        {
            get => _isProcessingEnable;
            set
            {
                _isProcessingEnable = value;
                OnPropertyChanged(nameof(IsProcessingEnable));
            }
        }

        public string CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                OnPropertyChanged(nameof(CurrentTime));
            }
        }

        public Stopwatch Stopwatch
        {
            get => _stopwatch;
            set
            {
                _stopwatch = value;
                OnPropertyChanged(nameof(Stopwatch));
            }
        }

        public string CurrentIteration
        {
            get => _currentIteration;
            set
            {
                _currentIteration = value;
                OnPropertyChanged(nameof(CurrentIteration));
            }
        }

        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        public int NumberOfIterations
        {
            get => _numberOfIterations;
            set
            {
                _numberOfIterations = value;
                OnPropertyChanged(nameof(NumberOfIterations));
            }
        }

        public double LearningRate
        {
            get => _learningRate;
            set
            {
                _learningRate = value;
                OnPropertyChanged(nameof(LearningRate));
            }
        }

        public double ProgressBar
        {
            get => _progressBar;
            set
            {
                _progressBar = value;
                OnPropertyChanged(nameof(ProgressBar));
            }
        }

        public BitmapImage ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        public Bitmap OriginalImage
        {
            get => _originalImage;
            set
            {
                _originalImage = value;
                OnPropertyChanged(nameof(OriginalImage));
            }
        }

        public Bitmap ChangedImage
        {
            get => _changedImage;
            set
            {
                _changedImage = value;
                OnPropertyChanged(nameof(OriginalImage));
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