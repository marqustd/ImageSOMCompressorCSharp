using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ImageSomCompressor.Core.Som.Vector;

namespace ImageSomCompressor.Core.Som.Extensions
{
    public static class VectorExtensions
    {
        public static Bitmap ToBitmap(this IEnumerable<IVector> vectors, int height, int width)
        {
            var bitmap = new Bitmap(width, height);

            foreach (var vector in vectors)
            {
                bitmap.SetPixel(vector.X, vector.Y,
                    Color.FromArgb((int)vector[0], (int)vector[1], (int)vector[2]));
            }

            return bitmap;
        }
    }
}