using System;
using System.Collections.Generic;
using System.Text;

namespace SliderPuzzleSolver
{
    /// <summary>
    /// Represents a pattern used by Patterns Database to solve harder puzzles.
    /// A pattern is a mini-puzzle having only 5 tiles placed in same initial postion.
    /// Each board of 4x4  si composed of states from 3 different patterns
    /// </summary>
    public enum  Pattern555 : byte
    {
        /// <summary>
        /// Left pattern = {1, 2, 5, 6, 9}
        /// </summary>
         Left5 = 0, 

        /// <summary>
        /// Right pattern = {3, 4, 7, 8, 12}
        /// </summary>
        Right5 = 1,

        /// <summary>
        /// Right pattern = {10, 11, 13, 14, 15}
        /// </summary>
        Bottom5 = 2,
    }
}
