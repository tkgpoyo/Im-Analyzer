using OpenCvSharp;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using OpenCvSharp.WpfExtensions;
using System.Windows;

namespace Im_Analyzer.Models.Analyzer
{
    internal class DetectColor
    {
        public Mat Source { get; private set; }

        public DetectColor(Mat source)
        {
            this.Source = source;
        }

        public PlotModel ColorGraphModel(string rgb)
        {
            Mat dst = new Mat(this.Source.Size(), MatType.CV_8UC3);
            this.Source.ConvertTo(dst, MatType.CV_8UC3);

            int[] data = Enumerable.Repeat<int>(0, 256).ToArray();
            this.ColorList(data, rgb);
            // 配列dataにR,G,Bのうち文字列rgbで指定した色の0～255の出現回数が格納される

            PlotModel model = new PlotModel();

            model.Axes.Add(new OxyPlot.Axes.LinearAxis() { Position = AxisPosition.Left });
            model.Axes.Add(new OxyPlot.Axes.LinearAxis { Position = AxisPosition.Bottom });

            LineSeries series = new LineSeries();
            series.Title = $"{rgb}";
            switch (rgb)
            {
                case "R":
                    series.Color = OxyColor.FromRgb(255, 0, 0);
                    break;
                case "G":
                    series.Color = OxyColor.FromRgb(0, 255, 0);
                    break;
                case "B":
                    series.Color = OxyColor.FromRgb(0, 0, 255);
                    break;
            }

            for (int i = 0; i < data.Length; i++)
            {
                series.Points.Add(new DataPoint(i, data[i]));
            }

            model.Series.Add(series);

            return model;

        }

        private void ColorList(int[] data, string rgb)
        {
            int col_index = -1, pixel;
            
            switch (rgb)
            {
                case "R":
                    col_index = 2;
                    break;
                case "G":
                    col_index = 1;
                    break;
                case "B":
                    col_index = 0;
                    break;
            }

            for (int y = 0; y < this.Source.Rows;  y++)
            {
                for (int x = 0; x < this.Source.Cols; x++)
                {
                    pixel = this.Source.At<Vec3b>(y, x)[col_index];
                    data[pixel]++;
                }
            }

        }

        public BitmapSource Filter(Vec3b col)
        {
            Vec3b inverted_col = new Vec3b(Convert.ToByte(255 - col[0]), Convert.ToByte(255 - col[1]), Convert.ToByte(255 - col[2]));

            double[] Lab;
            double[] base_Lab = BGRtoLab(col);

            double _L = base_Lab[0];
            double _a = base_Lab[1];
            double _b = base_Lab[2];

            using (Mat dst = this.Source.Clone())
            {
                double L, a, b, Ld, ad, bd, Delta_E;
                Vec3b pixel;

                for (int y = 0; y < this.Source.Rows; y++)
                {
                    for (int x = 0; x <= this.Source.Cols; x++)
                    {
                        pixel = this.Source.At<Vec3b>(y, x);
                        Lab = BGRtoLab(pixel);

                        L = Lab[0];
                        a = Lab[1];
                        b = Lab[2];

                        Ld = L - _L;
                        ad = a - _a;
                        bd = b - _b;

                        Delta_E = Math.Sqrt(Ld * Ld + ad * ad + bd * bd);

                        if (Delta_E > 30.0)
                            dst.Set(y, x, inverted_col);

                    }
                }
                return BitmapSourceConverter.ToBitmapSource(dst);
            }
        }

        private double[] BGRtoLab(Vec3b rgb)
        {
            double[] rgb_l = BGRtoBGRlinear(rgb);
            
            double x = 0.4124 * rgb_l[2] + 0.3576 * rgb_l[1] + 0.1805 * rgb_l[0];
            double y = 0.2126 * rgb_l[2] + 0.7152 * rgb_l[1] + 0.0722 * rgb_l[0];
            double z = 0.0193 * rgb_l[2] + 0.1192 * rgb_l[1] + 0.9505 * rgb_l[0];

            double Xn = 95.039;
            double Yn = 100;
            double Zn = 108.88;

            double L = 116.0 * Func(100 * y / Yn) - 16;
            double a = 500.0 * (Func(100 * x / Xn) - Func(100 * y / Yn));
            double b = 200.0 * (Func(100 * y / Yn) - Func(100 * z / Zn));

            return new double[] { L, a, b };

        }

        private double[] BGRtoBGRlinear(Vec3b rgb)
        {
            double b = (double)rgb[0] / 255.0 <= 0.04045 ? (double)rgb[0] / (255.0 * 12.92) : Math.Pow(((double)rgb[0] / 255.0 + 0.055) / 1.055, 2.4);
            double g = (double)rgb[1] / 255.0 <= 0.04045 ? (double)rgb[1] / (255.0 * 12.92) : Math.Pow(((double)rgb[1] / 255.0 + 0.055) / 1.055, 2.4);
            double r = (double)rgb[2] / 255.0 <= 0.04045 ? (double)rgb[2] / (255.0 * 12.92) : Math.Pow(((double)rgb[2] / 255.0 + 0.055) / 1.055, 2.4);

            return new double[] { b, g, r };

        }

        private double Func(double x)
        {
            return x <= Math.Pow(6.0 / 29.0, 3) ? (1.0 / 3.0) * Math.Pow(29.0 / 6.0, 2) * x + 4.0 / 29.0 : Math.Pow(x, 1.0 / 3.0);
        }

    }
}
