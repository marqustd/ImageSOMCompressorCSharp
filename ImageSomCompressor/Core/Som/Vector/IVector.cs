using System.Collections.Generic;

namespace ImageSomCompressor.Core.Som.Vector
{
    public interface IVector : IList<double>
    {
        double EuclideanDistance(IVector vector);
    }
}