using SliderPuzzleSolver.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SliderPuzzleSolver
{
    enum Direction : byte
    {
        Right,
        Left,
        Up,
        Down
    }

    public sealed class Board : IBoard, IEquatable<IBoard>
    {
        #region Fields
        private readonly byte[,] _tiles;
        #endregion

        #region Proprieties
        public byte Dimension { get; private set; }
        public (byte Row, byte Column) BlankTilePosition { get; private set; }
        #endregion

        #region Public Methods
        public Board(byte[,] tiles)
        {
            if (tiles is null)
            {
                throw new ArgumentNullException();
            }

            Dimension = (byte)tiles.GetLength(0);
            _tiles = new byte[Dimension, Dimension];

            for (byte i = 0; i < Dimension; i++)
            {
                for (byte j = 0; j < Dimension; j++)
                {
                    _tiles[i, j] = tiles[i, j];
                    if (_tiles[i, j] == ConstantHelper.BlankTileValue)
                    {
                        BlankTilePosition = (i, j);
                    }
                }
            }
        }


        //gets the value in a specific tile
        public byte Tile(byte row, byte column)
        {
            return _tiles[row, column];
        }

        public byte[] GetTiles()
        {
            var tiles = new byte[Dimension  * Dimension ];
            int index = 0;
            foreach(var tile in _tiles)
            {
                tiles[index++] = tile;
            }
            return tiles;
        }
        //the string representation of the board
        public override string ToString()
        {
            var representation = new StringBuilder();

            for (int rowId = 0; rowId < Dimension; rowId++)
            {
                representation.AppendLine();
                for (int columnId = 0; columnId < Dimension; columnId++)
                {
                    representation.Append($"{_tiles[rowId, columnId]} ");
                }
            }

            return representation.ToString();
        }

        //determines if two boards are the same
        public bool Indentic(object that)
        {
            var otherBoard = (IBoard)that;
            if (this.Dimension != otherBoard.Dimension) return false;

            for (byte rowId = 0; rowId < Dimension; rowId++)
                for (byte columnId = 0; columnId < Dimension; columnId++)
                {
                    if (_tiles[rowId, columnId] != otherBoard.Tile(rowId, columnId))
                        return false;
                }

            return true;
        }

        //all the neightbors of the current board
        public IEnumerable<IBoard> GetNeighbors()
        {
            List<IBoard> allNeighbors = new List<IBoard>();
            foreach (var direction in ConstantHelper.DirectionsTransfom.Keys)
            {
                var transformedTiles = MoveBlankTile(direction);
                if (transformedTiles != null)
                {
                    allNeighbors.Add(new Board(transformedTiles));
                }
            }
            return allNeighbors as IEnumerable<IBoard>;
        }

        //Hamming distance: number of tiles which are in wrong position
        public ushort Hamming()
        {
            ushort hammingCount = 0;
            for (int rowId = 0; rowId < Dimension; rowId++)
                for (int colomnId = 0; colomnId < Dimension; colomnId++)
                {
                    if (_tiles[rowId, colomnId] == ConstantHelper.BlankTileValue) continue;
                    var (Row, Column) = GoalPosition(_tiles[rowId, colomnId]);
                    if (Row != rowId || Column != colomnId)
                        hammingCount++;
                }
            return hammingCount;
        }

        //Is the board solved? 
        //The board is solved when no tiles are in the wrong position
        public bool IsSolved()
        {
            return Hamming() == 0;
        }

        /// <summary>
        /// If Dimension is odd, then puzzle instance is solvable if number of inversions is even in the input state.
        /// If Dimension is even, puzzle instance is solvable if: 
        ///     the blank is on an even row counting from the bottom and number of inversions is odd.
        ///     the blank is on an odd row counting from the bottom and number of inversions is even.
        /// </summary>
        /// <returns></returns>
        public bool CanBeSolved()
        {
            var invCount = InversionCount();
            if (Dimension % 2 == 1)
            {
                return invCount % 2 == 0;
            }

            return (Dimension - BlankTilePosition.Row) % 2 != invCount % 2;
        }

        //Manhattan distance: total Manhattan distance to each tile to its goal position
        public ushort Manhattan()
        {
            ushort distance = 0;
            for (int rowId = 0; rowId < Dimension; rowId++)
                for (int colomnId = 0; colomnId < Dimension; colomnId++)
                {
                    if (_tiles[rowId, colomnId] != ConstantHelper.BlankTileValue)
                    {
                        var goalPosition = GoalPosition(_tiles[rowId, colomnId]);
                        distance += Manhattan((rowId, colomnId), goalPosition);
                    }
                }
            return distance;
        }

        //generate the goal tiles 
        public static IBoard GetGoalBoard(int dimension)
        {
            var tiles = new byte[dimension, dimension];
            for (int rowId = 0; rowId < dimension; rowId++)
                for (int columnId = 0; columnId < dimension; columnId++)
                {
                    tiles[rowId, columnId] = (byte)(rowId * dimension + columnId + 1);
                }
            tiles[dimension - 1, dimension - 1] = 0;
            return new Board(tiles);
        }

        //Returns a board twin the current one
        //A twin is obtained by swapping any two tiles which are not blank.
        public IBoard Twin()
        {
            var newTiles = _tiles.Clone() as byte[,];
            var rand = new Random();
            //determines random the value of first tile
            var firstValuePos = (Row: rand.Next() % Dimension, Colomn: rand.Next() % Dimension);
            //move one row if gets to the blank position
            if (firstValuePos == BlankTilePosition)
            {
                firstValuePos.Row = (firstValuePos.Row + 1) % Dimension;
            }
            //the second tile is on another colomn to avoid swapping same tile
            var secondValuePos = (Row: firstValuePos.Row, Colomn: (firstValuePos.Colomn + 1) % Dimension);

            if (secondValuePos == BlankTilePosition)
            {
                secondValuePos.Row = (secondValuePos.Row + 1) % Dimension;
            }

            Swap(newTiles, firstValuePos, secondValuePos);

            return new Board(newTiles);

        }

        //Determines if two boards have the arrangement of the numbers
        public bool Equals(IBoard obj)
        {
            return obj is Board board &&
                   EqualityComparer<byte[,]>.Default.Equals(_tiles, board._tiles) &&
                   BlankTilePosition.Equals(board.BlankTilePosition);
        }

        /// <summary>
        /// Calculates the hash code of the object. 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(_tiles, BlankTilePosition);
        }

        private ushort Manhattan((int Row, int Colomn) currentPosition, (int Row, int Colomn) goalPosition)
        {
            return (ushort)(Math.Abs(goalPosition.Row - currentPosition.Row) + Math.Abs(goalPosition.Colomn - currentPosition.Colomn));
        }

        #endregion

        #region Private Methods
        //if the move of the blank til is possible return tile array where the blank tile is moved according to the direction
        //if the move is not possible, return null
        private byte[,] MoveBlankTile(Direction direction)
        {
            var (Row, Column) = ConstantHelper.DirectionsTransfom[direction];
            var newPosition = (Row: BlankTilePosition.Row + Row, Column: BlankTilePosition.Column + Column);

            if (newPosition.Row < 0 || newPosition.Row >= Dimension) return null;
            if (newPosition.Column < 0 || newPosition.Column >= Dimension) return null;

            byte[,] transformedArray = new byte[Dimension, Dimension];

            for (int rowId = 0; rowId < Dimension; rowId++)
                for (int columnId = 0; columnId < Dimension; columnId++)
                {
                    transformedArray[rowId, columnId] = _tiles[rowId, columnId];
                }

            //move the value from the old position to the blank tile
            var valueToBeMoved = transformedArray[newPosition.Row, newPosition.Column];
            transformedArray[BlankTilePosition.Row, BlankTilePosition.Column] = valueToBeMoved;
            //move the blank tile to the new position
            transformedArray[newPosition.Row, newPosition.Column] = ConstantHelper.BlankTileValue;

            return transformedArray;
        }

        //gets the position when a specific tile should be
        private (byte Row, byte Colomn) GoalPosition(int value)
        {
            var valueBasedZero = value - 1;
            var goalPosition = (Row: (byte)(valueBasedZero / Dimension), Colomn: (byte)(valueBasedZero % Dimension));
            return goalPosition;
        }

        private ushort InversionCount()
        {
            byte s = 0;
            int n = Dimension * Dimension;
            byte[] copyArr = new byte[n];
            for (byte i = 0; i < Dimension; i++)
                for (byte j = 0; j < Dimension; j++)
                {
                    copyArr[i * Dimension + j] = Tile(i, j);
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

        private void Swap(byte[,] a, (int Row, int Colomn) firstValue, (int Row, int Colomn) secondValue)
        {
            byte tmp = a[firstValue.Row, firstValue.Colomn];
            a[firstValue.Row, firstValue.Colomn] = a[secondValue.Row, secondValue.Colomn];
            a[secondValue.Row, secondValue.Colomn] = tmp;
        }
        #endregion
    }
}
