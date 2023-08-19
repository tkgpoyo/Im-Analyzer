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

namespace Im_Analyzer.ViewModels
{
    class SelectPageViewModel : ViewModelBase
    {
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
            get { return _snapshotCommand ?? (_snapshotCommand = new _DelegateCommand(Snapshot)); }
        }

        private async void StartCapture()
        {
            camera = new Camera();
            await ShowImage();
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
