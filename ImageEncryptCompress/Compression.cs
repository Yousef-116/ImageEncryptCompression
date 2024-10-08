﻿using System;
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
        }


        // Image Data Structures 
        // Frequency Dictionaries
        public static Dictionary<byte, int> RedFrequency;
        public static Dictionary<byte, int> GreenFrequency;
        public static Dictionary<byte, int> BlueFrequency;

        // Priority Queue for each color
        public static PriorityQueue<HuffmanNode> RedQueue;
        public static PriorityQueue<HuffmanNode> GreenQueue;
        public static PriorityQueue<HuffmanNode> BlueQueue;

        // Compressed map for each color
        public static Dictionary<int, string> RedBinaryCode;
        public static Dictionary<int, string> GreenBinaryCode;
        public static Dictionary<int, string> BlueBinaryCode;

        // Huffman Tree each short
        public static Dictionary<short, Tuple<short, short>> RedHuffmanTree;
        public static Dictionary<short, Tuple<short, short>> GreenHuffmanTree;
        public static Dictionary<short, Tuple<short, short>> BlueHuffmanTree;

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
            Console.WriteLine("time normal Copresss ->: " + sws.ElapsedMilliseconds);
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
            RedBinaryCode = new Dictionary<int, string>();
            GreenBinaryCode = new Dictionary<int, string>();
            BlueBinaryCode = new Dictionary<int, string>();

            // Huffman Tree each short
            RedHuffmanTree = new Dictionary<short, Tuple<short, short>>();
            GreenHuffmanTree = new Dictionary<short, Tuple<short, short>>();
            BlueHuffmanTree = new Dictionary<short, Tuple<short, short>>();

            RedCompressedBits = GreenCompressedBits = BlueCompressedBits = 0;
            ImageHeight = ImageWidth = 0;
            redHuffmanTreeRoot = null;
            greenHuffmanTreeRoot = null;
            blueHuffmanTreeRoot = null;

            isEncrypted = false;
        }

        private static void CalcFrequency(RGBPixel[,] ImageMatrix)
        {

            for (int i = 0; i < ImageHeight; i++)
            {
                for (int j = 0; j < ImageWidth; j++)
                {

                    byte red = ImageMatrix[i, j].red; // Extract red component
                    //Console.Write(red + " ");
                    byte green = ImageMatrix[i, j].green;
                    byte blue = ImageMatrix[i, j].blue;

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
            //Console.WriteLine("Red Color Frequencies");
            foreach (var RedPixel in RedFrequency)
            {
                HuffmanNode node = new HuffmanNode();

                node.Hexa = RedPixel.Key;
                node.left = null;
                node.right = null;
                node.frequency = RedPixel.Value;

                RedQueue.Enqueue(node);
                //Console.WriteLine(node.Hexa + ":" + node.frequency);
            }

            //Console.WriteLine("\nGreen Color Frequencies");
            foreach (var GreenPixel in GreenFrequency)
            {
                HuffmanNode node = new HuffmanNode();

                node.Hexa = GreenPixel.Key;
                node.left = null;
                node.right = null;
                node.frequency = GreenPixel.Value;

                GreenQueue.Enqueue(node);
                //Console.WriteLine(node.Hexa + ":" + node.frequency);
            }

            //Console.WriteLine("\nBlue Color Frequencies");
            foreach (var BluePixel in BlueFrequency)
            {
                HuffmanNode node = new HuffmanNode();

                node.Hexa = BluePixel.Key;
                node.left = null;
                node.right = null;
                node.frequency = BluePixel.Value;

                BlueQueue.Enqueue(node);
                //Console.WriteLine(node.Hexa + ":" + node.frequency);
            }
        }

        private static void BuildHuffmanTrees()
        {
            short nodeNum = 260;
            for (int k = 0; k < RedFrequency.Count - 1; k++)
            {
                HuffmanNode node = new HuffmanNode();

                node.Hexa = nodeNum++;
                //MessageBox.Show($"Removing red pixel name: {node.left.Hexa} with frequency = {node.left.frequency}");
                node.right = RedQueue.Dequeue();
                node.left = RedQueue.Dequeue();
                //MessageBox.Show($"Removing red pixel name: {node.right.Hexa} with frequency = {node.right.frequency}");
                node.frequency = node.left.frequency + node.right.frequency;

                //RedTop = node;
                RedQueue.Enqueue(node);

                RedHuffmanTree.Add(node.Hexa, new Tuple<short, short>(node.left.Hexa, node.right.Hexa));
            }
            //Console.WriteLine("\nRed Huffman Tree");
            //foreach(var node in RedHuffmanTree)
            //{
            //    Console.WriteLine($"{node.Key}: {node.Value.Item1}, {node.Value.Item2}");
            //}
            //Console.WriteLine("Red tree size: " + RedHuffmanTree.Count);

            nodeNum = 260;
            for (int k = 0; k < GreenFrequency.Count - 1; k++)
            {
                HuffmanNode node = new HuffmanNode();

                node.Hexa = nodeNum++;
                //MessageBox.Show($"Removing green pixel name: {node.left.Hexa} with frequency = {node.left.frequency}");
                node.right = GreenQueue.Dequeue();
                node.left = GreenQueue.Dequeue();
                //MessageBox.Show($"Removing green pixel name: {node.right.Hexa} with frequency = {node.right.frequency}");
                node.frequency = node.left.frequency + node.right.frequency;

                //GreenTop = node;
                GreenQueue.Enqueue(node);

                GreenHuffmanTree.Add(node.Hexa, new Tuple<short, short>(node.left.Hexa, node.right.Hexa));
            }

            nodeNum = 260;
            for (int k = 0; k < BlueFrequency.Count - 1; k++)
            {
                HuffmanNode node = new HuffmanNode();

                node.Hexa = nodeNum++;
                //MessageBox.Show($"Removing blue pixel name: {node.left.Hexa} with frequency = {node.left.frequency}");
                node.right = BlueQueue.Dequeue();
                //MessageBox.Show($"Removing blue pixel name: {node.right.Hexa} with frequency = {node.right.frequency}");
                node.left = BlueQueue.Dequeue();
                node.frequency = node.left.frequency + node.right.frequency;

                //BlueTop = node;
                BlueQueue.Enqueue(node);

                BlueHuffmanTree.Add(node.Hexa, new Tuple<short, short>(node.left.Hexa, node.right.Hexa));
            }
        }

        private static void BinaryCode()
        {
            //Console.WriteLine("\n\nRed Colors Binary Code");
            redHuffmanTreeRoot = RedQueue.Dequeue();
            GetBinaryCode(redHuffmanTreeRoot, new int[RedFrequency.Count], 0, Color.RED);

            greenHuffmanTreeRoot = GreenQueue.Dequeue();
            //Console.WriteLine("\nGreen Colors Binary Code");
            GetBinaryCode(greenHuffmanTreeRoot, new int[GreenFrequency.Count], 0, Color.GREEN);

            blueHuffmanTreeRoot = BlueQueue.Dequeue();
            //Console.WriteLine("\nBlue Colors Binary Code");
            GetBinaryCode(blueHuffmanTreeRoot, new int[BlueFrequency.Count], 0, Color.BLUE);
        }

        private static void GetBinaryCode(HuffmanNode root, int[] arr, int top, Color color)
        {
            if (root.left != null)
            {
                arr[top] = 0;
                GetBinaryCode(root.left, arr, top + 1, color);
            }

            if (root.right != null)
            {
                arr[top] = 1;
                GetBinaryCode(root.right, arr, top + 1, color);
            }

            if (root.left == null && root.right == null)
            {
                //MessageBox.Show($"Printing node name: {root.data}");
                string bits = string.Empty;
                for (int i = 0; i < top; i++)
                {
                    bits +=  (char)(arr[i] + '0');
                }

                if (color == Color.RED)
                {
                    //Console.WriteLine(root.Hexa + ":" + bits);
                    RedBinaryCode.Add(root.Hexa, bits);
                    RedCompressedBits += RedFrequency[(byte)root.Hexa] * bits.Length;
                }
                else if (color == Color.GREEN)
                {
                    GreenBinaryCode.Add(root.Hexa, bits);
                    GreenCompressedBits += GreenFrequency[(byte)root.Hexa] * bits.Length;
                }
                else if (color == Color.BLUE)
                {
                    BlueBinaryCode.Add(root.Hexa, bits);
                    BlueCompressedBits += BlueFrequency[(byte)root.Hexa] * bits.Length;
                }
            }
        }

        private static void CalcCompressionRatio()
        {
            double original_size = ImageHeight * ImageWidth * 24;
            double compressed_size = RedCompressedBits + GreenCompressedBits + BlueCompressedBits;
            
            /*
            int frequency, size;

            foreach (var RedPixel in RedFrequency)
            {
                frequency = RedPixel.Value;
                size = RedBinaryCode[RedPixel.Key].Length;

                RedCompressedBits += frequency * size;
            }
            foreach (var GreenPixel in GreenFrequency)
            {
                frequency = GreenPixel.Value;
                size = GreenBinaryCode[GreenPixel.Key].Length;

                GreenCompressedBits += frequency * size;
            }
            foreach (var BluePixel in BlueFrequency)
            {
                frequency = BluePixel.Value;
                size = BlueBinaryCode[BluePixel.Key].Length;

                BlueCompressedBits += frequency * size;
            }
            */
            
            Console.WriteLine($"\n\nOriginal size = {original_size} bits = {original_size / 8} bytes = {original_size / (8 * 1024)} KB");
            Console.WriteLine($"Compressed size = {compressed_size} bits = {compressed_size / 8} bytes = {compressed_size / (8 * 1024)} KB");
            Console.WriteLine($"Compression ratio = {(compressed_size / original_size) * 100}%");

            Console.WriteLine($"\nTree Size: {RedHuffmanTree.Count * 6} bytes = {RedHuffmanTree.Count * 6 / 1024.0} KB");
        }

        private static List<byte>[] CreateCompressedImage(RGBPixel[,] ImageMatrix)
        {
            List<byte>[] CompressedImage = new List<byte>[3];
            CompressedImage[0] = new List<byte>() { 0 };
            CompressedImage[1] = new List<byte>() { 0 };
            CompressedImage[2] = new List<byte>() { 0 };


            string redBinaryCode, greenBinaryCode, blueBinaryCode;
            int redStartIndex = 0, greenStartIndex = 0, blueStartIndex = 0;

            for (int i = 0; i < ImageHeight; ++i)
            {
                for (int j = 0; j < ImageWidth; ++j)
                {
                    redBinaryCode = RedBinaryCode[ImageMatrix[i, j].red];
                    AddBits(CompressedImage[0], redBinaryCode, ref redStartIndex);

                    greenBinaryCode = GreenBinaryCode[ImageMatrix[i, j].green];
                    AddBits(CompressedImage[1], greenBinaryCode, ref greenStartIndex);

                    blueBinaryCode = BlueBinaryCode[ImageMatrix[i, j].blue];
                    AddBits(CompressedImage[2], blueBinaryCode, ref blueStartIndex);
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