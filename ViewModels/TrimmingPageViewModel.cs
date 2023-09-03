using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using Im_Analyzer.Views;

namespace Im_Analyzer.ViewModels
{
	public class TrimmingPageViewModel : BindableBase, INavigationAware
	{
		private IRegionManager _regionManager;
		
		private double _left;
		public double Left
		{
			get { return _left; }
			set
			{
				double d = Math.Round((double)value, MidpointRounding.AwayFromZero);
				SetProperty(ref _left, value);
			}
		}

		private double _right;
		public double Right
		{
			get { return _right; }
			set
			{
				double d = Math.Round((double)value, MidpointRounding.AwayFromZero);
				SetProperty(ref _right, d);
				Width = Math.Abs(Right - Left);
			}
		}

		private double _top;
		public double Top
		{
			get { return _top; }
			set
			{
				double d = Math.Round((double)value, MidpointRounding.AwayFromZero);
				SetProperty(ref _top, d);
			}
		}

		private double _bottom;
		public double Bottom
		{
			get { return _bottom; }
			set
			{
				double d = Math.Round((double)value, MidpointRounding.AwayFromZero);
				SetProperty(ref _bottom, d);
				Height = Math.Abs(Bottom - Top);
			}
		}

		private double _width;
		public double Width
		{
			get { return _width; }
			set { SetProperty(ref _width, value); }
		}

		private double _height;
		public double Height
		{
			get { return _height; }
			set { SetProperty(ref _height, value); }
		}
		
		private bool isDrag;
		
		private BitmapSource _img;
		public BitmapSource Img
		{
			get { return _img; }
			set { SetProperty(ref _img, value); }
		}

		public DelegateCommand<object> StartPointingCommand { get; private set; }

		public DelegateCommand EndPointingCommand { get; private set; }

		public DelegateCommand<object> MouseOutOfCanvasCommand { get; private set; }

		public DelegateCommand<object[]> TrimmingCommand { get; private set; }

		public DelegateCommand BackPageCommand { get; private set; }

		public TrimmingPageViewModel(IRegionManager regionManager)
		{
			_regionManager = regionManager;
			
			StartPointingCommand = new DelegateCommand<object>(StartPointing);
			EndPointingCommand = new DelegateCommand(EndPointing);
			MouseOutOfCanvasCommand = new DelegateCommand<object>(MouseOutOfCanvas);

			TrimmingCommand = new DelegateCommand<object[]>(Trimming);
			BackPageCommand = new DelegateCommand(BackPage);
		}

		private async void StartPointing(object param)
		{
			var element = (System.Windows.IInputElement)param;
			var pos = Mouse.GetPosition(element);
			var init_pos = new System.Windows.Point(pos.X, pos.Y);

			isDrag = true;

			while(isDrag)
			{
				pos = Mouse.GetPosition(element);

				if (pos.X < init_pos.X)
				{
					Left = pos.X;
					Right = init_pos.X;
				}
				else
				{
					Left = init_pos.X;
					Right = pos.X;
				}
				

				if (pos.Y < init_pos.Y)
				{
					Top = pos.Y;
					Bottom = init_pos.Y;
				}
				else
				{
					Top = init_pos.Y;
					Bottom = pos.Y;
				}

				await Task.Delay(30);
			}
		}

		private void EndPointing()
		{
			isDrag = false;
		}

		private async void MouseOutOfCanvas(object canvas)
		{
            while (isDrag)
            {
				var c = (System.Windows.Controls.Canvas)canvas;
				double[] bound = new double[4]
				{
					0,				// upper bound
					c.ActualHeight, // lower bound
					0,				// left  bound
					c.ActualWidth,  // right bound
				};

				if (Top < bound[0])
					Top = bound[0];
				if (Bottom > bound[1])
					Bottom = bound[1];
				if (Left < bound[2])
					Left = bound[2];
				if (Right > bound[3])
					Right = bound[3];

				await Task.Delay(20);
			}

		}

		private void Trimming(object[] objects)
		{
			var c = (System.Windows.Controls.Canvas)objects[0];
			var i = (System.Windows.Controls.Image)objects[1];

			double horizontal_gap = (c.ActualWidth - i.ActualWidth) / 2;
			double vertical_gap = (c.ActualHeight - i.ActualHeight) / 2;

			int l = (int)(Img.PixelWidth * ((Left - horizontal_gap) / i.ActualWidth));
			int t = (int)(Img.PixelHeight * ((Top - vertical_gap) / i.ActualHeight));
			int w = (int)(Img.PixelWidth * (Width / i.ActualWidth));
			int h = (int)(Img.PixelHeight * (Height / i.ActualHeight));
			
			try
			{
                Img = new CroppedBitmap(Img, new Int32Rect(l, t, w, h));

				var param = new NavigationParameters();
				param.Add("Image", BitmapSourceConverter.ToMat(Img));
				_regionManager.RequestNavigate("ContentRegion", nameof(RetouchMenu), param);
            }
			catch (System.ArgumentException e)
			{

				MessageBox.Show("範囲外の数値が渡されました。");
			}
        }

        private void BackPage()
		{
			Models.Navigation.Navigation.NavigateToPath(_regionManager, "ContentRegion", nameof(RetouchMenu));
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

			Left = 0;
			Right = 0;
			Top = 0;
			Bottom = 0;
		}
	}
}