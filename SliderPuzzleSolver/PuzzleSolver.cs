using BinaryHeap;
using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SliderPuzzleSolver
{

  /// <summary>
  /// Class responsable for solving the puzzles
  /// </summary>
  public sealed class PuzzleSolver : IPuzzleSolver
  {
    #region Constants

    private const int NodeFound = -1;
    private const int NodeNotFound = Int32.MaxValue;
    #endregion Constants


    #region Private Fields

    private Dictionary<int, byte> _bottom5Indexes;
    private Dictionary<int, byte> _left5Indexes;
    private Dictionary<int, byte> _right5Indexes;
    private Func<IBoard, int> GetCostForBoard;
    private bool _isPatternDatabaseLoaded = false;

    #endregion Private Fields


    #region Public methods

    /// <summary>
    /// Sequence of directions in which the blank tile should be moved in order to solve the puzzle.
    /// </summary>
    /// <param name="boardToSolve">the puzzle to solve.</param>
    /// <returns>an IEnumerable of directions</returns>
    public IEnumerable<SlideDirection> GetSolutionDirections(IBoard boardToSolve)
    {
      if (!boardToSolve.IsSolvable())
      {
        Console.WriteLine("Unsolvable puzzle");
        return new List<SlideDirection>() as IEnumerable<SlideDirection>;
      }

      var lastNode = IDASolve(boardToSolve);

      return BuildSolutionDirections(lastNode);
    }

    /// <summary>
    /// Sequence of steps fromt the initial board to the goal board.
    /// </summary>
    /// <param name="boardToSolve">the puzzle to solve.</param>
    /// <returns>an IEnumerable of directiond</returns>
    public IEnumerable<IBoard> SolutionSteps(IBoard boardToSolve)
    {
      if (!boardToSolve.IsSolvable())
      {
        Console.WriteLine("Unsolvable puzzle");
        return new List<IBoard>() as IEnumerable<IBoard>;
      }
  
      Stopwatch stopWatch = Stopwatch.StartNew();
      var lastNode = IDASolve(boardToSolve);
      stopWatch.Stop();
      Console.WriteLine($"Done in {stopWatch.ElapsedMilliseconds} ms.");
      return BuildSolutionSteps(lastNode) as IEnumerable<IBoard>;
    }

    #endregion Public methods

    #region Private Methods

    /// <summary>
    /// Loads the 5-5-5 Pattern Databases if not already loaded.
    /// </summary>
    private void LoadsPatternDatabase()
    {
      if (_isPatternDatabaseLoaded) return;
      _right5Indexes = PatternGenerator.ReadDbFromFile(@"C:\users\neo_c\desktop\Right5.db");
      _left5Indexes =  PatternGenerator.ReadDbFromFile(@"C:\users\neo_c\desktop\Right5.db");
      _bottom5Indexes = PatternGenerator.ReadDbFromFile(@"C:\users\neo_c\desktop\Right5.db");
      _isPatternDatabaseLoaded = false;
    }

    /// <summary>
    /// Use IDA* to search for the optimal solution to a slider puzzle.
    /// </summary>
    /// <param name="startingBoard">the starting board that needs to be solved.</param>
    /// <returns>the node of the goal board used to construct the steps backwards to the starting boards</returns>
    private Node IDASolve(IBoard startingBoard)
    {
      ///setting the cost function.
      GetCostForBoard = GetManhattanCost;
      if (startingBoard.Rows == 4)
      {
        GetCostForBoard = GetPatternDatabaseCost;
        //loading is made only once
        LoadsPatternDatabase();
      }

      //startingNode has no parent node
      var startingNode = CreateNode(startingBoard, null);

      //used to remember each node in the path from the board that needs to be solved to the goal board
      var path = new Stack<Node>();
      path.Push(startingNode);

      int thresholdDepth = GetCostForBoard(startingBoard);
      while (true)
      {
        var searchResult = BreadthFirstSearch(path, thresholdDepth);

        if (searchResult == NodeFound) return path.Peek();
        //there are no solution to the puzzle
        if (searchResult == NodeNotFound) { return null; }

        //the depth limit is increasead and the search is restarted
        thresholdDepth = searchResult;
      }
    }


    //makes a breadth first search into the search graph for the current puzzle
    //the search stops when the search cost exceeds a given threshold as parameter
    //it returns
    // - NodeFound (-1) if the goal node was reached.
    // - NodeNotFound (Int32.MaxValue) if no solution were found
    // - an integer representing the new threshold for the BFS search
    private int BreadthFirstSearch(Stack<Node> path, int threshold)
    {
      var node = path.Peek();
      var currentCost = node.Depth + GetCostForBoard(node.Board);

      //we increase the threshold and start over.
      if (currentCost > threshold) return currentCost;

      //we found the goal board
      if (GetCostForBoard(node.Board) == 0) return NodeFound;

      var min = NodeNotFound;
      foreach (IBoard childBoard in node.Board.GetNeighbordBoards())
      {
        var neighborNode = CreateNode(childBoard, node);
        if (node.ParentNode != null && childBoard.Equals(node.ParentNode.Board)) continue;

        path.Push(neighborNode);
        var result = BreadthFirstSearch(path, threshold);

        if (result == NodeFound) return NodeFound;
        min = Math.Min(result, min);
        path.Pop();
      }
      return min;
    }

    /// <summary>
    ///  Builds the sequence of boards starting from the goal node backwards to the starting node.
    /// </summary>
    /// <param name="lastNode">the node with the solution board.</param>
    /// <returns>a sequence of boards as IEnumerable</returns>
    private IEnumerable<IBoard> BuildSolutionSteps(Node lastNode)
    {
      Stack<IBoard> sol = new Stack<IBoard>();
      while (lastNode != null)
      {
        sol.Push(lastNode.Board);
        lastNode = lastNode.ParentNode;
      }

      return sol as IEnumerable<IBoard>;
    }


    /// <summary>
    ///  Builds the sequence directions requred  to solve the puzzles.
    /// </summary>
    /// <param name="lastNode">the node with the solution board.</param>
    /// <returns>a sequence of directionsas IEnumerable</returns>
    private IEnumerable<SlideDirection> BuildSolutionDirections(Node lastNode)
    {
      Stack<SlideDirection> directions = new Stack<SlideDirection>();
      while (lastNode.ParentNode != null)
      {
        directions.Push(GetMoveDirection(lastNode.ParentNode.Board, lastNode.Board));
        lastNode = lastNode.ParentNode;
      }
      return directions;
    }


  /// <summary>
  /// Creates a node for a board to be added in the search tree. 
  /// The search for a solution is made in a tree where each board has a single parent. 
  /// </summary>
  /// <param name="board">the board for which the node is created.</param>
  /// <param name="previousNode">the parent node.</param>
  /// <returns>the node created for the board<returns>
  private Node CreateNode(IBoard board, Node previousNode)
    {
      return new Node
      {
        Board = board,
        ParentNode = previousNode,
        Depth = (ushort)(previousNode is null ? 0 : previousNode.Depth + 1)
      };
    }

    /// <summary>
    /// Gets the 555 pattern database cost for a board.
    /// Used only for 4x4 puzzles.
    /// </summary>
    /// <param name="board">the board for which the cost is required.</param>
    /// <returns>an integer representing the cost</returns>
    private int GetPatternDatabaseCost(IBoard board)
    {
      return _right5Indexes[board.GetIndexForPattern(Pattern555.Right5)] +
             _bottom5Indexes[board.GetIndexForPattern(Pattern555.Bottom5)] +
             _left5Indexes[board.GetIndexForPattern(Pattern555.Left5)];
    }

    /// <summary>
    /// Gets the Manhattan cost for a board.
    /// </summary>
    /// <param name="board">the board for which the cost is required.</param>
    /// <returns>an integer representing the cost</returns>
    private int GetManhattanCost(IBoard board)
    {
      return board.ManhattanDistance;
    }

    /// <summary>
    /// Gets the direction blank tile has been traveled between two boards.
    /// </summary>
    /// <param name="fromBoard">The initial board acting as the source board.</param>
    /// <param name="toBoard">The final board acting as the destination. </param>
    /// <returns></returns>
    private SlideDirection GetMoveDirection(IBoard fromBoard, IBoard toBoard)
    {
      var horizontalMove = toBoard.BlankTilePosition.Column - fromBoard.BlankTilePosition.Column;
      var verticalMove = toBoard.BlankTilePosition.Row - fromBoard.BlankTilePosition.Row;

      if (horizontalMove == -1) return SlideDirection.Left;
      if (horizontalMove == 1) return SlideDirection.Right;
      if (verticalMove == -1) return SlideDirection.Up;
      return SlideDirection.Down;
    }
    
    /// <summary>
    /// [not used, only for demo & benchmarking]
    /// Solves a puzzle by using the A* algorithm.
    /// </summary>
    /// <param name="board">the board to be solved.</param>
    /// <returns>the node with the solved board used to constuct the solution steps.</returns>
    private Node Solve(IBoard board)
    {
      IHeap<Node> minPriorityQueue = new MinHeap<Node>(new ManhattanPriorityFunction());
      //no previous node, 0 moves to get to the root
      var root = CreateNode(board, null);

      minPriorityQueue.Add(root);
      Node dequeuedNode;
      do
      {
        dequeuedNode = minPriorityQueue.PopMin();
        foreach (var neighbor in dequeuedNode.Board.GetNeighbordBoards())
        {
          if (dequeuedNode.ParentNode is null ||
              !dequeuedNode.ParentNode.Board.Equals(neighbor))
          {
            //calculate the priority function and updates the moves needed to get to the created node
            var childNode = CreateNode(neighbor, dequeuedNode);

            minPriorityQueue.Add(childNode);
          }
        }
      } while (!dequeuedNode.Board.IsSolved());

      return dequeuedNode;
    }

    #endregion Private Methods

    #region Helper Classes

    /// <summary>
    /// Compare used to compare nodes in the A* search algorithm
    /// </summary>
    public class ManhattanPriorityFunction : IComparer<Node>
    {
      #region Public Methods

      public int Compare(Node currentNode, Node otherNode)
      {
        var currentNodeDistance = currentNode.Board.ManhattanDistance + currentNode.Depth;
        var otherNodeDistance = otherNode.Board.ManhattanDistance + otherNode.Depth;

        if (currentNodeDistance < otherNodeDistance) return -1;
        if (currentNodeDistance > otherNodeDistance) return 1;

        return 0;
      }

      #endregion Public Methods
    }

    /// <summary>
    /// Node used in the search tree.
    /// Each node has a board associated.
    /// </summary>
    public class Node : IComparable
    {
      #region Public Properties

      /// <summary>
      /// The board associated with the node.
      /// </summary>
      public IBoard Board { get; set; }

      /// <summary>
      /// The depth of the node.
      /// It gives the numbers of steps required to reach this node from the initial position.
      /// </summary>
      public ushort Depth { get; set; }

      /// <summary>
      /// The previous node from which the current node has been reached.
      /// </summary>
      public Node ParentNode { get; set; }

      #endregion Public Properties

      #region Public Methods

      ///used for comparing nodes to maintain the MinHeap property
      public int CompareTo(object obj)
      {
        return new ManhattanPriorityFunction().Compare(this, obj as Node);
      }

      #endregion Public Methods
    }

    #endregion Helper Classes
  }
}