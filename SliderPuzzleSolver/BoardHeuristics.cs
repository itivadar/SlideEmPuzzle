using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;

namespace SliderPuzzleSolver
{
  /// <summary>
  /// Class used for determining heuristics regarding a specific puzzle board.
  /// </summary>
  public class BoardHeuristics
  {
    #region Private fields
    private readonly IBoard _board;
    private readonly Dictionary<Pattern555, int> _patternIndexes;
    private byte? _manhattanDistance;

    #endregion

    #region Constructor

    /// <summary>
    /// Initialis board heuristics for a given board.
    /// </summary>
    /// <param name="board">the board associated with the heuristics</param>
    public BoardHeuristics(IBoard board)
    {
      _board = board;
      _patternIndexes = new Dictionary<Pattern555, int>();
    }
    #endregion

    #region Heuristics related to the 8 Puzzle

    /// <summary>
    /// Gets the Manhattan distance for a board.
    /// </summary>
    public byte ManhattanDistance
    {
      set
      {
        _manhattanDistance = value;
      }

      get
      {
        if (!_manhattanDistance.HasValue)
        {
          _manhattanDistance = CalculateManhattan();
        }

        return _manhattanDistance.Value;
      }
    }

    /// <summary>
    /// Calculates the Manhattan distance for the entire board.
    /// Manhattan distance: total Manhattan distance to each tile to its goal position
    /// </summary>
    /// <returns>the Manhattan distance for the entire board.</returns>
    private byte CalculateManhattan()
    {
      byte distance = 0;
      for (int row = 0; row < _board.Rows; row++)
        for (int column = 0; column < _board.Rows; column++)
        {
          var value = _board.GetTileAt(row, column);
          if (value == ConstantHelper.BlankTileValue)
          {
            continue;
          }
          distance += GetManhattanForSingleTile(value, row, column);
        }

      return distance;
    }

    /// <summary>
    /// Calculates the Manhattan distance for a single tile.
    /// </summary>
    /// <param name="tile">the tile</param>
    /// <param name="x">current row of the tile</param>
    /// <param name="y">current column of the tile</param>
    /// <returns>the Manhattan distance for the current tile</returns>
    public byte GetManhattanForSingleTile(int tile, int x, int y)
    {
      var value = tile - 1;
      var goalRow = (value) / _board.Rows;
      var goalCol = (value) % _board.Rows;
      return (byte)(Math.Abs(goalRow - x) + Math.Abs(goalCol - y));
    }

    #endregion

    #region Heuristics related to the 15 Puzzle


    /// <summary>
    /// Sets the index state for a specific pattern.
    /// The index is changed when a tile is moved.
    /// </summary>
    /// <param name="pattern">the pattern for which the index is changed.</param>
    /// <param name="newIndex">the new value.</param>
    public void SetIndexForPattern(Pattern555 pattern, int value)
    {
      _patternIndexes[pattern] = value;
    }

    /// <summary>
    /// Gets the index board of a specific pattern.
    /// Each board is defined by a state in each of the three patterns.
    /// Index is computed by transformed each tile position into the bits.
    /// </summary>
    /// <param name="pattern">the pattern</param>
    /// <returns>an int represeting the index for the requested pattern.</returns>
    public int GetIndexForPattern(Pattern555 pattern)
    {
      if (!_patternIndexes.ContainsKey(pattern))
      {
        _patternIndexes[pattern] = CalculateIndexForPattern(pattern);
      }

      return _patternIndexes[pattern];
    }

    /// <summary>
    /// Gets the position bits for the given tile.
    /// </summary>
    /// <param name="tile">the tile value</param>
    /// <param name="row">the row of the tile</param>
    /// <param name="column">the column of the tile</param>
    /// <param name="mask">retrives the mask having the bits used for position set on 1.</param>
    /// <returns>an int having the bits according to the tile position</returns>
    public int GetPositionBits(byte tile, int row, int column, out int mask)
    {
      var positionMask = 0;
      positionMask |= row << (ConstantHelper.ValueToIndexPositionMap[tile] * 4 + 2);
      positionMask |= column << (ConstantHelper.ValueToIndexPositionMap[tile] * 4);

      mask = 0xf << ConstantHelper.ValueToIndexPositionMap[tile] * 4;
      return positionMask;
    }

    /// <summary>
    /// Gets the index for the neighbor board.
    /// </summary>
    /// <param name="neighborBoard">the neighbor board</param>
    /// <param name="pattern">the pattern for which the index is requested.</param>
    /// <returns>the index of the pattern for the neighbor board.</returns>
    public int GetFuturePatternIndex(IBoard neighborBoard, Pattern555 pattern)
    {
      var slidedTile = _board.GetTileAt(neighborBoard.BlankTilePosition.Row, neighborBoard.BlankTilePosition.Column);

      var isPatternFound = ConstantHelper.TileToPatternMap.TryGetValue(slidedTile, out Pattern555 patternChanged);

      if (isPatternFound && patternChanged == pattern)
      {
        var positionMask = GetPositionBits(slidedTile, _board.BlankTilePosition.Row, _board.BlankTilePosition.Column, out int mask);
        var index = GetIndexForPattern(pattern);

        return (index & ~mask) | (positionMask & mask);
      }
      else
      {
        //index for this db remains the same
        return GetIndexForPattern(pattern);
      }
    }

    /// <summary>
    /// Calculates an index of the specified pattern.
    /// Sets 20 bitis: 2 bits for row, 2 for colomnfor each tile position in the pattern.
    /// </summary>
    /// <param name="pattern">the pattern requested.</param>
    /// <returns>the index of the board</returns>
    private int CalculateIndexForPattern(Pattern555 pattern)
    {
      int index = 0;
      for (int rowId = 0; rowId < _board.Rows; rowId++)
        for (int colomnId = 0; colomnId < _board.Rows; colomnId++)
        {
          var value = _board.GetTileAt(rowId, colomnId);

          if (ConstantHelper.ValueToIndexPositionMap.ContainsKey(value) &&
              ConstantHelper.TileToPatternMap[value] == pattern)
          {
            index |= GetPositionBits(value, rowId, colomnId, out _);
          }
        }

      return index;
    }

    #endregion
  }
}