using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using static ImageEncryptCompress.Compression;

namespace ImageEncryptCompress
{
    internal class BinaryFileOperations
    {
        public static void CreateBinaryFile(List<byte>[] CompressedImage)
        {
            string saveFilePath = string.Empty;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Binary File|*.bin";
            saveFileDialog1.Title = "Save a Compressed Image";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                saveFilePath = saveFileDialog1.FileName;
            }
            else
            {
                return;
            } 


            using (BinaryWriter writer = new BinaryWriter(File.Open(saveFilePath, FileMode.Create)))
            {
                /*
                 * isEncrypted
                 * 
                 * Initial Seed length
                 * Initial SeedList length
                 * Tap position
                 * 
                 * Height
                 * Width
                 * 
                 * Red Huffman Tree length
                 * Red Huffman Tree 
                 * Green Huffman Tree length
                 * Green Huffman Tree 
                 * Blue Huffman Tree length
                 * Blue Huffman Tree
                 * 
                 * Red Binary Code length
                 * Red Binary Code
                 * Green Binary Code length
                 * Green Binary Code
                 * Blue Binary Code length
                 * Blue Binary Code
                 */

                /*
                 * isEncrypted
                 * 
                 * Initial Seed length
                 * Initial SeedList length
                 * Tap position
                 * 
                 * Height
                 * Width
                 * 
                 * Red Huffman Tree 
                 * Green Huffman Tree 
                 * Blue Huffman Tree
                 * 
                 * Red Binary Code length
                 * Red Binary Code
                 * Green Binary Code length
                 * Green Binary Code
                 * Blue Binary Code length
                 * Blue Binary Code
                 */

                #region Seed
                if (Compression.isEncrypted == true)
                {
                    writer.Write(true);
                    writer.Write((ushort)EncryptImage.Seed.Length);

                    List<byte> seedList = EncryptImage.GetBinarySeed();
                    writer.Write(seedList.Count);

                    foreach (byte b in seedList)
                    {
                        //Console.Write(b + " ");
                        writer.Write(b);
                    }

                    writer.Write(EncryptImage.TapPosition);
                }
                else
                {
                    writer.Write(false);
                }
                #endregion

                writer.Write(Compression.ImageHeight);
                writer.Write(Compression.ImageWidth);

                #region HuffmanTree

                Queue<Compression.HuffmanNode> queue = new Queue<Compression.HuffmanNode>();
                Compression.HuffmanNode tempNode;

                //Console.WriteLine("\n\n Red Tree");
                queue.Enqueue(Compression.redHuffmanTreeRoot);
                while (queue.Count > 0)
                {
                    tempNode = queue.Dequeue();
                    if (tempNode.Hexa == 333)
                    {
                        queue.Enqueue(tempNode.left);
                        queue.Enqueue(tempNode.right);
                    }

                    writer.Write(tempNode.Hexa);
                    Console.WriteLine(tempNode.Hexa);
                }

                //Console.WriteLine("\n\n Green Tree");
                queue.Enqueue(Compression.greenHuffmanTreeRoot);
                while (queue.Count > 0)
                {
                    tempNode = queue.Dequeue();
                    if (tempNode.Hexa == 333)
                    {
                        queue.Enqueue(tempNode.left);
                        queue.Enqueue(tempNode.right);
                    }

                    writer.Write(tempNode.Hexa);
                    Console.WriteLine(tempNode.Hexa);
                }

                //Console.WriteLine("\n\n Blue Tree");
                queue.Enqueue(Compression.blueHuffmanTreeRoot);
                while (queue.Count > 0)
                {
                    tempNode = queue.Dequeue();
                    if (tempNode.Hexa == 333)
                    {
                        queue.Enqueue(tempNode.left);
                        queue.Enqueue(tempNode.right);
                    }

                    writer.Write(tempNode.Hexa);
                    Console.WriteLine(tempNode.Hexa);
                }

                #endregion

                #region BinaryCode

                for (int i = 0; i < 3; ++i)
                {
                    writer.Write(CompressedImage[i].Count);
                    //Console.WriteLine(CompressedImage[i].Count);
                    foreach (byte b in CompressedImage[i])
                    {
                        //Console.Write(b + " ");
                        writer.Write(b);
                    }
                }

                #endregion
            }
        }

        public static void ReedFromBinaryFile(string filePath)
        {
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("File does not exist");
                    return;
                }


