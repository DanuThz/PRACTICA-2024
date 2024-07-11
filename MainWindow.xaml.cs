using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PRACTICA2024PICTURE
{
    public partial class MainWindow : Window
    {
        private BitmapImage? _originalImage;
        private Bitmap? _editableBitmap;

        private float _brightness = 0;
        private float _contrast = 0;
        private string _textToAdd = "";
        private float _fontSize = 20;
        private int _positionX = 0;
        private int _positionY = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                _originalImage = new BitmapImage(new Uri(openFileDialog.FileName));
                DisplayedImage.Source = _originalImage;
                _editableBitmap = new Bitmap(openFileDialog.FileName);
                WidthSlider.Value = _editableBitmap.Width;
                HeightSlider.Value = _editableBitmap.Height;
            }
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png";
            if (saveFileDialog.ShowDialog() == true)
            {
                _editableBitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
            }
        }

        private void ResetImage_Click(object sender, RoutedEventArgs e)
        {
            if (_originalImage != null)
            {
                _editableBitmap = BitmapImageToBitmap(_originalImage);
                UpdateDisplayedImage();
            }
        }

        private Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        private void RotateImage_Click(object sender, RoutedEventArgs e)
        {
            _editableBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            UpdateDisplayedImage();
        }

        private void ApplySepia_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter(GetSepiaColorMatrix());
        }

        private void ApplyBlackAndWhite_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter(GetBlackAndWhiteColorMatrix());
        }

        private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _brightness = (float)BrightnessSlider.Value;
            ApplyBrightness(_brightness);
        }

        private void ContrastSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _contrast = (float)ContrastSlider.Value;
            ApplyContrast(_contrast);
        }

        private void WidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_editableBitmap != null)
            {
                ResizeImage((int)WidthSlider.Value, _editableBitmap.Height);
            }
        }

        private void HeightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_editableBitmap != null)
            {
                ResizeImage(_editableBitmap.Width, (int)HeightSlider.Value);
            }
        }

        private void TextToAdd_TextChanged(object sender, TextChangedEventArgs e)
        {
            _textToAdd = TextToAdd.Text;
        }

        private void FontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _fontSize = (float)FontSizeSlider.Value;
        }

        private void PositionXSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _positionX = (int)PositionXSlider.Value;
        }

        private void PositionYSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _positionY = (int)PositionYSlider.Value;
        }

        private void AddText_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_textToAdd))
            {
                MessageBox.Show("Text de adăugat nu poate fi gol.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (Graphics g = Graphics.FromImage(_editableBitmap))
            {
                using (Font font = new Font("Arial", _fontSize))
                {
                    using (SolidBrush brush = new SolidBrush(System.Drawing.Color.White))
                    {
                        g.DrawString(_textToAdd, font, brush, new System.Drawing.PointF(_positionX, _positionY));
                    }
                }
            }
            UpdateDisplayedImage();
        }

        private void UpdateDisplayedImage()
        {
            using (MemoryStream memory = new MemoryStream())
            {
                _editableBitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                DisplayedImage.Source = bitmapImage;
            }
        }

        private void ApplyFilter(ColorMatrix colorMatrix)
        {
            using (Graphics g = Graphics.FromImage(_editableBitmap))
            {
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);
                g.DrawImage(_editableBitmap, new Rectangle(0, 0, _editableBitmap.Width, _editableBitmap.Height),
                            0, 0, _editableBitmap.Width, _editableBitmap.Height, GraphicsUnit.Pixel, attributes);
            }
            UpdateDisplayedImage();
        }

        private void ApplyBrightness(float brightness)
        {
            // Implementați funcția de ajustare a luminozității
        }

        private void ApplyContrast(float contrast)
        {
            // Implementați funcția de ajustare a contrastului
        }

        private void ResizeImage(int newWidth, int newHeight)
        {
            Bitmap resizedBitmap = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(resizedBitmap))
            {
                g.DrawImage(_editableBitmap, 0, 0, newWidth, newHeight);
            }
            _editableBitmap = resizedBitmap;
            UpdateDisplayedImage();
        }

        private ColorMatrix GetSepiaColorMatrix()
        {
            return new ColorMatrix(new float[][]
            {
                new float[] { .393f, .349f, .272f, 0, 0 },
                new float[] { .769f, .686f, .534f, 0, 0 },
                new float[] { .189f, .168f, .131f, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { 0, 0, 0, 0, 1 }
            });
        }

        private ColorMatrix GetBlackAndWhiteColorMatrix()
        {
            return new ColorMatrix(new float[][]
            {
                new float[] { .3f, .3f, .3f, 0, 0 },
                new float[] { .59f, .59f, .59f, 0, 0 },
                new float[] { .11f, .11f, .11f, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { 0, 0, 0, 0, 1 }
            });
        }
    }
}
