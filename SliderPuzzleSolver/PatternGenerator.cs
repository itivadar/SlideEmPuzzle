using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace SliderPuzzleSolver
{
  /// <summary>
  /// Class responsable for generating 555 Pattern Databse.
  /// Improvement for solving 15 puzzles.
  /// </summary>
  public class PatternGenerator
  {
    #region Private Constants

    /// <summary>
    /// The total number of different configuration in each pattern
    /// </summary>
    private const int EntriesCount = 524160;

    #endregion Private Constants

    #region Private Fields

    /// <summary>
    /// The tiles in the bottom pattern
    /// The 255 means we are not interested in that tiles. Sorry :(.
    /// </summary>
    private readonly byte[,] BottomPattern =
    {
           { 255, 255, 255, 255 },
           { 255, 255, 255, 255 },
           { 255, 10, 11, 255 },
           { 13, 14, 15, 0}
        };

    /// <summary>
    /// The tiles in the left pattern
    /// The 255 means we are not interested in that tiles. Sorry :(.
    /// </summary>
    private readonly byte[,] LeftPattern =
    {
           { 1, 2, 255, 255 },
           { 5, 6, 255, 255 },
           { 9, 255, 255, 255 },
           { 255, 255, 255, 0}
    };

    /// <summary>
    /// The tiles in the right pattern
    /// The 255 means we are not interested in that tiles. Sorry :(.
    /// </summary>
    private readonly byte[,] RightPattern =
    {
           { 255, 255, 3, 4 },
           { 255, 255, 7, 8 },
           { 255, 255, 255, 12 },
           { 255, 255, 255, 0}
    };

    /// <summary>
    /// Unique index board and the associated cost.
    /// </summary>
    private Dictionary<int, byte> _boardsCostMap;

    /// <summary>
    /// Visited nodes while running BFS.
    /// </summary>
    private Dictionary<int, bool> _visitedNodes;
    #endregion Private Fields

    #region Public Constructors
    /// <summary>
    /// Initializez a new <see cref="PatternGenerator"/>
    /// </summary>
    public PatternGenerator()
    {
      _visitedNodes = new Dictionary<int, bool>();
      _boardsCostMap = new Dictionary<int, byte>();
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    /// Generatess all 3 pattern database. At once!!
    /// </summary>
    public void GeneratAllPDBs()
    {
      GeneratPatternDb(Pattern555.Left5, ConstantHelper.LeftPatternFilePath);
      GeneratPatternDb(Pattern555.Bottom5, ConstantHelper.BottomPatternFilePath);
      GeneratPatternDb(Pattern555.Right5, ConstantHelper.RightPatternFilePath);
    }

    /// <summary>
    /// Genetes a pattern database for the subpuzzles of 5 tiles.
    /// </summary>
    /// <param name="pattern">The pattern for which the database is generated</param>
    /// <param name="filePath">The file path where the generated db is saved.</param>
    public void GeneratPatternDb(Pattern555 pattern, string filePath)
    {
      _visitedNodes.Clear();
      _boardsCostMap.Clear();
      var initialBoard = new Board(GetTile4Pattern(pattern));
      var rootNode = new Node { ParentNode = null, Cost = 0, Board = initialBoard };
      GenerateBFSNodes(rootNode, pattern);
      WriteDatabase(filePath);
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    /// Reads a Pattern database from the file.
    /// </summary>
    /// <param name="filePath">the file path</param>
    /// <returns>a dictionary of indexes mapped to their costs</returns>
    public static Dictionary<int, byte> ReadDbFromFile(string filePath)
    {
      var dictionary = new Dictionary<int, byte>();
      using var binaryReader = new BinaryReader(new FileStream(filePath, FileMode.Open));
      for (int index = 0; index < EntriesCount; index++)
      {
        var key = binaryReader.ReadInt32();
        var value = binaryReader.ReadByte();
        dictionary[key] = value;
      }

      return dictionary;
    }

    /// <summary>
    /// Performs Breath-First-Search to generate all the possible boards configuration for a pattern.
    /// </summary>
    /// <param name="initialNode">the initial start-up board.</param>
    /// <param name="pattern">the pattern for which the boards are generated</param>
    private void GenerateBFSNodes(Node initialNode, Pattern555 pattern)
    {
      Queue<Node> bigNodes = new Queue<Node>();
      bigNodes.Enqueue(initialNode);

      while (bigNodes.TryDequeue(out initialNode))
      {
        var currentNodeKey = initialNode.GetBlankTileIndex(pattern);
        if (!IsNodeVisited(currentNodeKey))
        {
          foreach (var childNode in initialNode.GetChildNodes(pattern))
          {
            bigNodes.Enqueue(childNode);
          }

          SaveMinCostForBoard(initialNode.Board.GetIndexForPattern(pattern), initialNode.Cost);
          MarkAsVisited(currentNodeKey);
        }
      }
    }

    /// <summary>
    /// Gets the initial tiles setup for a specific board.
    /// </summary>
    /// <param name="pattern555">The pattern for which the initial board is requested.</param>
    /// <returns></returns>
    private byte[,] GetTile4Pattern(Pattern555 pattern555)
    {
      return pattern555 switch
      {
        Pattern555.Left5 => LeftPattern,
        Pattern555.Right5 => RightPattern,
        _ => BottomPattern,
      };
    }
    /// <summary>
    /// Checks if the node has been visited.
    /// </summary>
    /// <param name="nodeKey">the node key which is the pattern index.</param>
    /// <returns>true if node exists already in the dictionary otherwise returns false</returns>
    private bool IsNodeVisited(int nodeKey)
    {
      return _visitedNodes.ContainsKey(nodeKey);
    }

    /// <summary>
    /// Marks a node as visited.
    /// </summary>
    /// <param name="nodeKey">the pattern index of the node.</param>
    private void MarkAsVisited(int nodeKey)
    {
      _visitedNodes[nodeKey] = true;
    }

    /// <summary>
    /// Saves the minimum cost of the board.
    /// A pattern board can have multiple costs depending on the position of the blank tile.
    /// We take the 
    /// </summary>
    /// <param name="boardIndex">the index of the board.</param>
    /// <param name="currentCost">the cost at visited node.</param>
    private void SaveMinCostForBoard(int boardIndex, int currentCost)
    {
      int foundCost = int.MaxValue;
      if (_boardsCostMap.ContainsKey(boardIndex))
      {
        foundCost = _boardsCostMap[boardIndex];
      }

      _boardsCostMap[boardIndex] = (byte)Math.Min(foundCost, currentCost);
    }

    /// <summary>
    /// Writes the generated database into a binary file.
    /// </summary>
    /// <param name="filePath">the path of the file where the database needs to be saved.</param>
    private void WriteDatabase(string filePath)
    {
      using var bw = new BinaryWriter(new FileStream(filePath, FileMode.Create));
      foreach (KeyValuePair<int, byte> kvp in _boardsCostMap)
      {
          bw.Write(kvp.Key);
          bw.Write(kvp.Value);
      };
    }
    #endregion Private Methods

    #region Public Classes

    /// <summary>
    /// Node used in the search tree.
    /// Each node has a board associated.
    /// </summary>
    public class Node
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
      public int Cost { get; set; }

      /// <summary>
      /// The previous node from which the current node has been reached.
      /// </summary>
      public Node ParentNode { get; set; }

      public int GetBlankTileIndex(Pattern555 pattern)
      {
        var rowMask = Board.BlankTilePosition.Row << 2;
        return ((Board.GetIndexForPattern(pattern) << 4) | rowMask) | Board.BlankTilePosition.Column;
      }

      #endregion Public Properties

      #region Public Methods

      /// <summary>
      /// Generates all the child nodes for this node.
      /// </summary>
      /// <param name="pattern">the pattern used</param>
      /// <returns>a list of all child nodes for the node</returns>
      public List<Node> GetChildNodes(Pattern555 pattern)
      {
        List<Node> childNodes = new List<Node>();
        foreach (var childBoard in Board.GetNeighbordBoards())
        {
          var newNode = new Node { ParentNode = this, Board = childBoard, Cost = this.Cost };

          // a tile from the pattern has been moved so we increased the cost
          // if no tile from the pattern ( moves between 255 tiles and 0) we dont increase the cost
          if (newNode.GetBlankTileIndex(pattern) >> 4 != Board.GetIndexForPattern(pattern))
          {
            newNode.Cost++;
          }

          childNodes.Add(newNode);
        }

        return childNodes;
      }

      #endregion Public Methods
    }

    #endregion Public Classes
  }
}