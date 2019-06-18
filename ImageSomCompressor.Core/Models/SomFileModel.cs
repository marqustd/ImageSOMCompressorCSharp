using System.Collections.Generic;
using ImageSomCompressor.Core.Som.Neuron;

namespace ImageSomCompressor.Core.Models
{
    public sealed class SomFileModel
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public IList<INeuron> Neurons { get; set; }
        public IList<byte> Input { get; set; }
    }
}