using Microsoft.Win32;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Im_Analyzer.Models.Save
{
    class Save
    {
        public static void SaveImage(BitmapSource img)
        {
                
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Bitmapファイル|*.bmp|Pngファイル|*.png|JPegファイル|*.jpg";
            var result = saveFileDialog.ShowDialog();

            switch (result)
            {
                case true:
                    switch (saveFileDialog.FilterIndex)
                    {
                        case 1:
                            Bitmap bmp = BitmapConverter.ToBitmap(BitmapSourceConverter.ToMat(img));
                            bmp.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;
                        case 2:
                            Bitmap png = BitmapConverter.ToBitmap(BitmapSourceConverter.ToMat(img));
                            png.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        case 3:
                            Bitmap jpg = BitmapConverter.ToBitmap(BitmapSourceConverter.ToMat(img));
                            jpg.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                    }

                    break;
                case false:
                    break;
            }
        }
    }
}
