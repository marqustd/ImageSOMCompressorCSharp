using System.ComponentModel;
using ImageSomCompressor.Core.Som.Vector;

namespace ImageSomCompressor.Core.Som.Lattice
{
    public interface ILattice
    {
        void Train(IVector[] input, BackgroundWorker worker);
        IVector[] GenerateResult(IVector[] input);
    }
}