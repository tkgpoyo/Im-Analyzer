using Prism.Mvvm;
using Prism.Regions;
using Im_Analyzer.Views;

namespace Im_Analyzer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private IRegionManager _regionManager;

        private string _title = "Im_Analyzer";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _regionManager.RegisterViewWithRegion("ContentRegion", nameof(StartUpPage));
        }
    }
}