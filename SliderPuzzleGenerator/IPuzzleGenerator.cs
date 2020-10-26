using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SliderPuzzleGenerator
{
    public interface IPuzzleGenerator
    {

        /// <summary>
        /// Generates a random puzzle board.
        /// </summary>
        /// <param name="dimension">The dimension of the board expressed in number of rows.</param>
        /// <returns></returns>
        IBoard GenerateRandomPuzzle(int dimension);
    }
}
