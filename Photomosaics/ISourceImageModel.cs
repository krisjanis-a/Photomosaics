using System;
using System.Drawing;

namespace Photomosaics
{
    public interface ISourceImageModel
    {
        Bitmap SourceImage { get; set; }

        string ImageName { get; set; }
        string FilePath { get; set; }

        Tuple<int,int,int> AverageImageColorRGB { get; set; }

        Tuple<int, int, int>[,] ImageRGBPixelArray { get; set; }
        Tuple<int, int, int>[,] ImageSectionAverageRGBComponentArray { get; set; }
        Tuple<int, int, int>[,,,] ImageSectionsRGBPixelArray { get; set; }
    }
}

