using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Photomosaics
{
    public static class ImageManipulation
    {
        public static Image ResizeImage(Image imgToResize, Size size)
        {
            int destWidth = size.Width;
            int destHeight = size.Height;

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage(b);

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Draw image with new width and height  
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return b;
        }

        public static Image ResizeSquaredImage(Image imgToResize, Size size)
        {
            int destHeight = size.Height;
            int destWidth = size.Width;

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage(b);

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Draw image with new width and height  
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return b;
        }

        public static Tuple<int,int,int>[,] ImageToPixelRGBValueArray (Bitmap image)
        {
            Tuple<int, int, int>[,] rgbPixelArray = new Tuple<int, int, int>[image.Height, image.Width];

            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    Color currentPixel = image.GetPixel(j, i);

                    int colorRed = currentPixel.R;
                    int colorGreen = currentPixel.G;
                    int colorBlue = currentPixel.B;

                    rgbPixelArray[i, j] = new Tuple<int, int, int>(colorRed, colorGreen, colorBlue);
                }
            }

            return rgbPixelArray;
        }

        public static Tuple<int,int,int>[,,,] PixelArrayToSections (Tuple<int,int,int>[,] pixelArray, int sectionHeight, int sectionWidth)
        {
            int horizontalSectionCount = pixelArray.GetLength(1)/sectionHeight;
            int verticalSectionCount = pixelArray.GetLength(0)/ sectionWidth;

            Tuple<int, int, int>[,,,] imageSections = new Tuple<int, int, int>[verticalSectionCount, horizontalSectionCount, sectionHeight, sectionWidth];

            for (int currentVerticalSection = 0; currentVerticalSection < verticalSectionCount;)
            {
                for (int currentHorizontalSection = 0; currentHorizontalSection < horizontalSectionCount;)
                {
                    for (int currentVerticalSectionPixel = 0; currentVerticalSectionPixel < sectionHeight;)
                    {
                        for (int currentHorizontalSectionPixel = 0; currentHorizontalSectionPixel < sectionWidth;)
                        {

                            int currentVerticalImagePixel = currentVerticalSection * sectionHeight + currentVerticalSectionPixel;
                            int currentHorizontalImagePixel = currentHorizontalSection * sectionWidth + currentHorizontalSectionPixel;

                            imageSections[currentVerticalSection, currentHorizontalSection, currentVerticalSectionPixel, currentHorizontalSectionPixel] = pixelArray[currentVerticalImagePixel,currentHorizontalImagePixel];

                            currentHorizontalSectionPixel++;
                        }

                        currentVerticalSectionPixel++;
                    }

                    currentHorizontalSection++;
                }

                currentVerticalSection++;
            }

            return imageSections;
        }

        public static Tuple<int, int, int>[,] SectionsToAverageColorValueRGB(Tuple<int, int, int>[,,,] sectionsArray)
        {
            int arrayHeight = sectionsArray.GetLength(0);
            int arrayWidth = sectionsArray.GetLength(1);

            int verticalSectionCount = sectionsArray.GetLength(0);
            int horizontalSectionCount = sectionsArray.GetLength(1);
            int sectionHeight = sectionsArray.GetLength(2);
            int sectionWidth = sectionsArray.GetLength(3);

            Tuple<int, int, int>[,] pixelArray = new Tuple<int, int, int>[arrayHeight,arrayWidth];

            for (int currentVerticalSection = 0; currentVerticalSection < verticalSectionCount;)
            {
                for (int currentHorizontalSection = 0; currentHorizontalSection < horizontalSectionCount;)
                {
                    Tuple<int, int, int>[,] currentSection = new Tuple<int, int, int>[sectionHeight, sectionWidth];

                    for (int currentVerticalSectionPixel = 0; currentVerticalSectionPixel < sectionHeight;)
                    {
                        for (int currentHorizontalSectionPixel = 0; currentHorizontalSectionPixel < sectionWidth;)
                        {
                            currentSection[currentVerticalSectionPixel, currentHorizontalSectionPixel] = sectionsArray[currentVerticalSection,currentHorizontalSection,currentVerticalSectionPixel,currentHorizontalSectionPixel];

                            currentHorizontalSectionPixel++;
                        }

                        currentVerticalSectionPixel++;
                    }

                    pixelArray[currentVerticalSection, currentHorizontalSection] = GetAverageColorRGB(currentSection);


                    currentHorizontalSection++;
                }

                currentVerticalSection++;
            }

            return pixelArray;
        }

        public static Tuple<int,int,int> GetAverageColorRGB(Tuple<int,int,int>[,] pixelArray)
        {
            int arrayHeight = pixelArray.GetLength(0);
            int arrayWidth = pixelArray.GetLength(1);

            int sumRed = 0;
            int sumGreen = 0;
            int sumBlue = 0;

            for (int i = 0; i < arrayHeight;)
            {
                for (int j = 0; j < arrayWidth;)
                {
                    sumRed += pixelArray[i, j].Item1;
                    sumGreen += pixelArray[i, j].Item2;
                    sumBlue += pixelArray[i, j].Item3;
                    j++;
                }

                i++;
            }

            int averageRed = sumRed / (arrayHeight * arrayWidth);
            int averageGreen = sumGreen / (arrayHeight * arrayWidth);
            int averageBlue = sumBlue / (arrayHeight * arrayWidth);

            Tuple<int, int, int> averageColors = new Tuple<int, int, int>(averageRed, averageGreen, averageBlue);

            return averageColors;
        }

        public static void SaveToOutputImageFromRGBPixelArray(Tuple<int,int,int>[,] pixelArray, string fileName)
        {
            // Turn pixel array to byte array

            int arrayHeight = pixelArray.GetLength(0);
            int arrayWidth = pixelArray.GetLength(1);

            int imageHeight = arrayHeight;
            int imageWidth = arrayWidth;

            //byte[] redBytes = new byte[arrayHeight * arrayWidth];
            //byte[] greenBytes = new byte[arrayHeight * arrayWidth];
            //byte[] blueBytes = new byte[arrayHeight * arrayWidth];

            byte[] pixelBytes = new byte[3 * arrayHeight * arrayWidth];

            int currentByte = 0;

            for (int i = 0; i < arrayHeight;)
            {
                for (int j = 0; j < arrayWidth;)
                {
                    //redBytes[currentByte] = (byte)pixelArray[i, j].Item1;
                    //currentByte++;
                    //j++;

                    //greenBytes[currentByte] = (byte)pixelArray[i, j].Item2;
                    //currentByte++;
                    //j++;

                    //blueBytes[currentByte] = (byte)pixelArray[i, j].Item3;
                    //currentByte++;
                    //j++;

                    pixelBytes[currentByte] = (byte)pixelArray[i, j].Item1;
                    currentByte++;

                    pixelBytes[currentByte] = (byte)pixelArray[i, j].Item2;
                    currentByte++;

                    pixelBytes[currentByte] = (byte)pixelArray[i, j].Item3;
                    currentByte++;
                    j++;
                }

                i++;
            }

            // Create empty Bitmap object

            Bitmap bitmapRGB = new Bitmap(imageWidth, imageHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // Create rectangle and lock bitmap into system memory

            Rectangle rectangle = new Rectangle(0, 0, imageWidth, imageHeight);
            BitmapData bitmapData = bitmapRGB.LockBits(rectangle, ImageLockMode.WriteOnly, bitmapRGB.PixelFormat);

            // Calculate padding

            int padding = bitmapData.Stride - 3 * imageWidth;

            // Assign adress of first pixel data to a pointer

            currentByte = 0;

            unsafe
            {
                byte* ptr = (byte*)bitmapData.Scan0;

                for (int i = 0; i < imageHeight;)
                {
                    for (int j = 0; j < imageWidth;)
                    {
                        ptr[2] = pixelBytes[currentByte];
                        currentByte++;

                        ptr[1] = pixelBytes[currentByte];
                        currentByte++;

                        ptr[0] = pixelBytes[currentByte];
                        currentByte++;

                        // Move pointer to next pixel
                        ptr += 3;
                        j++;
                    }

                    // Add padding at the end of each row
                    ptr += padding;
                    i++;
                }
            }

            // Unlock bits

            bitmapRGB.UnlockBits(bitmapData);

            // Save image to file

            string filePath = "/Users/Kr1sh/Desktop/unknown/programming scripts/projects/Photomosaics/output_folder/" + fileName;

            bitmapRGB.Save(@filePath, ImageFormat.Jpeg);

        }

        public static string[,] CompareAverageColorsSectionAndSourceImages(List<ISourceImageModel> sourceImageModels, Tuple<int,int,int>[,] inputImageSectionAverageRGBValueArray)
        {
            int verticalSectionCount = inputImageSectionAverageRGBValueArray.GetLength(0);
            int horizontalSectionCount = inputImageSectionAverageRGBValueArray.GetLength(1);

            string[,] sourceImagePaths = new string[verticalSectionCount,horizontalSectionCount];

            for (int i = 0; i < verticalSectionCount;)
            {
                for (int j = 0; j < horizontalSectionCount;)
                {
                    Tuple<int, int, int> currentSectionAverageRGBValues = inputImageSectionAverageRGBValueArray[i, j];

                    double minimumColorDifference = 195075; // Largest possible color difference

                    foreach(ISourceImageModel sourceImage in sourceImageModels)
                    {
                        Tuple<int, int, int> sourceImageAverageRGBValues = sourceImage.AverageImageColorRGB;

                        int diffRedComponent = -currentSectionAverageRGBValues.Item1 + sourceImageAverageRGBValues.Item1;
                        int diffGreenComponent = -currentSectionAverageRGBValues.Item2 + sourceImageAverageRGBValues.Item2;
                        int diffBlueComponent = -currentSectionAverageRGBValues.Item3 + sourceImageAverageRGBValues.Item3;

                        int colorDifference = diffRedComponent * diffRedComponent + diffGreenComponent* diffGreenComponent + diffBlueComponent* diffBlueComponent;

                        if(colorDifference < minimumColorDifference)
                        {
                            sourceImagePaths[i, j] = sourceImage.FilePath;
                            minimumColorDifference = colorDifference;
                        }
                    }

                    j++;
                }

                i++;
            }

            return sourceImagePaths;
        }

        public static Tuple<int,int,int>[,] OutputImagePixelArrayFromSourceImages(string[,] sourceImagePaths, Tuple<int,int,int>[,] inputImagePixelArray, Tuple<int, int, int>[,,,] inputImageSections, List<ISourceImageModel> sourceImageModels)
        {
            int outputImageHeight = inputImagePixelArray.GetLength(0);
            int outputImageWidth = inputImagePixelArray.GetLength(1);

            Tuple<int, int, int>[,] outputImagePixelArray = new Tuple<int, int, int>[outputImageHeight,outputImageWidth];

            int verticalSectionCount = inputImageSections.GetLength(0);
            int horizontallSectionCount = inputImageSections.GetLength(1);
            int sectionHeight = inputImageSections.GetLength(2);
            int sectionWidth = inputImageSections.GetLength(3);

            for (int currentVerticalSection = 0; currentVerticalSection< verticalSectionCount;)
            {
                for (int currentHorizontalSection = 0; currentHorizontalSection < horizontallSectionCount;)
                {
                    // Load the correct image for section

                    Tuple<int, int, int>[,] sourceImagePixelArray = new Tuple<int, int, int>[sectionHeight,sectionHeight];

                    string currentSourceImageFilePath = sourceImagePaths[currentVerticalSection, currentHorizontalSection];
                    foreach (ISourceImageModel sourceImageModel in sourceImageModels)
                    {
                        if(sourceImageModel.FilePath == currentSourceImageFilePath)
                        {
                           sourceImagePixelArray = sourceImageModel.ImageRGBPixelArray;
                            break;
                        }
                    }

                    for (int sectionVerticalPosition = 0; sectionVerticalPosition < sectionHeight;)
                    {
                        for (int sectionHorizontalPosition = 0; sectionHorizontalPosition < sectionWidth;)
                        {
                            outputImagePixelArray[currentVerticalSection * sectionHeight + sectionVerticalPosition, currentHorizontalSection * sectionWidth + sectionHorizontalPosition] = sourceImagePixelArray[sectionVerticalPosition,sectionHorizontalPosition];
                            sectionHorizontalPosition++;
                        }

                        sectionVerticalPosition++;
                    }

                    currentHorizontalSection++;
                }

                currentVerticalSection++;
            }

            return outputImagePixelArray;
        }

        public static void CreateOutputImageUsingDrawing()
        {
            throw new NotImplementedException();
        }
    }
}
