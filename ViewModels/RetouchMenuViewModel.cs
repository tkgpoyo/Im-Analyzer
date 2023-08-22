using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

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

		private DelegateCommand<string> _navigateCommand;
		public DelegateCommand<string> NavigateCommand
		{
			get { return _navigateCommand; }
			set { SetProperty(ref _navigateCommand, value); }
		}

		public RetouchMenuViewModel(IRegionManager regionManager)
		{
			_regionManager = regionManager;
			NavigateCommand = new DelegateCommand<string>(Navigate);
		}

		private void Navigate(string navigatePath)
		{
			var param = new NavigationParameters();
			param.Add("Image", BitmapSourceConverter.ToMat(Img));
			_regionManager.RequestNavigate("ContentRegion", navigatePath, param);
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
		}
	}
}