using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageEncryptCompress
{
    internal class Decompressoin
    {
        // Huffman Tree each color
        public static Dictionary<short, Tuple<short, short>> RedHuffmanTree;
        public static Dictionary<short, Tuple<short, short>> GreenHuffmanTree;
        public static Dictionary<short, Tuple<short, short>> BlueHuffmanTree;

        public static StringBuilder redBinaryCode, greenBinaryCode, blueBinaryCode;
        public static int ImageHeight, ImageWidth;
        public static short redHuffmanTreeRoot, greenHuffmanTreeRoot, blueHuffmanTreeRoot;

        public static StringBuilder seedString;
        public static ushort seedLength;
        public static short TapPosition;
        public static bool isEncrypted;

        public static RGBPixel[,] DecompressImage(string BinaryFilePath)
        {
            Initializer();

            redBinaryCode = new StringBuilder();
            greenBinaryCode = new StringBuilder();
            blueBinaryCode = new StringBuilder();

            BinaryFileOperations.ReedFromBinaryFile(BinaryFilePath);

            RGBPixel[,] ImageMatrix = new RGBPixel[ImageHeight, ImageWidth];
            //Console.WriteLine(ImageHeight + " " + ImageWidth);

            DecodeBinaryCode_V1(ImageMatrix);
            //MessageBox.Show("Done");

            return ImageMatrix;
        }

        private static void Initializer()
        {
            // Huffman Tree each color
            RedHuffmanTree = new Dictionary<short, Tuple<short, short>>();
            GreenHuffmanTree = new Dictionary<short, Tuple<short, short>>();
            BlueHuffmanTree = new Dictionary<short, Tuple<short, short>>();

            redBinaryCode = null;
            greenBinaryCode = null;
            blueBinaryCode = null;
            ImageHeight = ImageWidth = 0;
            redHuffmanTreeRoot = greenHuffmanTreeRoot = blueHuffmanTreeRoot = 0;

            seedString = null;
            seedLength = 0;
            TapPosition = 0;
            isEncrypted = false;
        }
        private static void DecodeBinaryCode_V1(RGBPixel[,] ImageMatrix)
        {
            Task redTask = Task.Factory.StartNew(() =>
            {
                int cnt = 0, i, j;
                short currNode = redHuffmanTreeRoot;
                string binaryCode = redBinaryCode.ToString();
                foreach (char bit in binaryCode)
                {
                    if (bit == '0') // left
                    {
                        currNode = RedHuffmanTree[currNode].Item1;
                    }
                    else // right
                    {
                        currNode = RedHuffmanTree[currNode].Item2;
                    }

                    //Console.WriteLine("here1");
                    if (currNode < 256) // leaf node
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
            });

            Task greenTask = Task.Factory.StartNew(() =>
            {
                int cnt = 0, i, j;
                short currNode = greenHuffmanTreeRoot;
                string binaryCode = greenBinaryCode.ToString();
                foreach (char bit in binaryCode)
                {
                    if (bit == '0') // left
                    {
                        currNode = GreenHuffmanTree[currNode].Item1;
                    }
                    else // right
                    {
                        currNode = GreenHuffmanTree[currNode].Item2;
                    }

                    if (currNode < 256) // leaf node
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
            });

            Task blueTask = Task.Factory.StartNew(() =>
            {
                int cnt = 0, i, j;
                short currNode = blueHuffmanTreeRoot;
                string binaryCode = blueBinaryCode.ToString();
                foreach (char bit in binaryCode)
                {
                    if (bit == '0') // left
                    {
                        currNode = BlueHuffmanTree[currNode].Item1;
                    }
                    else // right
                    {
                        currNode = BlueHuffmanTree[currNode].Item2;
                    }

                    if (currNode < 256) // leaf node
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
            });

            Task.WaitAll(redTask, greenTask, blueTask);
        }
    }
}
