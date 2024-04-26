using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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


            using (BinaryWriter writer = new BinaryWriter(File.Open(saveFilePath, FileMode.Create)))
            {
                /*
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


                writer.Write(Compression.ImageHeight);
                writer.Write(Compression.ImageWidth);

                #region HuffmanTree

                writer.Write((byte)Compression.RedHuffmanTree.Count);
                foreach (var node in Compression.RedHuffmanTree)
                {
                    writer.Write(node.Key);
                    writer.Write(node.Value.Item1);
                    writer.Write(node.Value.Item2);
                }

                writer.Write((byte)Compression.GreenHuffmanTree.Count);
                foreach (var node in Compression.GreenHuffmanTree)
                {
                    writer.Write(node.Key);
                    writer.Write(node.Value.Item1);
                    writer.Write(node.Value.Item2);
                }

                writer.Write((byte)Compression.BlueHuffmanTree.Count);
                foreach (var node in Compression.BlueHuffmanTree)
                {
                    writer.Write(node.Key);
                    writer.Write(node.Value.Item1);
                    writer.Write(node.Value.Item2);
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
                    // Height, Width
                    Decompressoin.ImageHeight = reader.ReadInt32();
                    Decompressoin.ImageWidth = reader.ReadInt32();


                    #region HuffmanTree

                    //Red Huffman Tree length
                    byte treeSize = reader.ReadByte();
                    //Red Huffman Tree
                    byte parent, child1, child2;

                    Console.WriteLine("\nRed Huffman Tree");
                    while (treeSize-- > 0)
                    {
                        parent = reader.ReadByte();
                        child1 = reader.ReadByte();
                        child2 = reader.ReadByte();
                        Decompressoin.RedHuffmanTree.Add(parent, new Tuple<byte, byte>(child1, child2));
                        Console.WriteLine($"{parent}: {child1}, {child2}");
                    }

                    //Green Huffman Tree length
                    treeSize = reader.ReadByte();
                    //Green Huffman Tree
                    Console.WriteLine("\nGreen Huffman Tree");
                    while (treeSize-- > 0)
                    {
                        parent = reader.ReadByte();
                        child1 = reader.ReadByte();
                        child2 = reader.ReadByte();
                        Decompressoin.GreenHuffmanTree.Add(parent, new Tuple<byte, byte>(child1, child2));
                        Console.WriteLine($"{parent}: {child1}, {child2}");
                    }

                    //Blue Huffman Tree length
                    treeSize = reader.ReadByte();
                    //Blue Huffman Tree
                    Console.WriteLine("\nBlue Huffman Tree");
                    while (treeSize-- > 0)
                    {
                        parent = reader.ReadByte();
                        child1 = reader.ReadByte();
                        child2 = reader.ReadByte();
                        Decompressoin.BlueHuffmanTree.Add(parent, new Tuple<byte, byte>(child1, child2));
                        Console.WriteLine($"{parent}: {child1}, {child2}");
                    }

                    #endregion

                    #region

                    int listLength = reader.ReadInt32();
                    Console.WriteLine(listLength);
                    byte byteValue = 0;

                    while (listLength-- > 0)
                    {
                        byteValue = reader.ReadByte();
                        Console.Write(byteValue + " ");
                        //Console.WriteLine(Convert.ToString(byteValue, 2).PadLeft(8, '0'));
                        Decompressoin.redBinaryCode.Append(Convert.ToString(byteValue, 2).PadLeft(8, '0'));
                    }
                    //Console.WriteLine(Decompressoin.redBinaryCode);

                    listLength = reader.ReadInt32();
                    Console.WriteLine(listLength);
                    while (listLength-- > 0)
                    {
                        byteValue = reader.ReadByte();
                        //Console.WriteLine(Convert.ToString(byteValue, 2).PadLeft(8, '0'));
                        Decompressoin.greenBinaryCode.Append(Convert.ToString(byteValue, 2).PadLeft(8, '0'));
                    }
                    //Console.WriteLine("\n\n" + Decompressoin.greenBinaryCode);

                    listLength = reader.ReadInt32();
                    Console.WriteLine(listLength);
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
