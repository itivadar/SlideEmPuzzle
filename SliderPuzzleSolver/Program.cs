using BinaryHeap;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Diagnostics;

namespace SliderPuzzleSolver
{
  class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.Write("Tiles: ");
            IPuzzleSolver solver = new PuzzleSolver();
           
            IBoard board =  new Board(GetTiles("8 10 6 7 12 11 15 14 4 3 0 1 5 13 2 6"));
             board = new Board(GetTiles("0 3 2 1")); //17 moves
            Console.Write("The board to solve: ");
            Console.Write(board);
            Console.WriteLine(board.IsSolvable());

            if (board.IsSolvable())
             {
                Stopwatch stopwatch = Stopwatch.StartNew();
                var steps = solver.SolutionSteps(board);
                stopwatch.Stop();
                int i = 0;
                foreach (var step in steps)
                {
                    Console.Write($"Step {i++}: ");
                    Console.WriteLine(step);
                    Console.WriteLine();
                }
                Console.WriteLine($"Done in {stopwatch.ElapsedMilliseconds} ms.");
            }
            else
            {
                Console.WriteLine("Unsolvable puzzle");
            }
            
            
            Console.ReadLine();
        }

        private static byte[,] GetTiles(string stringRepresentation)
        {
            var tilesValues = stringRepresentation.Split(" ");
            int n = (int)Math.Sqrt(tilesValues.Length);
            byte[,] tiles = new byte[n, n];

            for(int i=0; i<n;i++)
                for(int j=0; j<n; j++)
                {
                    tiles[i, j] =byte.Parse(tilesValues[i * n + j]);
                }
            return tiles;
        }
    }
}
