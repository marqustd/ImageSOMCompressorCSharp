using System;
using System.Collections.Generic;

namespace ImageSomCompressor.Core
{
    public class KohonenSom
    {
        private const int InputVectors = 3;
        private readonly List<Node> nodes = new List<Node>();
        private double cellHeight;
        private double cellWidth;
        private bool done = false;
        private double influence = 0;
        private int iterationCount = 1;
        private int iterations;
        private double learningRate = 0.1;
        private double mapRadius;
        private double neighbourhoodRadius = 0;
        private double timeConstant;
        private Node winningNode;

        public void Create(int x, int y, int cellsUp, int cellsAcross, int iterations)
        {
            cellWidth = (double) x / cellsAcross;
            cellHeight = (double) y / cellsUp;

            this.iterations = iterations;

            for (var row = 0; row < cellsUp; ++row)
            {
                for (var col = 0; col < cellsAcross; ++col)
                {
                    nodes.Add(new Node(col * cellWidth,
                        (col + 1) * cellWidth, //right
                        row * cellHeight, //top
                        (row + 1) * cellHeight, //bottom
                        InputVectors));
                }
            }

            mapRadius = Math.Max(5, 5) / 2.0d;
            timeConstant = this.iterations / Math.Log(mapRadius);
        }
    }
}