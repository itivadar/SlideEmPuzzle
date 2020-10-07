using System;
using System.Collections.Generic;
using System.Text;

namespace SliderPuzzleSolver.Interfaces
{
    public interface IPuzzleSolver
    {
        //sequence of boards which lead to solution
        IEnumerable<IBoard> SolutionSteps(IBoard boardToSolve);

        //generate a random solvable board of a given dimension
        IBoard GenerateRandomBoard(int dimension);
    }
}
