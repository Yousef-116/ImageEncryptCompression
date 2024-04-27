using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if(BinaryFileCheckBox.Checked == false)
            {
                openFileDialog1.Filter = "Image File |*.bmp;*.png;*.jpg";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //Open the browsed image and display it
                    string OpenedFilePath = openFileDialog1.FileName;
                    ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                    imageOpened = true;
                }
            }
            else
            {
                openFileDialog1.Filter = "Binary File |*.bin";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //Open the browsed image and display it
                    string OpenedFilePath = openFileDialog1.FileName;
                    ImageMatrix = Decompressoin.DecompressImage(OpenedFilePath);
                    imageOpened = true;
                }
            }

            if (imageOpened == true)
            {
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
                txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
                txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
            }
        }

        private void compress_btn_Click(object sender, EventArgs e)
        {
            if (imageOpened == false)
            {
                MessageBox.Show("You must open an image first");
                return;
            }
            Compression.CompressImage(ImageMatrix);
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

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int seedLength = Convert.ToInt32(SeedLength.Text);

            EncryptedImageMatrix = EncryptImage.breakEncrypt(ImageMatrix,seedLength);
            ImageOperations.DisplayImage(EncryptedImageMatrix, pictureBox2);
            //Console.WriteLine("Done");

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

            EncryptedImageMatrix = EncryptImage.Encrypt(ImageMatrix, seed , Tap_position );
            ImageOperations.DisplayImage(EncryptedImageMatrix, pictureBox2);

            // save image //
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Bitmap Image|*.bmp";
            saveFileDialog1.Title = "Save an Image File";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string saveFilePath = saveFileDialog1.FileName;
                Bitmap bitmap = ImageOperations.ConvertToBitmap(EncryptedImageMatrix);
                ImageOperations.SaveImage(bitmap, saveFilePath);
            }
        }
    }
}