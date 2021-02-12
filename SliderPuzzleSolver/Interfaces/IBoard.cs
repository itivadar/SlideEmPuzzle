using System;
using System.Collections.Generic;
using System.Text;

namespace SliderPuzzleSolver.Interfaces
{
  public interface IBoard
  {
    /// <summary>
    /// Gets the number of rows (equal to the numbers of colomns)
    /// </summary>
    byte Rows { get; }

    /// <summary>
    /// Gets the position of the blank tile
    /// </summary>
    (byte Row, byte Column) BlankTilePosition { get; }

    /// <summary>
    /// Gets the Manhattan distance of the board.
    /// </summary>
    byte ManhattanDistance { get; }

    /// <summary>
    /// Gets the index board of a specific pattern.
    /// Each board is defined by a state in each of the three patterns.
    /// </summary>
    /// <param name="pattern">the pattern</param>
    /// <returns>an int represeting the index for the requested pattern.</returns>
    int GetIndexForPattern(Pattern555 pattern);

    /// <summary>
    /// Gets the tile value at a specific position.
    /// </summary>
    /// <param name="row">the row of the tile.</param>
    /// <param name="colomn">the column of the tile</param>
    /// <returns>a byte represeting the tile value at requested position</returns>
    byte GetTileAt(int row, int colomn);

    /// <summary>
    /// Moves the blank tile in a given direction.
    /// Swaps the blank tile with a tile.
    /// </summary>
    /// <param name="direction">the requested direction</param>
    void MoveBlankTile(SlideDirection direction);

    /// <summary>
    /// Checks if the board is solved.
    /// The board is solved when all tiles are are sorted in ascending order.
    /// </summary>
    /// <returns>true is the board is solved otherwise returns false.</returns>
    bool IsSolved();

    /// <summary>
    /// If Dimension is odd, then puzzle instance is solvable if number of inversions is even in the input state.
    /// If Dimension is even, puzzle instance is solvable if:
    ///     the blank is on an even row counting from the bottom and number of inversions is odd.
    ///     the blank is on an odd row counting from the bottom and number of inversions is even.
    /// </summary>
    /// <returns>true if the board is solvable, otherwise returns false</returns>
    bool IsSolvable();


    /// <summary>
    /// Generates all the neighbord boards.
    /// A neighbord board is a board where the blank makes an move from the parent board.
    /// </summary>
    /// <returns>list with all the neighbords boards</returns>
    List<IBoard> GetNeighbordBoards();


    /// <summary>
    /// Generated a twin board of the current board.
    /// A twin is obtained by swapping any two tiles which are not blank.
    /// </summay>
    /// <returns>a board which is twin of the current one.</returns>
    public IBoard Twin();

    /// <summary>
    /// Clones the current board.
    /// </summary>
    /// <returns>an identic board to the one which is cloned.</returns>
    public IBoard Clone();

  }
}
