using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.IO;
using Im_Analyzer.Models;
using Im_Analyzer.Common;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace Im_Analyzer.ViewModels
{
    class SelectPageViewModel : ViewModelBase
    {
        private int status = 1;
        Camera camera;
        bool isTask = true;

        public SelectPageViewModel()
        {
            StartCapture();
        }

        private BitmapSource _bmp;
        public BitmapSource Bmp
        {
            get => _bmp;
            set
            {
                if (_bmp != value)
                {
                    _bmp = value;
                    RaisePropertyChanged("Bmp");
                }
            }
        }

        private _DelegateCommand _snapshotCommand;
        public _DelegateCommand SnapshotCommand
        {
            get { return _snapshotCommand ?? (_snapshotCommand = new _DelegateCommand(Snapshot, CanExecuteSnapshot)); }
        }

        // SelectPageの左側にカメラの映像を映す関数
        // カメラの映像を取得できない場合、エラーメッセージ画像を出力
        private async void StartCapture()
        {
            camera = new Camera(ref status);
            // statusには、カメラを開けたかどうかのステータスが格納されている

            if (status == -1)
            {

                // カメラを開けなかった場合
                Bitmap bitmap = new Bitmap(@"..\..\..\Common\VideoCaptureError.bmp");
                IntPtr hbitmap = bitmap.GetHbitmap();

                Bmp = Imaging.CreateBitmapSourceFromHBitmap(
                    hbitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            else
            {
                // 開けた場合
                await ShowImage();
            }
        }

        private void Snapshot()
        {
            isTask = false;
            Thread.Sleep(1000);
            var result = MessageBox.Show("この写真でよろしいですか。", "画像の確認", MessageBoxButton.YesNo);

            // ユーザーの選択によって分岐する
            // Yes: 画像処理の段階へ
            // No : 撮り直し
            switch (result)
            {
                case MessageBoxResult.Yes:
                    /* 画面遷移の処理 */
                    break;
                case MessageBoxResult.No:
                    /* 撮り直し */
                    isTask = true;
                    StartCapture();
                    break;
            }
        }

        private bool CanExecuteSnapshot()
        {
            // カメラを開ければ写真撮影可能
            // 開けなければ写真撮影不可能
            if (status == 1)
                return true;
            else
                return false;
        }

        private async Task ShowImage()
        {
            while (isTask)
            {
                Bmp = camera.Capture();
                if (Bmp == null)
                    break;

                await Task.Delay(30);
            }
        }
    }
}
