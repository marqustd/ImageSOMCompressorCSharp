using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ImageSomCompressor.Core.Models;
using ImageSomCompressor.Core.Som.Neuron;

namespace ImageSomCompressor.Core
{
    public sealed class FileHelper
    {
        public void SaveToFile(string filename, IList<byte> input, IList<INeuron> neurons, int width, int height)
        {
            var model = new SomFileModel
            {
                Neurons = neurons,
                Height = height,
                Width = width,
                Input = input
            };

            SaveToFile(filename, model);
        }

        public Bitmap ReadFromFile(string filename)
        {
            using (var inputFile = new BinaryReader(new FileStream(filename, FileMode.Open)))
            {
                var neuronsNumber = inputFile.ReadInt32();
                var list = new List<Pixel>();
                for (var i = 0; i < neuronsNumber; i++)
                {
                    list.Add(new Pixel
                    {
                        R = inputFile.ReadByte(),
                        G = inputFile.ReadByte(),
                        B = inputFile.ReadByte()
                    });
                }

                var width = inputFile.ReadInt32();
                var height = inputFile.ReadInt32();


            }
        }

        private void SaveToFile(string filename, SomFileModel model)
        {
            using (var outputFile = new BinaryWriter(new FileStream(filename, FileMode.OpenOrCreate)))
            {
                outputFile.Write((byte) model.Neurons.Count);
                foreach (var neuron in model.Neurons)
                {
                    outputFile.Write(neuron.R);
                    outputFile.Write(neuron.G);
                    outputFile.Write(neuron.B);
                }

                outputFile.Write(model.Width);
                outputFile.Write(model.Height);

                var former = model.Input[0];
                var count = 0;

                for (var i = 0; i < model.Input.Count; i++)
                {
                    if (model.Input[i] == former)
                    {
                        count++;

                        if (i == model.Input.Count - 1)
                        {
                            outputFile.Write(former);
                            outputFile.Write(count);
                        }
                    }
                    else
                    {
                        outputFile.Write(former);
                        outputFile.Write(count);

                        former = model.Input[i];
                        count = 1;
                    }
                }
            }
        }
    }
}