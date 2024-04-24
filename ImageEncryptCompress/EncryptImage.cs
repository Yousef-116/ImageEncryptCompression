using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEncryptCompress
{
    internal class EncryptImage
    {
        public static RGBPixel[,] Encrypt(RGBPixel[,] ImageMatrix, String seed , int  Tap_position )
        {
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            RGBPixelD Item1;
            RGBPixel Item2;

            generateNew8Bits(seed , Tap_position);
            for (int j = 0; j < Width; j++)
            {
                for (int i = 0; i < Height; i++)
                {

                    Item2 = ImageMatrix[i, j];

                    Item1.red = 5 * ImageMatrix[i,j].red;
                    
                    //generateNew8Bits(seed , Tap_position);
                    
                    ImageMatrix[i, j].green = 200;
                    
                    //generateNew8Bits(seed , Tap_position);
                    
                    ImageMatrix[i, j].blue = 60;
                }
            }

            return ImageMatrix;
        }

        public static void generateNew8Bits(String seed , int Tap_position )
        {
            int tapbit ,lastbit ,newbit;
            //int colorrepresitation = 8;
            Console.WriteLine("seed :" + seed);
            for (int i = 0; i < 8; i++)
            {

                tapbit = seed[seed.Length - Tap_position -1  ]-'0';
                lastbit = seed[0] - '0';

                newbit = tapbit ^ lastbit;

                Console.WriteLine("tap : " + tapbit + " last bit: " + lastbit + " newbit: " + newbit);


                seed = seed.Substring(1) + newbit.ToString();  



                Console.WriteLine("result "+i +"  = "+seed);
            }

            /**/

        }
    }
}
