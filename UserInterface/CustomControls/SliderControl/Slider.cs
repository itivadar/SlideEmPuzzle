using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UserInterface.CustomControls
{
    public class Slider : ItemsControl
    {
        private const int RowCount = 4;
        private byte[,] _state;

        public Slider()
        {
            Tiles = new ObservableCollection<UIElement>();
            InitState();
            InitSlider();
            
        }
        

       
        public ObservableCollection<UIElement> Tiles  { get; set;  }


        private void InitState()
        {
            _state = new byte[RowCount, RowCount];
            for (byte tileIndex = 0; tileIndex < RowCount * RowCount; tileIndex++)
            {
                _state[tileIndex / RowCount,tileIndex % RowCount] = tileIndex;
            }
        }

        private void InitSlider()
        {
            Canvas c = new Canvas()
            {
                Background = Brushes.White,
                Width = 50,
                Height = 50,
            };
            Tiles.Add(c);
            ItemsSource = Tiles;
        }
    }
}
