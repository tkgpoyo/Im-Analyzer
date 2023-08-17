using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Im_Analyzer.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _activeContent;
        public ViewModelBase ActiveContent
        {
            get => _activeContent;
            set
            {
                _activeContent = value;
                RaisePropertyChanged("ActiveContent");
            }
        }
        public MainWindowViewModel()
        {
            ActiveContent = new StartUpPageViewModel();
        }
    }
}
