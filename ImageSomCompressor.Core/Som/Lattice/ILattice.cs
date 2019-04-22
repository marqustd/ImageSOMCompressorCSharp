using System.Collections.Generic;
using System.ComponentModel;
using ImageSomCompressor.Core.Som.Vector;

namespace ImageSomCompressor.Core.Som.Lattice
{
    public interface ILattice
    {
        void Train(IEnumerable<IVector> input, BackgroundWorker worker);
        IEnumerable<IVector> GenerateResult(IEnumerable<IVector> input);
    }
}