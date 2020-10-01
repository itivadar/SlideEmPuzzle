using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace UserInterface.CustomControls
{
    class MenuViewItem : ListViewItem
    {
        public string DisplayedContent { get; set; }
        public Uri PageUri { get; set; }

        public MenuAction Action { get; set; }
    }
}
