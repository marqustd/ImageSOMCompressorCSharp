﻿using System;
using ImageSomCompressor.Core.Som.Vector;

namespace ImageSomCompressor.Core.Som.Neuron
{
    internal class Neuron : INeuron
    {
        public Neuron(int numOfWeights)
        {
            var random = new Random();
            Weights = new Vector.Vector(X, Y);

            for (var i = 0; i < numOfWeights; i++)
            {
                Weights.Add(random.NextDouble());
            }
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int R { get; private set; }
        public int G { get; private set; }
        public int B { get; private set; }
        public IVector Weights { get; }

        public void SetWeight(int index, double value)
        {
            if (index >= Weights.Count)
            {
                throw new ArgumentException("Wrong index!");
            }

            Weights[index] = value;
        }

        public double GetWeight(int index)
        {
            if (index >= Weights.Count)
            {
                throw new ArgumentException("Wrong index!");
            }

            return Weights[index];
        }

        public void UpdateWeights(IVector input, double distanceDecay, double learningRate)
        {
            if (input.Count != Weights.Count && input.Count != 3)
            {
                throw new ArgumentException("Wrong input!");
            }

            for (var i = 0; i < Weights.Count; i++)
            {
                Weights[i] += distanceDecay * learningRate * (input[i] - Weights[i]);
            }

            R = (int) ((R + input[0]) / 2);
            G = (int) ((R + input[1]) / 2);
            B = (int) ((R + input[2]) / 2);
        }

        public double Distance(INeuron neuron)
        {
            return Math.Pow(X - neuron.X, 2)
                   + Math.Pow(Y - neuron.Y, 2);
        }
    }
}