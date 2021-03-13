using SliderPuzzleSolver;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;

namespace SliderPuzzleGenerator
{
	public class PuzzleGenerator : IPuzzleGenerator
	{
		/// <summary>
		/// Enum for different board types
		/// </summary>
		private enum BoardType
		{
			Easy,
			Medium,
			Hard
		}

		#region Private Fields

		/// <summary>
		/// Array of predefined 2x2 boards.
		/// </summary>
		private static readonly string[] EasyBoards =
		{
					"0 3 2 1",
					"3 1 2 0",
					"2 3 0 1",
					"3 0 2 1",
					"2 3 1 0",
		};

		/// <summary>
		/// Array of predefined 3x3 boards.
		/// </summary>
		private static readonly string[] MediumBoards =
		{
					"4 1 2 3 0 6 5 7 8",
					"6 0 8 4 3 5 1 2 7",
          "4 1 2 3 0 6 5 7 8", //22 moves
          "1 0 2 4 6 3 7 5 8", //5 moves
					"1 4 3 7 0 8 6 5 2", //18 moves
					"1 3 5 7 2 6 8 0 4", //11 moves
    };

		/// <summary>
		/// Array of predefined 4x4 boards.
		/// </summary>
		private static readonly string[] HardBoards =
		{
					"1 2 8 3 5 11 6 4 0 10 7 12 9 13 14 15", //14 moves
					"5 1 3 4 9 2 7 8 13 0 10 12 14 6 11 15", //11 moves
					"5 1 2 4 9 6 3 7 13 10 0 8 14 15 11 12", //12 moves
					"5 2 4 0 6 1 3 8 13 11 7 12 10 9 14 15", //17 moves 
					"6 3 7 4 2 9 10 8 1 5 12 15 13 0 14 11", //20 moves
					"1 4 8 3 7 2 10 11 5 6 0 15 9 13 14 12", //22 moves
					"1 2 4 12 5 6 3 0 9 10 8 7 13 14 11 15", //10 moves
    };

		//We map last selected position to each dimension to avoid generating same board twice
		private readonly Dictionary<BoardType, int> _lastChosenIndexByDimension;

		private readonly Random _rand;

		#endregion Private Fields

		#region Public Constructors

		public PuzzleGenerator()
		{
			_rand = new Random();
			_lastChosenIndexByDimension = new Dictionary<BoardType, int>();
		}

		#endregion Public Constructors

		#region Public Methods

		/// <summary>
		///Generate a random puzzle by starting from a  predefined position which is randomly selected
		/// Makes legal moves until it is a more random solution
		/// </summary>
		/// <param name="dimension">The dimension of the requested puzzle</param>
		/// <returns>a random puzzle represented by <see cref="IBoard"/></returns>
		public IBoard GenerateRandomPuzzle(int dimension)
		{
			return dimension switch
			{
				2 => GenerateEasyBoard(),
				3 => GenerateMediumBoard(),
				4 => GenerateHardBoard(),
				_ => null,
			};
		}

		#endregion Public Methods

		#region Private Methods

		/// <summary>
		/// Generate an easy 2x2 board by picking randomly one board from the easy boards.
		/// </summary>
		/// <returns>an random easy board</returns>
		private IBoard GenerateEasyBoard()
		{
			var randIndex = GetNextIndex(EasyBoards, BoardType.Easy);
			return new Board(EasyBoards[randIndex]);
		}

		/// <summary>
		/// Generates a medium 3x3 by picking randomly from the predefined boards.
		/// Moves the blank tile in random directions.
		/// </summary>
		/// <returns>an random medium board</returns>
		private IBoard GenerateMediumBoard()
		{
			return GetRandomBoard(MediumBoards, BoardType.Medium, 8);
		}

		/// <summary>
		/// Generates an hard 4x4 by picking randomly from the predefined boards.
		/// Moves the blank tile in random directions.
		/// </summary>
		/// <returns>an random hard board</returns>
		private IBoard GenerateHardBoard()
		{
			return GetRandomBoard(HardBoards, BoardType.Hard, 6);
		}

		/// <summary>
		/// Choose a random index to pick a board from the specific board array
		/// </summary>
		/// <param name="boards">The boards array.</param>
		/// <param name="boardType">The board type to be picked</param>
		/// <returns>an integer representing the index of the board that should be picked.</returns>
		private int GetNextIndex(string[] boards, BoardType boardType)
		{
			var randIndex = _rand.Next(boards.Length);
			if (_lastChosenIndexByDimension.ContainsKey(boardType) &&
					randIndex == _lastChosenIndexByDimension[boardType])
			{
				randIndex = (randIndex + 1) % boards.Length;
			}
			_lastChosenIndexByDimension[boardType] = randIndex;
			return randIndex;
		}

		/// <summary>
		/// Generates a random puzzle starting from a board randomly selected from the boards space
		/// Moves the blank tile
		/// </summary>
		/// <param name="boardsSpace">The board array.</param>
		/// <param name="boardType">The type of the board.</param>
		/// <param name="movesCount">The moves made by the blank tile</param>
		/// <returns>a random board</returns>
		private IBoard GetRandomBoard(string[] boardsSpace, BoardType boardType, int movesCount)
		{
			var startingBoardIndex = GetNextIndex(boardsSpace, boardType);

			IBoard currentBoard = new Board(boardsSpace[startingBoardIndex]);
			for (int move = 0; move < movesCount; move++)
			{
				var neighbors = currentBoard.GetNeighbordBoards();
				currentBoard = neighbors[_rand.Next(neighbors.Count)];
			}

			if (currentBoard.IsSolved()) return GetRandomBoard(boardsSpace, boardType, movesCount);
			return currentBoard;
		}

		#endregion Private Methods
	}
}