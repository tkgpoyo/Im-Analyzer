using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace Im_Analyzer.ViewModels
{
    public class StartUpPageViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        public DelegateCommand<string> NavigateCommand { get; private set; }
        
        public StartUpPageViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            this.NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        private void Navigate(string navigatePath)
        {
            _regionManager.RequestNavigate("ContentRegion", navigatePath);
        }
    }
}