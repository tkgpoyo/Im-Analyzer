using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Windows.Media.Imaging;

namespace Im_Analyzer.Models
{
    class Camera
    {
        public VideoCapture capture;
        private Mat frame;

        public Camera(ref int status)
        {
            try
            {
                capture = new VideoCapture(0);
                if (!capture.IsOpened())
                    throw new Exception("capture initialization failed");
                frame = new Mat();

            } catch (Exception e)
            {
                status = -1;
            }
        }

        public BitmapSource Capture()
        {
            capture.Read(frame);
            if (frame.Empty())
                return null;

            return BitmapSourceConverter.ToBitmapSource(frame);
        }
    }
}
