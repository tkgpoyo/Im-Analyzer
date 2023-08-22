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
	public class FunctionPageViewModel : BindableBase, INavigationAware
	{
		private IRegionManager _regionManager;
		
		private BitmapSource _img;
		public BitmapSource Img
		{
			get { return _img; }
			set { SetProperty(ref _img, value); }
		}

		private DelegateCommand<string> _showMenuCommand;
		public DelegateCommand<string> ShowMenuCommand { get; private set; }

		public FunctionPageViewModel(IRegionManager regionManager)
		{
			_regionManager = regionManager;
			ShowMenuCommand = new DelegateCommand<string>(ShowMenu);
		}

		private void ShowMenu(string navigatePath)
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