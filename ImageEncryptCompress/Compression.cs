using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        
        // Image Data Structures 
        // Frequency Dictionaries
        public static Dictionary<int, int> RedFrequency = new Dictionary<int, int>();
        public static Dictionary<int, int> GreenFrequency = new Dictionary<int, int>();
        public static Dictionary<int, int> BlueFrequency = new Dictionary<int, int>();

        // Priority Queue for each color
        public static PriorityQueue<HuffmanNode> RedQueue = new PriorityQueue<HuffmanNode>(new HuffmanNodeComparer());
        public static PriorityQueue<HuffmanNode> GreenQueue = new PriorityQueue<HuffmanNode>(new HuffmanNodeComparer());
        public static PriorityQueue<HuffmanNode> BlueQueue = new PriorityQueue<HuffmanNode>(new HuffmanNodeComparer());

        // Compressed map for each color
        public static Dictionary<string, string> RedCompressed = new Dictionary<string, string>();
        public static Dictionary<string, string> GreenCompressed = new Dictionary<string, string>();
        public static Dictionary<string, string> BlueCompressed = new Dictionary<string, string>();

        // Tree Node
        public class HuffmanNode
        {
            public int data;
            public int frequency;
            public HuffmanNode left, right;
            
        }


        // Compress Function
        public static RGBPixel[,] CompressImage(RGBPixel[,] ImageMatrix)
        {
            int Height = ImageMatrix.GetLength(0);  // rows
            int Width = ImageMatrix.GetLength(1);   // columns   

            // Count Frequencies for each color
            for (int i = 0; i < Height; i++) 
            {
                for (int j = 0; j < Width; j++)
                {

                    int green = ImageMatrix[i, j].green;
                    int blue = ImageMatrix[i, j].blue;
                    int red = ImageMatrix[i, j].red; // Extract red component

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
            }

            //foreach (var RedPixel in RedFrequency)
            //{
            //    MessageBox.Show($"Red -- {RedPixel.Key}: {RedPixel.Value}");
            //}
            //foreach (var GreenPixel in GreenFrequency)
            //{
            //    MessageBox.Show($"green -- {GreenPixel.Key}: {GreenPixel.Value}");
            //}
            //foreach (var BluePixel in BlueFrequency)
            //{
            //    MessageBox.Show($"blue -- {BluePixel.Key}: {BluePixel.Value}");
            //}

            // Initializing Queues
            foreach (var RedPixel in RedFrequency)
            {
                HuffmanNode node = new HuffmanNode();
                
                node.data = RedPixel.Key;
                node.left = null;
                node.right = null;
                node.frequency = RedPixel.Value;

                RedQueue.Enqueue(node);
                
            }
            foreach (var GreenPixel in GreenFrequency)
            {
                HuffmanNode node = new HuffmanNode();

                node.data = GreenPixel.Key;
                node.left = null;
                node.right = null;
                node.frequency = GreenPixel.Value;

                GreenQueue.Enqueue(node);
            }
            foreach (var BluePixel in BlueFrequency)
            {
                HuffmanNode node = new HuffmanNode();

                node.data = BluePixel.Key;
                node.left = null;
                node.right = null;
                node.frequency = BluePixel.Value;

                BlueQueue.Enqueue(node);
            }



            // Huffman Code for each color 
            //HuffmanNode RedTop, GreenTop, BlueTop;
            for (int k=0; k < RedFrequency.Count - 1; k++) 
            {
                HuffmanNode node = new HuffmanNode();

                node.data = -1;
                //MessageBox.Show($"Removing red pixel name: {node.left.data} with frequency = {node.left.frequency}");
                node.right = RedQueue.Dequeue();
                node.left = RedQueue.Dequeue();
                //MessageBox.Show($"Removing red pixel name: {node.right.data} with frequency = {node.right.frequency}");
                node.frequency = node.left.frequency + node.right.frequency;

                //RedTop = node;
                RedQueue.Enqueue(node);
            }

            for (int k = 0; k < GreenFrequency.Count - 1; k++)
            {
                HuffmanNode node = new HuffmanNode();

                node.data = -1;
                //MessageBox.Show($"Removing green pixel name: {node.left.data} with frequency = {node.left.frequency}");
                node.right = GreenQueue.Dequeue();
                node.left = GreenQueue.Dequeue();
                //MessageBox.Show($"Removing green pixel name: {node.right.data} with frequency = {node.right.frequency}");
                node.frequency = node.left.frequency + node.right.frequency;

                //GreenTop = node;
                GreenQueue.Enqueue(node);
            }

            for (int k = 0; k < BlueFrequency.Count - 1; k++)
            {
                HuffmanNode node = new HuffmanNode();

                node.data = -1;
                //MessageBox.Show($"Removing blue pixel name: {node.left.data} with frequency = {node.left.frequency}");
                node.right = BlueQueue.Dequeue();
                //MessageBox.Show($"Removing blue pixel name: {node.right.data} with frequency = {node.right.frequency}");
                node.left = BlueQueue.Dequeue();
                node.frequency = node.left.frequency + node.right.frequency;

                //BlueTop = node;
                BlueQueue.Enqueue(node);
            }

            PrintBits(RedQueue.Dequeue(), new int[RedFrequency.Count], 0, 0);
            PrintBits(GreenQueue.Dequeue(), new int[GreenFrequency.Count], 0, 1);
            PrintBits(BlueQueue.Dequeue(), new int[BlueFrequency.Count], 0, 2);

            // calcualte ratio
            double original_size = Height * Width * 24;
            double compressed_size = 0;

            foreach (var RedPixel in RedFrequency)
            {
                int frequency = RedPixel.Value;
                int size = RedCompressed[RedPixel.Key.ToString()].Length;

                compressed_size += frequency * size;
            }
            foreach (var GreenPixel in GreenFrequency)
            {
                int frequency = GreenPixel.Value;
                int size = GreenCompressed[GreenPixel.Key.ToString()].Length;

                compressed_size += frequency * size;
            }
            foreach (var BluePixel in BlueFrequency)
            {
                int frequency = BluePixel.Value;
                int size = BlueCompressed[BluePixel.Key.ToString()].Length;

                compressed_size += frequency * size;
            }

            MessageBox.Show($"Original size = {original_size} bits = {original_size / 8} bytes = {original_size / (8*1024)} KB");
            MessageBox.Show($"Compressed size = {compressed_size} bits = {compressed_size / 8} bytes = {compressed_size / (8 * 1024)} KB");
            MessageBox.Show($"Compression ratio = {(compressed_size / original_size) * 100}%");

            return ImageMatrix;
        }


        

        

        public static void PrintBits(HuffmanNode root, int[] arr, int top, int color)
        {
            if (root.left != null)
            {
                arr[top] = 0;
                PrintBits(root.left, arr, top + 1, color);
            }

            if (root.right != null)
            {
                arr[top] = 1;
                PrintBits(root.right, arr, top + 1, color);
            }

            if (root.left == null && root.right == null)
            {
                //MessageBox.Show($"Printing node name: {root.data}");
                string bits = string.Empty;
                for (int i = 0; i < top; i++)
                {
                    if (arr[i] == 1)
                        bits += '1';
                    else
                        bits += '0';

                    //MessageBox.Show($"{arr[i]}");
                }

                if (color == 0) //red
                {
                    RedCompressed.Add(root.data.ToString(), bits);
                }
                else if (color == 1)    //green
                {
                    GreenCompressed.Add(root.data.ToString(), bits);
                }
                else if (color == 2)  //Blue
                {
                    BlueCompressed.Add(root.data.ToString(), bits);
                }
            }
        }
        
    }
}