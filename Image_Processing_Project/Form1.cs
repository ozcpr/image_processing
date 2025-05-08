using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Image_Processing_Project
{

    public partial class Form1 : Form
    {
        private Point cropStart;
        private Point cropEnd;
        private Rectangle cropRect;
        private bool isSelecting = false;
        private bool hasUserSelection = false;


        private Bitmap originalImage;
        private Bitmap originalImage2;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            comboBox1.Items.Add("Toplama");
            comboBox1.Items.Add("Çarpma");
            comboBox1.Items.Add("Grayscale");
            comboBox1.Items.Add("Binary");
            comboBox1.Items.Add("HSV");
            comboBox1.Items.Add("CMY");
            comboBox1.Items.Add("YCbCr");
            comboBox1.Items.Add("Histogram Germe");
            comboBox1.Items.Add("Histogram Genişletme");
            comboBox1.Items.Add("Salt & Pepper Gürültü Ekle");
            comboBox1.Items.Add("Mean Filtre");
            comboBox1.Items.Add("Median Filtre");
            comboBox1.Items.Add("Adaptif Eşikleme");
            comboBox1.Items.Add("Sobel Kenar Algılama");
            comboBox1.Items.Add("Gaussian Blur");
            comboBox1.Items.Add("90 Derece Döndür");
            comboBox1.Items.Add("Parlaklık Artır");
            comboBox1.Items.Add("Genişleme (Dilation)");
            comboBox1.Items.Add("Aşınma (Erosion)");
            comboBox1.Items.Add("Açma (Opening)");
            comboBox1.Items.Add("Kapama (Closing)");



        }


        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Resim Dosyaları|*.bmp;*.jpg;*.jpeg;*.png;";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                originalImage = new Bitmap(openFileDialog.FileName);
                pictureBox1.Image = ResizeImage(originalImage, 256, 256);

                // Orijinal görüntünün histogramını güncelle
                UpdateHistogram(originalImage);
            }
        }

        private void UploadSecondImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                originalImage2 = new Bitmap(openFileDialog.FileName);
                pictureBox3.Image = ResizeImage(originalImage2, 256, 256);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool showSecondImage = comboBox1.SelectedItem?.ToString() == "Toplama" ||
                         comboBox1.SelectedItem?.ToString() == "Çarpma";

            UploadSecondImage.Visible = showSecondImage;
            pictureBox3.Visible = showSecondImage;
            label4.Visible = showSecondImage;
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            // dispose images
            if (originalImage != null)
            {
                originalImage.Dispose();
                originalImage = null;
            }
            if (originalImage2 != null)
            {
                originalImage2.Dispose();
                originalImage2 = null;
            }

            pictureBox1.Image = null;
            pictureBox2.Image = null;
            pictureBox3.Image = null;

            // clear chart
            chart1.Series.Clear();
            chart1.ChartAreas[0].AxisY.Maximum = double.NaN;

            // reset dropdown
            comboBox1.SelectedIndex = -1;

            this.Refresh();
        }

       



        private void button1_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Lütfen önce bir resim yükleyin.");
                return;
            }

            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir işlem seçin.");
                return;
            }

            string selectedOperation = comboBox1.SelectedItem.ToString();
            Bitmap processedImage = null;

            if (selectedOperation == "Toplama" || selectedOperation == "Çarpma")
                
