using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows;
using Microsoft.Win32;
using Im_Analyzer.Models;
using Im_Analyzer.Views;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

namespace Im_Analyzer.ViewModels
{
    public class SelectPageViewModel : BindableBase, INavigationAware
    {
        private int status = 1;
        Camera camera;
        bool isTask;

        private IRegionManager _regionManager;

        private BitmapSource _bmp;
        public BitmapSource Bmp
        {
            get { return _bmp; }
            set { SetProperty(ref _bmp, value); }
        }

        public DelegateCommand SnapshotCommand { get; private set; }
        public DelegateCommand SelectImageCommand { get; private set; }
        public DelegateCommand BackToStartUpCommand { get; private set; }

        
        // コンストラクタ
        public SelectPageViewModel(IRegionManager regionManager)
        {
            // カメラの実行状態をOnにする
            isTask = true;
            // ページ遷移のためのregion設定
            _regionManager = regionManager;
            // 各ボタンのコマンド定義
            SnapshotCommand = new DelegateCommand(Snapshot, CanExecuteSnapshot);
            SelectImageCommand = new DelegateCommand(SelectImage);
            BackToStartUpCommand = new DelegateCommand(BackToStartUp);

        }



        // カメラから写真を撮るコマンド
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
                    // 遷移先のページに渡すデータの定義
                    var param = new NavigationParameters();
                    param.Add("Image", BitmapSourceConverter.ToMat(Bmp));
                    // 画面遷移の処理 
                    _regionManager.RequestNavigate("ContentRegion", nameof(FunctionPage), param);
                    break;
                case MessageBoxResult.No:
                    // 撮り直し 
                    isTask = true;
                    StartCapture();
                    break;
            }
        }

        // Snapshotコマンドの実行条件
        private bool CanExecuteSnapshot()
        {
            // カメラを開ければ写真撮影可能
            // 開けなければ写真撮影不可能
            if (status == 1 && isTask == true)
                return true;
            else
                return false;
        }


        // ローカルのフォルダから写真を選択するコマンド
        private void SelectImage()
        {
            isTask = false;

            var dialog = new OpenFileDialog();
            dialog.Filter = "画像ファイル(*.jpg,*.png)|*.jpg;*.png";

            var result = dialog.ShowDialog();

            isTask = true;
            switch (result)
            {
                case true:
                    // 遷移先のページに渡すデータの定義
                    var filepath = dialog.FileName;
                    var param = new NavigationParameters();
                    param.Add("Image", new Mat(filepath));
                    // 画面遷移
                    _regionManager.RequestNavigate("ContentRegion", nameof(FunctionPage), param);
                    break;
                case false:
                    isTask = true;
                    StartCapture();
                    break;
            }
        }


        // StartUpPageに戻るボタン
        private void BackToStartUp()
        {
            _regionManager.RequestNavigate("ContentRegion", nameof(StartUpPage));
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
        

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            // 特になし
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            isTask = true;
            StartCapture();
        }
    }
}