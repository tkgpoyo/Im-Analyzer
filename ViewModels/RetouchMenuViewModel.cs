using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Microsoft.Win32;
using System.Drawing;
using OpenCvSharp.Extensions;

namespace Im_Analyzer.ViewModels
{
	public class RetouchMenuViewModel : BindableBase, INavigationAware
	{
		private IRegionManager _regionManager;
		
		private BitmapSource _img;
		public BitmapSource Img
		{
			get { return _img; }
			set { SetProperty(ref _img, value); }
		}

		private BitmapSource original_bmp;

		public DelegateCommand<string> NavigateCommand { get; private set; }
		public DelegateCommand SaveCommand { get; private set; }
		public DelegateCommand ResetCommand { get; private set; }

		public RetouchMenuViewModel(IRegionManager regionManager)
		{
			_regionManager = regionManager;
			NavigateCommand = new DelegateCommand<string>(Navigate);
			SaveCommand = new DelegateCommand(Save);
			ResetCommand = new DelegateCommand(Reset);
		}

		private void Navigate(string navigatePath)
		{
			var param = new NavigationParameters();
			param.Add("Image", BitmapSourceConverter.ToMat(Img));
			Models.Navigation.Navigation.NavigateToPath(_regionManager, "ContentRegion", navigatePath, param);
		}

		private void Save()
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
							Bitmap bmp = BitmapConverter.ToBitmap(BitmapSourceConverter.ToMat(Img));
							bmp.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
							break;
						case 2:
                            Bitmap png = BitmapConverter.ToBitmap(BitmapSourceConverter.ToMat(Img));
							png.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            break;
						case 3:
                            Bitmap jpg = BitmapConverter.ToBitmap(BitmapSourceConverter.ToMat(Img));
							jpg.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
					}
					
					break;
				case false:
					break;
			}

		}

		private void Reset()
		{
			var result = MessageBox.Show("変更は元に戻りません。よろしいですか？", "変更確認", MessageBoxButton.YesNo, MessageBoxImage.Warning);

			switch (result)
			{
				case MessageBoxResult.Yes:
					Img = original_bmp.Clone();
					break;
				default:
					break;
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
			var sent_img = navigationContext.Parameters["Image"] as Mat;
			if (sent_img != null)
				Img = BitmapSourceConverter.ToBitmapSource(sent_img);

			if (original_bmp == null)
				original_bmp = Img.Clone();
		}
	}
}