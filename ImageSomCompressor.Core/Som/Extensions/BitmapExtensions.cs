using System.Collections.Generic;
using System.Drawing;
using ImageSomCompressor.Core.Som.Vector;

namespace ImageSomCompressor.Core.Som.Extensions
{
    public static class BitmapExtensions
    {
        public static IEnumerable<IVector> ToVectors(this Bitmap bitmap)
        {
            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    yield return new Vector.Vector(x, y)
                    {
                        pixel.R,
                        pixel.G,
                        pixel.B
                    };
                }
            }
        }
    }
}