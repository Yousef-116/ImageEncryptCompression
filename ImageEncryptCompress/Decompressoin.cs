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
        //public static Dictionary<short, Tuple<short, short>> RedHuffmanTree;
        //public static Dictionary<short, Tuple<short, short>> GreenHuffmanTree;
        //public static Dictionary<short, Tuple<short, short>> BlueHuffmanTree;

        public static StringBuilder redBinaryCode, greenBinaryCode, blueBinaryCode;
        public static int ImageHeight, ImageWidth;
        //public static short redHuffmanTreeRoot, greenHuffmanTreeRoot, blueHuffmanTreeRoot;
        public static Compression.HuffmanNode redHuffmanTreeRoot, greenHuffmanTreeRoot, blueHuffmanTreeRoot;

        public static StringBuilder seedString;
        public static ushort seedLength;
        public static short TapPosition;
        public static bool isEncrypted;

        public static RGBPixel[,] DecompressImage(string BinaryFilePath)
        {
            Initialize();

            BinaryFileOperations.ReedFromBinaryFile(BinaryFilePath);

            RGBPixel[,] ImageMatrix = new RGBPixel[ImageHeight, ImageWidth];
            //Console.WriteLine(ImageHeight + " " + ImageWidth);

            //DecodeBinaryCode_V1(ImageMatrix);
            DecodeBinaryCode_V2(ImageMatrix);
            //MessageBox.Show("Done");

            //Console.WriteLine("\n\nRed Tree");
            //BFS(redHuffmanTreeRoot);
            //Console.WriteLine("\n\nGreen Tree");
            //BFS(greenHuffmanTreeRoot);
            //Console.WriteLine("\n\nBlue Tree");
            //BFS(blueHuffmanTreeRoot);

            return ImageMatrix;
        }

        private static void BFS(Compression.HuffmanNode root)
        {
            Queue<Compression.HuffmanNode> queue = new Queue<Compression.HuffmanNode>();
            Compression.HuffmanNode tempNode;

            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                tempNode = queue.Dequeue();
                if (tempNode.Hexa == 333)
                {
                    queue.Enqueue(tempNode.left);
                    queue.Enqueue(tempNode.right);
                }

                Console.WriteLine(tempNode.Hexa);
            }
        }

        private static void Initialize()
        {
            // Huffman Tree each color
            //RedHuffmanTree = new Dictionary<short, Tuple<short, short>>();
            //GreenHuffmanTree = new Dictionary<short, Tuple<short, short>>();
            //BlueHuffmanTree = new Dictionary<short, Tuple<short, short>>();

            redBinaryCode = null;
            greenBinaryCode = null;
            blueBinaryCode = null;
            ImageHeight = ImageWidth = 0;

            //redHuffmanTreeRoot = greenHuffmanTreeRoot = blueHuffmanTreeRoot = 0;
            redHuffmanTreeRoot = null;
            greenHuffmanTreeRoot = null;
            blueHuffmanTreeRoot = null;

            seedString = null;
            seedLength = 0;
            TapPosition = 0;
            isEncrypted = false;

            redBinaryCode = new StringBuilder();
            greenBinaryCode = new StringBuilder();
            blueBinaryCode = new StringBuilder();
        }

        private static void DecodeBinaryCode_V2(RGBPixel[,] ImageMatrix)
        {
            Task redTask = Task.Factory.StartNew(() =>
            {
                int cnt = 0, i, j;
                Compression.HuffmanNode currNode = redHuffmanTreeRoot;
                string binaryCode = redBinaryCode.ToString();
                foreach (char bit in binaryCode)
                {
                    if (bit == '0') // left
                    {
                        currNode = currNode.left;
                    }
                    else // right
                    {
                        currNode = currNode.right;
                    }

                    //Console.WriteLine("here1");
                    if (currNode.Hexa != 333) // leaf node
                    {
                        //Console.WriteLine("here3");
                        i = cnt / ImageWidth;
                        if (i >= ImageHeight)
                        {
                            break;
                        }
                        j = cnt % ImageWidth;
                        ImageMatrix[i, j].red = (byte)currNode.Hexa;

                        currNode = redHuffmanTreeRoot;
                        cnt++;
                    }
                }
            });

            Task greenTask = Task.Factory.StartNew(() =>
            {
                int cnt = 0, i, j;
                Compression.HuffmanNode currNode = greenHuffmanTreeRoot;
                string binaryCode = greenBinaryCode.ToString();
                foreach (char bit in binaryCode)
                {
                    if (bit == '0') // left
                    {
                        currNode = currNode.left;
                    }
                    else // right
                    {
                        currNode = currNode.right;
                    }

                    //Console.WriteLine("here1");
                    if (currNode.Hexa != 333) // leaf node
                    {
                        //Console.WriteLine("here3");
                        i = cnt / ImageWidth;
                        if (i >= ImageHeight)
                        {
                            break;
                        }
                        j = cnt % ImageWidth;
                        ImageMatrix[i, j].green = (byte)currNode.Hexa;

                        currNode = greenHuffmanTreeRoot;
                        cnt++;
                    }
                }
            });

            Task blueTask = Task.Factory.StartNew(() =>
            {
                int cnt = 0, i, j;
                Compression.HuffmanNode currNode = blueHuffmanTreeRoot;
                string binaryCode = blueBinaryCode.ToString();
                foreach (char bit in binaryCode)
                {
                    if (bit == '0') // left
                    {
                        currNode = currNode.left;
                    }
                    else // right
                    {
                        currNode = currNode.right;
                    }

                    //Console.WriteLine("here1");
                    if (currNode.Hexa != 333) // leaf node
                    {
                        //Console.WriteLine("here3");
                        i = cnt / ImageWidth;
                        if (i >= ImageHeight)
                        {
                            break;
                        }
                        j = cnt % ImageWidth;
                        ImageMatrix[i, j].blue = (byte)currNode.Hexa;

                        currNode = blueHuffmanTreeRoot;
                        cnt++;
                    }
                }
            });

            Task.WaitAll(redTask, greenTask, blueTask);
        }

        /*
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
                        ImageMatrix[i, j].red = (byte)currNode;

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
        */
    }
}
