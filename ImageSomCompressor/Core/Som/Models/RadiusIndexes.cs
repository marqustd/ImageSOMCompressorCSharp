namespace ImageSomCompressor.Core.Som.Models
{
    internal struct RadiusIndexes
    {
        public int XStart { get; set; }
        public int XEnd { get; set; }
        public int YStart { get; set; }
        public int YEnd { get; set; }

        public RadiusIndexes(int xStart, int xEnd, int yStart, int yEnd)
        {
            XStart = xStart;
            XEnd = xEnd;
            YStart = yStart;
            YEnd = yEnd;
        }
    }
}