using System;
using ImageSomCompressor.Core.Neuron;
using ImageSomCompressor.Core.Vector;

namespace ImageSomCompressor.Core.SomLattice
{
    public class SomLattice : ISomLattice
    {
        private readonly int height;
        private readonly INeuron[,] lattice;
        private readonly double matrixRadius;
        private readonly double numberOfIterations;
        private readonly double timeConstant;
        private readonly int width;
        private int iteration;
        private double learningRate;

        public SomLattice(int width, int height, int inputDimension, int numberOfIterations, double learningRate)
        {
            this.width = width;
            this.height = height;
            lattice = new INeuron[width, height];
            this.numberOfIterations = numberOfIterations;
            this.learningRate = learningRate;
            iteration = 0;

            matrixRadius = Math.Max(width, height) / 2;
            timeConstant = numberOfIterations / Math.Log(matrixRadius);

            InitializeConnections(inputDimension);
        }

        public void Train(IVector[] input)
        {
            while (iteration < numberOfIterations)
            {
                var currentRadius = CalculateNeighborhoodRadius(iteration);

                foreach (var currentInput in input)
                {
                    var bmu = CalculateBMU(currentInput);

                    var (xStart, xEnd, yStart, yEnd) = GetRadiusIndexes(bmu, currentRadius);

                    for (var x = xStart; x < xEnd; x++)
                    {
                        for (var y = yStart; y < yEnd; y++)
                        {
                            var processingNeuron = GetNeuron(x, y);
                            var distance = bmu.Distance(processingNeuron);
                            if (distance <= Math.Pow(currentRadius, 2.0))
                            {
                                var distanceDrop = GetDistanceDrop(distance, currentRadius);
                                processingNeuron.UpdateWeights(currentInput, learningRate, distanceDrop);
                            }
                        }
                    }
                }

                iteration++;
                learningRate = learningRate * Math.Exp(-(double) iteration / numberOfIterations);
            }
        }

        internal (int xStart, int xEnd, int yStart, int yEnd) GetRadiusIndexes(INeuron bmu, double currentRadius)
        {
            var xStart = (int) (bmu.X - currentRadius - 1);
            xStart = xStart < 0 ? 0 : xStart;

            var xEnd = (int) (xStart + currentRadius * 2 + 1);
            if (xEnd > width)
            {
                xEnd = width;
            }

            var yStart = (int) (bmu.Y - currentRadius - 1);
            yStart = yStart < 0 ? 0 : yStart;

            var yEnd = (int) (yStart + currentRadius * 2 + 1);
            if (yEnd > height)
            {
                yEnd = height;
            }

            return (xStart, xEnd, yStart, yEnd);
        }

        private INeuron GetNeuron(int indexX, int indexY)
        {
            if (indexX > width || indexY > height)
            {
                throw new ArgumentException("Wrong index!");
            }

            return lattice[indexX, indexY];
        }

        private double CalculateNeighborhoodRadius(double iteration)
        {
            return matrixRadius * Math.Exp(-iteration / timeConstant);
        }

        private double GetDistanceDrop(double distance, double radius)
        {
            return Math.Exp(-(Math.Pow(distance, 2.0) / Math.Pow(radius, 2.0)));
        }

        private INeuron CalculateBMU(IVector input)
        {
            var bmu = lattice[0, 0];
            var bestDist = input.EuclideanDistance(bmu.Weights);

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var distance = input.EuclideanDistance(lattice[i, j].Weights);
                    if (distance < bestDist)
                    {
                        bmu = lattice[i, j];
                        bestDist = distance;
                    }
                }
            }

            return bmu;
        }

        private void InitializeConnections(int inputDimension)
        {
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    lattice[i, j] = new Neuron.Neuron(inputDimension) {X = i, Y = j};
                }
            }
        }
    }
}