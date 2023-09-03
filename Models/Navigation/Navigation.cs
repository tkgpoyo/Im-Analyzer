using Prism.Regions;

namespace Im_Analyzer.Models.Navigation
{
    internal class Navigation
    {
        public static void NavigateToPath(IRegionManager regionManager, string regionContent, string navigationPath, NavigationParameters param = null)
        {
            if (param == null)
            {
                regionManager.RequestNavigate(regionContent, navigationPath);
            }
            else
            {
                regionManager.RequestNavigate(regionContent, navigationPath, param);
            }
        }
    }
}
