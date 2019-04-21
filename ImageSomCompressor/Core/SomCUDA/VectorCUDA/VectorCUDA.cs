using System;
using System.Threading.Tasks;
using Hybridizer.Runtime.CUDAImports;

namespace ImageSomCompressor.Core.SomCUDA.VectorCUDA
{
    public class VectorCUDA
    {
        public double[] Inputs { get; set; }
        public int Count { get; set; }

        public double EuclideanDistance(double[] input, int inputSize)
        {
            var distance = 0.0d;

            Parallel.For(0, inputSize, i => { distance += (Inputs[i] - input[i]) * (Inputs[i] - input[i]); });

            return distance;
        }
    }
}