using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;

using Im_Analyzer.Models;
using System.Windows.Media.Imaging;
using OpenCvSharp.WpfExtensions;
using OpenCvSharp;
using System.Windows;
using System.Windows.Media;

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

		private Brush _backColor;
		public Brush BackColor
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
				ChangeColor();
				SetProperty(ref _value_Red, value);
			}
		}

        private int _value_Green;
        public int Value_Green
        {
            get { return _value_Green; }
            set
			{
				ChangeColor();
				SetProperty(ref _value_Green, value);
			}
        }

        private int _value_Blue;
        public int Value_Blue
        {
            get { return _value_Blue; }
            set
			{
				ChangeColor();
				SetProperty(ref _value_Blue, value);
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

		public DelegateCommand FilterCommand { get; private set; }
		public DelegateCommand<string> NavigateCommand { get; private set; }
		
		public DetectColorMenuViewModel(IRegionManager regionManager)
		{
			_regionManager = regionManager;
			FilterCommand = new DelegateCommand(Filter);
			NavigateCommand = new DelegateCommand<string>(Navigate);
		}

		private void Filter()
		{
			Img = dc.Filter(new Vec3b(Convert.ToByte(Value_Blue),Convert.ToByte(Value_Green),Convert.ToByte(Value_Red)), Delta_E);
		}

		private void Navigate(string navigatePath)
		{
			Models.Navigation.Navigation.NavigateToPath(_regionManager, "ContentRegion", navigatePath);
		}

		private void ChangeColor()
		{
			BackColor = new SolidColorBrush(Color.FromRgb(Convert.ToByte(Value_Red), Convert.ToByte(Value_Green), Convert.ToByte(Value_Blue)));
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