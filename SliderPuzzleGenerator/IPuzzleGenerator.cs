using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SliderPuzzleGenerator
{
    public interface IPuzzleGenerator
    {
        IBoard GenerateRandomPuzzle(int dimension);
    }
}
