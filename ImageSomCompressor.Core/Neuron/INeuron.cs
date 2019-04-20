﻿using ImageSomCompressor.Core.Vector;

namespace ImageSomCompressor.Core.Neuron
{
    public interface INeuron
    {
        int X { get; set; }
        int Y { get; set; }
        IVector Weights { get; }

        double Distance(INeuron neuron);
        void SetWeight(int index, double value);
        double GetWeight(int index);
        void UpdateWeights(IVector input, double distanceDecay, double learningRate);
    }
}