using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Im_Analyzer.ViewModels
{
	public class TrimmingPageViewModel : BindableBase, INavigationAware
	{
		private double _left;
		public double Left
		{
			get { return _left; }
			set { SetProperty(ref _left, value); }
		}

		private double _right;
		public double Right
		{
			get { return _right; }
			set
			{
				SetProperty(ref _right, value);
				Width = Math.Abs(Right - Left);
			}
		}

		private double _top;
		public double Top
		{
			get { return _top; }
			set { SetProperty(ref _top, value); }
		}

		private double _bottom;
		public double Bottom
		{
			get { return _bottom; }
			set
			{
				SetProperty(ref _bottom, value);
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

		private DelegateCommand<object> _startPointingCommand;
		public DelegateCommand<object> StartPointingCommand { get; private set; }

		private DelegateCommand _endPointingCommand;
		public DelegateCommand EndPointingCommand { get; private set; }

		public TrimmingPageViewModel()
		{
			StartPointingCommand = new DelegateCommand<object>(StartPointing);
			EndPointingCommand = new DelegateCommand(EndPointing);
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

				Width = Math.Abs(pos.X - init_pos.X);
				Height = Math.Abs(pos.Y - init_pos.Y);

				await Task.Delay(30);
			}
		}

		private void EndPointing()
		{
			isDrag = false;
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