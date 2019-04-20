using ImageSomCompressor.Core.Som.Vector;

namespace ImageSomCompressor.Core.Som.Lattice
{
    internal interface ILattice
    {
        void Train(IVector[] input);
    }
}