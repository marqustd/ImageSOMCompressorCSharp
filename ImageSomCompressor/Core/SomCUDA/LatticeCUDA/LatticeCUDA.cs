using System;
using System.Threading.Tasks;
using Hybridizer.Runtime.CUDAImports;
using ImageSomCompressor.Core.Som.Models;

namespace ImageSomCompressor.Core.SomCUDA.LatticeCUDA
{
    public class LatticeCUDA
    {
        private const int Count = 3;
        private readonly int _height;
        private readonly NeuronCUDA.NeuronCUDA[,] _lattice;
        private readonly double _matrixRadius;
        private readonly int _numberOfIterations;
        private readonly double _timeConstant;
        private readonly int _width;
        private int _iteration;
        private double _learningRate;

        public LatticeCUDA(int width, int height, int inputDimension, int numberOfIterations, double learningRate)
        {
            _width = width;
            _height = height;
            _lattice = new NeuronCUDA.NeuronCUDA[width, height];
            _numberOfIterations = numberOfIterations;
            _learningRate = learningRate;
            _iteration = 0;

            _matrixRadius = Math.Max(width, height) / 2;
            _timeConstant = numberOfIterations / Math.Log(_matrixRadius);

            //InitializeConnections(inputDimension);
        }

        [EntryPoint]
        public void Train(VectorCUDA.VectorCUDA[] input, int count)
        {
            while (_iteration < _numberOfIterations)
            {
                var currentRadius = CalculateNeighborhoodRadius(_iteration);

                for(var i =0; i <count; i++)
                {
                    var currentInput = input[i];
                    var bmu = CalculateBmu(currentInput, Count);

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
                                processingNeuron.UpdateWeights(currentInput.Inputs, 3, _learningRate, distanceDrop);
                            }
                        }
                    }
                }

                //worker.ReportProgress((int) (iteration / (float) numberOfIterations * 100));
                _iteration++;
                _learningRate = _learningRate * Math.Exp(-(double) _iteration / _numberOfIterations);
            }

            //worker.ReportProgress(100);
        }

        private RadiusIndexes GetRadiusIndexes(NeuronCUDA.NeuronCUDA bmu, double currentRadius)
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

        private NeuronCUDA.NeuronCUDA GetNeuron(int indexX, int indexY)
        {
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

        private NeuronCUDA.NeuronCUDA CalculateBmu(VectorCUDA.VectorCUDA input, int inputSize)
        {
            var bmu = _lattice[0, 0];
            var bestDist = input.EuclideanDistance(bmu.Weights, bmu.Count);

            Parallel.For(0, _width, x =>
            {
                for (int y = 0; y < _height; y++)
                {
                    var distance = input.EuclideanDistance(_lattice[x, y].Weights, _lattice[x, y].Count);
                    if (distance < bestDist)
                    {
                        bmu = _lattice[x, y];
                        bestDist = distance;
                    }
                }
            });

            return bmu;
        }

        //[Kernel]
        //private void InitializeConnections(int inputDimension)
        //{
        //    Parallel.For(0, _width,
        //        x =>
        //        {
        //            Parallel.For(0, _height,
        //                y => { _lattice[x, y] = new NeuronCUDA.NeuronCUDA(inputDimension) {X = x, Y = y}; });
        //        });
        //}
    }
}