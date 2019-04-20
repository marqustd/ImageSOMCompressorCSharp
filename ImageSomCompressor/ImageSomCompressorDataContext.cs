using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using ImageSomCompressor.Annotations;

namespace ImageSomCompressor
{
    internal class ImageSomCompressorDataContext : INotifyPropertyChanged
    {
        private Bitmap image;
        private BitmapImage imageSource;
        private double progressBar;

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