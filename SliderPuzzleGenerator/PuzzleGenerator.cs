using SliderPuzzleSolver;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SliderPuzzleGenerator
{
    public class PuzzleGenerator : IPuzzleGenerator
    {

        //Predefined 2 x 2 boards
        private readonly string[] _2x2Boards =
        {
          "0 3 2 1",
          "3 1 2 0",
          "2 3 0 1",
          "3 0 2 1",
          "2 3 1 0",
        };

        //Predefined 3 X 3 starting boards 
        private readonly string[] _3x3Boards =
        {
          "4 1 2 3 0 6 5 7 8",
          "6 0 8 4 3 5 1 2 7",
          "6 4 7 8 5 0 3 2 1", //31 moves
          "4 1 2 3 0 6 5 7 8", //22 moves   
          "1 0 2 4 6 3 7 5 8"  //5 moves
        };


        //Predefined 4 X 4 starting boards 
        private readonly string[] _4x4Boards =
        {
          "1 2 4 12 5 6 3 0 9 10 8 7 13 14 11 15", //10 moves
          "1 2 3 4 5 6 7 8 10 0 11 12 9 13 14 15", //5 moves
          "2 3 4 0 1 6 7 8 5 10 11 12 9 13 14 15", //9 moves
          "1 2 8 3 5 11 6 4 0 10 7 12 9 13 14 15", //14 moves
          "1 2 3 4 6 10 7 8 5 0 11 12 9 13 14 15", //7 moves
          "6 3 7 4 2 9 10 8 1 5 12 15 13 0 14 11" //20 moves
        };

        private Random _rand;

        //We map last selected position to each dimension to avoid generating same board twice
        private Dictionary<int, int> _lastRandomIndexByDimension;

        public PuzzleGenerator()
        {
            _rand = new Random();
            _lastRandomIndexByDimension = new Dictionary<int, int>()
            {
                { 2, -1 },
                { 3, -1 },
                { 4, -1 }
            };
        }

        //generate a random puzzle by starting from a  predefined position which is randomly selected
        //make legal moves until it is a more random solution
        public IBoard GenerateRandomPuzzle(int dimension)
        {
            switch (dimension)
            {
                case 2: return Generate2x2Board();
                case 3: return Generate3x3Board();
                case 4: return Generate4x4Board();
            }
            return null;
        }

        //for 2 X 2 boards it is enough to pick one randomly from the possible boards;
        IBoard Generate2x2Board()
        {
            var randIndex = _rand.Next(_2x2Boards.Length);

            //choose next if generated last time
            if (randIndex == _lastRandomIndexByDimension[2])
            {
                GetNextIndex(_2x2Boards.Length, ref randIndex);
            }

            _lastRandomIndexByDimension[2] = randIndex;
            return new Board(_2x2Boards[randIndex]);
        }

        //for 3 X 3 puzzle start from a random position from the predefined 
        //move the blank randomly
        IBoard Generate3x3Board()
        {
            return GetRandomBoard(3, _3x3Boards, 8);
        }

        //for 4 X 4 puzzle start from a random position from the predefined 
        //move the blank randomly
        IBoard Generate4x4Board()
        {
            return GetRandomBoard(4, _4x4Boards, 6);
        }

        //generates a random puzzle starting from a board randomly selected from the boards space
        //moves the blank tile
        private IBoard GetRandomBoard(int dimension, string[] boardsSpace, int movesCount)
        {
            var startingBoardIndex = _rand.Next(boardsSpace.Length);

            //choose next if same board generated last time
            if (startingBoardIndex == _lastRandomIndexByDimension[dimension])
            {
                GetNextIndex(boardsSpace.Length, ref startingBoardIndex);
            }

            IBoard currentBoard = new Board(boardsSpace[startingBoardIndex]);
            var genBoards = new Dictionary<IBoard, bool>();
            for (int move = 0; move < movesCount; move++)
            {
                var neighbors = currentBoard.GetChildBoards();
                currentBoard = neighbors[_rand.Next(neighbors.Count)];
            }

            if (currentBoard.IsSolved()) return GetRandomBoard(dimension, boardsSpace, movesCount);
            _lastRandomIndexByDimension[dimension] = startingBoardIndex;
            return currentBoard;
        }

        private void GetNextIndex(int arrayLength, ref int randIndex)
        {
            randIndex = (randIndex + 1) % arrayLength;
        }
    }
}
