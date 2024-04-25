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

            RGBPixelD Item1D;
            RGBPixel Item2;
            int key;
            RGBPixel[,] EncryptedImageMatrix = new RGBPixel[Height, Width];

            //generateNew8Bits(seed , Tap_position);

            //for (int i = 0; i < Width-1; i++)
            //{
            //    Console.WriteLine("index " + i + " red : " + ImageMatrix[i, 0].red + " Green : " + ImageMatrix[i, 0].green + " Blue : " + ImageMatrix[i, 0].blue);
            //}


            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {

                    Item2 = ImageMatrix[i, j];
                    key = generateNew8Bits(ref seed, Tap_position);

                    Item1D.red = key ^ Item2.red ;
                    
                    key = generateNew8Bits(ref seed , Tap_position);

                    Item1D.green = key ^ Item2.green;

                    key = generateNew8Bits(ref seed , Tap_position);

                    Item1D.blue = key ^ Item2.blue;

                    EncryptedImageMatrix[i, j].red = (byte)Item1D.red;
                    EncryptedImageMatrix[i, j].green = (byte)Item1D.green;
                    EncryptedImageMatrix[i, j].blue = (byte)Item1D.blue;
                }
            }

            //for (int i = 0; i<Width; i++)
            //{
            //    Console.WriteLine("index "+i+" red : "+ImageMatrix[i, 0].red + " Green : " + ImageMatrix[i, 0].green + " Blue : "+ ImageMatrix[i, 0].blue); 
            //}


            return EncryptedImageMatrix;
        }

        public static Byte generateNew8Bits(ref String seed , int Tap_position )
        {
            int tapbit ,lastbit ,newbit;
            String keybits = "";

            //Console.WriteLine("seed :" + seed);
            for (int i = 0; i < 8; i++)
            {

                tapbit = seed[seed.Length - Tap_position -1  ]-'0';
                lastbit = seed[0] - '0';

                newbit = tapbit ^ lastbit;

               // Console.WriteLine("tap : " + tapbit + " last bit: " + lastbit + " newbit: " + newbit);

                keybits += newbit.ToString();

                seed = seed.Substring(1) + newbit.ToString();

             //   Console.WriteLine("result "+i +"  = "+seed);
            }

            Byte key = Convert.ToByte(keybits, 2);
           // Console.WriteLine("key = " + key);
            return key;


        }
    }
}
