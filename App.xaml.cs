using Prism.Ioc;
using Im_Analyzer.Views;
using System.Windows;

namespace Im_Analyzer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.StartUpPage>();
            containerRegistry.RegisterForNavigation<Views.SelectPage>();
            containerRegistry.RegisterForNavigation<Views.FunctionPage>();
            containerRegistry.RegisterForNavigation<Views.RetouchMenu>();
            containerRegistry.RegisterForNavigation<Views.TrimmingPage>();
            containerRegistry.RegisterForNavigation<Views.AnalyzeMenu>();
            containerRegistry.RegisterForNavigation<Views.LabelingMenu>();
        }
    }
}