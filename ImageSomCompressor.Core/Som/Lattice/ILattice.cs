using System.Collections.Generic;
using System.ComponentModel;
using ImageSomCompressor.Core.Som.Vector;

namespace ImageSomCompressor.Core.Som.Lattice
{
    internal interface ILattice
    {
        void Train(IEnumerable<IVector> input, BackgroundWorker worker);
    }
}