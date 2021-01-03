using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UserInterface.BootstraperSpace
{
    public interface INavigationService
    {

        /// <summary>
        /// Resolve a view from the container.
        /// </summary>
      
        TViewType GetView<TViewType>() where TViewType : FrameworkElement;

        /// <summary>
        /// Gets a page by name. 
        /// </summary>
        /// <param name="pageName">The page name</param>
        /// <returns>a page</returns>
        Page GetPage(string pageName);

        /// <summary>
        /// Sets the main page
        /// </summary>
        /// <param name="page"></param>
        void SetMainPage(string page);
    }
}
