using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.CompilerServices;

namespace ImageEncryptCompress
{
    internal class Compression
    {

        public class HuffmanNodeComparer : IComparer<HuffmanNode>
        {
            public int Compare(HuffmanNode x, HuffmanNode y)
            {
                // Higher priority should come first
                return x.frequency.CompareTo(y.frequency);
            }
        }

        private enum Color
        {
            RED, GREEN, BLUE,
        }

        // Tree Node
        public class HuffmanNode
        {
            public short Hexa;
            public int frequency;
            public HuffmanNode left, right;

            public HuffmanNode(short hexa)
            {
                this.Hexa = hexa;
                frequency = 0;
                left = null;
                right = null;
            }

            public HuffmanNode()
            {
                frequency = 0;
                left = null;
                right = null;
            }
        }


        // Image Data Structures 
        // Frequency Dictionaries
        static Dictionary<byte, int> RedFrequency;
        static Dictionary<byte, int> GreenFrequency;
        static Dictionary<byte, int> BlueFrequency;

        // Priority Queue for each color
        public static PriorityQueue<HuffmanNode> RedQueue;
        public static PriorityQueue<HuffmanNode> GreenQueue;
        public static PriorityQueue<HuffmanNode> BlueQueue;

        // Compressed map for each color
        public static Dictionary<short, string> RedBinaryCode;
        public static Dictionary<short, string> GreenBinaryCode;
        public static Dictionary<short, string> BlueBinaryCode;

        // Huffman Tree each short
        //public static Dictionary<short, Tuple<short, short>> RedHuffmanTree;
        //public static Dictionary<short, Tuple<short, short>> GreenHuffmanTree;
        //public static Dictionary<short, Tuple<short, short>> BlueHuffmanTree;

        private static int RedCompressedBits, GreenCompressedBits, BlueCompressedBits;
        public static int ImageHeight, ImageWidth;
        public static HuffmanNode redHuffmanTreeRoot, greenHuffmanTreeRoot, blueHuffmanTreeRoot;

        public static bool isEncrypted;

        // Compress Function
        public static void CompressImage(RGBPixel[,] ImageMatrix, bool isencrypted)
        {
            Stopwatch sws = Stopwatch.StartNew();

            Intialize();

            ImageHeight = ImageOperations.GetHeight(ImageMatrix);  // rows
            ImageWidth = ImageOperations.GetWidth(ImageMatrix);   // columns   
            isEncrypted = isencrypted;

            // Count Frequencies for each color
            CalcFrequency(ImageMatrix);

            // Initializing Queues
            InitColorQueues();

            // Huffman Code for each color 
            BuildHuffmanTrees();

            // Binary codes
            BinaryCode();

            // Compressed Image
            List<byte>[] CompressedImage = CreateCompressedImage(ImageMatrix);

            // calcualte compression ratio
            CalcCompressionRatio();

            sws.Stop();
            Console.WriteLine("Time Copresss: " + sws.ElapsedMilliseconds);
            // Save in Binary File
            BinaryFileOperations.CreateBinaryFile(CompressedImage);
        }

        private static void Intialize()
        {
            RedFrequency = new Dictionary<byte, int>();
            BlueFrequency = new Dictionary<byte, int>();
            GreenFrequency = new Dictionary<byte, int>();

            // Priority Queue for each color
            RedQueue = new PriorityQueue<HuffmanNode>(new HuffmanNodeComparer());
            GreenQueue = new PriorityQueue<HuffmanNode>(new HuffmanNodeComparer());
            BlueQueue = new PriorityQueue<HuffmanNode>(new HuffmanNodeComparer());

            // Compressed map for each color
            RedBinaryCode = new Dictionary<short, string>();
            GreenBinaryCode = new Dictionary<short, string>();
            BlueBinaryCode = new Dictionary<short, string>();

            // Huffman Tree each short
            //RedHuffmanTree = new Dictionary<short, Tuple<short, short>>();
            //GreenHuffmanTree = new Dictionary<short, Tuple<short, short>>();
            //BlueHuffmanTree = new Dictionary<short, Tuple<short, short>>();

            RedCompressedBits = GreenCompressedBits = BlueCompressedBits = 0;
            ImageHeight = ImageWidth = 0;
            redHuffmanTreeRoot = null;
            greenHuffmanTreeRoot = null;
            blueHuffmanTreeRoot = null;

            isEncrypted = false;
        }

        private static void CalcFrequency(RGBPixel[,] ImageMatrix)
        {
            byte red, green, blue;
            for (int i = 0; i < ImageHeight; i++)
            {
                for (int j = 0; j < ImageWidth; j++)
                {

                    red = ImageMatrix[i, j].red;
                    green = ImageMatrix[i, j].green;
                    blue = ImageMatrix[i, j].blue;

                    // Update red frequency
                    if (!RedFrequency.ContainsKey(red))
                        RedFrequency[red] = 0;
                    RedFrequency[red]++;

                    if (!GreenFrequency.ContainsKey(green))
                        GreenFrequency[green] = 0;
                    GreenFrequency[green]++;

                    if (!BlueFrequency.ContainsKey(blue))
                        BlueFrequency[blue] = 0;
                    BlueFrequency[blue]++;

                    //MessageBox.Show($"[red, green, blue] = [{red}, {green}, {blue}]");
                }
                //Console.WriteLine();
            }
        }

