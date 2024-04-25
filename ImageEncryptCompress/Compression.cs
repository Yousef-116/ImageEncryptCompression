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

        // Text Data Structures
        // Frequency for each character
        public static Dictionary<char, int> frequency = new Dictionary<char, int>();

        // Priority Queue
        public static PriorityQueue<HuffmanNode> queue = new PriorityQueue<HuffmanNode>(new HuffmanNodeComparer());


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

            PrintBits(RedQueue.Dequeue(), new int[RedFrequency.Count], 0);
            //// Compressed Dictionary
            //Dictionary<int, int[]> RedCompressed = StoreCompressedBits(RedQueue.Dequeue(), new int[RedFrequency.Count], 0, new Dictionary<int, int[]>());
            //Dictionary<int, int[]> GreenCompressed = StoreCompressedBits(GreenQueue.Dequeue(), new int[GreenFrequency.Count], 0, new Dictionary<int, int[]>());
            //Dictionary<int, int[]> BlueCompressed = StoreCompressedBits(BlueQueue.Dequeue(), new int[BlueFrequency.Count], 0, new Dictionary<int, int[]>());

            //// Storing Compressed Image
            //RGBPixel[,] CompressedImage = new RGBPixel[Height, Width];
            //for (int i = 0; i < Height; i++)
            //{
            //    for (int j = 0; j < Width; j++)
            //    {
            //        int red = ImageMatrix[i, j].red;
            //        int green = ImageMatrix[i, j].green;
            //        int blue = ImageMatrix[i, j].blue;

            //        int new_red = ConvertBinaryToInt(RedCompressed[red]);
            //        int new_green = ConvertBinaryToInt(GreenCompressed[green]);
            //        int new_blue = ConvertBinaryToInt(BlueCompressed[blue]);

            //        CompressedImage[i, j].red = (byte)new_red;
            //        CompressedImage[i, j].blue = (byte)new_blue;
            //        CompressedImage[i, j].green = (byte)new_green;
                    
            //    }
            //}


            //// De-Compress
            
            return ImageMatrix;
        }


        

        //public static Dictionary<int, int[]> StoreCompressedBits(HuffmanNode root, int[] arr, int top, Dictionary<int, int[]> storage)
        //{
        //    if (root.left != null)
        //    {
        //        arr[top] = 0;
        //        StoreCompressedBits(root.left, arr, top+1, storage);
        //    }

        //    if (root.right != null) 
        //    {
        //        arr[top] = 1;
        //        StoreCompressedBits(root.right, arr, top+1, storage);
        //    }

        //    if (root.left == null && root.right == null)
        //    {
        //        storage.Add(root.data, new int[top + 1]);
        //        arr = new int[arr.Length];
        //        top = 0;

        //    }
        //    return storage;
        //}

        public static void PrintBits(HuffmanNode root, int[] arr, int top)
        {
            if (root.left != null)
            {
                arr[top] = 0;
                PrintBits(root.left, arr, top + 1);
            }

            if (root.right != null)
            {
                arr[top] = 1;
                PrintBits(root.right, arr, top + 1);
            }

            if (root.left == null && root.right == null)
            {
                MessageBox.Show($"Printing node name: {root.data}");
                for (int i = 0; i < top; i++)
                {
                    MessageBox.Show($"{arr[i]}");
                }
            }
        }
        
    }
}