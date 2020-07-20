using System;
using System.Collections.Generic;
using System.Text;

namespace SliderPuzzleSolver.Interfaces
{
  public interface IPuzzleSolver
  {
    //sequence of boards which lead to solution
    IEnumerable<IBoard> SolutionSteps();
  }
}