        private static void InitColorQueues()
        {
            Task redTask = Task.Factory.StartNew(() =>
            {
                //Console.WriteLine("Red Color Frequencies");
                HuffmanNode node;
                foreach (var RedPixel in RedFrequency)
                {
                    node = new HuffmanNode();

                    node.Hexa = RedPixel.Key;
                    node.left = null;
                    node.right = null;
                    node.frequency = RedPixel.Value;

                    RedQueue.Enqueue(node);
                    //Console.WriteLine(node.Hexa + ":" + node.frequency);
                }
            });

            Task greenTask = Task.Factory.StartNew(() =>
            {
                //Console.WriteLine("\nGreen Color Frequencies");
                HuffmanNode node;
                foreach (var GreenPixel in GreenFrequency)
                {
                    node = new HuffmanNode();

                    node.Hexa = GreenPixel.Key;
                    node.left = null;
                    node.right = null;
                    node.frequency = GreenPixel.Value;

                    GreenQueue.Enqueue(node);
                    //Console.WriteLine(node.Hexa + ":" + node.frequency);
                }
            });

            Task blueTask = Task.Factory.StartNew(() =>
            {
                //Console.WriteLine("\nBlue Color Frequencies");
                HuffmanNode node;
                foreach (var BluePixel in BlueFrequency)
                {
                    node = new HuffmanNode();

                    node.Hexa = BluePixel.Key;
                    node.left = null;
                    node.right = null;
                    node.frequency = BluePixel.Value;

                    BlueQueue.Enqueue(node);
                    //Console.WriteLine(node.Hexa + ":" + node.frequency);
                }
            });

            Task.WaitAll(redTask, greenTask, blueTask);
        }

        private static void BuildHuffmanTrees()
        {
            const short notLeaf = 333;
            Task redTask = Task.Factory.StartNew(() =>
            {
                while (RedQueue.Count > 1)
                {
                    HuffmanNode node = new HuffmanNode();

                    node.Hexa = notLeaf;
                    //MessageBox.Show($"Removing red pixel name: {node.left.Hexa} with frequency = {node.left.frequency}");
                    node.right = RedQueue.Dequeue();
                    node.left = RedQueue.Dequeue();
                    //MessageBox.Show($"Removing red pixel name: {node.right.Hexa} with frequency = {node.right.frequency}");
                    node.frequency = node.left.frequency + node.right.frequency;

                    //RedTop = node;
                    RedQueue.Enqueue(node);

                    //RedHuffmanTree.Add(node.Hexa, new Tuple<short, short>(node.left.Hexa, node.right.Hexa));
                }
            });

            Task greenTask = Task.Factory.StartNew(() =>
            {
                while (GreenQueue.Count > 1)
                {
                    HuffmanNode node = new HuffmanNode();

                    node.Hexa = notLeaf;
                    //MessageBox.Show($"Removing green pixel name: {node.left.Hexa} with frequency = {node.left.frequency}");
                    node.right = GreenQueue.Dequeue();
                    node.left = GreenQueue.Dequeue();
                    //MessageBox.Show($"Removing green pixel name: {node.right.Hexa} with frequency = {node.right.frequency}");
                    node.frequency = node.left.frequency + node.right.frequency;

                    //GreenTop = node;
                    GreenQueue.Enqueue(node);

                    //GreenHuffmanTree.Add(node.Hexa, new Tuple<short, short>(node.left.Hexa, node.right.Hexa));
                }
            });

            Task blueTask = Task.Factory.StartNew(() =>
            {
                while (BlueQueue.Count > 1)
                {
                    HuffmanNode node = new HuffmanNode();

                    node.Hexa = notLeaf;
                    //MessageBox.Show($"Removing blue pixel name: {node.left.Hexa} with frequency = {node.left.frequency}");
                    node.right = BlueQueue.Dequeue();
                    //MessageBox.Show($"Removing blue pixel name: {node.right.Hexa} with frequency = {node.right.frequency}");
                    node.left = BlueQueue.Dequeue();
                    node.frequency = node.left.frequency + node.right.frequency;

                    //BlueTop = node;
                    BlueQueue.Enqueue(node);

                    //BlueHuffmanTree.Add(node.Hexa, new Tuple<short, short>(node.left.Hexa, node.right.Hexa));
                }
            });

            Task.WaitAll(redTask, greenTask, blueTask);
        }

