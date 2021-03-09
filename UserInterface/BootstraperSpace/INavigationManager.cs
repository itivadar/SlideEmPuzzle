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
    /// Displays a page into the frame of the MainWindow.
    /// </summary>
    /// <param name="pageName">The name of the page.</param>
    void ShowPage(string page);

    /// <summary>
    /// Displays a page after a delay specified.
    /// </summary>
    /// <param name="pageName">The name of the page.</param>
    /// <param name="duration">The delay after which the page is displayed.</param>
    void ShowPageAfterDelay(string pageName, TimeSpan duration);
  }
}
