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



        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
