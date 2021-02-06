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
            IPuzzleSolver solver = new PuzzleSolver();

            IBoard board = new Board(GetTiles("0 15 14 13 12 11 10 9 8 7 6 5 4 3 2 1"));
            //board = new Board(GetTiles("6 4 7 8 5 0 3 2 1")); //17 moves
            Console.Write("The board to solve: ");
            Console.Write(board);
            Console.WriteLine(board.IsSolvable());

            if (board.IsSolvable())
            {
                
                var steps = solver.SolutionSteps(board);

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

            //var file = @"C:\users\neo_c\desktop\Left5.db";


            //Stopwatch stopwatch = Stopwatch.StartNew();
            //var gen = new PatternGenerator();
            //gen.GeneratPatternDb(Pattern555.Left5, file);

            //Console.WriteLine($"Done in {stopwatch.ElapsedMilliseconds / 1000} s.");
            //stopwatch.Stop();

            //Console.ReadLine();
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
