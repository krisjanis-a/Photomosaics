using System;
using System.Linq;
using System.IO;
using System.Drawing;
using Microsoft.Win32.SafeHandles;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using static Photomosaics.ImageManipulation;
using static Photomosaics.CreateSourceImageList;

namespace Photomosaics
{
    public static class MainClass
    {
        public static void Main(string[] args)
        {
            // Load input image
            
            Bitmap originalImage = new Bitmap(@"/Users/Kr1sh/Desktop/unknown/programming scripts/projects/Photomosaics/technoire_2.jpg");

            // Size - METHOD 1 - manually insert

            int desiredWidth = 2400; // in pixels and multiple of desired section width

            int desiredHeight = 3500; // in pixels and multiple of desired section height

            Size size = new Size(desiredWidth, desiredHeight);

            // Size - METHOD 2 - insert desired width, saves aspect ratio

            //Size size = GetSize(originalImage, desiredWidth);

            // Size - METHOD 3 - does not change dimensions

            //Size size = new Size(originalImage.Width, originalImage.Height);

            Bitmap inputImage = (Bitmap)ResizeImage(originalImage, size);

            originalImage.Dispose();

            Tuple<int, int, int>[,] inputImagePixelArray = ImageToPixelRGBValueArray(inputImage);

            // Output image name

            string fileName = "output_collage_21.jpeg";

            // Separate input image into square sections

            int sectionHeight = 25;
            int sectionWidth = 25;

            Tuple<int, int, int>[,,,] inputImageSections = PixelArrayToSections(inputImagePixelArray, sectionHeight, sectionWidth);

            // Calculate average color / RGB values of section and save to array

            Tuple<int, int, int>[,] inputImageSectionsAverageRGBValues = SectionsToAverageColorValueRGB(inputImageSections);

            //// Create pixelated version of original image

            //SaveToOutputImageFromRGBPixelArray(inputImageSectionsAverageRGBValues, fileName);

            // Create a list of source image models

            List<ISourceImageModel> sourceImageModels = CreateSourceImageModelList(sectionHeight,sectionWidth);

            // Compare average color / RGB values of each section to source images, find the best match

            string[,] sourceImagePaths = CompareAverageColorsSectionAndSourceImages(sourceImageModels, inputImageSectionsAverageRGBValues);

            // Create the collage and save to a new output image

            int outputMethod = 1; // 1 - from pixel array; 2 - using System.Drawing

            if (outputMethod == 1)
            {
                Tuple<int,int,int>[,] outputImagePixelArray = OutputImagePixelArrayFromSourceImages(sourceImagePaths, inputImagePixelArray, inputImageSections, sourceImageModels);

                SaveToOutputImageFromRGBPixelArray(outputImagePixelArray,fileName);
            }

            if (outputMethod == 2) // Not implemented
            {
                CreateOutputImageUsingDrawing();
            }
        }

        private static Size GetSize(Image originalImage, int desiredWidth)
        {
            decimal originalWidth = originalImage.Width;
            decimal originalHeight = originalImage.Height;

            decimal aspectRatio = originalWidth / originalHeight;

            decimal newWidth = desiredWidth;
            decimal newHeight = (decimal)(newWidth / aspectRatio);

            int newWidthInt = (int)Math.Round(newWidth);
            int newHeightInt = (int)Math.Round(newWidth / aspectRatio);

            Size newSize = new Size(newWidthInt,newHeightInt);

            return newSize;
        }
    }
}