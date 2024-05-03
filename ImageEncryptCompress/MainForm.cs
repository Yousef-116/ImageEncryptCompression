using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ImageEncryptCompress
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix, EncryptedImageMatrix;
        bool imageOpened = false;
        bool isEncrebted = false;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (BinaryFileCheckBox.Checked == false)
            {
                openFileDialog1.Filter = "Image File |*.bmp;*.png;*.jpg";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //Open the browsed image and display it
                    string OpenedFilePath = openFileDialog1.FileName;
                    ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                    imageOpened = true;
                    displayImage();
                }
            }
            else  // Decompression
            {
                openFileDialog1.Filter = "Binary File |*.bin";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    //Open the browsed image and display it
                    string OpenedFilePath = openFileDialog1.FileName;
                    ImageMatrix = Decompressoin.DecompressImage(OpenedFilePath);
                    stopwatch.Stop();
                    Console.WriteLine($"Decompression Time: {stopwatch.ElapsedMilliseconds} ms");

                    //imageOpened = true;
                    displayImage();

                    if (Decompressoin.isEncrypted == true)
                    {
                        stopwatch.Restart();
                        int count = Decompressoin.seedString.Length - Decompressoin.seedLength;
                        Init_seed.Text = Decompressoin.seedString.ToString().Remove(Decompressoin.seedLength, count);
                        Tap.Text = Decompressoin.TapPosition.ToString();
                        //int intseed = Convert.ToInt32(Init_seed.Text,2);
                        RGBPixel[,] DecryptedImageMatrix = EncryptImage.Encrypt(ImageMatrix, Init_seed.Text, Init_seed.Text.Length, Decompressoin.TapPosition);
                        stopwatch.Stop();
                        Console.WriteLine($"Decryption Time: {stopwatch.ElapsedMilliseconds} ms");

                        ImageOperations.DisplayImage(DecryptedImageMatrix, pictureBox2);
                        saveImage(DecryptedImageMatrix);
                    }
                    else
                    {
                        saveImage(ImageMatrix);
                    }
                }
            }

            //if (imageOpened == true)
            //{
            //    ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            //    txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            //    txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
            //}
        }

        private void displayImage()
        {
            // Clear pitureBoxs
            pictureBox1.Image = null;
            pictureBox2.Image = null;

            ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void compress_btn_Click(object sender, EventArgs e)
        {
            if (imageOpened == false)
            {
                MessageBox.Show("You must open an image first");
                return;
            }

            if (isEncrebted == false)
            {
                Compression.CompressImage(ImageMatrix, false);
            }
            else
            {
                Compression.CompressImage(EncryptedImageMatrix, true);
            }
        }

        private bool validation(String seed, int Tap_position)
        {
            if (Tap_position < 0 || Tap_position >= seed.Length)
            {
                MessageBox.Show("Tap Position is not valid");
                return false;
            }
            else if (imageOpened == false)
            {
                MessageBox.Show("You must open an image first");
                return false;
            }
            return true;
        }

        private void breakEncrypt_btn_Click(object sender, EventArgs e)
        {
            int seedLength = Convert.ToInt32(SeedLength.Text);
            EncryptedImageMatrix = EncryptImage.breakEncrypt(ImageMatrix,seedLength);
            Init_seed.Text = EncryptImage.Seed;
            Tap.Text = EncryptImage.TapPosition.ToString();
            ImageOperations.DisplayImage(EncryptedImageMatrix, pictureBox2);
            //Console.WriteLine("Done");
            // save image //
            saveImage(EncryptedImageMatrix);
        }

        private void encrypt_btn_Click(object sender, EventArgs e)
        {
            String seed = Init_seed.Text;
            String tempseed = "";
            int Tap_position = Convert.ToInt32(Tap.Text);

            if (IsAlphanumeric.Checked == true)
            {
               foreach(char c in seed)
               {
                    string charbinaryRepresentation = Convert.ToString(c, 2).PadLeft(8, '0');
                    tempseed += charbinaryRepresentation;
               }
               seed = tempseed;
            }

            if(!validation(seed, Tap_position))
            {
                return;
            }
            //int intseed = Convert.ToInt32(seed,2);
            EncryptedImageMatrix = EncryptImage.Encrypt(ImageMatrix, seed.ToString() ,seed.Length, Tap_position );
            ImageOperations.DisplayImage(EncryptedImageMatrix, pictureBox2);
            isEncrebted = true;

            // save image //
            saveImage(EncryptedImageMatrix);

            //SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //saveFileDialog1.Filter = "Bitmap Image|*.bmp";
            //saveFileDialog1.Title = "Save an Image File";

            //if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            //{
            //    string saveFilePath = saveFileDialog1.FileName;
            //    Bitmap bitmap = ImageOperations.ConvertToBitmap(EncryptedImageMatrix);
            //    ImageOperations.SaveImage(bitmap, saveFilePath);
            //}
        }

        private void saveImage(RGBPixel[,] img_matrix)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Bitmap Image|*.bmp";
            saveFileDialog1.Title = "Save an Image File";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string saveFilePath = saveFileDialog1.FileName;
                Bitmap bitmap = ImageOperations.ConvertToBitmap(img_matrix);
                ImageOperations.SaveImage(bitmap, saveFilePath);
            }
        }

    }
}