using System;
using System.Collections.Generic;
using Hybridizer.Runtime.CUDAImports;

namespace ImageSomCompressor.Core.SomCUDA.NeuronCUDA
{
    public class NeuronCUDA
    {
        public NeuronCUDA(int numOfWeights)
        {
            var weights = new List<double>();
            var random = new Random();

            for (var i = 0; i < numOfWeights; i++)
            {
                weights.Add(random.NextDouble());
            }

            Count = weights.Count;
            Weights = weights.ToArray();
        }

        public int Count { get; }

        public double[] Weights { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public int R { get; private set; }
        public int G { get; private set; }
        public int B { get; private set; }

        public void SetWeight(int index, double value)
        {
            Weights[index] = value;
        }

        public double GetWeight(int index)
        {
            return Weights[index];
        }

        public void UpdateWeights(double[] input, int inputCount, double distanceDecay, double learningRate)
        {
            for (var i = 0; i < Count; i++)
            {
                Weights[i] += distanceDecay * learningRate * (input[i] - Weights[i]);
            }

            R = (int) ((R + input[0]) / 2);
            G = (int) ((R + input[1]) / 2);
            B = (int) ((R + input[2]) / 2);
        }

        public double Distance(NeuronCUDA neuron)
        {
            return Math.Pow(X - neuron.X, 2)
                   + Math.Pow(Y - neuron.Y, 2);
        }
    }
}