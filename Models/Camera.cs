using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Im_Analyzer.Models
{
    class Camera
    {
        public VideoCapture capture;
        Mat frame;

        public Camera(ref int status)
        {
            try
            {
                capture = new VideoCapture(0);
                if (!capture.IsOpened())
                    throw new Exception("capture initialization failed");
                frame = new Mat();

            } catch (TypeInitializationException e)
            {
                status = -1;
            }
        }

        public BitmapSource Capture()
        {
            capture.Read(frame);
            if (frame.Empty())
                return null;

            IntPtr hBitmap = frame.ToBitmap().GetHbitmap();
            return Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
