using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
