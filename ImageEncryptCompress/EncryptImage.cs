using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageEncryptCompress
{
    internal class EncryptImage
    {
        public static string Seed;
        public static ushort TapPosition;

        public static RGBPixel[,] Encrypt(RGBPixel[,] ImageMatrix, int seedint, int SeedLength ,int Tap_position)
        {
            // Stopwatch sw = Stopwatch.StartNew();
            //Console.WriteLine("string int encrypt");
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);
            ushort TapPosition = (ushort)Tap_position;

            RGBPixelD Item1D;
            RGBPixel Item2;
            byte /*keyInt,*/keyString;
            int keyInt;
            //int keynum = 1;
            RGBPixel[,] EncryptedImageMatrix = new RGBPixel[Height, Width];
            


            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Item2 = ImageMatrix[i, j];
                    //keyInt = seed;
                    
                    //Console.WriteLine("Binary seed: " + seedString+ " intseed: "+seedint);
                    keyInt = generateNew8Bits(ref seedint, SeedLength, Tap_position);
                    //keyString = generateNew8Bits(ref seedString, Tap_position);
                    //Console.WriteLine(keynum+" = correct Key: " + keyString + " your Intkey: " + keyInt);
                    //keynum++;
                    Item1D.red = keyInt ^ Item2.red;


                    keyInt = generateNew8Bits(ref seedint, SeedLength, Tap_position);
                    //keyString = generateNew8Bits(ref seedString, Tap_position);
                    Item1D.green = keyInt ^ Item2.green;

                  //  keynum++;

                    keyInt = generateNew8Bits(ref seedint, SeedLength, Tap_position);
                    //keyString = generateNew8Bits(ref seedString, Tap_position);
                    Item1D.blue = keyInt ^ Item2.blue;

                   // keynum++;

                    EncryptedImageMatrix[i, j].red = (byte)Item1D.red;
                    EncryptedImageMatrix[i, j].green = (byte)Item1D.green;
                    EncryptedImageMatrix[i, j].blue = (byte)Item1D.blue;
                    //if (keynum > 20)
                    //{
                    //    break;
                    //}
                }
                //if(keynum>20)
                //    break;
            }

            //sw.Stop();
            // Console.WriteLine("time is ms : " + sw.ElapsedMilliseconds);

            return EncryptedImageMatrix;
        }


        public static int generateNew8Bits(ref int seed, int seedLength, int Tap_position)
        {
            int key = 0;

            for (int i = 0; i < 8; i++)
            {
                //int varfortap = ( seed >> TapPosition ) ;
                int tapBit = (seed >> (Tap_position)) & 1;

                int lastBit = (seed >> (seedLength - 1)) & 1;

                int newBit = tapBit ^ lastBit;
                key <<= 1;
                key |= newBit << 0;

                // Shift the seed to the right
                //seed = seed << 1;
                //seed |= newBit << (seedLength - 1);
                seed <<= 1;
                seed &= ~(1 << seedLength);
                seed |= newBit << 0;

            }

            return (byte)key;
        }
        






        public static RGBPixel[,] Encrypt(RGBPixel[,] ImageMatrix, String seed, int Tap_position)
        {
            Stopwatch sw = Stopwatch.StartNew();

            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);
            Seed = seed.ToString();
            TapPosition = (ushort)Tap_position;

            RGBPixelD Item1D;
            RGBPixel Item2;
            int key;
            RGBPixel[,] EncryptedImageMatrix = new RGBPixel[Height, Width];
            //StringBuilder seedBuilder= new StringBuilder();
            //seedBuilder.Append( seed );
            //generateNew8Bits(seed , Tap_position);

            //for (int i = 0; i < Width-1; i++)
            //{
            //    Console.WriteLine("index " + i + " red : " + ImageMatrix[i, 0].red + " Green : " + ImageMatrix[i, 0].green + " Blue : " + ImageMatrix[i, 0].blue);
            //}

            int keynum = 1;
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

            sw.Stop();

            Console.WriteLine("time is ms : " + sw.ElapsedMilliseconds);

            return EncryptedImageMatrix;
        }

        public static Byte generateNew8Bits(ref String seed, int Tap_position)
        {
            int tapbit, lastbit, newbit;
            StringBuilder keybits = new StringBuilder();

            //Console.WriteLine("seed :" + seed);
            for (int i = 0; i < 8; i++)
            {

                tapbit = seed[seed.Length - Tap_position - 1] - '0';
                lastbit = seed[0] - '0';

                newbit = tapbit ^ lastbit;

                // Console.WriteLine("tap : " + tapbit + " last bit: " + lastbit + " newbit: " + newbit);

                keybits.Append(newbit.ToString());

                seed = seed.Substring(1) + newbit.ToString();
                //seed.Remove(0, 1);
                //seed.Append(newbit.ToString());

                //   Console.WriteLine("result "+i +"  = "+seed);
            }

            Byte key = Convert.ToByte(keybits.ToString(), 2);
            //Console.WriteLine("key2 : " + key);



            return key;


        }

        public static RGBPixel[,] breakEncrypt(RGBPixel[,] ImageMatrix, int SeedLength)
        {

            Stopwatch sw = Stopwatch.StartNew();


            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);
            int maxDeviation = 0;
            int optimalTapPosition = 0 ;
            String optimalSeed = "";

            RGBPixel[,] TryImageMatrix = new RGBPixel[Height, Width];
            TryImageMatrix = ImageMatrix;
            RGBPixel Item;
            Console.WriteLine("Break Encrypt function :");
            for (int seed = 0; seed < Math.Pow(2, SeedLength); seed++)
            {
                for (int tapPosition = 0; tapPosition < SeedLength; tapPosition++)
                {
                    string seedBinary = Convert.ToString(seed, 2).PadLeft(SeedLength, '0');
                    //Console.WriteLine("TapPosition : " + tapPosition + " | Seed : " + binaryRepresentation);
                    int intSeed = Convert.ToInt32(seedBinary, 2);
                    RGBPixel[,] breakEncryptImageMatrix = Encrypt(TryImageMatrix, intSeed,SeedLength, tapPosition);

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

            sw.Stop();
            Console.WriteLine("break time is ms : " + sw.ElapsedMilliseconds);
            
            Console.WriteLine("predicate Seed : " + optimalSeed + " predicate Tap Position " + optimalTapPosition);
            int intseed = Convert.ToInt32(optimalSeed);

            Console.WriteLine("intseed : " +intseed+" seed.length : "+ optimalSeed.Length + " tap_position : " + optimalTapPosition);

            return Encrypt(ImageMatrix, intseed,optimalSeed.Length, optimalTapPosition);

        }

        public static List<byte> GetBinarySeed()
        {
            List<byte> seedList; seedList = new List<byte>() { 0 };
            int startIndex = 0;
            Compression.AddBits(seedList, Seed, ref startIndex);

            return seedList;
        }

    }
}
