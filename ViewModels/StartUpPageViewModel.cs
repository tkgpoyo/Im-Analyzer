using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows;

namespace Im_Analyzer.ViewModels
{
    public class StartUpPageViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        public DelegateCommand<string> NavigateCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }
        
        public StartUpPageViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            this.NavigateCommand = new DelegateCommand<string>(Navigate);
            this.CloseCommand = new DelegateCommand(Close);
        }

        private void Navigate(string navigatePath)
        {
            Models.Navigation.Navigation.NavigateToPath(_regionManager, "ContentRegion", navigatePath);
        }

        private void Close()
        {
            Application.Current.Shutdown();
        }
    }
}