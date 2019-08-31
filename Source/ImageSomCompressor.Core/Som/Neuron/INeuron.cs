using ImageSomCompressor.Core.Som.Vector;

namespace ImageSomCompressor.Core.Som.Neuron
{
    public interface INeuron
    {
        int X { get; set; }
        int Y { get; set; }
        byte R { get; }
        byte G { get; }
        byte B { get; }
        IVector Weights { get; }

        double Distance(INeuron neuron);
        void SetWeight(int index, double value);
        double GetWeight(int index);
        void UpdateWeights(IVector input, double distanceDecay, double learningRate);
    }
}