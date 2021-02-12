using System;
using System.Collections.Generic;
using System.Text;

namespace SliderPuzzleSolver.Interfaces
{
  public interface IPuzzleSolver
  {
    /// <summary>
    /// Sequence of steps fromt the initial board to the goal board.
    /// </summary>
    /// <param name="boardToSolve">the puzzle to solve.</param>
    /// <returns>an IEnumerable of directiond</returns>
    IEnumerable<IBoard> SolutionSteps(IBoard boardToSolve);

    /// <summary>
    /// Sequence of directions in which the blank tile should be moved in order to solve the puzzle.
    /// </summary>
    /// <param name="boardToSolve">the puzzle to solve.</param>
    /// <returns>an IEnumerable of directions</returns>
    IEnumerable<SlideDirection> GetSolutionDirections(IBoard boardToSolve);

  }
}
