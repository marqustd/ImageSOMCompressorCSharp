using ImageSomCompressor.Core.Vector;

namespace ImageSomCompressor.Core.SomLattice
{
    internal interface ISomLattice
    {
        void Train(IVector[] input);
    }
}