using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using ImageSomCompressor.Core.Som.Neuron;
using ImageSomCompressor.Core.Som.Vector;

namespace ImageSomCompressor.Core.Som.Lattice
{
    public class Lattice : ILattice
    {
        private readonly int _height;
        private readonly INeuron[,] _lattice;
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
            _lattice = new INeuron[width, height];
            _numberOfIterations = numberOfIterations;
            _learningRate = learningRate;
            _iteration = 0;

            _matrixRadius = Math.Max(width, height) / 2;
            _timeConstant = numberOfIterations / Math.Log(_matrixRadius);

            InitializeConnections(inputDimension);
        }

        public void Train(IEnumerable<IVector> input, BackgroundWorker worker)
        {
            while (_iteration < _numberOfIterations)
            {
                var currentRadius = CalculateNeighborhoodRadius(_iteration);

                Parallel.ForEach(input, currentInput =>
                {
                    var bmu = CalculateBmu(currentInput);

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
                                processingNeuron.UpdateWeights(currentInput, _learningRate, distanceDrop);
                            }
                        }
                    }
                });

                worker.ReportProgress((int) (_iteration / (float) _numberOfIterations * 100));
                _iteration++;
                _learningRate = _learningRate * Math.Exp(-(double) _iteration / _numberOfIterations);
            }

            worker.ReportProgress(100);
        }

        public IEnumerable<IVector> GenerateResult(IEnumerable<IVector> input)
        {
            foreach (var vector in input)
            {
                var bmu = CalculateBmu(vector);
                yield return new Vector.Vector(vector.X, vector.Y)
                {
                    bmu.R,
                    bmu.G,
                    bmu.B
                };
            }
        }

        public IList<INeuron> Neurons
        {
            get
            {
                var list = new List<INeuron>();
                for (var i = 0; i < _width; i++)
                {
                    for (var j = 0; j < _height; j++)
                    {
                        list.Add(_lattice[i, j]);
                    }
                }

                return list;
            }
        }

        public IList<byte> GenerateResultBytes(IEnumerable<IVector> input)
        {
            var list = new List<byte>();
            foreach (var vector in input)
            {
                var bmu = CalculateBmu(vector);
                list.Add((byte) (bmu.Y * _width + bmu.X));
            }

            return list;
        }

        private (int xStart, int xEnd, int yStart, int yEnd) GetRadiusIndexes(INeuron bmu, double currentRadius)
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

            return (xStart, xEnd, yStart, yEnd);
        }

        private INeuron GetNeuron(int indexX, int indexY)
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

        private static double GetDistanceDrop(double distance, double radius)
        {
            return Math.Exp(-(Math.Pow(distance, 2.0) / Math.Pow(radius, 2.0)));
        }

        private INeuron CalculateBmu(IVector input)
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
            for (var x = 0; x < _width; x++)
            {
                Parallel.For(0, _height,
                    y => { _lattice[x, y] = new Neuron.Neuron(inputDimension) {X = x, Y = y}; });
            }
        }
    }
}