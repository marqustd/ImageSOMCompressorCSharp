﻿using System.Collections.Generic;
using System.Drawing;

namespace ImageSomCompressor.Core.Som.Extensions
{
    public static class BitmapExtensions
    {
        public static IEnumerable<Vector.Vector> ToVectors(this Bitmap bitmap)
        {
            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    yield return new Vector.Vector
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