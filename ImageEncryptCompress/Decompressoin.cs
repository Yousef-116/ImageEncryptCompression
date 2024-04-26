using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEncryptCompress
{
    internal class Decompressoin
    {

        // Huffman Tree each color
        public static Dictionary<byte, Tuple<byte, byte>> RedHuffmanTree = new Dictionary<byte, Tuple<byte, byte>>();
        public static Dictionary<byte, Tuple<byte, byte>> GreenHuffmanTree = new Dictionary<byte, Tuple<byte, byte>>();
        public static Dictionary<byte, Tuple<byte, byte>> BlueHuffmanTree = new Dictionary<byte, Tuple<byte, byte>>();

        public static StringBuilder redBinaryCode, greenBinaryCode, blueBinaryCode;
        public static int ImageHeight = 0, ImageWidth = 0;

        public static RGBPixel[,] DecompressImage(string BinaryFilePath)
        {
            RGBPixel[,] ImageMatrix = new RGBPixel[ImageHeight, ImageWidth];

            redBinaryCode = new StringBuilder();
            greenBinaryCode = new StringBuilder();
            blueBinaryCode = new StringBuilder();
            BinaryFileOperations.ReedFromBinaryFile(BinaryFilePath);

            return ImageMatrix;
        }
    }
}
