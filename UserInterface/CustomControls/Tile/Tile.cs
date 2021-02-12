using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UserInterface.CustomControls
{
    public class Tile : Button
    {
        
        private byte _tileTag;
        private bool _isBlankTile;

        /// <summary>
        /// This will be the tile's margin after an animation is finished.
        /// </summary>
        public Thickness DestinationMargin { get; set; }

        /// <summary>
        /// Gets or sets the number displayed on the tile.
        /// </summary>
        public byte TileTag
        {
            get => _tileTag;
            set 
            {
                _tileTag = value;
                Content = value;
            }
        }

        /// <summary>
        /// Determines if the tiles is the blank tile.
        /// </summary>
        public bool IsBlankTile
        {
            get => _isBlankTile;
            set 
            {
                _isBlankTile = value;
                SetStyle();
             }
        }
    
        /// <summary>
        /// Set style for the tile.
        /// There are two different types of styles: 
        ///     -the style for tag tiles
        ///     -the style for blank tile
        /// </summary>
        private void SetStyle()
        {
            if (IsBlankTile)
            {
                Style = Application.Current.FindResource("BlankTileStyle") as Style;
            }
        }
    }
}