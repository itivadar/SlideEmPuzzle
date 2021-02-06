using System;
using System.Collections.Generic;
using System.Text;

namespace SliderPuzzleSolver
{
  static class ConstantHelper
  {
    /// <summary>
    /// Gets the transformations on each directions
    /// </summary>
    public static readonly Dictionary<SlideDirection, (int Row, int Column)> DirectionsTransfom =
    new Dictionary<SlideDirection, (int RowTransfom, int ColumnTransform)>
    {
            {SlideDirection.Left,    ( 0, -1 )},
            {SlideDirection.Right,   ( 0,  1 )},
            {SlideDirection.Down,    ( 1,  0 )},
            {SlideDirection.Up,      (-1,  0 )},
    };

    /// <summary>
    /// Gets the default value for the blankTileValue
    /// </summary>
    public static readonly byte BlankTileValue = 0;

    /// <summary>
    /// Associates each tile with a pattern in which it belongs.
    /// </summary>
    public static readonly Dictionary<byte, Pattern555> TileToPatternMap =
    new Dictionary<byte, Pattern555>
    {
            {1,   Pattern555.Left5},
            {2,   Pattern555.Left5},
            {3,   Pattern555.Right5},
            {4,   Pattern555.Right5},
            {5,   Pattern555.Left5},
            {6,   Pattern555.Left5},
            {7,   Pattern555.Right5},
            {8,   Pattern555.Right5},
            {9,   Pattern555.Left5},
            {10,  Pattern555.Bottom5},
            {11,  Pattern555.Bottom5},
            {12,  Pattern555.Right5},
            {13,  Pattern555.Bottom5},
            {14,  Pattern555.Bottom5},
            {15,  Pattern555.Bottom5},
    };

    /// <summary>
    /// Associates each tile with the order in its pattern.
    /// Each pattern have 5 tiles sorted in descent order.
    /// </summary>
    public static readonly Dictionary<byte, byte> ValueToIndexPositionMap =
    new Dictionary<byte, byte>
    {
            {1, 0},
            {2, 1},
            {5, 2},
            {6, 3},
            {9, 4},
            {3, 0},
            {4, 1},
            {7, 2},
            {8, 3},
            {12, 4},
            {10, 0},
            {11, 1},
            {13, 2},
            {14, 3},
            {15, 4},
    };
  }
}
