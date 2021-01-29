using System;
using System.Collections.Generic;
using System.Text;

namespace SliderPuzzleSolver.Interfaces
{
    public interface IPuzzleSolver
    {
        //sequence of boards which lead to solution
        IEnumerable<IBoard> SolutionSteps(IBoard boardToSolve);

        /// <summary>
        /// Sequence of direction in which the blank tile should be moved in order to solve the puzzle.
        /// </summary>
        /// <param name="boardToSolve">the puzzle to solve.</param>
        /// <returns>an IEnumerable of directiond</returns>
        IEnumerable<Direction> GetSolutionDirections(IBoard boardToSolve);

    }
}
