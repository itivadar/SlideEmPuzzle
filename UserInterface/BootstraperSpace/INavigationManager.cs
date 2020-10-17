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
        /// Gets a <see cref="Page"/> object by name. 
        /// </summary>
        /// <param name="pageName">The page name</param>
        /// <returns>a page</returns>
        TViewType GetView<TViewType>() where TViewType : FrameworkElement;

        /// <summary>
        /// Resolve a view from the container.
        /// </summary>
        Page GetPage(string pageName);
    }
}
