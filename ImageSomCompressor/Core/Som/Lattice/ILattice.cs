namespace ImageSomCompressor.Core.Som.Lattice
{
    public interface ILattice
    {
        void Train(Vector.Vector[] input, int count);
        Vector.Vector[] GenerateResult(Vector.Vector[] input);
    }
}