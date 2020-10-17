using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UserInterface.BootstraperSpace
{
    public interface INavigationService
    {
        TViewType GetView<TViewType>() where TViewType : FrameworkElement;
        Page GetPage(string pageName);
    }
}
