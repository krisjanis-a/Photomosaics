using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using static Photomosaics.ImageManipulation;

namespace Photomosaics
{
    public class CreateSourceImageList
    {
        public static List<ISourceImageModel> CreateSourceImageModelList(int sectionHeight, int sectionWidth)
        {
            List<ISourceImageModel> sourceImageModels = new List<ISourceImageModel>();

            string sourceDirectoryPath = "/Users/Kr1sh/Desktop/unknown/programming scripts/projects/Photomosaics/Photomosaics Source Images Squared";
            DirectoryInfo sourceDirectory = new DirectoryInfo(@sourceDirectoryPath);

            if (sourceDirectory.Exists)
            {
                FileInfo[] imageFiles = sourceDirectory.GetFiles();

                foreach (FileInfo file in imageFiles)
                {
                    if (file.Name[0] != '.') // Disregard hidden files
                    {
                        string fileName = file.Name;
                        string fullFilePath = sourceDirectoryPath + "/" + fileName;
                        Bitmap originalImage = new Bitmap(@fullFilePath);

                        int newHeight = sectionHeight;
                        int newWidth = sectionWidth;

                        Size size = new Size(newWidth, newHeight);

                        Bitmap image = (Bitmap)ResizeSquaredImage(originalImage, size);

                        originalImage.Dispose();

                        // Choose number of source image sections
                        int sourceImageSectionsCount = 4; // Valid numbers 1,4,9,16,25. Make sure the section width and height pixel count is divisable by square root of section count without remainder 

                        int sourceImageSectionHeight = image.Height / (int)Math.Sqrt(sourceImageSectionsCount);
                        int sourceImageSectionWidth = image.Width/ (int)Math.Sqrt(sourceImageSectionsCount); 

                        Tuple<int, int, int>[,] currentImageRGBPixelArray = ImageToPixelRGBValueArray(image);

                        Tuple<int, int, int>[,,,] currentImageSectionsRGBPixelArray = PixelArrayToSections(currentImageRGBPixelArray, sourceImageSectionHeight, sourceImageSectionWidth);

                        Tuple<int, int, int>[,] currentImageSectionAverageRGBComponentArray = SectionsToAverageColorValueRGB(currentImageSectionsRGBPixelArray);

                        Tuple<int, int, int> currentImageAverageColorRGB = GetAverageColorRGB(currentImageRGBPixelArray);

                        sourceImageModels.Add(new SourceImageModel { SourceImage = image, ImageName = fileName, FilePath = fullFilePath, ImageRGBPixelArray = currentImageRGBPixelArray, ImageSectionAverageRGBComponentArray = currentImageSectionAverageRGBComponentArray, ImageSectionsRGBPixelArray = currentImageSectionsRGBPixelArray, AverageImageColorRGB = currentImageAverageColorRGB  });

                        image.Dispose();
                    }
                }
            }

            return sourceImageModels;
        }
    }
}