                // Open the binary file for reading
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {

                    #region Seed
                    byte byteValue = 0;

                    //isEncrypted
                    Decompressoin.isEncrypted = reader.ReadBoolean();
                    if(Decompressoin.isEncrypted == true)
                    {
                        Decompressoin.seedLength = reader.ReadUInt16();
                        int seedListLength = reader.ReadInt32();
                        //Console.WriteLine(listLength);
                        byteValue = 0;

                        Decompressoin.seedString = new StringBuilder();
                        while (seedListLength-- > 0)
                        {
                            byteValue = reader.ReadByte();
                            //Console.Write(byteValue + " ");
                            //Console.WriteLine(Convert.ToString(byteValue, 2).PadLeft(8, '0'));
                            Decompressoin.seedString.Append(Convert.ToString(byteValue, 2).PadLeft(8, '0'));
                        }
                        //Console.WriteLine(seedString);

                        Decompressoin.TapPosition = reader.ReadInt16();
                    }
                    #endregion

                    // Height, Width
                    Decompressoin.ImageHeight = reader.ReadInt32();
                    Decompressoin.ImageWidth = reader.ReadInt32();

                    #region HuffmanTree

                    Queue<Compression.HuffmanNode> queue = new Queue<Compression.HuffmanNode>();
                    Compression.HuffmanNode node;

                    // Read Red Tree
                    short value = reader.ReadInt16();
                    Decompressoin.redHuffmanTreeRoot = new Compression.HuffmanNode(value);
                    if (value == 333)
                    {
                        queue.Enqueue(Decompressoin.redHuffmanTreeRoot);
                        while(queue.Count > 0)
                        {
                            node = queue.Dequeue();

                            //left
                            value = reader.ReadInt16();
                            node.left = new Compression.HuffmanNode(value);
                            if(value == 333) queue.Enqueue(node.left);

                            //right
                            value = reader.ReadInt16();
                            node.right = new Compression.HuffmanNode(value);
                            if (value == 333) queue.Enqueue(node.right);
                        }
                    }

                    // Read Green Tree
                    value = reader.ReadInt16();
                    Decompressoin.greenHuffmanTreeRoot = new Compression.HuffmanNode(value);
                    if (value == 333)
                    {
                        queue.Enqueue(Decompressoin.greenHuffmanTreeRoot);
                        while (queue.Count > 0)
                        {
                            node = queue.Dequeue();

                            //left
                            value = reader.ReadInt16();
                            node.left = new Compression.HuffmanNode(value);
                            if (value == 333) queue.Enqueue(node.left);

                            //right
                            value = reader.ReadInt16();
                            node.right = new Compression.HuffmanNode(value);
                            if (value == 333) queue.Enqueue(node.right);
                        }
                    }

                    // Read Blue Tree
                    value = reader.ReadInt16();
                    Decompressoin.blueHuffmanTreeRoot = new Compression.HuffmanNode(value);
                    if (value == 333)
                    {
                        queue.Enqueue(Decompressoin.blueHuffmanTreeRoot);
                        while (queue.Count > 0)
                        {
                            node = queue.Dequeue();

                            //left
                            value = reader.ReadInt16();
                            node.left = new Compression.HuffmanNode(value);
                            if (value == 333) queue.Enqueue(node.left);

                            //right
                            value = reader.ReadInt16();
                            node.right = new Compression.HuffmanNode(value);
                            if (value == 333) queue.Enqueue(node.right);
                        }
                    }

                    #endregion

                    #region BinaryCode

                    int listLength = reader.ReadInt32();
                    //Console.WriteLine(listLength);

                    while (listLength-- > 0)
                    {
                        byteValue = reader.ReadByte();
                        //Console.Write(byteValue + " ");
                        //Console.WriteLine(Convert.ToString(byteValue, 2).PadLeft(8, '0'));
                        Decompressoin.redBinaryCode.Append(Convert.ToString(byteValue, 2).PadLeft(8, '0'));
                    }
                    //Console.WriteLine(Decompressoin.redBinaryCode);

                    listLength = reader.ReadInt32();
                    //Console.WriteLine(listLength);
                    while (listLength-- > 0)
                    {
                        byteValue = reader.ReadByte();
                        //Console.WriteLine(Convert.ToString(byteValue, 2).PadLeft(8, '0'));
                        Decompressoin.greenBinaryCode.Append(Convert.ToString(byteValue, 2).PadLeft(8, '0'));
                    }
                    //Console.WriteLine("\n\n" + Decompressoin.greenBinaryCode);

                    listLength = reader.ReadInt32();
                    //Console.WriteLine(listLength);
                    while (listLength-- > 0)
                    {
                        byteValue = reader.ReadByte();
                        //Console.WriteLine(Convert.ToString(byteValue, 2).PadLeft(8, '0'));
                        Decompressoin.blueBinaryCode.Append(Convert.ToString(byteValue, 2).PadLeft(8, '0'));
                    }
                    //Console.WriteLine("\n\n" + Decompressoin.blueBinaryCode);

                    #endregion


                }
            }


        }
    }
}
