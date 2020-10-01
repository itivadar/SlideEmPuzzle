using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UserInterface.CustomControls
{
    public class Tile : Canvas
    {
            public Thickness FinalMargin { get; set; }
            public int TileTag { get; set; }
    }
}