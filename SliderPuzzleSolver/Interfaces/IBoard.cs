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

    // string representation of the board
    string ToString();

    //the tile number 
    byte Tile(byte row, byte column);

    ///get one dimiension array with the state of the board
    byte[] GetTiles();

    // numeber of tiles out of the place
    ushort Hamming();

    // sum of Manhattan disstances between tiles and goal
    int Manhattan();

    //determines if this is the board goal
    bool IsSolved();

    //determines if the board is solvable
    bool IsSolvable();

    //determines if two board are equal
    bool Equals(IBoard that);

    //all neighbors of the board and their Manhattan difference with the respect of the parent board. 
    Dictionary<IBoard,int> GetChildBoards();

    //// a board that is obtained by exchanging any pair of tiles
    public IBoard Twin();

  }
}
