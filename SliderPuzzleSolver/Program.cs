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
            var stringTiles = Console.ReadLine();

            var introducedTiles = GetTiles(stringTiles);
            IBoard board = new Board(introducedTiles);

            Console.Write("The board to solve: ");
            Console.Write(board);
            Console.WriteLine(board.CanBeSolved());
            
            
            for(int i=0; i<10; i++)
            {
                var twin = board.Twin();
                Console.Write(twin);
                Console.WriteLine(twin.CanBeSolved());
                if (i == 5)
                {
                    twin = twin.Twin();
                    Console.Write(twin);
                    Console.WriteLine(twin.CanBeSolved());
                }
            }

            IPuzzleSolver solver = new PuzzleSolver(board);

            if (board.CanBeSolved())
             {
                var steps = solver.SolutionSteps();
                int i = 0;
                foreach (var step in steps)
                {
                    Console.Write($"Step {i++}: ");
                    Console.WriteLine(step);
                    Console.WriteLine();
                }
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
