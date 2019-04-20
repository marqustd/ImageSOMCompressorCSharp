using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageSomCompressor.Core
{
    internal class Node
    {
        private readonly List<double> weights;
        private double bottom;
        private double left;
        private double right;
        private double top;

        public Node(double left, double right, double top, double bottom, int weights)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
            this.weights = new List<double>();
            var random = new Random();

            for (var i = 0; i < weights; i++)
            {
                this.weights.Add(random.NextDouble());
            }

            X = (left + right) / 2;
            Y = (bottom + top) / 2;
        }

        public double X { get; }
        public double Y { get; }

        public double GetDistance(IList<double> inputVector)
        {
            return weights.Select((t, i) => (inputVector[i] - t) * (inputVector[i] - t)).Sum();
        }

        public void AdjustWeights(IList<double> target, double learningRate, double influence)
        {
            for (var i = 0; i < target.Count; i++)
            {
                weights[i] += learningRate * influence * (target[i] - weights[i]);
            }
        }
    }
}