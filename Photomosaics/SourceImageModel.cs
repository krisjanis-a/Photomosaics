using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Photomosaics
{
    public class SourceImageModel : ISourceImageModel
    {
        public Bitmap SourceImage { get; set; }

        public string ImageName { get; set; }
        public string FilePath { get; set; }

        public Tuple<int, int, int> AverageImageColorRGB { get; set; }

        public Tuple<int,int,int>[,] ImageRGBPixelArray { get; set; }
        public Tuple<int, int, int>[,] ImageSectionAverageRGBComponentArray { get; set; }
        public Tuple<int, int, int>[,,,] ImageSectionsRGBPixelArray { get; set; }
    }
}
