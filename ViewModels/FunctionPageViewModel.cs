﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

namespace Im_Analyzer.ViewModels
{
	public class FunctionPageViewModel : BindableBase, INavigationAware
	{
		private IRegionManager _regionManager;
		
		private BitmapSource _img;
		public BitmapSource Img
		{
			get { return _img; }
			set { SetProperty(ref _img, value); }
		}

		public DelegateCommand<string> NavigateCommand { get; private set; }

		public FunctionPageViewModel(IRegionManager regionManager)
		{
			_regionManager = regionManager;
			NavigateCommand = new DelegateCommand<string>(Navigate);
		}

		private void Navigate(string navigatePath)
		{
            var param = new NavigationParameters();
            param.Add("Image", BitmapSourceConverter.ToMat(Img));
			Models.Navigation.Navigation.NavigateToPath(_regionManager, "ContentRegion", navigatePath, param);
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