using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Windows.Media.Imaging;

namespace Im_Analyzer.Models.Analyzer
{
    internal class Labeling
    {
        private Mat _source;
        public Mat Source
        {
            get { return _source; }
            set { _source = value; }
        }

        public Labeling(Mat mat) { 
            this.Source = mat.Clone();
        }

        public BitmapSource LabelProcess(string mode)
        {
            using (Mat bin = new Mat(Source.Size(), MatType.CV_8UC1))
            {
                // Sourceイメージの2値化
                Cv2.CvtColor(Source, bin, ColorConversionCodes.BGR2GRAY);
                Cv2.Threshold(bin, bin, 0, 255, ThresholdTypes.Otsu);

                int rows = bin.Rows;
                int columns = bin.Cols;

                using (var dst = this.Source.Clone())
                using (var label = new Mat<int>())
                using (var stats = new Mat<int>())
                using (var centroids = new Mat<double>())
                {
                    // ラベリング
                    var nlabels = Cv2.ConnectedComponentsWithStats(bin, label, stats, centroids, PixelConnectivity.Connectivity8, MatType.CV_32SC1);

                    switch (mode)
                    {
                        case "coloring":
                            // ランダムな色の生成
                            Vec3b[] col_list = new Vec3b[nlabels];
                            for (int i = 0; i < nlabels; i++)
                            {
                                col_list[i] = RandomColor();
                            }
                            // 0ラベル(背景)は黒色にする
                            col_list[0] = new Vec3b(0, 0, 0);


                            var labelIndexer = label.GetGenericIndexer<int>();
                            var dstIndexer = dst.GetGenericIndexer<Vec3b>();

                            for (int y = 0; y < rows; y++)
                            {
                                for (int x = 0; x < columns; x++)
                                {
                                    int labelValue = labelIndexer[y, x];
                                    dstIndexer[y, x] = col_list[labelValue];
                                }
                            }

                            return BitmapSourceConverter.ToBitmapSource(dst);

                        case "identify":
                            var area = rows * columns;
                            var statsIndexer = stats.GetGenericIndexer<int>();

                            for (int i = 0; i < nlabels; i++)
                            {
                                if (area * 0.0001 < statsIndexer[i, 4])
                                    dst.Rectangle(new OpenCvSharp.Rect(statsIndexer[i, 0], statsIndexer[i, 1], statsIndexer[i, 2], statsIndexer[i, 3]), new Scalar(128, 255, 128));
                            }

                            return BitmapSourceConverter.ToBitmapSource(dst);

                        default:
                            return BitmapSourceConverter.ToBitmapSource(this.Source);
                    }
                    
                }

            }

        }

        private Vec3b RandomColor()
        {
            Random rnd = new Random();
            
            uint r = (uint)rnd.Next(0, 255);
            uint g = (uint)rnd.Next(0, 255);
            uint b = (uint)rnd.Next(0, 255);

            return new Vec3b(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
        }
    }
}
