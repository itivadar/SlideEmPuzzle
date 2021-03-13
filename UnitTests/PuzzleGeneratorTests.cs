using Microsoft.VisualStudio.TestTools.UnitTesting;
using SliderPuzzleGenerator;
using SliderPuzzleSolver;
using System.Reflection;

namespace UnitTests
{
	[TestClass]
	public class PuzzleGeneratorTests
	{
		private IPuzzleGenerator _puzzleGenerator;

		[TestInitialize]
		public void Setup()
		{
			_puzzleGenerator = new PuzzleGenerator();
		}

		[TestMethod]
		public void AllEasyPuzzle_AreSolvable()
		{
			string[] states = (string[])typeof(PuzzleGenerator).GetField("EasyBoards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
			foreach(var currentState in states)
			{
				var puzzleBoard = new Board(currentState);
				Assert.IsTrue(puzzleBoard.IsSolvable(), $"Board {currentState} is not solvable");
			}
			Assert.IsNotNull(states);
		}

		[TestMethod]
		public void AllMediumPuzzle_AreSolvable()
		{
			string[] states = (string[])typeof(PuzzleGenerator).GetField("MediumBoards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
			foreach (var currentState in states)
			{
				var puzzleBoard = new Board(currentState);
				Assert.IsTrue(puzzleBoard.IsSolvable(), $"Board {currentState} is not solvable");
			}
			Assert.IsNotNull(states);
		}


		[TestMethod]
		public void AllHardPuzzle_AreSolvable()
		{
			string[] states = (string[])typeof(PuzzleGenerator).GetField("HardBoards", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
			foreach (var currentState in states)
			{
				var puzzleBoard = new Board(currentState);
				Assert.IsTrue(puzzleBoard.IsSolvable(), $"Board {currentState} is not solvable");
			}
			Assert.IsNotNull(states);
		}
	}
}