using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEncryptCompress
{
    internal class EncryptImage
    {
        public static RGBPixel[,] Encrypt(RGBPixel[,] ImageMatrix/*, int filterSize, double sigma*/)
        {
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            RGBPixelD Item1;
            RGBPixel Item2;

            for (int j = 0; j < Width; j++)
            {
                for (int i = 0; i < Height; i++)
                {
                    //Console.WriteLine(ImageMatrix[i, j].red + "," + ImageMatrix[i, j].green + "," + ImageMatrix[i, j].blue);
                    Item2 = ImageMatrix[i, j];
                    
                    Item1.red = 5 * ImageMatrix[i,j].red ;
                    ImageMatrix[i, j].blue = 60;
                    ImageMatrix[i, j].green = 200;


                }
            }

            return ImageMatrix;
        }


    }
}
