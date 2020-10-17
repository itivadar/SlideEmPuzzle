using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace UserInterface.CustomControls
{
    public class MenuViewItem : ListViewItem
    {
        public string DisplayedContent { get; set; }
        public string Page { get; set; }
        public Type PageType { get; set; }
        public MenuAction Action { get; set; }
    }
}
