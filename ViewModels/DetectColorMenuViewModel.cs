using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Windows.Media.Imaging;
using OpenCvSharp.WpfExtensions;
using OpenCvSharp;
using System.Windows.Media;
using System.Windows.Input;

namespace Im_Analyzer.ViewModels
{
	public class DetectColorMenuViewModel : BindableBase, INavigationAware
	{
		private IRegionManager _regionManager;

		private BitmapSource _initImg;
		public BitmapSource InitImg
		{
			get { return _initImg; }
			set { SetProperty(ref _initImg, value); }
		}
		
		private BitmapSource _img;
		public BitmapSource Img
		{
			get { return _img; }
			set { SetProperty(ref _img, value); }
		}

		private System.Windows.Media.Brush _backColor;
		public System.Windows.Media.Brush BackColor
		{
			get { return _backColor; }
			set { SetProperty(ref _backColor, value); }
		}

		private int _value_Red;
		public int Value_Red
		{
			get { return _value_Red; }
			set
			{
				SetProperty(ref _value_Red, value);
				ChangeColor();
			}
		}

        private int _value_Green;
        public int Value_Green
        {
            get { return _value_Green; }
            set
			{
				SetProperty(ref _value_Green, value);
				ChangeColor();
			}
        }

        private int _value_Blue;
        public int Value_Blue
        {
            get { return _value_Blue; }
            set
			{
				SetProperty(ref _value_Blue, value);
				ChangeColor();
			}
        }

		private int _delta_E;
		public int Delta_E
		{
			get { return _delta_E; }
			set { SetProperty(ref _delta_E, value); }
		}

        private OxyPlot.PlotModel _redModel;
		public OxyPlot.PlotModel RedModel
		{
			get { return _redModel; }
			set { SetProperty(ref _redModel, value); }
		}

        private OxyPlot.PlotModel _greenModel;
        public OxyPlot.PlotModel GreenModel
		{
			get { return _greenModel; }
			set { SetProperty(ref _greenModel, value); }
		}

        private OxyPlot.PlotModel _blueModel;
        public OxyPlot.PlotModel BlueModel
		{
			get { return _blueModel; }
			set { SetProperty(ref _blueModel, value); }
		}

        private Models.Analyzer.DetectColor dc;

		public DelegateCommand<object> GetColorCommand { get; private set; }

		public DelegateCommand FilterCommand { get; private set; }
		public DelegateCommand SaveCommand { get; private set; }
		public DelegateCommand<string> NavigateCommand { get; private set; }
		
		public DetectColorMenuViewModel(IRegionManager regionManager)
		{
			_regionManager = regionManager;

			GetColorCommand = new DelegateCommand<object>(GetColor);
			FilterCommand = new DelegateCommand(Filter);
			SaveCommand = new DelegateCommand(Save);
			NavigateCommand = new DelegateCommand<string>(Navigate);
		}

		private void GetColor(object image_element)
		{
			var element = (System.Windows.IInputElement)image_element;
			var pos = Mouse.GetPosition(element);

			var image = (System.Windows.Controls.Image)image_element;

			int x = (int)(Img.PixelWidth * pos.X / image.ActualWidth);
			int y = (int)(Img.PixelHeight * pos.Y / image.ActualHeight);

			if (x == Img.PixelWidth)
				x--;
			if (y == Img.PixelHeight)
				y--;

			using (Mat tmp = BitmapSourceConverter.ToMat(InitImg))
			{
				Vec3b pixel = tmp.At<Vec3b>(y, x);

				Value_Red = pixel[2];
				Value_Green = pixel[1];
				Value_Blue = pixel[0];
			}

		}

		private void Filter()
		{
			Img = dc.Filter(new Vec3b(Convert.ToByte(Value_Blue),Convert.ToByte(Value_Green),Convert.ToByte(Value_Red)), Delta_E);
		}

        private void Save()
        {
			Models.Save.Save.SaveImage(Img);
        }

        private void Navigate(string navigatePath)
		{
			Models.Navigation.Navigation.NavigateToPath(_regionManager, "ContentRegion", navigatePath);
		}

		private void ChangeColor()
		{
			BackColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(Convert.ToByte(Value_Red), Convert.ToByte(Value_Green), Convert.ToByte(Value_Blue)));
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
			InitImg = BitmapSourceConverter.ToBitmapSource(sent_img);
			Img = InitImg.Clone();

			dc = new Models.Analyzer.DetectColor(sent_img);

			Value_Red = 0;
			Value_Green = 0;
			Value_Blue = 0;

			RedModel = dc.ColorGraphModel("R");
			GreenModel = dc.ColorGraphModel("G");
			BlueModel = dc.ColorGraphModel("B");
		}
	}
}