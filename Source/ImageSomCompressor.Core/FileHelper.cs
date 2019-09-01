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
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                using (var inputFile = new BinaryReader(stream))
                {
                    var neuronsNumber = inputFile.ReadByte() + 1;
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

                    var pixels = new List<Pixel>();

                    while (inputFile.BaseStream.Position < inputFile.BaseStream.Length - 1)
                    {
                        var index = inputFile.ReadByte();
                        var number = inputFile.ReadByte();

                        for (var i = 0; i < number; i++)
                        {
                            pixels.Add(new Pixel(list[index]));
                        }
                    }

                    var bitmap = new Bitmap(width, height);
                    var p = 0;

                    for (var y = 0; y < height; y++)
                    {
                        for (var x = 0; x < width; x++)
                        {
                            var pixel = pixels[p++];
                            bitmap.SetPixel(x, y, Color.FromArgb(pixel.R, pixel.G, pixel.B));
                        }
                    }

                    return bitmap;
                }
            }
        }

        private void SaveToFile(string filename, SomFileModel model)
        {
            using (var outputFile = new BinaryWriter(new FileStream(filename, FileMode.OpenOrCreate)))
            {
                outputFile.Write((byte) (model.Neurons.Count - 1));
                foreach (var neuron in model.Neurons)
                {
                    outputFile.Write(neuron.R);
                    outputFile.Write(neuron.G);
                    outputFile.Write(neuron.B);
                }

                outputFile.Write(model.Width);
                outputFile.Write(model.Height);

                var former = model.Input[0];
                byte count = 0;

                for (var i = 0; i < model.Input.Count; i++)
                {
                    if (model.Input[i] == former && count < byte.MaxValue)
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