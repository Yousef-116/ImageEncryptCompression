using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageEncryptCompress
{
    internal class EncryptImage
    {
        public static RGBPixel[,] Encrypt(RGBPixel[,] ImageMatrix, String seed, int Tap_position)
        {
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            RGBPixelD Item1D;
            RGBPixel Item2;
            int key;
            RGBPixel[,] EncryptedImageMatrix = new RGBPixel[Height, Width];
            string startseed = seed;
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

                    Item1D.red = key ^ Item2.red;

                    key = generateNew8Bits(ref seed, Tap_position);

                    Item1D.green = key ^ Item2.green;

                    key = generateNew8Bits(ref seed, Tap_position);

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

            //Dictionary<byte, int> redFrequencyCounts = new Dictionary<byte, int>();
            //Dictionary<byte, int> greenFrequencyCounts = new Dictionary<byte, int>();
            //Dictionary<byte, int> blueFrequencyCounts = new Dictionary<byte, int>();
            //for (int i = 0; i < Height; i++)
            //{
            //    for (int j = 0; j < Width; j++)
            //    {
            //        Item = EncryptedImageMatrix[i, j];

            //        if (!redFrequencyCounts.ContainsKey(Item.red))
            //            redFrequencyCounts[Item.red] = 0;
            //        redFrequencyCounts[Item.red]++;


            //        if (!greenFrequencyCounts.ContainsKey(Item.green))
            //            greenFrequencyCounts[Item.green] = 0;
            //        greenFrequencyCounts[Item.green]++;

            //        if (!blueFrequencyCounts.ContainsKey(Item.blue))
            //            blueFrequencyCounts[Item.blue] = 0;
            //        blueFrequencyCounts[Item.blue]++;


            //    }
            //}

            //Console.WriteLine("red Frequency Count for seed : " + startseed + " tap : " + Tap_position + " num of colors : " + redFrequencyCounts.Count);

            //foreach (var reditem in redFrequencyCounts)
            //{
            //    Console.WriteLine("Reddegree : " + reditem.Key + " count : " + reditem.Value);
            //}

            //Console.WriteLine("green Frequency Count for seed : " + startseed + " tap : " + Tap_position + " num of colors : " + redFrequencyCounts.Count);

            //foreach (var greenitem in greenFrequencyCounts)
            //{
            //    Console.WriteLine("Reddegree : " + greenitem.Key + " count : " + greenitem.Value);
            //}

            //Console.WriteLine("blue Frequency Count for seed : " + startseed + " tap : " + Tap_position + " num of colors : " + redFrequencyCounts.Count);

            //foreach (var blueitem in blueFrequencyCounts)
            //{
            //    Console.WriteLine("Reddegree : " + blueitem.Key + " count : " + blueitem.Value);
            //}

            return EncryptedImageMatrix;
        }

        public static Byte generateNew8Bits(ref String seed, int Tap_position)
        {
            int tapbit, lastbit, newbit;
            String keybits = "";

            //Console.WriteLine("seed :" + seed);
            for (int i = 0; i < 8; i++)
            {

                tapbit = seed[seed.Length - Tap_position - 1] - '0';
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

        public static RGBPixel[,] breakEncrypt(RGBPixel[,] ImageMatrix, int SeedLength)
        {
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);
            int maxDeviation = 0;
            int optimalTapPosition = 0 ;
            String optimalSeed = "";


            RGBPixel Item;
            Console.WriteLine("Break Encrypt function :");
            for (int seed = 0; seed < Math.Pow(2, SeedLength); seed++)
            {
                for (int tapPosition = 0; tapPosition < SeedLength; tapPosition++)
                {
                    string seedBinary = Convert.ToString(seed, 2).PadLeft(SeedLength, '0');
                    //Console.WriteLine("TapPosition : " + tapPosition + " | Seed : " + binaryRepresentation);

                    RGBPixel[,] breakEncryptImageMatrix = Encrypt(ImageMatrix, seedBinary, tapPosition);

                    Dictionary<byte, int> redFrequencyCounts = new Dictionary<byte, int>();
                    Dictionary<byte, int> greenFrequencyCounts = new Dictionary<byte, int>();
                    Dictionary<byte, int> blueFrequencyCounts = new Dictionary<byte, int>();
                    for (int i = 0; i < Height; i++)
                    {
                        for (int j = 0; j < Width; j++)
                        {
                            Item = breakEncryptImageMatrix[i, j];

                            if (!redFrequencyCounts.ContainsKey(Item.red))
                                redFrequencyCounts[Item.red] = 0;
                            redFrequencyCounts[Item.red]++;


                            //if (!greenFrequencyCounts.ContainsKey(Item.green))
                            //    greenFrequencyCounts[Item.green] = 0;
                            //greenFrequencyCounts[Item.green]++;

                            //if (!blueFrequencyCounts.ContainsKey(Item.blue))
                            //    blueFrequencyCounts[Item.blue] = 0;
                            //blueFrequencyCounts[Item.blue]++;


                        }
                    }


                    //Console.WriteLine("red Frequency Count for seed : " + seedBinary + " tap : " + tapPosition + " num of colors : " + redFrequencyCounts.Count);

                    //foreach (var reditem in redFrequencyCounts)
                    //{
                    //    Console.WriteLine("Reddegree : " + reditem.Key + " count : " + reditem.Value);

                    //}

                    int deviation = 0;
                    foreach (var reditem in redFrequencyCounts)
                    {
                        int value = reditem.Key * reditem.Value;
                        int count128 = 128 * reditem.Value;
                        deviation += Math.Abs(value - count128);
                    }

                    if (deviation > maxDeviation)
                    {
                        maxDeviation = deviation;
                        optimalTapPosition = tapPosition;
                        optimalSeed = seedBinary;
                    }




                }
            }

            Console.WriteLine("predicate Seed : " + optimalSeed+ " predicate Tap Position " + optimalTapPosition);
            return Encrypt(ImageMatrix, optimalSeed, optimalTapPosition);

        }
    }
}
