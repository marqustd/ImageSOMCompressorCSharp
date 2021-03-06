﻿using System;
using System.Collections.Generic;

namespace ImageSomCompressor.Core.Som.Vector
{
    public class Vector : List<double>, IVector
    {
        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }

        public double EuclideanDistance(IVector vector)
        {
            if (vector.Count != Count)
            {
                throw new ArgumentException("Not the same size");
            }

            var distance = 0.0d;
            for (var i = 0; i < Count; i++)
            {
                distance += (this[i] - vector[i]) * (this[i] - vector[i]);
            }

            return distance;
        }
    }
}