{
                if (originalImage2 == null)
                {
                    MessageBox.Show("Lütfen ikinci bir resim yükleyin.");
                    return;
                }
            }

            switch (selectedOperation)
            {
                case "Grayscale":
                    processedImage = ApplyGrayscale(originalImage);
                    break;

                case "Binary":
                    processedImage = ApplyBinary(originalImage, 128); // Sabit eşik
                    break;

                case "HSV":
                    processedImage = ApplyHSVChannel(originalImage, "HSV");
                    break;

                case "CMY":
                    processedImage = ApplyCMYChannel(originalImage, "CMY");
                    break;

                case "YCbCr":
                    processedImage = ApplyYCbCrChannel(originalImage, "YCbCr");
                    break;


                case "Histogram Germe":
                    processedImage = ApplyHistogramGerme(originalImage, 50, 200, 0, 255);
                    break;

                case "Histogram Genişletme":
                    processedImage = ApplyHistogramGenisletme(originalImage);
                    break;

                case "Salt & Pepper Gürültü Ekle":
                    processedImage = ApplySaltAndPepperNoise(originalImage);
                    break;

                case "Mean Filtre":
                    processedImage = ApplyMeanFilter(originalImage);
                    break;
                case "Median Filtre":
                    processedImage = ApplyMedianFilter(originalImage);
                    break;
                case "Adaptif Eşikleme":
                    processedImage = ApplyAdaptiveThreshold(originalImage);
                    break;
                case "Sobel Kenar Algılama":
                    processedImage = ApplySobelEdgeDetection(originalImage);
                    break;
                case "Toplama":
                    processedImage = ApplyAddImages(originalImage, originalImage2);
                    break;
                case "Çarpma":
                    processedImage = ApplyMultiplyImages(originalImage, originalImage2);
                    break;
                case "Gaussian Blur":
                    processedImage = ApplyGaussianBlur(originalImage);
                    break;
                case "90 Derece Döndür":
                    Bitmap inputImage = pictureBox2.Image != null ? new Bitmap(pictureBox2.Image) : originalImage;
                    processedImage = ApplyRotate90(inputImage);
                    break;
                case "Parlaklık Artır":
                    {
                        Bitmap selectedImage;

                        // Eğer işlem yapılmışsa onu kullan, yoksa orijinali
                        if (pictureBox2.Image != null)
                        {
                            selectedImage = new Bitmap(pictureBox2.Image);
                        }
                        else
                        {
                            selectedImage = new Bitmap(originalImage);
                        }

                        processedImage = ApplyBrightness(selectedImage);
                        break;
                    }

                case "Genişleme (Dilation)":
                    {
                        Bitmap selectedImage = pictureBox2.Image != null ? new Bitmap(pictureBox2.Image) : originalImage;
                        processedImage = ApplyDilation(selectedImage);
                        break;
                    }

                case "Aşınma (Erosion)":
                    {
                        Bitmap selectedImage = pictureBox2.Image != null ? new Bitmap(pictureBox2.Image) : originalImage;
                        processedImage = ApplyErosion(selectedImage);
                        break;
                    }
                case "Açma (Opening)":
                    {
                        Bitmap selectedImage = pictureBox2.Image != null ? new Bitmap(pictureBox2.Image) : originalImage;
                        processedImage = ApplyOpening(selectedImage);
                        break;
                    }
                case "Kapama (Closing)":
                    {
                        Bitmap selectedImage = pictureBox2.Image != null ? new Bitmap(pictureBox2.Image) : originalImage;
                        processedImage = ApplyClosing(selectedImage);
                        break;
                    }


                default:
                    MessageBox.Show("Seçilen işlem desteklenmiyor.");
                    return;
               

            }

            pictureBox2.Image = ResizeImage(processedImage, 256, 256);
        }


        private void UpdateHistogram(Bitmap image)
        {
            int[] histGray = new int[256];

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color p = image.GetPixel(x, y);
                    int gray = (int)(p.R * 0.299 + p.G * 0.587 + p.B * 0.114);
                    gray = Math.Max(0, Math.Min(255, gray)); // Clamp alternatifi
                    histGray[gray]++;
                }
            }

            chart1.Series.Clear();

            var sGray = chart1.Series.Add("Gray");
            sGray.ChartType = SeriesChartType.Column;
            sGray.Color = Color.Gray;
            sGray.IsXValueIndexed = true;

            for (int i = 0; i < 256; i++)
            {
                sGray.Points.AddXY(i, histGray[i]);
            }

            var area = chart1.ChartAreas[0];
            area.AxisX.Title = "Intensity";
            area.AxisX.Minimum = 0;
            area.AxisX.Maximum = 255;
            area.AxisX.Interval = 50;
            area.AxisX.MajorGrid.Enabled = false;

            area.AxisY.Title = "Pixel Count";
            area.AxisY.Minimum = 0;

            // Her resim için dinamik Y maksimumu belirle ve değeri yuvarla
            int maxCount = histGray.Max();
            int roundedMax = ((maxCount + (int)(maxCount * 0.1)) / 100 + 1) * 100; // Y eksenini en yakın 100'e yuvarla
            area.AxisY.Maximum = roundedMax;
            area.AxisY.MajorGrid.Enabled = false;

            // Kaydırma varsa iptal et
            area.AxisX.ScrollBar.Enabled = false;
            area.AxisY.ScrollBar.Enabled = false;

            // Border (sınır) özelliklerini ekle
            area.BorderColor = Color.Black;      // Sınır rengi
            area.BorderWidth = 1;                // Sınır genişliği
            area.BorderDashStyle = ChartDashStyle.Solid; // Sınır stili
        }


        private Bitmap ApplyGrayscale(Bitmap source)
        {
            Bitmap grayImage = new Bitmap(source.Width, source.Height);

            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    Color pixel = source.GetPixel(x, y);
                    int gray = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
                    gray = Math.Max(0, Math.Min(255, gray));
                    Color newPixel = Color.FromArgb(gray, gray, gray);
                    grayImage.SetPixel(x, y, newPixel);
                }
            }

            return grayImage;
        }

        private Bitmap ApplyBinary(Bitmap source, int threshold)
        {
            Bitmap binaryImage = new Bitmap(source.Width, source.Height);

            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    Color pixel = source.GetPixel(x, y);
                    int gray = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
                    Color newPixel = gray >= threshold ? Color.FromArgb(255, 255, 255) : Color.FromArgb(0, 0, 0);
                    binaryImage.SetPixel(x, y, newPixel);
                }
            }




            return binaryImage;
        }

        private Bitmap ApplyHSVChannel(Bitmap source, string channel)
        {
            Bitmap hsvImage = new Bitmap(source.Width, source.Height);

            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    Color pixel = source.GetPixel(x, y);

                    // RGB'den HSV'ye dönüşüm
                    double r = pixel.R / 255.0;
                    double g = pixel.G / 255.0;
                    double b = pixel.B / 255.0;

                    double max = Math.Max(r, Math.Max(g, b));
                    double min = Math.Min(r, Math.Min(g, b));
                    double delta = max - min;

                    double h = 0;
                    if (delta != 0)
                    {
                        if (max == r)
                            h = 60 * (((g - b) / delta) % 6);
                        else if (max == g)
                            h = 60 * (((b - r) / delta) + 2);
                        else
                            h = 60 * (((r - g) / delta) + 4);
                    }
                    if (h < 0) h += 360;

                    double s = (max == 0) ? 0 : delta / max;
                    double v = max;

                    byte hValue = (byte)(h / 360 * 255); // Hue: 0-255
                    byte sValue = (byte)(s * 255);       // Saturation: 0-255
                    byte vValue = (byte)(v * 255);       // Value: 0-255

                    // HSV birleşik renkli çıktı
                    Color newPixel = Color.FromArgb(hValue, sValue, vValue);

                    hsvImage.SetPixel(x, y, newPixel);
                }
            }

            return hsvImage;
        }

        private Bitmap ApplyCMYChannel(Bitmap source, string channel)
        {
            Bitmap cmyImage = new Bitmap(source.Width, source.Height);

            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    Color pixel = source.GetPixel(x, y);

                    // RGB'den CMY'ye dönüşüm
                    double r = pixel.R / 255.0;
                    double g = pixel.G / 255.0;
                    double b = pixel.B / 255.0;

                    // CMY hesaplamaları
                    double c = 1 - r;
                    double m = 1 - g;
                    double yVal = 1 - b;

                    // CMY renk bileşenlerinin 0-255 aralığına dönüştürülmesi
                    byte cyan = (byte)(c * 255);
                    byte magenta = (byte)(m * 255);
                    byte yellow = (byte)(yVal * 255);

                    // CMY birleşik görüntü oluştur
                    Color newPixel = Color.FromArgb(cyan, magenta, yellow);
                    cmyImage.SetPixel(x, y, newPixel);
                }
            }

            return cmyImage;
        }


        private Bitmap ApplyYCbCrChannel(Bitmap source, string channel)
        {
            Bitmap ycbcrImage = new Bitmap(source.Width, source.Height);

            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    Color pixel = source.GetPixel(x, y);

                    // RGB'den YCbCr'ye dönüşüm
                    double r = pixel.R;
                    double g = pixel.G;
                    double b = pixel.B;

                    double Y = 0.299 * r + 0.587 * g + 0.114 * b;
                    double Cb = -0.168736 * r - 0.331264 * g + 0.5 * b + 128;
                    double Cr = 0.5 * r - 0.419636 * g - 0.081312 * b + 128;

                    byte yValue = (byte)(Math.Max(0, Math.Min(255, Y)));
                    byte cbValue = (byte)(Math.Max(0, Math.Min(255, Cb)));
                    byte crValue = (byte)(Math.Max(0, Math.Min(255, Cr)));

                    // YCbCr birleşik renkli görüntü oluştur
                    Color newPixel = Color.FromArgb(yValue, cbValue, crValue);

                    ycbcrImage.SetPixel(x, y, newPixel);
                }
            }

            return ycbcrImage;
        }

        private Bitmap ApplyHistogramGerme(Bitmap inputImage, int c, int d, int gmin, int gmax)
        {
            Bitmap outputImage = new Bitmap(inputImage.Width, inputImage.Height);

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    Color pixel = inputImage.GetPixel(x, y);

                    int newR = (int)((pixel.R - c) * ((double)(gmax - gmin) / (d - c)) + gmin);
                    int newG = (int)((pixel.G - c) * ((double)(gmax - gmin) / (d - c)) + gmin);
                    int newB = (int)((pixel.B - c) * ((double)(gmax - gmin) / (d - c)) + gmin);

                    // 0-255 arasında tut
                    newR = Math.Max(0, Math.Min(255, newR));
                    newG = Math.Max(0, Math.Min(255, newG));
                    newB = Math.Max(0, Math.Min(255, newB));

                    Color newPixel = Color.FromArgb(newR, newG, newB);
                    outputImage.SetPixel(x, y, newPixel);
                }
            }

            return outputImage;
        }


        private Bitmap ApplyHistogramGenisletme(Bitmap inputImage)
        {
            int width = inputImage.Width;
            int height = inputImage.Height;
            int L = 256;

            Bitmap outputImage = new Bitmap(width, height);

            int rMin = 255, rMax = 0;
            int gMin = 255, gMax = 0;
            int bMin = 255, bMax = 0;

            // Her kanal için min ve max değerlerini bul
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = inputImage.GetPixel(x, y);

                    if (pixel.R < rMin) rMin = pixel.R;
                    if (pixel.R > rMax) rMax = pixel.R;

                    if (pixel.G < gMin) gMin = pixel.G;
                    if (pixel.G > gMax) gMax = pixel.G;

                    if (pixel.B < bMin) bMin = pixel.B;
                    if (pixel.B > bMax) bMax = pixel.B;
                }
            }

            // Genişletme işlemi
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = inputImage.GetPixel(x, y);

                    int r = pixel.R;
                    int g = pixel.G;
                    int b = pixel.B;

                    // Kanal kanal genişletiyoruz
                    double rNew = ((double)(L - 1) / (rMax - rMin)) * (r - rMin);
                    double gNew = ((double)(L - 1) / (gMax - gMin)) * (g - gMin);
                    double bNew = ((double)(L - 1) / (bMax - bMin)) * (b - bMin);

                    int rFinal = (int)Math.Max(0, Math.Min(255, rNew));
                    int gFinal = (int)Math.Max(0, Math.Min(255, gNew));
                    int bFinal = (int)Math.Max(0, Math.Min(255, bNew));

                    Color newPixel = Color.FromArgb(rFinal, gFinal, bFinal);
                    outputImage.SetPixel(x, y, newPixel);
                }
            }

            return outputImage;
        }

        private Bitmap ResizeImage(Bitmap original, int width, int height)
        {
            return new Bitmap(original, new Size(width, height));
        }
        

        //---------------------------------------------------------------------------------------------------------------------------

        private Bitmap ApplySaltAndPepperNoise(Bitmap image, double noiseLevel = 0.15)//Random şekilde pikselleri siyah ya da beyaz olarak değiştiriyor
        {
            Random rand = new Random();
            Bitmap noisyImage = new Bitmap(image);

            for (int x = 0; x < noisyImage.Width; x++)
            {
                for (int y = 0; y < noisyImage.Height; y++)
                {
                    if (rand.NextDouble() < noiseLevel)
                    {
                        Color noiseColor = rand.NextDouble() < 0.5 ? Color.Black : Color.White;
                        noisyImage.SetPixel(x, y, noiseColor);
                    }
                }
            }

            return noisyImage;

        }

        private Bitmap ApplyMeanFilter(Bitmap image)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);

            int[,] kernel = {//3x3 ortalama bir kernel alıyoruz
            { 1, 1, 1 },
            { 1, 1, 1 },
            { 1, 1, 1 }
    };
            int kernelSize = 3;
            int kernelSum = 9;

            int offset = kernelSize / 2;
            //her pikseli dolaşıyoruz
            for (int y = offset; y < image.Height - offset; y++)
            {
                for (int x = offset; x < image.Width - offset; x++)
                {//renk toplamalrı 
                    int sumR = 0, sumG = 0, sumB = 0;

                    for (int ky = -offset; ky <= offset; ky++)
                    {
                        for (int kx = -offset; kx <= offset; kx++)
                        {//RGB değerlerini kernal ile çarpıyoruz
                            Color neighbor = image.GetPixel(x + kx, y + ky);
                            sumR += neighbor.R * kernel[ky + offset, kx + offset];
                            sumG += neighbor.G * kernel[ky + offset, kx + offset];
                            sumB += neighbor.B * kernel[ky + offset, kx + offset];
                        }
                    }
                    //Renk kanal ortalamaları
                    int avgR = sumR / kernelSum;
                    int avgG = sumG / kernelSum;
                    int avgB = sumB / kernelSum;
                    //Yeni piksellere ortalamaları yaz
                    result.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                }
            }

            return result;
        }

        private Bitmap ApplyMedianFilter(Bitmap image)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);
            int radius = 1; // 3x3 için yarıçap
            //pikselleri dolaşma
            for (int y = radius; y < image.Height - radius; y++)
            {
                for (int x = radius; x < image.Width - radius; x++)
                {//RGB değerlerini ayrı ayrı listele
                    var neighborhoodR = new List<byte>();
                    var neighborhoodG = new List<byte>();
                    var neighborhoodB = new List<byte>();
                    // 3x3 komşu pikselleri dolaş
                    for (int j = -radius; j <= radius; j++)
                    {
                        for (int i = -radius; i <= radius; i++)
                        {
                            Color neighbor = image.GetPixel(x + i, y + j);
                            neighborhoodR.Add(neighbor.R);
                            neighborhoodG.Add(neighbor.G);
                            neighborhoodB.Add(neighbor.B);
                        }
                    }

                    neighborhoodR.Sort();
                    neighborhoodG.Sort();
                    neighborhoodB.Sort();
                    // Ortanca değeri al
                    byte medianR = neighborhoodR[neighborhoodR.Count / 2];
                    byte medianG = neighborhoodG[neighborhoodG.Count / 2];
                    byte medianB = neighborhoodB[neighborhoodB.Count / 2];
                    // Bu ortanca değerlerle yeni pikseli oluştur ve sonuç görüntüsüne yerleştir
                    result.SetPixel(x, y, Color.FromArgb(medianR, medianG, medianB));
                }
            }

            return result;
        }

        private byte[,] GetGrayscaleMatrix(Bitmap image) //eşiklemedeki işlem yükünü azaltmak için gray diziyi burada alıyoruz
        {
            int width = image.Width;
            int height = image.Height;
            byte[,] gray = new byte[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    byte value = (byte)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
                    gray[x, y] = value;
                }
            }

            return gray;
        }

        private Bitmap ApplyAdaptiveThreshold(Bitmap image, int blockSize = 15, int c = 5) //blocksize= pikselin test edileceği çevredeki matrisin boyutu, int c = ortalamanın ne kadar altı ise siyah yapacağız 
        {
            int width = image.Width;
            int height = image.Height;
            // Gri tonlu resmi alıyoruz (yukarıdaki fonksiyondan)
            byte[,] gray = GetGrayscaleMatrix(image);
            Bitmap result = new Bitmap(width, height);
            int halfBlock = blockSize / 2;
            // Tüm pikselleri dolaş
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int sum = 0;
                    int count = 0;
                    // x,y pikselinin çevresindeki blockSize x blockSize alanı dolaş
                    for (int j = -halfBlock; j <= halfBlock; j++)
                    {
                        for (int i = -halfBlock; i <= halfBlock; i++)
                        {
                            int nx = x + i;
                            int ny = y + j;
                            // Görüntü sınırlarını kontrol et
                            if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                            {
                                sum += gray[nx, ny];
                                count++;
                            }
                        }
                    }
                    // Komşuların ortalamasını al
                    int average = sum / count;
                    byte pixelValue = gray[x, y];

                    if (pixelValue < average - c) // Eşikleme: eğer ortalamanın altında ise siyah, değilse beyaz
                        result.SetPixel(x, y, Color.Black);
                    else
                        result.SetPixel(x, y, Color.White);
                }
            }

            return result;
        }

        private Bitmap ApplySobelEdgeDetection(Bitmap image)
        {//gray haline çevirmek için üstteki fonksiyonu kullanzıyoruz
            Bitmap grayImage = ApplyGrayscale(image);
            Bitmap result = new Bitmap(image.Width, image.Height);

            // Sobel kernel tanımları
            int[,] gx = new int[,]//yataydaki kenar değişimleri
            {
            { -1, 0, 1 },
            { -2, 0, 2 },
            { -1, 0, 1 }
            };

            int[,] gy = new int[,]//dikeydeki değişimleri
            {
            { -1, -2, -1 },
            {  0,  0,  0 },
            {  1,  2,  1 }
            };
            //görüntüde gezme
            for (int y = 1; y < image.Height - 1; y++)
            {
                for (int x = 1; x < image.Width - 1; x++)
                {
                    int pixelGx = 0;
                    int pixelGy = 0;

                   
                    for (int j = -1; j <= 1; j++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            int pixelVal = grayImage.GetPixel(x + i, y + j).R;// Gri piksel değeri al
                            // Gx ve Gy çarpımlarını topla
                            pixelGx += gx[j + 1, i + 1] * pixelVal;
                            pixelGy += gy[j + 1, i + 1] * pixelVal;
                        }
                    }

                    // Kenar gücünü hesapla (gradyan büyüklüğü = √(Gx² + Gy²) formülü)
                    int magnitude = (int)Math.Sqrt(pixelGx * pixelGx + pixelGy * pixelGy);
                    // 0–255 aralığına sınırla (clamp)
                    magnitude = Math.Max(0, Math.Min(255, magnitude)); 

                    result.SetPixel(x, y, Color.FromArgb(magnitude, magnitude, magnitude));
                }
            }

            return result;
        }



        /////////////////#039

        private Bitmap ApplyAddImages(Bitmap img1, Bitmap img2)
        {
            if (img1.Size != img2.Size)
                throw new ArgumentException("Resimler aynı boyutta olmalı!");

            Bitmap result = new Bitmap(img1.Width, img1.Height);
            for (int y = 0; y < img1.Height; y++)
            {
                for (int x = 0; x < img1.Width; x++)
                {
                    Color p1 = img1.GetPixel(x, y);
                    Color p2 = img2.GetPixel(x, y);
                    int r = Math.Min(p1.R + p2.R, 255);
                    int g = Math.Min(p1.G + p2.G, 255);
                    int b = Math.Min(p1.B + p2.B, 255);
                    result.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            return result;
        }

        private Bitmap ApplyMultiplyImages(Bitmap img1, Bitmap img2)
        {
            if (img1.Size != img2.Size)
                throw new ArgumentException("Resimler aynı boyutta olmalı!");

            Bitmap result = new Bitmap(img1.Width, img1.Height);
            for (int y = 0; y < img1.Height; y++)
            {
                for (int x = 0; x < img1.Width; x++)
                {
                    Color p1 = img1.GetPixel(x, y);
                    Color p2 = img2.GetPixel(x, y);
                    int r = (p1.R * p2.R) / 255;
                    int g = (p1.G * p2.G) / 255;
                    int b = (p1.B * p2.B) / 255;
                    result.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            return result;
        }

        private double[,] GenerateGaussianKernel(int kernelSize, double sigma)
        {
            double[,] kernel = new double[kernelSize, kernelSize];
            double sum = 0;
            int radius = kernelSize / 2;

            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    double value = Math.Exp(-(x * x + y * y) / (2 * sigma * sigma));
                    kernel[y + radius, x + radius] = value;
                    sum += value;
                }
            }

            // Kernel normalizasyonu
            for (int i = 0; i < kernelSize; i++)
            {
                for (int j = 0; j < kernelSize; j++)
                {
                    kernel[i, j] /= sum;
                }
            }

            return kernel;
        }


        private Bitmap ApplyGaussianBlur(Bitmap image, int kernelSize = 5, double sigma = 7.5)
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height);
            double[,] kernel = GenerateGaussianKernel(kernelSize, sigma);
            int radius = kernelSize / 2;

            for (int y = radius; y < image.Height - radius; y++)
            {
                for (int x = radius; x < image.Width - radius; x++)
                {
                    double sumR = 0, sumG = 0, sumB = 0;
                    for (int ky = -radius; ky <= radius; ky++)
                    {
                        for (int kx = -radius; kx <= radius; kx++)
                        {
                            Color p = image.GetPixel(x + kx, y + ky);
                            double weight = kernel[ky + radius, kx + radius];
                            sumR += p.R * weight;
                            sumG += p.G * weight;
                            sumB += p.B * weight;
                        }
                    }
                    blurred.SetPixel(x, y, Color.FromArgb((int)sumR, (int)sumG, (int)sumB));
                }
            }
            return blurred;
        }

        private Bitmap ApplyRotate90(Bitmap image)
        {
            Bitmap rotated = (Bitmap)image.Clone();
            rotated.RotateFlip(RotateFlipType.Rotate90FlipNone);
            return rotated;
        }

        /*bu alanda yakınlaştırma uzaklaştırma ekik
        *
        *
        *
        *
        */
        private Bitmap ApplyBrightness(Bitmap image, int increaseAmount = 30)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);

                    int r = Math.Min(255, pixel.R + increaseAmount);
                    int g = Math.Min(255, pixel.G + increaseAmount);
                    int b = Math.Min(255, pixel.B + increaseAmount);

                    result.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return result;
        }

        //----------------------------------
        private Bitmap ApplyDilation(Bitmap src)//Genişleme
        {
            Bitmap result = new Bitmap(src.Width, src.Height);

            for (int y = 1; y < src.Height - 1; y++)
            {
                for (int x = 1; x < src.Width - 1; x++)
                {
                    int maxR = 0, maxG = 0, maxB = 0;

                    for (int j = -1; j <= 1; j++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            Color pixel = src.GetPixel(x + i, y + j);
                            maxR = Math.Max(maxR, pixel.R);
                            maxG = Math.Max(maxG, pixel.G);
                            maxB = Math.Max(maxB, pixel.B);
                        }
                    }

                    Color newColor = Color.FromArgb(maxR, maxG, maxB);
                    result.SetPixel(x, y, newColor);
                }
            }

            return result;
        }

        private Bitmap ApplyErosion(Bitmap src)//Aşınma
        {
            Bitmap result = new Bitmap(src.Width, src.Height);

            for (int y = 1; y < src.Height - 1; y++)
            {
                for (int x = 1; x < src.Width - 1; x++)
                {
                    int minR = 255, minG = 255, minB = 255;

                    for (int j = -1; j <= 1; j++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            Color pixel = src.GetPixel(x + i, y + j);
                            minR = Math.Min(minR, pixel.R);
                            minG = Math.Min(minG, pixel.G);
                            minB = Math.Min(minB, pixel.B);
                        }
                    }

                    Color newColor = Color.FromArgb(minR, minG, minB);
                    result.SetPixel(x, y, newColor);
                }
            }

            return result;
        }

        private Bitmap ApplyOpening(Bitmap src)
        {
            Bitmap eroded = ApplyErosion(src);      // İlk olarak aşındır
            Bitmap opened = ApplyDilation(eroded);  // Sonra genişlet
            return opened;
        }
        private Bitmap ApplyClosing(Bitmap src)
        {
            Bitmap dilated = ApplyDilation(src);     // Önce genişlet
            Bitmap closed = ApplyErosion(dilated);   // Sonra aşındır
            return closed;
        }


    }
}

 