using System.Collections.Generic;

namespace ImageSomCompressor.Core.Vector
{
    public interface IVector : IList<double>
    {
        double EuclideanDistance(IVector vector);
    }
}