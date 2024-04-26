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
        public static void CreateBinaryFile(List<byte>[] CompressedImage, int Height, int Width, 
            Dictionary<byte, Tuple<byte, byte>> RedHuffmanTree, Dictionary<byte, Tuple<byte, byte>> GreenHuffmanTree, Dictionary<byte, Tuple<byte, byte>> BlueHuffmanTree)
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
                 * Red Huffman Tree size
                 * Red Huffman Tree 
                 * Green Huffman Tree size
                 * Green Huffman Tree 
                 * Blue Huffman Tree size
                 * Blue Huffman Tree
                 * 
                 * Red Binary Code
                 * Green Binary Code
                 * Blue Binary Code
                 */


                writer.Write(Height);
                writer.Write(Width);

                #region HuffmanTree

                writer.Write((byte)RedHuffmanTree.Count);
                foreach(var node in RedHuffmanTree)
                {
                    writer.Write(node.Key);
                    writer.Write(node.Value.Item1);
                    writer.Write(node.Value.Item2);
                }

                writer.Write((byte)GreenHuffmanTree.Count);
                foreach(var node in GreenHuffmanTree)
                {
                    writer.Write(node.Key);
                    writer.Write(node.Value.Item1);
                    writer.Write(node.Value.Item2);
                }

                writer.Write((byte)BlueHuffmanTree.Count);
                foreach(var node in BlueHuffmanTree)
                {
                    writer.Write(node.Key);
                    writer.Write(node.Value.Item1);
                    writer.Write(node.Value.Item2);
                }

                #endregion

                #region BinaryCode

                for (int i = 0; i < 3; ++i)
                {
                    foreach (byte b in CompressedImage[i])
                    {
                        //Console.Write(b + " ");
                        writer.Write(b);
                    }
                }

                #endregion
            }
        }

    }
}
