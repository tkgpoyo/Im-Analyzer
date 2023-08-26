using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Im_Analyzer.ViewModels
{
	public class AnalyzeMenuViewModel : BindableBase, INavigationAware
	{
		private IRegionManager _regionManager;

		private BitmapSource _img;
		public BitmapSource Img
		{
			get { return _img; }
			set { SetProperty(ref _img, value); }
		}

		public DelegateCommand<string> NavigateCommand { get; private set; }
		
		public AnalyzeMenuViewModel(IRegionManager regionManager)
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
			// 特になし
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			var sent_img = navigationContext.Parameters["Image"] as Mat;
			Img = BitmapSourceConverter.ToBitmapSource(sent_img);
		}
	}
}