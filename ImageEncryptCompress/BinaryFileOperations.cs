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
                 * Initial Seed
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

                #region Seed
                if (Compression.isEncrypted == true)
                {
                    writer.Write((byte)1);
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
                #endregion

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


                    // Height, Width
                    Decompressoin.ImageHeight = reader.ReadInt32();
                    Decompressoin.ImageWidth = reader.ReadInt32();

                    #region HuffmanTree

                    //Red Huffman Tree length
                    short treeSize = reader.ReadByte();
                    //Red Huffman Tree
                    short parent, child1, child2;

                    //Console.WriteLine("\nRed Huffman Tree");
                    Decompressoin.redHuffmanTreeRoot = (short)(treeSize + 259);
                    while (treeSize-- > 0)
                    {
                        parent = reader.ReadInt16();
                        child1 = reader.ReadInt16();
                        child2 = reader.ReadInt16();
                        Decompressoin.RedHuffmanTree.Add(parent, new Tuple<short, short>(child1, child2));
                        //Console.WriteLine($"{parent}: {child1}, {child2}");
                    }

                    //Green Huffman Tree length
                    treeSize = reader.ReadByte();
                    //Green Huffman Tree
                    //Console.WriteLine("\nGreen Huffman Tree");
                    Decompressoin.greenHuffmanTreeRoot = (short)(treeSize + 259);
                    while (treeSize-- > 0)
                    {
                        parent = reader.ReadInt16();
                        child1 = reader.ReadInt16();
                        child2 = reader.ReadInt16();
                        Decompressoin.GreenHuffmanTree.Add(parent, new Tuple<short, short>(child1, child2));
                        //Console.WriteLine($"{parent}: {child1}, {child2}");
                    }

                    //Blue Huffman Tree length
                    treeSize = reader.ReadByte();
                    //Blue Huffman Tree
                    //Console.WriteLine("\nBlue Huffman Tree");
                    Decompressoin.blueHuffmanTreeRoot = (short)(treeSize + 259);
                    while (treeSize-- > 0)
                    {
                        parent = reader.ReadInt16();
                        child1 = reader.ReadInt16();
                        child2 = reader.ReadInt16();
                        Decompressoin.BlueHuffmanTree.Add(parent, new Tuple<short, short>(child1, child2));
                        //Console.WriteLine($"{parent}: {child1}, {child2}");
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
