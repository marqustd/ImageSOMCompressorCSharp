﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ImageSomCompressor.Core.Som.Models;

namespace ImageSomCompressor.Core.Som.Lattice
{
    public class Lattice : ILattice
    {
        private readonly int _height;
        private readonly Neuron.Neuron[,] _lattice;
        private readonly double _matrixRadius;
        private readonly int _numberOfIterations;
        private readonly double _timeConstant;
        private readonly int _width;
        private int _iteration;
        private double _learningRate;

        public Lattice(int width, int height, int inputDimension, int numberOfIterations, double learningRate)
        {
            _width = width;
            _height = height;
            _lattice = new Neuron.Neuron[width, height];
            _numberOfIterations = numberOfIterations;
            _learningRate = learningRate;
            _iteration = 0;

            _matrixRadius = Math.Max(width, height) / 2;
            _timeConstant = numberOfIterations / Math.Log(_matrixRadius);

            InitializeConnections(inputDimension);
        }

        public void Train(Vector.Vector[] input, int count)
        {
            while (_iteration < _numberOfIterations)
            {
                var currentRadius = CalculateNeighborhoodRadius(_iteration);

                Parallel.For(0, count, i =>
                {
                    var currentInput = input[i];
                    var bmu = CalculateBmu(currentInput);

                    var radiusIndexes = GetRadiusIndexes(bmu, currentRadius);

                    for (var x = radiusIndexes.XStart; x < radiusIndexes.XEnd; x++)
                    {
                        for (var y = radiusIndexes.YEnd; y < radiusIndexes.YEnd; y++)
                        {
                            var processingNeuron = GetNeuron(x, y);
                            var distance = bmu.Distance(processingNeuron);
                            if (distance <= Math.Pow(currentRadius, 2.0))
                            {
                                var distanceDrop = GetDistanceDrop(distance, currentRadius);
                                processingNeuron.UpdateWeights(currentInput, _learningRate, distanceDrop);
                            }
                        }
                    }
                });

                //worker.ReportProgress((int) (iteration / (float) numberOfIterations * 100));
                _iteration++;
                _learningRate = _learningRate * Math.Exp(-(double) _iteration / _numberOfIterations);
            }

            //worker.ReportProgress(100);
        }

        public Vector.Vector[] GenerateResult(Vector.Vector[] input)
        {
            return input.Select(CalculateBmu).Select(node => new Vector.Vector
            {
                node.R,
                node.G,
                node.B
            }).ToArray();
        }

        internal RadiusIndexes GetRadiusIndexes(Neuron.Neuron bmu, double currentRadius)
        {
            var xStart = (int) (bmu.X - currentRadius - 1);
            xStart = xStart < 0 ? 0 : xStart;

            var xEnd = (int) (xStart + currentRadius * 2 + 1);
            if (xEnd > _width)
            {
                xEnd = _width;
            }

            var yStart = (int) (bmu.Y - currentRadius - 1);
            yStart = yStart < 0 ? 0 : yStart;

            var yEnd = (int) (yStart + currentRadius * 2 + 1);
            if (yEnd > _height)
            {
                yEnd = _height;
            }

            return new RadiusIndexes(xStart, xEnd, yStart, yEnd);
        }

        private Neuron.Neuron GetNeuron(int indexX, int indexY)
        {
            if (indexX > _width || indexY > _height)
            {
                throw new ArgumentException("Wrong index!");
            }

            return _lattice[indexX, indexY];
        }

        private double CalculateNeighborhoodRadius(double iteration)
        {
            return _matrixRadius * Math.Exp(-iteration / _timeConstant);
        }

        private double GetDistanceDrop(double distance, double radius)
        {
            return Math.Exp(-(Math.Pow(distance, 2.0) / Math.Pow(radius, 2.0)));
        }

        private Neuron.Neuron CalculateBmu(Vector.Vector input)
        {
            var bmu = _lattice[0, 0];
            var bestDist = input.EuclideanDistance(bmu.Weights);

            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    var distance = input.EuclideanDistance(_lattice[i, j].Weights);
                    if (distance < bestDist)
                    {
                        bmu = _lattice[i, j];
                        bestDist = distance;
                    }
                }
            }

            return bmu;
        }

        private void InitializeConnections(int inputDimension)
        {
            Parallel.For(0, _width,
                x =>
                {
                    Parallel.For(0, _width,
                        y => { _lattice[x, y] = new Neuron.Neuron(inputDimension) {X = x, Y = y}; });
                });
        }
    }
}