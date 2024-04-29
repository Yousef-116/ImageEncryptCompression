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
        public static Dictionary<short, Tuple<short, short>> RedHuffmanTree = new Dictionary<short, Tuple<short, short>>();
        public static Dictionary<short, Tuple<short, short>> GreenHuffmanTree = new Dictionary<short, Tuple<short, short>>();
        public static Dictionary<short, Tuple<short, short>> BlueHuffmanTree = new Dictionary<short, Tuple<short, short>>();

        private static Dictionary<string, short> redBinaryToHexa = new Dictionary<string, short>();
        private static Dictionary<string, short> greenBinaryToHexa = new Dictionary<string, short>();
        private static Dictionary<string, short> blueBinaryToHexa = new Dictionary<string, short>();

        public static StringBuilder redBinaryCode, greenBinaryCode, blueBinaryCode;
        public static int ImageHeight = 0, ImageWidth = 0;
        public static short redHuffmanTreeRoot = 0, greenHuffmanTreeRoot = 0, blueHuffmanTreeRoot = 0;

        public static StringBuilder seedString;
        public static ushort seedLength;
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

            //DFS(redHuffmanTreeRoot, new StringBuilder(), RedHuffmanTree, redBinaryToHexa);
            //DFS(greenHuffmanTreeRoot, new StringBuilder(), GreenHuffmanTree, greenBinaryToHexa);
            //DFS(blueHuffmanTreeRoot, new StringBuilder(), BlueHuffmanTree, blueBinaryToHexa);
            //DecodeBinaryCode_V2(ImageMatrix);

            DecodeBinaryCode_V1(ImageMatrix);
            MessageBox.Show("Done");

            return ImageMatrix;
        }
        private static void DecodeBinaryCode_V1(RGBPixel[,] ImageMatrix)
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
            Console.WriteLine("here");
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
            Console.WriteLine("here");

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
            Console.WriteLine("here");

        }

        private static void DecodeBinaryCode_V2(RGBPixel[,] ImageMatrix)
        {
            char bit;
            int cnt = 0, i, j;
            short Hexa;
            StringBuilder binaryCode = new StringBuilder();


            for (int k = 0; k < redBinaryCode.Length; k++)
            {
                bit = redBinaryCode[k];
                binaryCode.Append(bit);
                //Console.Write($"{bit}");
                if (redBinaryToHexa.TryGetValue(binaryCode.ToString(), out Hexa))
                {
                    //Console.WriteLine($"{binaryCode.ToString()} -> {Hexa}");
                    i = cnt / ImageWidth;
                    if (i >= ImageHeight)
                    {
                        break;
                    }
                    j = cnt % ImageWidth;
                    //Console.WriteLine(i + " " + j);
                    ImageMatrix[i, j].red = (byte)Hexa;
                    binaryCode = new StringBuilder();
                    cnt++;
                }
            }

            cnt = 0;
            binaryCode = new StringBuilder();
            for (int k = 0; k < greenBinaryCode.Length; k++)
            {
                bit = greenBinaryCode[k];
                binaryCode.Append(bit);
                if (greenBinaryToHexa.TryGetValue(binaryCode.ToString(), out Hexa))
                {
                    i = cnt / ImageWidth;
                    if (i >= ImageHeight)
                    {
                        break;
                    }
                    j = cnt % ImageWidth;
                    //Console.WriteLine(i + " " + j);
                    ImageMatrix[i, j].green = (byte)Hexa;
                    binaryCode = new StringBuilder();
                    cnt++;
                }
            }

            cnt = 0;
            binaryCode = new StringBuilder();
            for (int k = 0; k < blueBinaryCode.Length; k++)
            {
                bit = blueBinaryCode[k];
                binaryCode.Append(bit);
                if (blueBinaryToHexa.TryGetValue(binaryCode.ToString(), out Hexa))
                {
                    i = cnt / ImageWidth;
                    if (i >= ImageHeight)
                    {
                        break;
                    }
                    j = cnt % ImageWidth;
                    //Console.WriteLine(i + " " + j);
                    ImageMatrix[i, j].blue = (byte)Hexa;
                    binaryCode = new StringBuilder();
                    cnt++;
                }
            }
        }
        private static void DFS(short root, StringBuilder BinaryCode, Dictionary<short, Tuple<short, short>> HuffmanTree, Dictionary<string, short> BinaryToHexa)
        {
            Tuple<short, short> adj = null;
            if (!HuffmanTree.TryGetValue(root, out adj)) // leaf
            {
                //Console.WriteLine(root + ":" + BinaryCode.ToString());
                BinaryToHexa.Add(BinaryCode.ToString(), root);
                return;
            }
            //Console.WriteLine("here1");
            BinaryCode.Append('0');
            DFS(adj.Item1, BinaryCode, HuffmanTree, BinaryToHexa);

            //Console.WriteLine("here2");
            BinaryCode[BinaryCode.Length - 1] = '1';
            DFS(adj.Item2, BinaryCode, HuffmanTree, BinaryToHexa);

            BinaryCode.Remove(BinaryCode.Length - 1, 1);
        }
    }
}
