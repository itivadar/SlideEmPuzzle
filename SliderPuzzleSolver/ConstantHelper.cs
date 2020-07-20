using System;
using System.Collections.Generic;
using System.Text;

namespace SliderPuzzleSolver
{
    static class ConstantHelper
    {
        public static readonly Dictionary<Direction, (int Row, int Column)> DirectionsTransfom = new Dictionary<Direction, (int RowTransfom, int ColumnTransform)>
        {
            {Direction.Left,    ( 0, -1 )},
            {Direction.Right,   ( 0,  1 )},
            {Direction.Down,    ( 1,  0 )},
            {Direction.Up,      (-1,  0 )},           
        };

        public static readonly byte BlankTileValue = 0;
    }
}
