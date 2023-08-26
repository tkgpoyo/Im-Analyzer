using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

using Im_Analyzer.Models;

namespace Im_Analyzer.ViewModels
{
	public class LabelingMenuViewModel : BindableBase, INavigationAware
	{
		private IRegionManager _regionManager;

		private Models.Analyzer.Labeling lb;

		private BitmapSource _img;
		public BitmapSource Img
		{
			get { return _img; }
			set { SetProperty(ref _img, value); }
		}

		public DelegateCommand<string> LabelingCommand { get; private set; }

		public DelegateCommand<string> NavigateCommand { get; private set; }
		
		public LabelingMenuViewModel(IRegionManager regionManager)
		{
			_regionManager = regionManager;
			LabelingCommand = new DelegateCommand<string>(Labeling);
			NavigateCommand = new DelegateCommand<string>(Navigate);
		}

		private void Labeling(string mode)
		{
			Img = lb.LabelProcess(mode);
		}

		private void Navigate(string navigatePath)
		{
			Models.Navigation.Navigation.NavigateToPath(_regionManager, "ContentRegion", navigatePath);
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

            lb = new Models.Analyzer.Labeling(BitmapSourceConverter.ToMat(Img));
        }
	}
}