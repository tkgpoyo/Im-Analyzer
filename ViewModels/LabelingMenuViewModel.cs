using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Media.Imaging;

namespace Im_Analyzer.ViewModels
{
	public class LabelingMenuViewModel : BindableBase, INavigationAware
	{
		private IRegionManager _regionManager;

		private Models.Analyzer.Labeling lb;

		private BitmapSource _original_Img;
		public BitmapSource Original_Img
		{
			get { return _original_Img; }
			set { SetProperty(ref _original_Img, value); }
		}
		
		private BitmapSource _img;
		public BitmapSource Img
		{
			get { return _img; }
			set { SetProperty(ref _img, value); }
		}

		public DelegateCommand<string> LabelingCommand { get; private set; }
		public DelegateCommand SaveCommand { get; private set; }

		public DelegateCommand<string> NavigateCommand { get; private set; }
		
		public LabelingMenuViewModel(IRegionManager regionManager)
		{
			_regionManager = regionManager;
			LabelingCommand = new DelegateCommand<string>(Labeling);
			SaveCommand = new DelegateCommand(Save);
			NavigateCommand = new DelegateCommand<string>(Navigate);
		}

		private void Labeling(string mode)
		{
			Img = lb.LabelProcess(mode);
		}

        private void Save()
        {
			Models.Save.Save.SaveImage(Img);
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
			Original_Img = Img.Clone();

            lb = new Models.Analyzer.Labeling(BitmapSourceConverter.ToMat(Img));
        }
	}
}