using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace UserInterface.CustomControls.MenuButton

{
    public class MenuButton : Button
    {
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(nameof(Image),
                                                                                              typeof(ImageSource),
                                                                                              typeof(MenuButton),
                                                                                              new PropertyMetadata(null));

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
    }

}
