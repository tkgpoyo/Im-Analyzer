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

		private DelegateCommand<string> _navigateCommand;
		public DelegateCommand<string> NavigateCommand { get; private set; }

		private DelegateCommand _resetCommand;
		public DelegateCommand ResetCommand { get; private set; }

		public RetouchMenuViewModel(IRegionManager regionManager)
		{
			_regionManager = regionManager;
			NavigateCommand = new DelegateCommand<string>(Navigate);
			ResetCommand = new DelegateCommand(Reset);
		}

		private void Navigate(string navigatePath)
		{
			var param = new NavigationParameters();
			param.Add("Image", BitmapSourceConverter.ToMat(Img));
			//_regionManager.RequestNavigate("ContentRegion", navigatePath, param);
			Models.Navigation.Navigation.NavigateToPath(_regionManager, "ContentRegion", navigatePath, param);
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

		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			var sent_img = navigationContext.Parameters["Image"] as Mat;
			Img = BitmapSourceConverter.ToBitmapSource(sent_img);

			if (original_bmp == null)
				original_bmp = Img.Clone();
		}
	}
}