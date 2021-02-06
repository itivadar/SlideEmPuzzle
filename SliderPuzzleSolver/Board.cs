using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SliderPuzzleSolver
{
  /// <summary>
  /// Class model a puzzle board
  /// </summary>
  public sealed class Board : IBoard, IEquatable<IBoard>
  {
    #region Fields
    /// <summary>
    /// The hash code of the board.
    /// Null if it was not calculated yet.
    /// </summary>
    private int? _hashCode;

    /// <summary>
    /// The heuristics for the current board.
    /// Used when solving puzzle.
    /// </summary>
    private BoardHeuristics _heuristics;

    /// <summary>
    /// The current state for the board
    /// each [x,y]  represents the tile number found at position (x,y)
    /// </summary>
    private byte[,] _tiles;
    #endregion Fields

    #region Constructors

    /// <summary>
    /// Intialies a new board from a given tiles array.
    /// </summary>
    /// <param name="tiles">the tiles array.</param>
    public Board(byte[,] tiles)
    {
      if (tiles is null)
      {
        throw new ArgumentNullException();
      }

      SetupBoad(tiles);
    }

    /// <summary>
    /// Initialies a new board from a given tiles array and the blank tile position.
    /// </summary>
    /// <param name="tiles">the tile array.</param>
    /// <param name="BlankTile">the position of the blank tile.</param>
    public Board(byte[,] tiles, (byte, byte) BlankTile)
    {
      Rows = (byte)tiles.GetLength(0);
      BlankTilePosition = BlankTile;
      _tiles = tiles;
      _heuristics = new BoardHeuristics(this);
    }

    /// <summary>
    /// Intialies a new board from a string.
    /// </summary>
    /// <param name="tiles">the string containing the tiles with spacing between two tiles.</param>
    public Board(string tilesRepresentation)
    {
      var tiles = GetTiles(tilesRepresentation);
      SetupBoad(tiles);
    }
    #endregion

    #region Proprieties

    /// <summary>
    /// Gets the position of the blank tile
    /// </summary>
    public (byte Row, byte Column) BlankTilePosition { get; private set; }

    /// <summary>
    /// Gets the Manhattan distance of the board.
    /// </summary>
    public byte ManhattanDistance
    {
      private set => _heuristics.ManhattanDistance = value;
      get => _heuristics.ManhattanDistance;
    }

    /// <summary>
    /// Gets the number of rows (equal to the numbers of colomns)
    /// </summary>
    public byte Rows { get; private set; }

    /// <summary>
    /// Gets the index board of a specific pattern.
    /// Each board is defined by a state in each of the three patterns.
    /// </summary>
    /// <param name="pattern">the pattern</param>
    /// <returns>an int represeting the index for the requested pattern.</returns>
    public int GetIndexForPattern(Pattern555 pattern)
    {
      return _heuristics.GetIndexForPattern(pattern);
    }

    /// <summary>
    /// Sets the index state for a specific pattern.
    /// The index is changed when a tile is moved.
    /// </summary>
    /// <param name="pattern">the pattern for which the index is changed.</param>
    /// <param name="newIndex">the new value.</param>
    public void SetIndexForPattern(Pattern555 pattern, int newIndex)
    {
      _heuristics.SetIndexForPattern(pattern, newIndex);
    }
    #endregion Proprieties

    #region Public Methods

    /// <summary>
    /// Gets the goal board of a specific dimension.
    /// The goal board have all the tiles sorted with the blank tile in the bottom left corner.
    /// </summary>
    /// <param name="dimension">the number of the rows of the requested board.</param>
    /// <returns>the goal board of the specifed dimension</returns>
    public static IBoard GetGoalBoard(int dimension)
    {
      var tiles = new byte[dimension, dimension];
      for (int row = 0; row < dimension; row++)
        for (int columnId = 0; columnId < dimension; columnId++)
        {
          tiles[row, columnId] = (byte)(row * dimension + columnId + 1);
        }
      tiles[dimension - 1, dimension - 1] = 0;
      return new Board(tiles);
    }

    /// <summary>
    /// Clones the current board.
    /// </summary>
    /// <returns>an identic board to the one which is cloned.</returns>
    public IBoard Clone()
    {
      var newTiles = _tiles.Clone() as byte[,];
      return new Board(newTiles);
    }

    /// <summary>
    /// Determines if two board are identical.
    /// </summary>
    /// <param name="obj">the board with which the comparation is made.</param>
    /// <returns>true if the boards are identical</returns>
    public bool Equals(IBoard obj)
    {
      var otherBoard = obj as Board;
      if (Rows != otherBoard.Rows) return false;
      for (int row = 0; row < Rows; row++)
        for (int columnId = 0; columnId < Rows; columnId++)
        {
          if (_tiles[row, columnId] != otherBoard._tiles[row, columnId])
          {
            return false;
          }
        }

      return true;
    }

    /// <summary>
    /// Calculates the hash code of the object.
    /// </summary>
    /// <returns>an int represinting the hash of the current board</returns>
    public override int GetHashCode()
    {
      return _hashCode ?? ComputeHash();
    }

    /// <summary>
    /// Generates all the neighbord boards.
    /// A neighbord board is a board where the blank makes an move from the parent board.
    /// </summary>
    /// <returns>list with all the neighbords boards</returns>
    public List<IBoard> GetNeighbordBoards()
    {
      var childBoards = new List<IBoard>();
      foreach (var direction in ConstantHelper.DirectionsTransfom.Keys)
      {
        var neighborTile = GetNeighborTiles(direction);

        if (neighborTile != null)
        {
          var childBoard = new Board(neighborTile, GetNeighbordBlankTilePosition(direction));

          UpdateHeuristicsForBoard(childBoard);
          childBoards.Add(childBoard);
        }
      }
      return childBoards;
    }

    /// <summary>
    /// Gets the tile value at a specific position.
    /// </summary>
    /// <param name="row">the row of the tile.</param>
    /// <param name="colomn">the column of the tile</param>
    /// <returns>a byte represeting the tile value at requested position</returns>
    public byte GetTileAt(int row, int column)
    {
      return _tiles[row, column];
    }


    /// <summary>
    /// If Dimension is odd, then puzzle instance is solvable if number of inversions is even in the input state.
    /// If Dimension is even, puzzle instance is solvable if:
    ///     the blank is on an even row counting from the bottom and number of inversions is odd.
    ///     the blank is on an odd row counting from the bottom and number of inversions is even.
    /// </summary>
    /// <returns>true if the board is solvable, otherwise returns false</returns>
    public bool IsSolvable()
    {
      var invCount = InversionCount();
      if (Rows % 2 == 1)
      {
        return invCount % 2 == 0;
      }

      return (Rows - BlankTilePosition.Row) % 2 != invCount % 2;
    }

    /// <summary>
    /// Checks if the board is solved.
    /// The board is solved when all tiles are are sorted in ascending order.
    /// </summary>
    /// <returns>true is the board is solved otherwise returns false.</returns>

    public bool IsSolved()
    {
      return ManhattanDistance == 0;
    }


    /// <summary>
    /// Moves the blank tile in a given direction.
    /// Swaps the blank tile with a tile.
    /// </summary>
    /// <param name="direction">the requested direction</param>
    public void MoveBlankTile(SlideDirection direction)
    {
      var (Row, Column) = ConstantHelper.DirectionsTransfom[direction];
      var newPosition = (Row: (byte)(BlankTilePosition.Row + Row), Column: (byte)(BlankTilePosition.Column + Column));

      if (newPosition.Row < 0 || newPosition.Row >= Rows) return;
      if (newPosition.Column < 0 || newPosition.Column >= Rows) return;

      Swap(_tiles, newPosition, BlankTilePosition);
      BlankTilePosition = newPosition;
    }

    /// <summary>
    /// Gets the string representation of the board.
    /// </summary>
    /// <returns>the string representation</returns>
    public override string ToString()
    {
      var representation = new StringBuilder();

      for (int row = 0; row < Rows; row++)
      {
        representation.AppendLine();
        for (int columnId = 0; columnId < Rows; columnId++)
        {
          representation.Append($"{_tiles[row, columnId]} ");
        }
      }

      return representation.ToString();
    }

    /// <summary>
    /// Generated a twin board of the current board.
    /// A twin is obtained by swapping any two tiles which are not blank.
    /// </summay>
    /// <returns>a board which is twin of the current one.</returns>
    public IBoard Twin()
    {
      var newTiles = _tiles.Clone() as byte[,];
      var rand = new Random();
      //determines random the value of first tile
      var firstValuePos = (Row: rand.Next() % Rows, column: rand.Next() % Rows);
      //move one row if gets to the blank position
      if (firstValuePos == BlankTilePosition)
      {
        firstValuePos.Row = (firstValuePos.Row + 1) % Rows;
      }
      //the second tile is on another column to avoid swapping same tile
      var secondValuePos = (Row: firstValuePos.Row, column: (firstValuePos.column + 1) % Rows);

      if (secondValuePos == BlankTilePosition)
      {
        secondValuePos.Row = (secondValuePos.Row + 1) % Rows;
      }

      Swap(newTiles, firstValuePos, secondValuePos);

      return new Board(newTiles);
    }
    #endregion Public Methods

    #region Private Methods

    /// <summary>
    /// Calculates the right hash for the curent board.
    /// </summary>
    /// <returns></returns>
    private int ComputeHash()
    {
      //TODO: maybe improves this algorithm
      int hash = 0;
      foreach (int tile in _tiles)
      {
        hash = HashCode.Combine(tile, hash);
      }
      _hashCode = hash;
      return _hashCode.Value;
    }

    /// <summary>
    /// Gets the Manhattan difference when a tile is moved.
    /// When a tile is moved its Manhattan distance to the goal can flactuate by either  -1 or +1.
    /// </summary>
    /// <param name="neighbordBoard">the neighboard board where the tile  is situated in the new position.</param>
    /// <returns>either -1 or +1</returns>
    private byte GetManhattanDeltaForNeighbor(Board neighbordBoard)
    {
      var tile = _tiles[neighbordBoard.BlankTilePosition.Row, neighbordBoard.BlankTilePosition.Column];


      var currentDistance = _heuristics.GetManhattanForSingleTile(tile,
                                                                 neighbordBoard.BlankTilePosition.Row,
                                                                 neighbordBoard.BlankTilePosition.Column);

      var futureDistance = _heuristics.GetManhattanForSingleTile(tile,
                                                                 BlankTilePosition.Row,
                                                                 BlankTilePosition.Column);

      return (byte)(futureDistance - currentDistance);
    }

    /// <summary>
    /// If the move of the blank til is possible return tile array where the blank tile is moved according to the direction
    /// If the move is not possible, return null
    /// </summary>
    /// <param name="direction">the direction in which the sliding is made.</param>
    /// <returns>a tile array having the blank tile in the new position</returns>
    private byte[,] GetNeighborTiles(SlideDirection direction)
    {
      var (Row, Column) = ConstantHelper.DirectionsTransfom[direction];
      var newPosition = (Row: BlankTilePosition.Row + Row, Column: BlankTilePosition.Column + Column);

      if (newPosition.Row < 0 || newPosition.Row >= Rows) return null;
      if (newPosition.Column < 0 || newPosition.Column >= Rows) return null;

      byte[,] transformedArray = new byte[Rows, Rows];

      for (int row = 0; row < Rows; row++)
        for (int columnId = 0; columnId < Rows; columnId++)
        {
          transformedArray[row, columnId] = _tiles[row, columnId];
        }

      Swap(transformedArray, newPosition, BlankTilePosition);

      return transformedArray;
    }

    /// <summary>
    /// Gets a tile array from a string representation.
    /// </summary>
    /// <param name="stringRepresentation">the string representation.</param>
    /// <returns>a tile array </returns>
    private byte[,] GetTiles(string stringRepresentation)
    {
      var tilesValues = stringRepresentation.Split(" ");
      int rows = (int)Math.Sqrt(tilesValues.Length);

      //the board is empty.
      if (rows == 1) return new byte[,] { };

      byte[,] tiles = new byte[rows, rows];

      for (int i = 0; i < rows; i++)
        for (int j = 0; j < rows; j++)
        {
          tiles[i, j] = byte.Parse(tilesValues[i * rows + j]);
        }

      return tiles;
    }

    /// <summary>
    /// Treats a board as a permutation and returns the number of inversions of that permutation
    /// Used to determined if a board is solvable
    /// </summary>
    /// <returns> the inversion count</returns>
    private ushort InversionCount()
    {
      byte s = 0;
      int n = Rows * Rows;
      byte[] copyArr = new byte[n];
      for (byte i = 0; i < Rows; i++)
        for (byte j = 0; j < Rows; j++)
        {
          copyArr[i * Rows + j] = GetTileAt(i, j);
        }

      for (var i = 0; i < n; i++)
      {
        for (var j = i; j < n; j++)
        {
          if (copyArr[i] == ConstantHelper.BlankTileValue || copyArr[j] == ConstantHelper.BlankTileValue) continue;
          if (copyArr[i] > copyArr[j]) s++;
        }
      }
      return s;
    }

    /// <summary>
    /// Sets the tile array from a provided tile array.
    /// </summary>
    /// <param name="tiles">the provided tile array.</param>
    private void SetupBoad(byte[,] tiles)
    {
      Rows = (byte)tiles.GetLength(0);
      _tiles = new byte[Rows, Rows];

      for (byte i = 0; i < Rows; i++)
      {
        for (byte j = 0; j < Rows; j++)
        {
          _tiles[i, j] = tiles[i, j];
          if (_tiles[i, j] == ConstantHelper.BlankTileValue)
          {
            BlankTilePosition = (i, j);
          }
        }
      }

      _heuristics = new BoardHeuristics(this);
    }

    /// <summary>
    /// Updates the heuristics for a neighbord board.
    /// Each heuristic can be determined from the previous board so its no need for recalculation.
    /// </summary>
    /// <param name="neighbordBoard">the neighboard for which the heuristics are updated</param>
    private void UpdateHeuristicsForBoard(Board neighbordBoard)
    {
      //update Manhattan heuristic
      neighbordBoard.ManhattanDistance = (byte)(ManhattanDistance + GetManhattanDeltaForNeighbor(neighbordBoard));

      //update 5-5-5 Pattern Database heuristics
      neighbordBoard.SetIndexForPattern(Pattern555.Left5, _heuristics.GetFuturePatternIndex(neighbordBoard, Pattern555.Left5));
      neighbordBoard.SetIndexForPattern(Pattern555.Right5, _heuristics.GetFuturePatternIndex(neighbordBoard, Pattern555.Right5));
      neighbordBoard.SetIndexForPattern(Pattern555.Bottom5, _heuristics.GetFuturePatternIndex(neighbordBoard, Pattern555.Bottom5));
    }

    /// <summary>
    /// Get the blank tile position after it is moved in the specified direction.
    /// </summary>
    /// <param name="direction">the specified direction</param>
    /// <returns>a tuple (byte, byte) with the row and the colomn of the blank tile</returns>
    private (byte Row, byte Colomn) GetNeighbordBlankTilePosition(SlideDirection direction)
    {
      var (Row, Column) = ConstantHelper.DirectionsTransfom[direction];
      var futureBlankTileRow = (byte)(BlankTilePosition.Row + Row);
      var futureBlankTileColumn = (byte)(BlankTilePosition.Column + Column);

      return (futureBlankTileRow, futureBlankTileColumn);
    }


    /// <summary>
    /// Swa[s two tiles in a array
    /// </summary>
    /// <param name="array">the array in which the swapping is made.</param>
    /// <param name="firstValue">the first tile positon.</param>
    /// <param name="secondValue">the second tile position.</param>
    private void Swap(byte[,] array, (int Row, int column) firstValue, (int Row, int column) secondValue)
    {
      byte tmp = array[firstValue.Row, firstValue.column];
      array[firstValue.Row, firstValue.column] = array[secondValue.Row, secondValue.column];
      array[secondValue.Row, secondValue.column] = tmp;
    }

    #endregion Private Methods
  }
}