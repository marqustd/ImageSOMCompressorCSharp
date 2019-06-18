namespace ImageSomCompressor.Core.Models
{
    public sealed class Pixel
    {
        public Pixel()
        {
            
        }

        public Pixel(Pixel pixel)
        {
            R = pixel.R;
            G = pixel.G;
            B = pixel.B;
        }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }
}