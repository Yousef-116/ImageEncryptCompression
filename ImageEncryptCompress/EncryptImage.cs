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

        public static RGBPixel[,] Encrypt(RGBPixel[,] ImageMatrix, string seed, int SeedLength, int Tap_position)
        {
            Stopwatch sw = Stopwatch.StartNew();

            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            TapPosition = (ushort)Tap_position;
            Seed = seed;
            long integerSeed = Convert.ToInt64(seed, 2);

            RGBPixelD Item1D;
            RGBPixel Item2;
            int key;
            RGBPixel[,] EncryptedImageMatrix = new RGBPixel[Height, Width];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {

                    Item2 = ImageMatrix[i, j];

                    key = generateNew8Bits(ref integerSeed, SeedLength, Tap_position);

                    Item1D.red = key ^ Item2.red;

                    key = generateNew8Bits(ref integerSeed, SeedLength, Tap_position);

                    Item1D.green = key ^ Item2.green;

                    key = generateNew8Bits(ref integerSeed, SeedLength, Tap_position);

                    Item1D.blue = key ^ Item2.blue;

                    EncryptedImageMatrix[i, j].red = (byte)Item1D.red;
                    EncryptedImageMatrix[i, j].green = (byte)Item1D.green;
                    EncryptedImageMatrix[i, j].blue = (byte)Item1D.blue;
                }
            }


            sw.Stop();

            Console.WriteLine("time normal encrypt : " + sw.ElapsedMilliseconds);

            return EncryptedImageMatrix;
        }
       
        public static int generateNew8Bits(ref long seed, int seedLength, int Tap_position)
        {
            long key = 0;

            for (int i = 0; i < 8; i++)
            {
                //int varfortap = ( seed >> TapPosition ) ;
                long tapBit = (seed >> (Tap_position)) & 1;

                long lastBit = (seed >> (seedLength - 1)) & 1;

                long newBit = tapBit ^ lastBit;
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

        public static RGBPixel[,] breakEncrypt(RGBPixel[,] ImageMatrix, int SeedLength)
        {

            Stopwatch sw = Stopwatch.StartNew();


            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);
            int maxDeviation = 0;
            int optimalTapPosition = 0;
            String optimalSeed = "";
            //RGBPixel[,] TryImageMatrix = new RGBPixel[Height, Width];
            //TryImageMatrix = ImageMatrix;
            RGBPixel Item;
            Console.WriteLine("Break Encrypt function :");
            for (long seed = 0; seed < Math.Pow(2, SeedLength); seed++)
            {
                for (int tapPosition = 0; tapPosition < SeedLength; tapPosition++)
                {
                    bool valid = true;
                    string seedBinary = Convert.ToString(seed, 2).PadLeft(SeedLength, '0');
                    //Console.WriteLine("TapPosition : " + tapPosition + " | Seed : " + seedBinary);
                    long intSeed = Convert.ToInt64(seedBinary, 2);
                    //sw.Stop();
                    RGBPixel[,] breakEncryptImageMatrix = Encrypt_for_break(ImageMatrix, intSeed, SeedLength, tapPosition, ref valid);
                    //sw.Start();
                    if (valid)
                    {
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


                                if (!greenFrequencyCounts.ContainsKey(Item.green))
                                    greenFrequencyCounts[Item.green] = 0;
                                greenFrequencyCounts[Item.green]++;

                                if (!blueFrequencyCounts.ContainsKey(Item.blue))
                                    blueFrequencyCounts[Item.blue] = 0;
                                blueFrequencyCounts[Item.blue]++;


                            }
                        }

                        int deviation = 0;
                        foreach (var reditem in redFrequencyCounts)
                        {
                            int value = reditem.Key * reditem.Value;
                            int count128 = 128 * reditem.Value;
                            deviation += Math.Abs(value - count128);
                        }

                        foreach (var greenitem in greenFrequencyCounts)
                        {
                            int value = greenitem.Key * greenitem.Value;
                            int count128 = 128 * greenitem.Value;
                            deviation += Math.Abs(value - count128);
                        }

                        foreach (var blueitem in greenFrequencyCounts)
                        {
                            int value = blueitem.Key * blueitem.Value;
                            int count128 = 128 * blueitem.Value;
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
            }

            sw.Stop();
            Console.WriteLine("break time is ms : " + sw.ElapsedMilliseconds);

            Console.WriteLine("predicate Seed : " + optimalSeed + " predicate Tap Position " + optimalTapPosition);
            //int integerseed = Convert.ToInt32(optimalSeed, 2);

            //Console.WriteLine("intseed : " +integerseed+" seed.length : "+ optimalSeed.Length + " tap_position : " + optimalTapPosition);
            //.Init_seed.Text = "hh";
            Seed = optimalSeed;
            TapPosition = (ushort)optimalTapPosition;
            return Encrypt(ImageMatrix, optimalSeed, SeedLength, optimalTapPosition);

        }

        public static RGBPixel[,] Encrypt_for_break(RGBPixel[,] ImageMatrix, long integerSeed, int SeedLength, int Tap_position, ref bool valid)
        {
            //Stopwatch sw = Stopwatch.StartNew();

            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);
            TapPosition = (ushort)Tap_position;
            RGBPixelD Item1D;
            RGBPixel Item2;
            int key;
            RGBPixel[,] EncryptedImageMatrix = new RGBPixel[Height, Width];
            int count = 0;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Item2 = ImageMatrix[i, j];

                    key = generateNew8Bits(ref integerSeed, SeedLength, Tap_position);

                    Item1D.red = key ^ Item2.red;

                    key = generateNew8Bits(ref integerSeed, SeedLength, Tap_position);

                    //Item1D.green = key ^ Item2.green;

                    key = generateNew8Bits(ref integerSeed, SeedLength, Tap_position);

                    //Item1D.blue = key ^ Item2.blue;
                    if (ImageMatrix[i, j].red == (byte)Item1D.red)
                    {
                        count++;
                    }
                    else
                        count = -1;
                    EncryptedImageMatrix[i, j].red = (byte)Item1D.red;
                    //EncryptedImageMatrix[i, j].green = (byte)Item1D.green;
                    //EncryptedImageMatrix[i, j].blue = (byte)Item1D.blue;
                    if (count >= 30)
                    {
                        //Console.WriteLine("hear");
                        valid = false;
                        break;
                    }
                }
                if (!valid)
                    break;
            }
            //sw.Stop();
            //Console.WriteLine("time in break try: " + sw.ElapsedMilliseconds);
            return EncryptedImageMatrix;
        }

        public static List<byte> GetBinarySeed()
        {
            List<byte> seedList = new List<byte>() { 0 };
            int startIndex = 0;
            Compression.AddBits(seedList, Seed, ref startIndex);

            return seedList;
        }

    }
}