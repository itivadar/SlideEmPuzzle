using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SliderPuzzleGenerator
{
    public interface IPuzzleGenerator
    {

        /// <summary>
        /// Generates a random puzzle board of the given dimenion.
        /// </summary>
        /// <param name="dimension">The dimension of the board expressed in number of rows.</param>
        /// <returns>a board represeting a puzzle</returns>
        IBoard GenerateRandomPuzzle(int dimension);

        /// <summary>
        /// Generates a random puzzle board of the given type.
        /// </summary>
        /// <param name="type">The dimension of the board expressed in number of rows.</param>
        /// <returns>a board represeting a puzzle</returns>
        IBoard GenerateRandomPuzzle(string type);
    }
}
