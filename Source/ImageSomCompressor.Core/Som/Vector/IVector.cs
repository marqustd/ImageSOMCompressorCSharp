using System.Collections.Generic;

namespace ImageSomCompressor.Core.Som.Vector
{
    public interface IVector : IList<double>
    {
        int X { get; }
        int Y { get; }
        double EuclideanDistance(IVector vector);
    }
}