        private static void BinaryCode()
        {
            //Task redTask = Task.Factory.StartNew(() =>
            //{
                //Console.WriteLine("\n\nRed Colors Binary Code");
                redHuffmanTreeRoot = RedQueue.Dequeue();
                GetBinaryCode(redHuffmanTreeRoot, new StringBuilder(), Color.RED);
            //});

            //Task greenTask = Task.Factory.StartNew(() =>
            //{
                greenHuffmanTreeRoot = GreenQueue.Dequeue();
                //Console.WriteLine("\nGreen Colors Binary Code");
                GetBinaryCode(greenHuffmanTreeRoot, new StringBuilder(), Color.GREEN);
            //});

            //Task blueTask = Task.Factory.StartNew(() =>
            //{
                blueHuffmanTreeRoot = BlueQueue.Dequeue();
                //Console.WriteLine("\nBlue Colors Binary Code");
                GetBinaryCode(blueHuffmanTreeRoot, new StringBuilder(), Color.BLUE);
            //});

            //Task.WaitAll(redTask, greenTask, blueTask);
        }

        private static void GetBinaryCode(HuffmanNode root, StringBuilder bits, Color color)
        {
            if (root.left != null)
            {
                bits.Append('0');
                GetBinaryCode(root.left, bits, color);
                bits.Remove(bits.Length - 1, 1);
            }

            if (root.right != null)
            {
                bits.Append('1');
                GetBinaryCode(root.right, bits, color);
                bits.Remove(bits.Length - 1, 1);
            }

            if (root.left == null && root.right == null)
            {
                //MessageBox.Show($"Printing node name: {root.data}");

                if (color == Color.RED)
                {
                    //Console.WriteLine(root.Hexa + ":" + bits);
                    RedBinaryCode.Add(root.Hexa, bits.ToString());
                    RedCompressedBits += RedFrequency[(byte)root.Hexa] * bits.Length;
                }
                else if (color == Color.GREEN)
                {
                    GreenBinaryCode.Add(root.Hexa, bits.ToString());
                    GreenCompressedBits += GreenFrequency[(byte)root.Hexa] * bits.Length;
                }
                else if (color == Color.BLUE)
                {
                    BlueBinaryCode.Add(root.Hexa, bits.ToString());
                    BlueCompressedBits += BlueFrequency[(byte)root.Hexa] * bits.Length;
                }
            }
        }

        private static void CalcCompressionRatio()
        {
            double original_size = ImageHeight * ImageWidth * 24;
            double compressed_size = RedCompressedBits + GreenCompressedBits + BlueCompressedBits;

            
            Console.WriteLine($"\n\nOriginal size = {original_size} bits = {original_size / 8} bytes = {original_size / (8 * 1024)} KB");
            Console.WriteLine($"Compressed size = {compressed_size} bits = {compressed_size / 8} bytes = {compressed_size / (8 * 1024)} KB");
            Console.WriteLine($"Compression ratio = {(compressed_size / original_size) * 100}%");

            //Console.WriteLine($"\nTree Size: {RedHuffmanTree.Count * 6} bytes = {RedHuffmanTree.Count * 6 / 1024.0} KB");
        }

        private static List<byte>[] CreateCompressedImage(RGBPixel[,] ImageMatrix)
        {
            List<byte>[] CompressedImage = new List<byte>[3];
            CompressedImage[0] = new List<byte>() { 0 };
            CompressedImage[1] = new List<byte>() { 0 };
            CompressedImage[2] = new List<byte>() { 0 };

            RGBPixel pixel;
            string redBinaryCode, greenBinaryCode, blueBinaryCode;
            int redStartIndex = 0, greenStartIndex = 0, blueStartIndex = 0;

            for (int i = 0; i < ImageHeight; ++i)
            {
                for (int j = 0; j < ImageWidth; ++j)
                {
                    pixel = ImageMatrix[i, j];
                    //Task redTask = Task.Factory.StartNew(() =>
                    //{
                        redBinaryCode = RedBinaryCode[pixel.red];
                        AddBits(CompressedImage[0], redBinaryCode, ref redStartIndex);
                    //});

                    //Task greenTask = Task.Factory.StartNew(() =>
                    //{
                        greenBinaryCode = GreenBinaryCode[pixel.green];
                        AddBits(CompressedImage[1], greenBinaryCode, ref greenStartIndex);
                    //});

                    //Task blueTask = Task.Factory.StartNew(() =>
                    //{
                        blueBinaryCode = BlueBinaryCode[pixel.blue];
                        AddBits(CompressedImage[2], blueBinaryCode, ref blueStartIndex);
                    //});

                    //Task.WaitAll(redTask, greenTask, blueTask);
                }
            }

            return CompressedImage;
        }

        public static void AddBits(List<byte> channel, string BinaryCode, ref int StartIndex)
        {
            foreach(char c in BinaryCode)
            {
                if(c  == '1')
                {
                    channel[channel.Count - 1] |= (byte)(1 << (7 - StartIndex));
                }
                if(++StartIndex == 8)
                {
                    channel.Add(0);
                    StartIndex = 0;
                }
            }
            //Console.WriteLine(channel[channel.Count - 1]);

        }

    }
}