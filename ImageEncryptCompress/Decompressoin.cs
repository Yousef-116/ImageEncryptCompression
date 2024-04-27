using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEncryptCompress
{
    internal class Decompressoin
    {
        
        // Huffman Tree each color
        public static Dictionary<short, Tuple<short, short>> RedHuffmanTree = new Dictionary<short, Tuple<short, short>>();
        public static Dictionary<short, Tuple<short, short>> GreenHuffmanTree = new Dictionary<short, Tuple<short, short>>();
        public static Dictionary<short, Tuple<short, short>> BlueHuffmanTree = new Dictionary<short, Tuple<short, short>>();

        public static StringBuilder redBinaryCode, greenBinaryCode, blueBinaryCode;
        public static int ImageHeight = 0, ImageWidth = 0;
        public static short redHuffmanTreeRoot = 0, greenHuffmanTreeRoot = 0, blueHuffmanTreeRoot = 0;

        public static StringBuilder seedString;
        public static short TapPosition;
        public static bool isEncrypted;

        public static RGBPixel[,] DecompressImage(string BinaryFilePath)
        {

            redBinaryCode = new StringBuilder();
            greenBinaryCode = new StringBuilder();
            blueBinaryCode = new StringBuilder();

            BinaryFileOperations.ReedFromBinaryFile(BinaryFilePath);

            RGBPixel[,] ImageMatrix = new RGBPixel[ImageHeight, ImageWidth];
            //Console.WriteLine(ImageHeight + " " + ImageWidth);



            //Console.WriteLine("RedHuffmanTree");
            //Console.WriteLine($"redHuffmanTreeRoot:{redHuffmanTreeRoot}");
            //foreach (var node in RedHuffmanTree)
            //    Console.WriteLine($"{node.Key}: {node.Value.Item1}, {node.Value.Item2}");

            //Console.WriteLine("GreenHuffmanTree");
            //Console.WriteLine($"greenHuffmanTreeRoot:{greenHuffmanTreeRoot}");
            //foreach (var node in GreenHuffmanTree)
            //    Console.WriteLine($"{node.Key}: {node.Value.Item1}, {node.Value.Item2}");

            //Console.WriteLine("BlueHuffmanTree");
            //Console.WriteLine($"blueHuffmanTreeRoot:{blueHuffmanTreeRoot}");
            //foreach (var node in BlueHuffmanTree)
            //    Console.WriteLine($"{node.Key}: {node.Value.Item1}, {node.Value.Item2}");


            DecodeBinaryCode(ImageMatrix);

            return ImageMatrix;
        }

        private static void DecodeBinaryCode(RGBPixel[,] ImageMatrix)
        {
            char bit;
            short currNode = redHuffmanTreeRoot;
            int cnt = 0, i, j;

            //Console.WriteLine("\nred Binary Code length:" + redBinaryCode.Length);
            for (int k = 0; k < redBinaryCode.Length; k++)
            {
                bit = redBinaryCode[k];
                if (bit == '0') // left
                {
                    currNode = RedHuffmanTree[currNode].Item1;
                }
                else // right
                {
                    currNode = RedHuffmanTree[currNode].Item2;
                }

                //Console.WriteLine("here1");
                if (!RedHuffmanTree.ContainsKey(currNode)) // leaf node
                {
                    //Console.WriteLine("here3");
                    i = cnt / ImageWidth;
                    if (i >= ImageHeight)
                    {
                        break;
                    }
                    j = cnt % ImageWidth;
                    //Console.WriteLine(i + " " + j);
                    ImageMatrix[i, j].red = (byte)currNode;
                    //Console.Write(currNode + " ");
                    //Console.WriteLine("here4");

                    currNode = redHuffmanTreeRoot;
                    cnt++;
                }
            }

            cnt = 0;
            currNode = greenHuffmanTreeRoot;
            for (int k = 0; k < greenBinaryCode.Length; k++)
            {
                bit = greenBinaryCode[k];
                if (bit == '0') // left
                {
                    currNode = GreenHuffmanTree[currNode].Item1;
                }
                else // right
                {
                    currNode = GreenHuffmanTree[currNode].Item2;
                }

                if (!GreenHuffmanTree.ContainsKey(currNode)) // leaf node
                {
                    i = cnt / ImageWidth;
                    if (i >= ImageHeight)
                    {
                        break;
                    }
                    j = cnt % ImageWidth;
                    ImageMatrix[i, j].green = (byte)currNode;

                    currNode = greenHuffmanTreeRoot;
                    cnt++;
                }
            }

            cnt = 0;
            currNode = blueHuffmanTreeRoot;
            for (int k = 0; k < blueBinaryCode.Length; k++)
            {
                bit = blueBinaryCode[k];
                if (bit == '0') // left
                {
                    currNode = BlueHuffmanTree[currNode].Item1;
                }
                else // right
                {
                    currNode = BlueHuffmanTree[currNode].Item2;
                }

                if (!BlueHuffmanTree.ContainsKey(currNode)) // leaf node
                {
                    i = cnt / ImageWidth;
                    if (i >= ImageHeight)
                    {
                        break;
                    }
                    j = cnt % ImageWidth;
                    ImageMatrix[i, j].blue = (byte)currNode;

                    currNode = blueHuffmanTreeRoot;
                    cnt++;
                }
            }

        }
    }
}
