using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UserInterface.CustomControls
{
    public class Tile : Canvas
    {
        private int _tileTag;
        private bool _isBlankTile;

        public Thickness DestinationMargin { get; set; }
        public int TileTag
        {
            get => _tileTag;
            set 
            {
                _tileTag = value;
                SetTileTag();
            }
        }

        public bool IsBlankTile
        {
            get => _isBlankTile;
            set 
            {
                _isBlankTile = value;
                SetBackgroundColor();
             }
        }

        public Tile() : base()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            IsBlankTile = false;
        }

        private void SetTileTag()
        {
            Children.Clear();

            var tileTag = CreateTileTag();
            Children.Add(tileTag);
        }

        private UIElement CreateTileTag()
        {
            return new TextBlock
            {
                Text = TileTag.ToString(),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 50
            };
        }

        private void SetBackgroundColor()
        {
            if (IsBlankTile)
            {
                Background = Brushes.Transparent;
                Children.Clear();
            }
            else
            {
                Background = Brushes.Wheat;
            }
        }

        
    }
}