using System;
using System.Collections.Generic;
using System.Text;

namespace SliderPuzzleSolver.Interfaces
{
  public interface IBoard
  {
    //board dimension n
    byte Dimension { get; }

    //get the position of the blank tile
    (byte Row, byte Column) BlankTilePosition { get; }

    // string represantion of the board
    string ToString();

    //the tile number 
    byte Tile(byte row, byte column);

    // numeber of tiles out of the place
    ushort Hamming();

    // sum of Manhattan disstances between tiles and goal
    ushort Manhattan();

    //determines if this is the board goal
    bool IsSolved();

    //determines if the board is solvable
    bool CanBeSolved();

    //determines if two board are equal
    bool Equals(object that);

    //all neighbors of the board
    IEnumerable<IBoard> GetNeighbors();

    //// a board that is obtained by exchanging any pair of tiles
    public IBoard Twin();

  }
}
